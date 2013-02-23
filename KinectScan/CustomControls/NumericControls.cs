using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CustomControls
{
    public partial class DoubleBox : TextBox
    {
        public DoubleBox()
        {
            TextAlign = HorizontalAlignment.Right;
            OnTextChanged(null);   
        }
        double d;
        public double Value
        {
            get
            {
                return d;
            }
            set
            {
                d = value;
                Text = d.ToString();
            }
        }
        protected override void OnTextChanged(EventArgs e)
        {            
            double dt;
            if (double.TryParse(Text, out dt))
            {
                BackColor = SystemColors.Window;
                d = dt;
                base.OnTextChanged(e);
            }
            else
            {
                BackColor = Color.Red;
            }
        }
    }

    public partial class IntegerBox : TextBox
    {
        public IntegerBox()
        {
            TextAlign = HorizontalAlignment.Right;
            OnTextChanged(null);
        }
        int i;
        public int Value
        {
            get
            {
                return i;
            }
            set
            {
                i = value;
                Text = i.ToString();
            }
        }
        protected override void OnTextChanged(EventArgs e)
        {
            int it;
            if (int.TryParse(Text, out it))
            {
                BackColor = SystemColors.Window;
                i = it;
                base.OnTextChanged(e);
            }
            else
            {
                BackColor = Color.Red;
            }
        }
    }

    public class SuperLabel : Label
    {
        public SuperLabel()
        {
            MaximumSize = new Size(1000, 15);
            MinimumSize = new Size(2, 15);
            BorderStyle = BorderStyle.FixedSingle;
            TextAlign = ContentAlignment.MiddleRight;
            Height = 15;
            Decimals = 2;
            DoubleValue = 0;
        }

        private double d = 0;
        public double DoubleValue
        {
            get
            {
                return d;
            }
            set
            {
                d = value;
                Text = d.ToString(doubleFormat);
            }
        }

        string doubleFormat;
        private int decimals;
        public int Decimals
        {
            get
            {
                return decimals;
            }
            set
            {
                decimals = value;
                doubleFormat = "F" + decimals.ToString();
                Text = d.ToString(doubleFormat);
            }
        }

        private int i;
        public int IntValue
        {
            get
            {
                return i;
            }
            set
            {
                i = value;
                Text = i.ToString();
            }
        }

        private DateTime dateTime;
        public DateTime DateValue
        {
            get
            {
                return dateTime;
            }
            set
            {
                dateTime = value;
                Text = dateTime.ToString("hh:mm:ss.fff");
            }
        }
    }
}
