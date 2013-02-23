using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CustomControls;
using System.ComponentModel;

namespace CustomControls
{

    
    [System.ComponentModel.DefaultEvent("SelectedIndexChanged")]
    public class DropDownButton : Button
    {
        ContextMenuStrip DropDownMenu;
        
        public DropDownButton()
        {
            DropDownMenu = new ContextMenuStrip();
            DropDownMenu.RenderMode = ToolStripRenderMode.System;
            TextAlign = ContentAlignment.MiddleLeft;
            Image = Properties.Resources.dropdown;
            ImageAlign = ContentAlignment.MiddleRight;
            items = new string[0];
            selectedIndex = -1;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            DropDownMenu.Show(this, 0, Height);
        }

        string[] items;
        [Localizable(true)]
        public string[] Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                while (DropDownMenu.Items.Count > 0)
                {
                    DropDownMenu.Items[0].Dispose();
                }
                base.Text = "";
                selectedIndex = -1;
                for (int i = 0; i < items.Length; i++)
                {
                    ToolStripMenuItem MI = new ToolStripMenuItem(items[i]);
                    MI.Tag = i;
                    MI.Click += new EventHandler(MI_Click);
                    DropDownMenu.Items.Add(MI);
                }
                if (value.Length > 0)
                    SelectIndex(0);
                else
                    selectedIndex = -1;
            }
        }

        public event EventHandler SelectedIndexChanged;
        private int selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                SelectIndex(value);
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i] == value)
                    {
                        SelectIndex(i);
                        break;
                    }
                }
            }
        }

        private void SelectIndex(int index)
        {
            if (index < items.Length && index >= 0)
            {
                if (selectedIndex != -1)
                    (DropDownMenu.Items[selectedIndex] as ToolStripMenuItem).Checked = false;
                selectedIndex = index;
                base.Text = items[index];
                (DropDownMenu.Items[index] as ToolStripMenuItem).Checked = true;
                if (SelectedIndexChanged != null) SelectedIndexChanged(this, null);
            }
        }

        void MI_Click(object sender, EventArgs e)
        {
            SelectIndex((int)(sender as ToolStripMenuItem).Tag);
        }
    }

    
}
