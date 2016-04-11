using COM.ZCTT.AGI.Plugin;
using Plugin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace COM.ZCTT.AGI.Help
{
    public partial class About : DockContent, IPlugin
    {
        public About()
        {
            InitializeComponent();
        }
        public new void Load()
        {
            //MessageBox.Show("AirGazer Intelligence(AGI) V0.1", "About");
            About about = new About();
            //
            about.Show();
            //about.ShowDialog();
            //about.TopMost = true;
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0xa1 && (int)m.WParam == 0x3)
            {
                return;
            }
            if (m.Msg == 0xa3 && ((int)m.WParam == 0x3 || (int)m.WParam == 0x2))
            {
                return;
            }
            if (m.Msg == 0xa4 && ((int)m.WParam == 0x2 || (int)m.WParam == 0x3))
            {
                return;
            }
            if (m.Msg == 0x112 && (int)m.WParam == 0xf100)
            {
                return;
            }
            base.WndProc(ref m);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
