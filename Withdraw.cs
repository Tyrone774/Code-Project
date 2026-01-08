using MySql.Data.MySqlClient;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Banking_Management_System
{
    public partial class Withdraw : Form
    {

        public Withdraw()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(WithdrawAmount.Text))
            {
                MessageBox.Show("Please input an amount", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string amount = WithdrawAmount.Text.Trim();
            int amountInt = int.Parse(amount);
            if (amountInt == 0)
            {
                MessageBox.Show("The amount must be greater than zero", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime selectedDate = DP2.Value;
            DateTime currentDateTime = DateTime.Now;
            if (selectedDate.Date != currentDateTime.Date)
            {
                MessageBox.Show("Please select today's date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Rason.SelectedItem == null)
            {
                MessageBox.Show("Please select a reason", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Login login = new Login();
            string accountNumber = Login.getAccNum;
            string selected = Rason.SelectedItem.ToString();

            try
            {
                using (MySqlConnection conn = new MySqlConnection("server=127.0.0.1;user=root;database=bms;password="))
                {
                    conn.Open();

                    MySqlCommand checkAccountCmd = new MySqlCommand("SELECT Amount FROM accounts WHERE AccNum = @AccNum", conn);
                    checkAccountCmd.Parameters.AddWithValue("@AccNum", accountNumber);
                    MySqlDataReader accountResult = checkAccountCmd.ExecuteReader();

                    if (!accountResult.Read())
                    {
                        MessageBox.Show("Account not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int currentBalance = (int)accountResult["Amount"];
                    accountResult.Close();

                    if (amountInt > currentBalance)
                    {
                        MessageBox.Show("Insufficient balance to withdraw", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    int newBalance = currentBalance - amountInt;

                    MySqlCommand updateBalanceCmd = new MySqlCommand("UPDATE accounts SET Amount = @NewBalance WHERE AccNum = @AccNum", conn);
                    updateBalanceCmd.Parameters.AddWithValue("@NewBalance", newBalance);
                    updateBalanceCmd.Parameters.AddWithValue("@AccNum", accountNumber);
                    updateBalanceCmd.ExecuteNonQuery();

                    MySqlCommand insertRecordCmd = new MySqlCommand("INSERT INTO records (AccNum, RunningBalance, Amount, Date, Time, Type, Reason) VALUES (@AccNum, @RunningBalance, @Amount, @Date, @Time, @Type, @Reason)", conn);
                    insertRecordCmd.Parameters.AddWithValue("@AccNum", accountNumber);
                    insertRecordCmd.Parameters.AddWithValue("@RunningBalance", currentBalance);
                    insertRecordCmd.Parameters.AddWithValue("@Amount", amountInt);
                    insertRecordCmd.Parameters.AddWithValue("@Date", currentDateTime.Date);
                    insertRecordCmd.Parameters.AddWithValue("@Time", currentDateTime.TimeOfDay);
                    insertRecordCmd.Parameters.AddWithValue("@Type", "Withdraw");
                    insertRecordCmd.Parameters.AddWithValue("@Reason",selected);
                    insertRecordCmd.ExecuteNonQuery();
                }

                DialogResult option = MessageBox.Show("Withdraw Successfully.\n\nDo you want to print the receipt?", "Print Receipt", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (option == DialogResult.Yes)
                {
                    Receipt receiptFrame = new Receipt(accountNumber, "Withdraw", amount);
                    receiptFrame.Show();
                }
                WithdrawAmount.Text = "";
                Rason.SelectedIndex = -1;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void WithdrawAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            string input = WithdrawAmount.Text + e.KeyChar;
            foreach (char c in input)
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
                {
                    e.Handled = true;
                    ErrorWithdraw.Text = "Must be digits only!";
                    return;
                }
            }
            ErrorWithdraw.Text = "";
        }
    }

    }

        
