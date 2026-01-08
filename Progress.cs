using System;
using System.Windows.Forms;

namespace Banking_Management_System
{
    public partial class Progress : Form
    {
        public Progress()
        {
            InitializeComponent();
        }


        private void Progress_Load_1(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {  
            bar.PerformStep();

            if (bar.Value >= bar.Maximum)
            {
                timer1.Stop();
                this.Dispose();

                Menu menuForm = new Menu();
                menuForm.Show();
            }
        }

    }
}
