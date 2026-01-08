using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;


namespace Banking_Management_System
{
    public partial class Deposit : Form
    {
        public Deposit()
        {
            InitializeComponent();
        }

        private static bool depositbutton = false;
        private void button1_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(Amount.Text))
            {
                MessageBox.Show("Please input an amount", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }

            string amount = Amount.Text.Trim();
            int amountInt = int.Parse(amount);
            if (amountInt == 0)
            {
                MessageBox.Show("The amount must be greater than zero", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime selectedDate = DP1.Value;
            DateTime currentDateTime = DateTime.Now;
            if (selectedDate.Date != currentDateTime.Date)
            {
                MessageBox.Show("Please select today's date.", "Invalid Date", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            depositbutton = true;
            Login login = new Login();
            string accountNumber = Login.getAccNum;
            string transactionType = "Deposit";
            string selected = Rason.SelectedItem.ToString();

            try
            {
                using (var connection = new MySqlConnection("server=127.0.0.1;user=root;database=bms;password="))
                {
                    connection.Open();

                    string query = "SELECT Amount FROM accounts WHERE AccNum = @AccNum";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@AccNum", accountNumber);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show("Account not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        int currentBalance = reader.GetInt32("Amount");
                        reader.Close();

                        int newBalance = currentBalance + amountInt;

                        query = "UPDATE accounts SET Amount = @NewBalance WHERE AccNum = @AccNum";
                        command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@NewBalance", newBalance);
                        command.Parameters.AddWithValue("@AccNum", accountNumber);
                        command.ExecuteNonQuery();

                        query = "INSERT INTO records (AccNum, RunningBalance, Amount, Date, Time, Type, Reason) VALUES (@AccNum, @RunningBalance, @Amount, @Date, @Time, @Type, @Reason)";
                        command = new MySqlCommand(query, connection);
                        command.Parameters.AddWithValue("@AccNum", accountNumber);
                        command.Parameters.AddWithValue("@RunningBalance", currentBalance);
                        command.Parameters.AddWithValue("@Amount", amountInt);
                        command.Parameters.AddWithValue("@Date", currentDateTime.Date);
                        command.Parameters.AddWithValue("@Time", currentDateTime.TimeOfDay);
                        command.Parameters.AddWithValue("@Type", transactionType);
                        command.Parameters.AddWithValue("@Reason", selected);
                        command.ExecuteNonQuery();
                    }
                }

                DialogResult option = MessageBox.Show("Deposit Successfully.\n\nDo you want to print the receipt?", "Print Receipt", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (option == DialogResult.Yes)
                {
                    Receipt receiptFrame = new Receipt(accountNumber, "Deposit", amount);
                    receiptFrame.Show();
                }
                Amount.Text = "";
                Rason.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Amount_KeyPress(object sender, KeyPressEventArgs e)
        {
            string input = Amount.Text + e.KeyChar;
            foreach (char c in input)
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)&& e.KeyChar != ' ')
                {
                    e.Handled = true;
                    Error1.Text = "Must be digits only!";
                    return;
                }
            }
            Error1.Text = "";
        }

        
    }
    }

