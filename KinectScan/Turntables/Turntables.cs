using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace Turntables
{
    public class Turntable : IDisposable
    {
        static SynchronizationContext SC;
        static Timer PollTimer;
        public static int DeviceCount { get; private set; }
        public static int OpenedDevices { get; private set; }
        public static event EventHandler DeviceConnected, DeviceDisconnected;
        static Turntable()
        {
            SC = SynchronizationContext.Current;
            PollTimer = new Timer(DevicePollTimerCallback, null, 1000, 1000);
        }

        static void DevicePollTimerCallback(object o)
        {
            int deviceCount = GetDevices().Length;
            SC.Post(DevicePollCallback, deviceCount);
        }

        static void DevicePollCallback(object o)
        {
            int oldDeviceCount = DeviceCount;
            DeviceCount = (int)o + OpenedDevices;
            if (DeviceCount > oldDeviceCount && DeviceConnected != null) DeviceConnected(null, null);
            if (DeviceCount < oldDeviceCount && DeviceDisconnected != null) DeviceDisconnected(null, null);
        }

        public static Turntable DefaultDevice
        {
            get
            {
                if (DeviceCount == 0) return null;
                Turntable T = null;
                try
                {
                    T = new Turntable(GetDevices()[0]);
                    return T;
                }
                catch
                {
                    if (T != null) T.Dispose();
                    return null;
                }
            }
        }

        [Flags]
        private enum States : ushort { None = 0, Rotating = 0x0001, RotatingUp = 0x0002}
        public enum Commands : int { About = 0, Stop, StartUp, StartDown, Status, Step, StepBack, ClearCounter, TurnOff, ToOrigin }
        private static readonly string[] CommandStrings = new string[] { "?", "S", "U", "D", "C", "A", "V", "Z", "E", "O" };
        private const string DeviceSignal = "Basch-step";
        private SerialPort Port;
        private int Origin;
        public int ToStep { get; private set; }
        public int PositionInSteps { get; private set; }
        public double PositionInRadians 
        {
            get
            {
                return (double)PositionInSteps / PiSteps*Math.PI;
            }
        }
        public double PositionInDegrees
        {
            get
            {
                double degrees = PositionInSteps * 360d / PiSteps / 2d;
                if (degrees > 360) return degrees - 360d;
                if (degrees < 0) return degrees + 360d;
                return degrees;
            }
        }

        public const int PiSteps = 11000;

        private static void InitPort(SerialPort port)
        {
            port.BaudRate = 9600;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;
            port.Handshake = Handshake.None;
            port.RtsEnable = true;
            port.DtrEnable = true;
            port.ReadTimeout = Timeout.Infinite;
            port.NewLine = "\r\n";
        }

        public static string[] GetDevices()
        {
            SerialPort SP = new SerialPort();
            InitPort(SP);

            List<string> devices = new List<string>();
            string[] ports = SerialPort.GetPortNames();

            int i;
            for (i = 1; i < ports.Length; i++)
            {
                SP.PortName = ports[i];
                try
                {
                    SP.Open();
                    SP.DiscardInBuffer();

                    SP.Write(CommandStrings[(int)Commands.About]);
                    if (SP.ReadLine().StartsWith(DeviceSignal))
                    {
                        devices.Add(SP.PortName);
                        SP.Write(CommandStrings[(int)Commands.TurnOff]);
                    }
                }
                catch (UnauthorizedAccessException) { }
                finally
                {
                    if (SP.IsOpen) SP.Close();
                }
            }
            SP.Dispose();
            return devices.ToArray();
        }

        public void TurnOff()
        {
            SendCommandAsync(Commands.TurnOff);
        }

        private SynchronizationContext SyncContext;
        private bool CommunicationThreadOn;
        private Thread CommunicationThread;
        private Queue<string> CommandQueue;
        private AutoResetEvent SendARE;
        private void Communicate()
        {
            string command, answer;
            bool rotating;
            States statusWord;
            while (CommunicationThreadOn)
            {
                SendARE.WaitOne(PositionRefreshPeriod);
                while (CommandQueue.Count > 0)
                {
                    lock (CommandQueue) command = CommandQueue.Dequeue();
                    Port.Write(command);
                    Port.ReadLine();
                    if (command == CommandStrings[(int)Commands.ToOrigin])
                    {
                        CommandQueue.Enqueue(CommandStrings[(int)Commands.ClearCounter]);
                        SyncContext.Post(MotorStoppedCallback, null);                        
                    }
                }
                Port.Write(CommandStrings[(int)Commands.Status]);
                answer = Port.ReadLine();
                int h;

                int ss = Convert.ToInt32(answer.Substring(6, 6), 16);
                if (ToStep > 0)
                {
                    if (ss > 32767)
                        h = ToStep - (65536 - ss);
                    else
                        h = ToStep - ss;
                    if (h <= ToStep)
                        PositionInSteps = Origin + h;
                }
                else
                {
                    if (ss > 32767)
                        h = ToStep + (65536 - ss);
                    else
                        h = ToStep + ss;
                    if (h >= ToStep)
                        PositionInSteps = Origin + h;
                }
                if (PositionInSteps >= stopOn)
                {
                    stopOn = int.MaxValue;
                    CommandQueue.Enqueue(CommandStrings[(int)Commands.Stop]);
                    SendARE.Set();
                    SyncContext.Post(TurnCompleteCallback, null);
                }
                statusWord = (States)Convert.ToInt16(answer.Substring(0, 2), 16);
                rotating = statusWord.HasFlag(States.Rotating);
                if (Rotating && !rotating) SyncContext.Post(MotorStoppedCallback, null);
                Rotating = rotating;
                RotationDirection = statusWord.HasFlag(States.RotatingUp);
            }
            Port.Write(CommandStrings[(int)Commands.Stop]);
            Port.Write(CommandStrings[(int)Commands.TurnOff]);
        }

        int stopOn=int.MaxValue;
        public void TurnOnce()
        {
            stopOn = PiSteps * 2;
            SendCommandAsync(Commands.ClearCounter);
            SendCommandAsync(Commands.StartUp);
        }

        public event EventHandler MotorStopped;
        private void MotorStoppedCallback(object o)
        {
            if (MotorStopped != null) MotorStopped(this, null);
        }

        public event EventHandler TurnComplete;
        private void TurnCompleteCallback(object o)
        {
            if (TurnComplete != null) TurnComplete(this, null);
        }

        public int PositionRefreshPeriod { get; set; }
        public int StepCount { get; private set; }
        public bool Rotating { get; private set; }
        public bool RotationDirection { get; private set; }
        public bool HighEmergencyStop { get; private set; }
        public bool LowEmergencyStop { get; private set; }
        public Turntable(string portName)
        {
            Port = new SerialPort();
            Port.PortName = portName;
            InitPort(Port);
            Port.Open();
            OpenedDevices++;

            CommandQueue = new Queue<string>();
            SendARE = new AutoResetEvent(true);
            SyncContext = SynchronizationContext.Current;
            PositionRefreshPeriod = 1;

            CommunicationThreadOn = true;
            CommunicationThread = new Thread(Communicate);
            CommunicationThread.Start();
        }

        public void SendCommandAsync(Commands command, int steps = 0)
        {
            string s = CommandStrings[(int)command];
            if (steps != 0)
            {
                s += string.Format("{0:X6}", steps);
                switch (command)
                {
                    case Commands.Step:
                        Origin = PositionInSteps;
                        ToStep = steps;
                        break;
                    case Commands.StepBack:
                        Origin = PositionInSteps;
                        ToStep = -steps;
                        break;
                }
            }
            lock (CommandQueue) CommandQueue.Enqueue(s);
            SendARE.Set();
        }

        public void RotateTo(double angle)
        {
            int steps = (int)(angle / Math.PI) * PiSteps - PositionInSteps;
            if (steps > 0)
                SendCommandAsync(Turntable.Commands.Step, steps);
            else
                SendCommandAsync(Turntable.Commands.StepBack, -steps);
        }

        bool disposed = false;
        public void Dispose()
        {
            disposed = true;
            CommunicationThreadOn = false;
            SendARE.Set();
            CommunicationThread.Join();
            Port.Dispose();
            Port = null;
            OpenedDevices--;
        }

        ~Turntable()
        {
            if (!disposed) Dispose();
        }
    }
}
