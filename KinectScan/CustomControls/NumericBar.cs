using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace CustomControls
{
    [System.ComponentModel.DefaultEvent("ValueChanged")]
    public partial class NumericBar : UserControl
    {
        class NumericBarTrackBar : TrackBar
        {
            protected override void WndProc(ref Message m)
            {

                if (m.Msg == 0x20A)
                {
                    (Parent as NumericBar).PassMessage(Message.Create(Parent.Handle, m.Msg, m.WParam, m.LParam));
                }
                else
                {
                    base.WndProc(ref m);
                }
            }
        }

        class NumericBarNumericUpDown : NumericUpDown
        {
            protected override void WndProc(ref Message m)
            {

                if (m.Msg == 0x20A)
                {
                    (Parent as NumericBar).PassMessage(Message.Create(Parent.Handle, m.Msg, m.WParam, m.LParam));
                }
                else
                {
                    base.WndProc(ref m);
                }
            }
        }
        public void PassMessage(Message m)
        {
            WndProc(ref m);
        }
        public NumericBar()
        {
            InitializeComponent();
            ValueChangeEventEnabled = true;
        }

        [Localizable(true)]
        public string Title
        {
            get
            {
                return LTitle.Text;
            }
            set
            {
                LTitle.Text = value;
            }
        }

        public double Maximum
        {
            get
            {
                return (double)NUD.Maximum;
            }
            set
            {
                NUD.Maximum = (decimal)value;
                NUD_ValueChanged(this, null);
            }
        }

        public double Minimum
        {
            get
            {
                return (double)NUD.Minimum;
            }
            set
            {
                NUD.Minimum = (decimal)value;
                NUD_ValueChanged(this, null);
            }
        }

        public float Value
        {
            get
            {
                return (float)NUD.Value;
            }
            set
            {
                if (value < Minimum)
                {
                    NUD.Value = (decimal)Minimum;
                    return;
                }
                if (value > Maximum)
                {
                    NUD.Value = (decimal)Maximum;
                    return;
                }
                NUD.Value = (decimal)value;
            }
        }

        public double DoubleValue
        {
            get
            {
                return (double)NUD.Value;
            }
            set
            {
                if (value < Minimum)
                {
                    NUD.Value = (decimal)Minimum;
                    return;
                }
                if (value > Maximum)
                {
                    NUD.Value = (decimal)Maximum;
                    return;
                }
                NUD.Value = (decimal)value;
            }
        }

        public byte ByteValue
        {
            get
            {
                return (byte)NUD.Value;
            }
            set
            {
                NUD.Value = value;
            }
        }

        public int Int32Value
        {
            get
            {
                return (int)NUD.Value;
            }
            set
            {
                NUD.Value = value;
            }
        }

        public bool ValueChangeEventEnabled { get; set; }
        private void NUD_ValueChanged(object sender, EventArgs e)
        {
            TB.ValueChanged -= TB_ValueChanged;
            TB.Value = (int)(((double)NUD.Value - Minimum) / (Maximum - Minimum) * 100);
            TB.ValueChanged += TB_ValueChanged;
            if (ValueChanged != null && ValueChangeEventEnabled) ValueChanged(this, null);
        }

        private void TB_ValueChanged(object sender, EventArgs e)
        {
            NUD.ValueChanged -= NUD_ValueChanged;
            NUD.Value = (decimal)(TB.Value / 100d * (Maximum - Minimum) + Minimum);
            NUD.ValueChanged += NUD_ValueChanged;
            if (ValueChanged != null && ValueChangeEventEnabled) ValueChanged(this, null);
        }

        public event EventHandler ValueChanged;

        private bool integerValue;
        public bool IntegerValue
        {
            get
            {
                return integerValue;
            }
            set
            {
                integerValue = value;
                if (value)
                {
                    NUD.DecimalPlaces = 0;
                }
                else
                {
                    NUD.DecimalPlaces = decimalPlaces;
                }
            }
        }

        public decimal Increment
        {
            get
            {
                return NUD.Increment;
            }
            set
            {
                NUD.Increment = value;
            }
        }

        int decimalPlaces = 2;
        public int DecimalPlaces
        {
            get
            {
                return decimalPlaces;
            }
            set
            {
                decimalPlaces = value;
                if (!integerValue) NUD.DecimalPlaces = decimalPlaces;
            }
        }
    }
}
