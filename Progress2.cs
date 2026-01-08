using System;
using System.Windows.Forms;

namespace Banking_Management_System
{
    public partial class Progress2 : Form
    {
        public Progress2()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bar2.PerformStep();
            if (bar2.Value >= bar2.Maximum)
            {
                timer1.Stop();
                this.Dispose();

                Menu menuForm = new Menu();
                menuForm.Show();
            }
        }

        private void Progress2_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
