#define Win
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Xml;

namespace Excalibur
{
    public class ExcaliburServer
    {
        private const int TIMEOUT = 2000;
        SynchronizationContext SyncContext;
        Socket ListenerSocket;
        Thread ListenerThread;
        bool ListeningEnabled;
        long ProtocolIdentifier;
        public List<ExcaliburClient> Clients;
        public int Timeout
        {
            get
            {
                return ListenerSocket.ReceiveTimeout;
            }
            set
            {
                ListenerSocket.ReceiveTimeout = value;
            }
        }
        public ExcaliburServer(int port, long protocolIdentifier)
        {
            if (SynchronizationContext.Current != null)
            {
                SyncContext = SynchronizationContext.Current;
            }
            else
            {
                SyncContext = new SynchronizationContext();
            }
            ListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ListenerSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            ListeningEnabled = true;
            ProtocolIdentifier = protocolIdentifier;
            Timeout = TIMEOUT;
            Clients = new List<ExcaliburClient>();
            ListenerThread = new Thread(Listen);
            ListenerThread.Name = "Listener thread";
            ListenerThread.Start();
        }

        public int Port
        {
            get
            {
                return (ListenerSocket.LocalEndPoint as IPEndPoint).Port;
            }
        }

        public void StopListening()
        {
            ListeningEnabled = false;
            ListenerSocket.Close();
        }

        private void Listen()
        {
            while (ListeningEnabled)
            {
                try
                {
                    ListenerSocket.Listen(1);
                    Socket ClientSocket = ListenerSocket.Accept();
                    ClientSocket.ReceiveTimeout = ClientSocket.SendTimeout = Timeout;
                    byte[] identifier = new byte[8];
                    ClientSocket.Receive(identifier);
                    if (BitConverter.ToInt64(identifier, 0) == ProtocolIdentifier)
                    {
                        ClientSocket.Send(identifier);
                        ExcaliburClient EC = new ExcaliburClient(ClientSocket, ProtocolIdentifier, SyncContext);
                        EC.Disconnected += new ExcaliburClient.DisconnectEventHandler(EC_Disconnected);
                        Clients.Add(EC);
                        SyncContext.Post(ClientConnectedCallback, EC);
                    }
                    else
                    {
                        ClientSocket.Close();
                    }
                }
                catch
                {

                }
            }
        }

        void EC_Disconnected(object sender, ExcaliburClient.DisconnectEventArgs e)
        {
            if (ClientDisconnected != null)
            {
                ClientDisconnected(this, new ClientEventArgs(sender as ExcaliburClient));
            }
        }

        private void ClientConnectedCallback(object o)
        {
            if (ClientConnected != null)
            {
                ClientConnected(this, new ClientEventArgs(o as ExcaliburClient));
            }
        }

        public class ClientEventArgs : EventArgs
        {
            public ExcaliburClient Client;
            public ClientEventArgs(ExcaliburClient client)
            {
                Client = client;
            }
        }
        public delegate void ClientEventHandler(object sender, ClientEventArgs e);
        public event ClientEventHandler ClientConnected, ClientDisconnected;
    }

    public class ExcaliburClient
    {
        private const int TIMEOUT = 500000;
        private const int PACKETSIZE = 4096;
        private const byte FRAMEDELIMITER = 170;
        Socket ClientSocket;
        long ProtocolIdentifier;
        Thread ReceiveThread, SendThread;
        IPEndPoint ClientEndPoint;
        SynchronizationContext SyncContext;
        int NextSendID = 0;
        Dictionary<int, Packet> PacketsSending, PacketsReceiving;
        Queue<Packet> PacketsToSend;
        AutoResetEvent SendARE, ConnectARE;
        enum MessageTypes : byte { Header = 254, Data = 1, AcceptPacket = 2, KeepAlive = 3, CancelBySender = 4, CancelByReceiver = 5, Close = 127, Footer = 255 };
        Timer KeepAliveTimer;
        bool ThreadsEnabled;
        public int Timeout
        {
            get
            {
                if (ThreadsEnabled)
                    return ClientSocket.ReceiveTimeout;
                else
                    return 0;
            }
            set
            {
                if (ThreadsEnabled)
                {
                    ClientSocket.ReceiveTimeout = value;
                    KeepAliveTimer.Change(value / 4, value / 2);
                }
            }
        }
        public int PacketSize { get; set; }
        public ExcaliburClient(IPEndPoint clientEndPoint, long protocolIdentifier)
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ProtocolIdentifier = protocolIdentifier;
            ClientEndPoint = clientEndPoint;
            CommonInit();
            ReceiveThread = new Thread(ConnectAndReceiveWorker);
            ReceiveThread.Start();
        }

        void ConnectAndReceiveWorker()
        {
            ClientSocket.Connect(ClientEndPoint);
            ClientSocket.Send(BitConverter.GetBytes(ProtocolIdentifier));
            byte[] identifier = new byte[8];
            ClientSocket.Receive(identifier);
            if (BitConverter.ToInt64(identifier, 0) == ProtocolIdentifier)
            {
                ReceiveWorker();
            }
        }

        internal ExcaliburClient(Socket clientSocket, long protocolIdentifier, SynchronizationContext sc)
        {
            ProtocolIdentifier = protocolIdentifier;
            ClientEndPoint = clientSocket.LocalEndPoint as IPEndPoint;
            ClientSocket = clientSocket;
            SyncContext = sc;
            CommonInit();
            ReceiveThread = new Thread(ReceiveWorker);
            ReceiveThread.Start();
        }

        void CommonInit()
        {
            if (SyncContext == null)
            {
                if (SynchronizationContext.Current != null)
                {
                    SyncContext = SynchronizationContext.Current;
                }
                else
                {
                    SyncContext = new SynchronizationContext();
                }
            }
            ThreadsEnabled = true;
            ClientSocket.Blocking = true;
            ClientSocket.NoDelay = false;
            KeepAliveTimer = new Timer(KeepAlive, null, TIMEOUT / 2, TIMEOUT / 2);
            Timeout = TIMEOUT;
            PacketSize = PACKETSIZE;
            DisconnectType = DisconnectTypes.Unknown;
            PacketsSending = new Dictionary<int, Packet>();
            PacketsReceiving = new Dictionary<int, Packet>();
            PacketsToSend = new Queue<Packet>();
            SendARE = new AutoResetEvent(false);
            ConnectARE = new AutoResetEvent(false);
            SendThread = new Thread(SendWorker);
            SendThread.Start();
        }

        void KeepAlive(object o)
        {
            if (SendARE != null)
                SendARE.Set();
        }

        public class PacketEventArgs : EventArgs
        {
            public Packet P;
            public PacketEventArgs(Packet p)
            {
                P = p;
            }
        }
        public delegate void PacketEventHandler(object sender, PacketEventArgs e);
        public event PacketEventHandler ReceivingStarted, FragmentReceived, ReceivingFinished, ReceivingCancelledBySender, SendCancelledByReceiver, PacketSent;

        struct PacketEventProperties
        {
            public PacketEventTypes PacketEventHandler;
            public PacketEventArgs PacketEventArgs;
            public PacketEventProperties(PacketEventTypes eventHandler, PacketEventArgs eventArgs)
            {
                PacketEventHandler = eventHandler;
                PacketEventArgs = eventArgs;
            }
        }

        enum PacketEventTypes { ReceivingStarted, FragmentReceived, ReceivingFinished, ReceivingCancelledBySender, SendCancelledByReceiver, Disconnected, PacketSent };
        void RaisePacketEventAsync(PacketEventTypes eventType, PacketEventArgs e)
        {
            SyncContext.Post(RaisePacketEventCallback, new PacketEventProperties(eventType, e));
        }

        public enum DisconnectTypes { Direct, Indirect, Unknown };
        DisconnectTypes DisconnectType;
        public class DisconnectEventArgs : EventArgs
        {
            public DisconnectTypes DisconnectType;
            public DisconnectEventArgs(DisconnectTypes type)
            {
                DisconnectType = type;
            }
        }
        public delegate void DisconnectEventHandler(object sender, DisconnectEventArgs e);
        public event DisconnectEventHandler Disconnected;

        void RaisePacketEventCallback(object o)
        {
            PacketEventProperties PEP = (PacketEventProperties)o;
            PacketEventHandler eventHandler;
            switch (PEP.PacketEventHandler)
            {
                case PacketEventTypes.ReceivingStarted:
                    eventHandler = ReceivingStarted;
                    break;
                case PacketEventTypes.FragmentReceived:
                    eventHandler = FragmentReceived;
                    break;
                case PacketEventTypes.ReceivingFinished:
                    if (ReceivingFinished == null)
                    {
                        PEP.PacketEventArgs.P.Data.Dispose();
                    }
                    eventHandler = ReceivingFinished;
                    break;
                case PacketEventTypes.ReceivingCancelledBySender:
                    eventHandler = ReceivingCancelledBySender;
                    break;
                case PacketEventTypes.SendCancelledByReceiver:
                    eventHandler = SendCancelledByReceiver;
                    break;
                case PacketEventTypes.PacketSent:
                    eventHandler = PacketSent;
                    break;
                default:
                    eventHandler = null;
                    break;
            }
            if (eventHandler != null)
            {
                eventHandler(this, PEP.PacketEventArgs);
            }
            switch (PEP.PacketEventHandler)
            {
                case PacketEventTypes.ReceivingStarted:
                    if (PEP.PacketEventArgs.P.Data == null)
                    {
                        PEP.PacketEventArgs.P.Data = new MemoryStream((int)PEP.PacketEventArgs.P.Length);
                    }
                    Packet P = new Packet();
                    P.InternalID = NextSendID++;
                    P.ID = PEP.PacketEventArgs.P.InternalID;
                    P.Type = Packet.Types.AcceptPacket;
                    PEP.PacketEventArgs.P.State |= Packet.States.Handshake;
                    lock (PacketsToSend)
                    {
                        PacketsToSend.Enqueue(P);
                    }
                    SendARE.Set();
                    break;
            }
        }

        bool DisconnectReported = false;
        void DisconnectCallback(object o)
        {
            if (Disconnected != null && !DisconnectReported)
            {
                DisconnectReported = true;
                Disconnected(this, new DisconnectEventArgs(DisconnectType));
            }
        }

        void ReceiveWorker()
        {
            byte[] lengthBuffer = new byte[5];
            byte[] buffer = new byte[PacketSize];
            MemoryStream bufferStream = new MemoryStream(buffer);
            BinaryReader bufferReader = new BinaryReader(bufferStream);
            int internalID, length, id;
            MessageTypes messageType;
            Packet P;
            ConnectARE.Set();
            while (ThreadsEnabled)
            {
                try
                {
                    ClientSocket.Read(lengthBuffer, 5);
                }
                catch
                {
                    break;
                }
                if (lengthBuffer[0] != FRAMEDELIMITER)
                    throw new Exception();
                length = BitConverter.ToInt32(lengthBuffer, 1);
                if (length == 0)
                    throw new Exception();
                if (buffer.Length < length)
                {
                    buffer = new byte[length];
                    bufferStream = new MemoryStream(buffer);
                    bufferReader = new BinaryReader(bufferStream);
                }
                else
                {
                    bufferStream.Position = 0;
                }
                try
                {
                    ClientSocket.Read(buffer, length);
                }
                catch
                {
                    break;
                }
                messageType = (MessageTypes)bufferReader.ReadByte();
                internalID = bufferReader.ReadInt32();
                switch (messageType)
                {
                    case MessageTypes.Header:
                        P = new Packet();
                        P.InternalID = internalID;
                        P.ID = bufferReader.ReadInt32();
                        P.Length = bufferReader.ReadInt64();
                        P.State = Packet.States.Header;
                        PacketsReceiving.Add(P.InternalID, P);
                        RaisePacketEventAsync(PacketEventTypes.ReceivingStarted, new PacketEventArgs(P));
                        break;
                    case MessageTypes.Data:
                        P = PacketsReceiving[internalID];
                        P.Data.Write(buffer, (int)bufferStream.Position, length - (int)bufferStream.Position);
                        P.Position = P.Data.Position;
                        if ((P.Settings & Packet.Options.ShowFragments) == Packet.Options.ShowFragments)
                        {
                            RaisePacketEventAsync(PacketEventTypes.FragmentReceived, new PacketEventArgs(P));
                        }
                        break;
                    case MessageTypes.Footer:
                        P = PacketsReceiving[internalID];
                        P.State |= Packet.States.Data | Packet.States.Footer;
                        RaisePacketEventAsync(PacketEventTypes.ReceivingFinished, new PacketEventArgs(P));
                        PacketsReceiving.Remove(internalID);
                        break;
                    case MessageTypes.AcceptPacket:
                        id = bufferReader.ReadInt32();
                        PacketsSending[id].State |= Packet.States.Handshake;
                        PacketsSending[id].Processing = true;
                        SendARE.Set();
                        break;
                    case MessageTypes.CancelBySender:
                        if (PacketsReceiving.ContainsKey(internalID))
                        {
                            P = PacketsReceiving[internalID];
                            P.State |= Packet.States.CancelledBySender;
                            RaisePacketEventAsync(PacketEventTypes.ReceivingCancelledBySender, new PacketEventArgs(P));
                            PacketsReceiving.Remove(internalID);
                        }
                        break;
                    case MessageTypes.CancelByReceiver:
                        id = bufferReader.ReadInt32();
                        if (PacketsSending.ContainsKey(id))
                        {
                            P = PacketsSending[id];
                            P.State |= Packet.States.CancelledByReceiver | Packet.States.StreamComplete;
                            RaisePacketEventAsync(PacketEventTypes.SendCancelledByReceiver, new PacketEventArgs(P));
                        }
                        break;
                    case MessageTypes.Close:
                        DisconnectType = DisconnectTypes.Indirect;
                        ThreadsEnabled = false;
                        break;
                }
            }
            ThreadsEnabled = false;
            foreach (Packet p in PacketsReceiving.Values)
            {
                if (p.Data != null)
                {
                    p.Data.Dispose();
                }
            }
            PacketsReceiving.Clear();
            PacketsReceiving = null;
            bufferStream.Dispose();
            bufferReader.Dispose();
            if (DisconnectType == DisconnectTypes.Direct)
            {
                ClientSocket.Dispose();
                ClientSocket = null;
            }
            if (SendARE != null) SendARE.Set();
            SyncContext.Post(DisconnectCallback, null);
            KeepAliveTimer.Dispose();
        }

        public void CancelReceiveOperation(int id)
        {
            if (ThreadsEnabled && PacketsReceiving.ContainsKey(id))
            {
                Packet P = new Packet();
                P.ID = id;
                P.Type = Packet.Types.CancelStream;
                P.InternalID = NextSendID++;
                lock (PacketsToSend)
                {
                    PacketsToSend.Enqueue(P);
                }
                SendARE.Set();
            }
        }

        public void CancelSendOperation(int id)
        {
            if (ThreadsEnabled && PacketsSending.ContainsKey(id))
            {
                PacketsSending[id].State |= Packet.States.CancelledBySender;
                SendARE.Set();
            }
        }

        public class ClientDisconnectedException : Exception { }
        public void Disconnect()
        {
            if (ThreadsEnabled)
            {
                ThreadsEnabled = false;
                DisconnectType = DisconnectTypes.Direct;
                SendARE.Set();
            }
        }

        public int Send(int ID, Stream stream, long length)
        {
            if (ThreadsEnabled)
            {
                Packet P = new Packet();
                P.InternalID = NextSendID++;
                P.Data = stream;
                P.ID = ID;
                P.Length = length;
                P.Type = Packet.Types.Stream;
                lock (PacketsToSend)
                {
                    PacketsToSend.Enqueue(P);
                }
                SendARE.Set();
                return P.InternalID;
            }
            else
                throw new ClientDisconnectedException();
        }

        private void SendWorker()
        {
            byte[] buffer = new byte[PacketSize];
            MemoryStream bufferStream = new MemoryStream(buffer);
            BinaryWriter bufferWriter = new BinaryWriter(bufferStream);
            int dataLength, packetsToProcess;
            Packet P;
            Packet[] Packets;
            ConnectARE.WaitOne();
            ConnectARE.Dispose();
            ConnectARE = null;
            while (true)
            {
                SendARE.WaitOne();
                if (buffer.Length != PacketSize)
                {
                    buffer = new byte[PacketSize];
                }

                packetsToProcess = 1;
                while (packetsToProcess > 0)
                {
                    if (!ThreadsEnabled)
                    {
                        if (DisconnectType == DisconnectTypes.Direct)
                        {
                            bufferStream.Position = 0L;
                            bufferWriter.Write(FRAMEDELIMITER);
                            bufferWriter.Write(5);
                            bufferWriter.Write((byte)MessageTypes.Close);
                            bufferWriter.Write(-1);
                            try
                            {
                                ClientSocket.Send(buffer, 10, SocketFlags.None);
                            }
                            catch
                            {

                            }
                            ClientSocket.Disconnect(false);
                            ClientSocket.Close();
                        }
                        else
                        {
                            ClientSocket.Dispose();
                            ClientSocket = null;
                        }
                        bufferStream.Dispose();
                        bufferWriter.Close();
                        PacketsToSend.Clear();
                        PacketsSending.Clear();
                        PacketsToSend = null;
                        PacketsSending = null;
                        SendARE.Dispose();
                        SendARE = null;
                        KeepAliveTimer.Dispose();
                        SyncContext.Post(DisconnectCallback, null);
                        return;
                    }

                    lock (PacketsToSend)
                    {
                        while (PacketsToSend.Count > 0)
                        {
                            P = PacketsToSend.Dequeue();
                            PacketsSending.Add(P.InternalID, P);
                        }
                    }

                    Packets = PacketsSending.Values.ToArray<Packet>();
                    packetsToProcess = 0;
                    for (int i = 0; i < Packets.Length; i++)
                    {
                        P = Packets[i];
                        bufferStream.Position = 0L;
                        bufferWriter.Write(FRAMEDELIMITER);
                        bufferStream.Position = 5L;
                        switch (P.Type)
                        {
                            case Packet.Types.Stream:
                                if ((P.State & Packet.States.CancelledBySender) == Packet.States.CancelledBySender)
                                {
                                    bufferWriter.Write((byte)MessageTypes.CancelBySender);
                                    bufferWriter.Write(P.InternalID);
                                    P.Processing = false;
                                    P.State |= Packet.States.StreamComplete;
                                }
                                else
                                {
                                    switch (P.State)
                                    {
                                        case Packet.States.None:
                                            bufferWriter.Write((byte)MessageTypes.Header);
                                            bufferWriter.Write(P.InternalID);
                                            bufferWriter.Write(P.ID);
                                            bufferWriter.Write(P.Length);
                                            P.State = Packet.States.Header;
                                            P.Processing = false;
                                            break;
                                        case Packet.States.SendingData:
                                            bufferWriter.Write((byte)MessageTypes.Data);
                                            bufferWriter.Write(P.InternalID);
                                            if (P.Position != P.Data.Position)
                                            {
                                                P.Data.Position = P.Position;
                                            }
                                            int sentBytes = P.Data.Read(buffer, (int)bufferStream.Position, PacketSize - (int)bufferStream.Position);
                                            P.Position += sentBytes;
                                            bufferStream.Position += sentBytes;
                                            if (P.Position >= P.Length)
                                            {
                                                P.State |= Packet.States.Data;
                                            }
                                            break;
                                        case Packet.States.DataComplete:
                                            bufferWriter.Write((byte)MessageTypes.Footer);
                                            bufferWriter.Write(P.InternalID);
                                            P.Processing = false;
                                            P.State |= Packet.States.Footer;
                                            break;
                                    }
                                }
                                if ((P.State & Packet.States.StreamComplete) == Packet.States.StreamComplete)
                                {
                                    PacketsSending.Remove(P.InternalID);
                                    RaisePacketEventAsync(PacketEventTypes.PacketSent, new PacketEventArgs(P));
                                }
                                break;
                            case Packet.Types.AcceptPacket:
                                bufferWriter.Write((byte)MessageTypes.AcceptPacket);
                                bufferWriter.Write(P.InternalID);
                                bufferWriter.Write(P.ID);
                                P.Processing = false;
                                PacketsSending.Remove(P.InternalID);
                                break;
                            case Packet.Types.CancelStream:
                                bufferWriter.Write((byte)MessageTypes.CancelByReceiver);
                                bufferWriter.Write(P.InternalID);
                                bufferWriter.Write(P.ID);
                                P.Processing = false;
                                PacketsSending.Remove(P.InternalID);
                                break;
                        }
                        if (bufferStream.Position != 5L)
                        {
                            dataLength = (int)bufferStream.Position;
                            bufferStream.Position = 1;
                            bufferWriter.Write(dataLength - 5);
                            if (ThreadsEnabled)
                            {
                                try
                                {
                                    ClientSocket.Send(buffer, dataLength, SocketFlags.None);
                                }
                                catch
                                {
                                    ThreadsEnabled = false;
                                    break;
                                }
                            }
                            else break;
                        }
                        if (P.Processing) packetsToProcess++;
                    }
                }

                if (ThreadsEnabled)
                {
                    bufferStream.Position = 0L;
                    bufferWriter.Write(FRAMEDELIMITER);
                    bufferWriter.Write(5);
                    bufferWriter.Write((byte)MessageTypes.KeepAlive);
                    bufferWriter.Write(-1);
                    try
                    {
                        ClientSocket.Send(buffer, 10, SocketFlags.None);
                    }
                    catch
                    {
                        ThreadsEnabled = false;
                    }
                }
            }
        }
        public class Packet
        {
            internal bool Processing = true;
            internal enum Types : byte
            {
                Stream = 0,
                AcceptPacket,
                CancelStream
            }
            internal Types Type;
            [Flags]
            internal enum States : byte
            {
                None = 0,
                Header = 1,
                Data = 2,
                Footer = 4,
                Body = 7,
                Handshake = 8,
                SendingData = 9,
                DataComplete = 11,
                StreamComplete = 15,
                CancelledBySender = 16,
                CancelledByReceiver = 32
            };
            internal States State;
            [Flags]
            public enum Options : byte
            {
                None = 0,
                ShowFragments = 1
            }
            public Options Settings;
            public int ID;
            public int InternalID { get; internal set; }
            public Stream Data;
            internal long Position, Length;
        }
    }

    public static class Extensions
    {
        public static void Read(this Socket socket, byte[] buffer, int count)
        {
            int readedbytes = 0;
            while (readedbytes < count && socket.Connected)
            {
                readedbytes += socket.Receive(buffer, readedbytes, count - readedbytes, SocketFlags.None);
            }
            if (!socket.Connected)
                throw new SocketException();
        }
    }
}
