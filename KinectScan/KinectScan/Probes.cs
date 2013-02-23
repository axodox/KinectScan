using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using KinectScan.Properties;
using System.Threading;

namespace KinectScan
{
    public partial class MainForm
    {
        int ProbeInterval = 5;
        bool ProbesOn;
        enum ProbeModes { None, Add, MoveStart, MoveEnd, Remove };
        ProbeModes ProbeMode;
        ProbeForm PF;
        ProbeCollection Probes;
        Thread GPUDownloadThread;
        AutoResetEvent GPUDownloadARE;
        RenderTarget2D GPUDownloadCache;        
        bool GPUDownloadOn;
        bool DownloadComplete;
        const int ProbeRadius2 = 32 * 32;
        Probe ProbeToMove;
        private void InitProbes()
        {
            PF = new ProbeForm();
            PF.TSBAddProbe.Click += new EventHandler(TSBAddProbe_Click);
            PF.TSBMoveProbe.Click += new EventHandler(TSBMoveProbe_Click);
            PF.TSBRemoveProbe.Click += new EventHandler(TSBRemoveProbe_Click);
            Probes = new ProbeCollection(PF.LVProbes);
            ProbeMode = ProbeModes.None;

            PF.FormClosing += (object sender, FormClosingEventArgs e) =>
            {
                SetProbeState(false);
            };
            XPanel.MouseUp += new MouseEventHandler(ProbeMouseUp);
            XPanel.MouseMove += new MouseEventHandler(XPanel_MouseMove);
            XPanel.MouseDown += (object sender, MouseEventArgs e) =>
                {
                    if (ProbesOn)
                    {
                        switch(ProbeMode)
                        {
                            case ProbeModes.MoveStart:
                                if (Probes.SelectedProbe != null)
                                {
                                    ProbeToMove = Probes.SelectedProbe;
                                    ProbeMode = ProbeModes.MoveEnd;
                                    PF.SBStatus.Text = LocalizedStrings.ProbeMoveEndHelp;
                                }
                                break;
                            case ProbeModes.MoveEnd:
                                ProbeToMove.Move(e.X, e.Y);
                                ProbeMode = ProbeModes.MoveStart;
                                PF.SBStatus.Text = LocalizedStrings.ProbeMoveStartHelp;
                                break;
                        }
                    }
                };

            GPUDownloadThread = new Thread(GPUDownload);
            GPUDownloadThread.Name = "GPUDownloadThread";
            GPUDownloadARE = new AutoResetEvent(false);
            GPUDownloadOn = true;
            GPUDownloadThread.Start();
        }

        void XPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (ProbesOn)
            {
                int minlen = ProbeRadius2, len, dx, dy, x = e.Location.X, y = e.Location.Y;
                Probe sel = null;
                foreach (Probe P in Probes.Probes)
                {
                    dx = P.U - x;
                    dy = P.V - y;
                    len = dx * dx + dy * dy;
                    if (len < minlen)
                    {
                        sel = P;
                        minlen = len;
                    }
                }
                if (sel != null)
                    sel.Selected = true;
                else
                {
                    Probes.ClearSelection();
                }
            }
        }

        void TSBRemoveProbe_Click(object sender, EventArgs e)
        {
            if (ProbeMode != ProbeModes.Remove)
                SetProbeMode(ProbeModes.Remove);
            else
                SetProbeMode(ProbeModes.None);
        }

        void TSBMoveProbe_Click(object sender, EventArgs e)
        {
            if (ProbeMode != ProbeModes.MoveStart)
                SetProbeMode(ProbeModes.MoveStart);
            else
                SetProbeMode(ProbeModes.None);
        }

        void TSBAddProbe_Click(object sender, EventArgs e)
        {
            if (ProbeMode != ProbeModes.Add)
                SetProbeMode(ProbeModes.Add);
            else
                SetProbeMode(ProbeModes.None);
        }

        private void SetProbeMode(ProbeModes mode)
        {
            PF.TSBAddProbe.CheckState = PF.TSBMoveProbe.CheckState = PF.TSBRemoveProbe.CheckState = CheckState.Unchecked;
            switch (ProbeMode)
            {
                case ProbeModes.MoveStart:
                case ProbeModes.MoveEnd:
                    ProbeToMove = null;
                    break;
            }
            ProbeMode = mode;
            switch (ProbeMode)
            {
                case ProbeModes.Add:
                    PF.TSBAddProbe.CheckState = CheckState.Checked;
                    PF.SBStatus.Text = LocalizedStrings.ProbeAddHelp;
                    break;
                case ProbeModes.MoveStart:
                    PF.TSBMoveProbe.CheckState = CheckState.Checked;
                    PF.SBStatus.Text = LocalizedStrings.ProbeMoveStartHelp;
                    break;
                case ProbeModes.Remove:
                    PF.TSBRemoveProbe.CheckState = CheckState.Checked;
                    PF.SBStatus.Text = LocalizedStrings.ProbeRemoveHelp;
                    break;
                case ProbeModes.None:
                    PF.SBStatus.Text = LocalizedStrings.ProbeHelp;
                    break;
            }
        }

        private void XInitProbes()
        {
            Probes.SetDevice(XDevice, XSprite);
        }

        private void PipelineInitProbes()
        {
            GPUDownloadCache = new RenderTarget2D(XDevice, XPanel.Width, XPanel.Height, false, SurfaceFormat.Vector4, DepthFormat.None);
        }

        private void PipelineShutdownProbes()
        {
            if (GPUDownloadCache != null)
            {
                GPUDownloadCache.Dispose();
                GPUDownloadCache = null;
            }
        }

        private void ShutdownProbes()
        {
            GPUDownloadOn = false;
            GPUDownloadARE.Set();
            GPUDownloadThread.Join();
        }

        private void ProcessProbes(Texture2D depthTexture)
        {
            if (ProbesOn)
            {
                if (DownloadComplete)
                {
                    DownloadComplete = false;
                    Probes.Refresh();
                }
                if (FrameID % ProbeInterval == 0 && !ProbeDownloadInProgress)
                {
                    XDevice.SetRenderTarget(GPUDownloadCache);
                    XDevice.Clear(ClearOptions.Target, Microsoft.Xna.Framework.Color.Black, 0f, 0);
                    XDevice.DepthStencilState = DepthStencilState.Default;
                    XDevice.RasterizerState = RasterizerState.CullNone;
                    XEffect.CurrentTechnique = ReprojectionOutputTechnique;
                    EFDepthTexture.SetValue(depthTexture);
                    XEffect.CurrentTechnique.Passes[0].Apply();
                    Plane.Draw();
                    XDevice.SetRenderTarget(null);
                    GPUDownloadARE.Set();
                }
            }
        }

        void ProbeMouseUp(object sender, MouseEventArgs e)
        {
            if (ProbesOn)
            {
                switch (ProbeMode)
                {
                    case ProbeModes.Add:
                        if(BaseCursorPos == e.Location)
                        {
                            Probes.AddProbe(e.X, e.Y);
                        }
                        break;
                    case ProbeModes.Remove:
                        if (BaseCursorPos == e.Location && Probes.SelectedProbe!=null)
                        {
                            Probes.RemoveProbe(Probes.SelectedProbe);
                        }
                        break;
                }
                
            }
        }

        private void TSBProbes_Click(object sender, EventArgs e)
        {
            SetProbeState(!ProbesOn);
        }

        private void SetProbeState(bool enabled)
        {
            ProbesOn = enabled;
            TSBProbes.Checked = enabled;
            PF.Visible = enabled;
            if (enabled)
            {
                EFMove.SetValue(Vector2.Zero);
                EFScale.SetValue(1f);
                MoveMode = false;
            }
            else
            {
                EFMove.SetValue(ReprojectionMove);
                EFScale.SetValue(ReprojectionScale);
            }
        }

        bool ProbeDownloadInProgress = false;

        private void GPUDownload()
        {
            while (GPUDownloadOn)
            {
                GPUDownloadARE.WaitOne();
                if (!GPUDownloadOn) break;
                ProbeDownloadInProgress = true;
                Probes.DownloadData(GPUDownloadCache);
                ProbeDownloadInProgress = false;
                DownloadComplete = true;
            }
        }

        private void ReprojectProbes(Matrix m, Vector2 scale)
        {
            Vector4 v;
            float zLimit=SF.NBZLimit.Value;
            float width=XPanel.Width,height=XPanel.Height;

            foreach (Probe P in Probes.Probes)
            {
                if (!P.Initalized) continue;
                v = new Vector4(P.X0, P.Y0, P.Z0, 1f);
                v = Vector4.Transform(v, Matrix.Transpose( m));
                v.X = (v.X / v.Z / 1280 * 2 - 1) * scale.X;
                v.Y = (v.Y / v.Z / 1024 * 2 - 1) * scale.Y;
                v.X = (v.X + 1f)/2f * width;
                v.Y = (v.Y + 1f)/2f * height;
                P.Transform((int)Math.Round(v.X), (int)Math.Round(v.Y));
            }
        }
    }

    class ProbeCollection
    {
        public List<Probe> Probes;
        ListView LV;
        GraphicsDevice XDevice;
        SpriteBatch XSprite;
        public Probe SelectedProbe { get; private set; }

        public ProbeCollection(ListView listView)
        {
            Probes = new List<Probe>();
            LV = listView;
            LV.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(LV_ItemSelectionChanged);
            SelectedProbe = null;
        }

        void LV_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                Probe P = e.Item.Tag as Probe;
                if (!P.Selected) P.Selected = true;
            }
        }

        public void AddProbe(int x, int y)
        {
            Probe P = new Probe(x, y);
            P.SetDevice(XDevice, XSprite);
            P.Select += (object o, EventArgs e) =>
                {
                    Probe p = o as Probe;
                    if (SelectedProbe != null && SelectedProbe != P)
                    {
                        SelectedProbe.Selected = false;
                    }
                    SelectedProbe = p;
                };
            LV.Items.Add(P.LVI);
            Probes.Add(P);
        }

        public void RemoveProbe(Probe P)
        {
            Probes.Remove(P);
            P.Dispose();
        }

        public void ClearSelection()
        {
            if (SelectedProbe != null)
            {
                SelectedProbe.Selected = false;
            }
            SelectedProbe = null;
        }

        public void SetDevice(GraphicsDevice device, SpriteBatch sprite)
        {
            XDevice = device;
            XSprite = sprite;
            foreach (Probe P in Probes)
            {
                P.SetDevice(XDevice, XSprite);
            }
        }

        public void Refresh()
        {
            foreach (Probe P in Probes)
            {
                P.Refresh();
            }
        }

        public void Draw()
        {
            XSprite.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            foreach (Probe P in Probes)
            {
                P.Draw();
            }
            XSprite.End();
        }

        public void DownloadData(Texture2D source)
        {
            Microsoft.Xna.Framework.Rectangle rect;
            rect.Width = rect.Height = 1;
            Vector4[] pos = new Vector4[1];
            Probe P;
            int w=source.Width,h=source.Height;
            for (int i = 0; i < Probes.Count; i++)
            {
                P = Probes[i];
                rect.X = P.U;
                rect.Y = P.V;
                if (rect.X > 0 && rect.Y > 0 && rect.X < w && rect.Y < h)
                {
                    source.GetData<Vector4>(0, rect, pos, 0, 1);
                    if (pos[0].Z == 0)
                        P.Update(float.NaN, float.NaN, float.NaN, -1);
                    else
                        P.Update(pos[0].X, pos[0].Y, pos[0].Z, (int)pos[0].W);
                }
                else
                {
                    P.Update(float.NaN, float.NaN, float.NaN, -1);
                }
            }
        }
    }

    class Probe : IDisposable
    {
        private static int NextID = 0;
        public int ID { get; private set; }
        public int U { get; private set; }
        public int V { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public int RawDepth { get; private set; }
        public bool Initalized { get; private set; }
        public float X0 { get; private set; }
        public float Y0 { get; private set; }
        public float Z0 { get; private set; }
        private string[] StringData;
        private bool selected;
        public bool Selected 
        {
            get
            {
                return selected;
            }
            set
            {
                selected = LVI.Selected = value;
                if (value)
                {
                    LVI.BackColor = SystemColors.Highlight;
                    LVI.ForeColor = SystemColors.HighlightText;
                }
                else
                {
                    LVI.BackColor = SystemColors.Window;
                    LVI.ForeColor = SystemColors.WindowText;
                    
                }
                if (value && Select != null)
                {
                    Select(this, null);
                }
            }
        }
        public event EventHandler Select;

        Bitmap BIndicator;
        Texture2D TIndicator;
        GraphicsDevice XDevice;
        SpriteBatch XSprite;
        public ListViewItem LVI { get; private set; }

        public Probe(int x, int y)
        {
            ID = NextID++;
            if (NextID == 100) NextID = 0;
            CreateIndicator(ID);
            LVI = new ListViewItem(ID.ToString());
            LVI.Tag = this;
            Move(x, y);
            Selected = false;
        }

        public void Move(int x, int y)
        {
            U = x;
            V = y;
            X0 = Y0 = Z0 = float.NaN;
            Update(float.NaN, float.NaN, float.NaN, -1);
            Initalized = false;
        }

        public void Transform(int x, int y)
        {
            U = x;
            V = y;
            Update(float.NaN, float.NaN, float.NaN, -1);
        }

        private static string ff = "F4";
        public void Update(float x, float y, float z, int raw)
        {
            if (!Initalized && !(float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(z)))
            {
                X0 = x;
                Y0 = y;
                Z0 = z;
                Initalized = true;
            }
            X = x;
            Y = y;
            Z = z;
            RawDepth = raw;
            StringData = new string[] { ID.ToString(), X.ToString(ff), Y.ToString(ff), Z.ToString(ff), U.ToString(), V.ToString(), RawDepth.ToString() };
        }

        public void Refresh()
        {
            for (int i = 0; i < StringData.Length; i++)
            {
                if (LVI.SubItems.Count < i + 1) LVI.SubItems.Add(new ListViewItem.ListViewSubItem());
                LVI.SubItems[i].Text = StringData[i];
            }
        }

        private void CreateIndicator(int index)
        {
            string text = index.ToString();
            BIndicator = new Bitmap(Resources.probe64);
            using (Graphics G = Graphics.FromImage(BIndicator))
            using (Font F = new Font("Tahoma", 10, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                SizeF textSize = G.MeasureString(text, F);
                G.DrawString(text, F, Brushes.Black, 32 - textSize.Width / 2, 48);
                G.Flush();
            }
        }

        public void SetDevice(GraphicsDevice device, SpriteBatch sprite)
        {
            if (TIndicator != null) TIndicator.Dispose();
            XDevice = device;
            XSprite = sprite;
            if (XDevice != null)
            {
                TIndicator = BIndicator.ToTexture2D(device);
            }
        }

        public void Draw()
        {
            if (Selected)
            {
                XSprite.Draw(TIndicator, new Vector2(U - 32, V - 32), Microsoft.Xna.Framework.Color.Yellow);
            }
            else
            {
                XSprite.Draw(TIndicator, new Vector2(U - 32, V - 32), Microsoft.Xna.Framework.Color.White);
            }
        }

        public void Dispose()
        {
            if (TIndicator != null)
            {
                TIndicator.Dispose();
                TIndicator = null;
            }
            if (BIndicator != null)
            {
                BIndicator.Dispose();
                BIndicator = null;
            }
            LVI.Remove();
        }
    }
}