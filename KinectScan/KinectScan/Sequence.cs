using System;
using System.Windows.Forms;
using System.IO;
using KinectScan.Properties;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace KinectScan
{
    public partial class MainForm
    {
        private void InitSequence()
        {
            SaveAREs = new AutoResetEvent[SaveThreadCount];
            for (int i = 0; i < SaveAREs.Length; i++)
                SaveAREs[i] = new AutoResetEvent(false);
            SequenceTasks = new Queue<SequenceTask>();
            TSLSequencePerformance.Maximum = SaveThreadCount;
            GUITimer.Tick += new EventHandler(GUITimer_Tick);
        }

        void GUITimer_Tick(object sender, EventArgs e)
        {
            if(TSBSequenceSave.Checked)
                TSLSequencePerformance.Value = SaveThreadsActive;
        }
        const int SaveThreadCount = 4;
        AutoResetEvent[] SaveAREs;
        private void InitSequenceScanner(Scanner scanner)
        {
            scanner.RawFrameIn += scanner_RawFrameIn;
        }

        void scanner_RawFrameIn(object sender, Scanner.RawFrameEventArgs e)
        {
            if (SequenceRecording && e.FrameID % SequenceInterval == 0)
            {
                string path = WorkingDirectory + '\\' + SequenceLabel + '_' + SequenceCounter.ToString("D5");
                switch (e.FrameType)
                {
                    case Scanner.FrameTypes.Depth:
                        SetSequenceCounter(SequenceCounter + 1);
                        path += ".rwd";
                        break;
                    case Scanner.FrameTypes.Color:
                        path += ".rwc";
                        break;
                }
                SequenceTasks.Enqueue(new SequenceTask()
                {
                    Data = e.Data,
                    FileName = path
                });
                
                for (int i = 0; i < SaveAREs.Length; i++)
                    SaveAREs[i].Set();
            }
        }

        private void TSBSequenceSave_Click(object sender, EventArgs e)
        {
            SetSequenceMode(!SequenceMode);
        }

        bool SequenceMode = false;
        private void SetSequenceMode(bool value)
        {
            if (SequenceMode != value)
            {
                SequenceMode = value;
                TSSequence.Visible = value;
                TSBSequenceSave.Checked = value;
                if (!value) StopSequence();
            }
        }

        private int SequenceInterval;
        private void SetSequenceInterval(int value)
        {
            SequenceInterval = value;
            TSLSequenceInterval.Text = value.ToString();
        }
        private void TSBSequenceDecreaseInterval_Click(object sender, EventArgs e)
        {
            if (SequenceInterval > 1) SetSequenceInterval(SequenceInterval - 1);
        }

        private void TSBSequenceIncreaseInterval_Click(object sender, EventArgs e)
        {
            SetSequenceInterval(SequenceInterval + 1);
        }

        bool SequenceRecording;
        string SequenceLabel;
        int SequenceCounter = 0;
        private void SetSequenceCounter(int value) { SequenceCounter = value; TSLSequenceCounter.Text = value.ToString(); }

        private void TSBSequenceRecord_Click(object sender, EventArgs e)
        {
            RecordSequence();
        }

        private void RecordSequence()
        {
            if (!SequenceRecording)
            {
                if (SequenceCounter == 0)
                {
                    if (string.IsNullOrEmpty(TSTBLabel.Text))
                    {
                        MessageBox.Show(LocalizedStrings.LabelSpecificationNeeded, Resources.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string[] files = null;
                    try
                    {
                        files = Directory.GetFiles(WorkingDirectory);
                    }
                    catch { }
                    if (files != null)
                    {
                        string file;
                        for (int i = 0; i < files.Length; i++)
                        {
                            file = Path.GetFileName(files[i]);
                            if (file.StartsWith(TSTBLabel.Text))
                            {
                                if (MessageBox.Show(LocalizedStrings.LabelNameConflict, Resources.AppTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                                break;
                            }
                        }
                    }
                }
                SequenceLabel = TSTBLabel.Text;
                TSTBLabel.Enabled = false;
                SequenceRecording = true;
                TSBSequenceRecord.Checked = true;
                TSBSequencePause.Checked = false;

                for (int i = 0; i < SaveThreadCount; i++)
                {
                    Thread T = new Thread(SaveThread);
                    T.Start(i);
                }
            }
        }

        
        struct SequenceTask
        {
            public byte[] Data;
            public string FileName;
        }
        Queue<SequenceTask> SequenceTasks;
        int SaveThreadsActive;
        private void SaveThread(object o)
        {
            int id = (int)o;
            bool taskfound;
            SequenceTask task = new SequenceTask();
            while (SequenceRecording)
            {
                SaveAREs[id].WaitOne();
                Interlocked.Increment(ref SaveThreadsActive);
                while (SequenceTasks.Count > 0)
                {
                    lock (SequenceTasks)
                    {
                        if (SequenceTasks.Count > 0)
                        {
                            task = SequenceTasks.Dequeue();
                            taskfound = true;
                        }
                        else taskfound = false;
                    }

                    if (taskfound)
                    {
                        try
                        {
                            if (File.Exists(task.FileName)) File.Delete(task.FileName);
                            File.WriteAllBytes(task.FileName, task.Data);
                        }
                        catch { }
                    }
                }
                if (SaveThreadsActive == 0) Debugger.Break();
                Interlocked.Decrement(ref SaveThreadsActive);
            }
        }

        private void TSBSequencePause_Click(object sender, EventArgs e)
        {
            PauseSequence();
        }

        private void PauseSequence()
        {
            if (SequenceRecording)
            {
                SequenceRecording = false;
                TSBSequenceRecord.Checked = false;
                TSBSequencePause.Checked = true;
                for (int i = 0; i < SaveAREs.Length; i++)
                    SaveAREs[i].Set();
            }
        }

        private void TSBSequenceStop_Click(object sender, EventArgs e)
        {
            StopSequence();
        }

        private void StopSequence()
        {
            SequenceRecording = false;
            TSBSequenceRecord.Checked = false;
            TSBSequencePause.Checked = false;
            TSTBLabel.Enabled = true;
            SetSequenceCounter(0);
            for (int i = 0; i < SaveAREs.Length; i++)
                SaveAREs[i].Set();
        }
    }
}