using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Banking_Management_System
{
    public partial class DeleteOption : Form
    {
        public DeleteOption()
        {
            InitializeComponent();
        }

        private void DeleteOption_Load(object sender, EventArgs e)
        {
            FetchAccBalance();
        }
        private void FetchAccBalance()
        {
            try
            {
                string accountNumber = Login.getAccNum;

                string query = "SELECT Amount FROM accounts WHERE AccNum = @AccNum";

                using (MySqlConnection connection = new MySqlConnection("server=127.0.0.1;user=root;database=bms;password="))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AccNum", accountNumber);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int balance = Convert.ToInt32(reader["Amount"]);
                                DisplayAmount.Text = balance.ToString(); 
                            }
                            else
                            {
                                DisplayAmount.Text = "0"; 
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error fetching account balance: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
        private void WithdrawOption_Click(object sender, EventArgs e)
        {
            string accountNumber = Login.getAccNum;
            string remaining = DisplayAmount.Text;
            int amountInt = int.Parse(remaining);
            int currentBalance;

            try
            {
                using (MySqlConnection connect = new MySqlConnection("server=127.0.0.1;user=root;database=bms;password="))
                {
                    connect.Open();

                    MySqlCommand checkAccountCmd = new MySqlCommand("SELECT Amount FROM accounts WHERE AccNum = @AccNum", connect);
                    checkAccountCmd.Parameters.AddWithValue("@AccNum", accountNumber);
                    MySqlDataReader accountResult = checkAccountCmd.ExecuteReader();

                    if (!accountResult.Read())
                    {
                        MessageBox.Show("Account not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    currentBalance = (int)accountResult["Amount"];
                    accountResult.Close();

                    int newBalance = currentBalance - amountInt;

                    MySqlCommand updateBalanceCmd = new MySqlCommand("UPDATE accounts SET Amount = @NewBalance WHERE AccNum = @AccNum", connect);
                    updateBalanceCmd.Parameters.AddWithValue("@NewBalance", newBalance);
                    updateBalanceCmd.Parameters.AddWithValue("@AccNum", accountNumber);
                    updateBalanceCmd.ExecuteNonQuery();
                }

                Progress2 progressForm = new Progress2();
                progressForm.ShowDialog();
                progressForm.Dispose();

                MessageBox.Show("Withdraw Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult confirm = MessageBox.Show("Are you sure you want to delete your account?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    using (MySqlConnection connect = new MySqlConnection("server=127.0.0.1;user=root;database=bms;password="))
                    {
                        connect.Open();

                        using (MySqlCommand deleteCmd = new MySqlCommand())
                        {
                            deleteCmd.Connection = connect;
                            deleteCmd.CommandText = "DELETE FROM accounts WHERE AccNum = @AccNum";
                            deleteCmd.Parameters.AddWithValue("@AccNum", accountNumber);
                            deleteCmd.ExecuteNonQuery();

                            deleteCmd.CommandText = "DELETE FROM records WHERE AccNum = @AccNum";
                            deleteCmd.ExecuteNonQuery();

                            deleteCmd.CommandText = "DELETE FROM transferhistory WHERE Sender = @Sender";
                            deleteCmd.Parameters.AddWithValue("@Sender", accountNumber);
                            deleteCmd.ExecuteNonQuery();

                        }
                    }

                    MessageBox.Show("Thank you for trusting our bank", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    foreach (Form form in Application.OpenForms.Cast<Form>().ToArray())
                    {
                        form.Hide();
                    }

                    Login loginForm = new Login();
                    loginForm.StartPosition = FormStartPosition.CenterScreen;
                    loginForm.Show();
                }
                else
                {
                    foreach (Form form in Application.OpenForms.Cast<Form>().ToArray())
                    {
                        form.Hide();
                    }

                    Menu menuForm = new Menu();
                    menuForm.StartPosition = FormStartPosition.CenterScreen;
                    menuForm.Show();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        bool isTransferClicked = false;
        private void TransferOption_Click(object sender, EventArgs e)
        {
            if (!isTransferClicked)
            {
                RecLabel.Visible = true;
                RecBox.Visible = true;
                RecButton.Visible = true;
                isTransferClicked = true;
            }
            else
            {
                RecLabel.Visible = false;
                RecBox.Visible = false;
                RecButton.Visible = false;
                isTransferClicked = false;
            }

        }

        private void RecButton_Click(object sender, EventArgs e)
        {
                string accountNumber = Login.getAccNum;
                string receiverAccountNumber = RecBox.Text;

                try
                {
                    using (MySqlConnection conn = new MySqlConnection("server=127.0.0.1;user=root;database=bms;password="))
                    {
                        conn.Open();

                        string query = "SELECT AccNum FROM accounts WHERE AccNum = @AccountNumber";
                        using (var checkReceiverPstmt = new MySqlCommand(query, conn))
                        {
                            checkReceiverPstmt.Parameters.AddWithValue("@AccountNumber", receiverAccountNumber);
                            using (var receiverResult = checkReceiverPstmt.ExecuteReader())
                            {
                                if (!receiverResult.Read())
                                {
                                    MessageBox.Show("Receiver account not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }

                        double senderBalance = 0;
                        query = "SELECT Amount FROM accounts WHERE AccNum = @AccountNumber";
                        using (var getSenderBalancePstmt = new MySqlCommand(query, conn))
                        {
                            getSenderBalancePstmt.Parameters.AddWithValue("@AccountNumber", accountNumber);
                            using (var senderResult = getSenderBalancePstmt.ExecuteReader())
                            {
                                if (senderResult.Read())
                                {
                                    senderBalance = senderResult.GetDouble("Amount");
                                }
                            }
                        }

                        query = "UPDATE accounts SET Amount = Amount + @SenderBalance WHERE AccNum = @ReceiverAccountNumber";
                        using (var updateReceiverBalancePstmt = new MySqlCommand(query, conn))
                        {
                            updateReceiverBalancePstmt.Parameters.AddWithValue("@SenderBalance", senderBalance);
                            updateReceiverBalancePstmt.Parameters.AddWithValue("@ReceiverAccountNumber", receiverAccountNumber);
                            updateReceiverBalancePstmt.ExecuteNonQuery();
                        }

                        query = "UPDATE accounts SET Amount = 0 WHERE AccNum = @AccountNumber";
                        using (var updateSenderBalancePstmt = new MySqlCommand(query, conn))
                        {
                            updateSenderBalancePstmt.Parameters.AddWithValue("@AccountNumber", accountNumber);
                            updateSenderBalancePstmt.ExecuteNonQuery();
                        }

                        Progress2 progressForm = new Progress2();
                        progressForm.ShowDialog();
                        progressForm.Dispose();

                        MessageBox.Show("Transfer Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        DialogResult confirm = MessageBox.Show("Are you sure you want to delete your account?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (confirm == DialogResult.Yes)
                        {
                            using (MySqlCommand deleteCmd = new MySqlCommand())
                            {
                                deleteCmd.Connection = conn;

                                deleteCmd.CommandText = "DELETE FROM accounts WHERE AccNum = @AccNum";
                                deleteCmd.Parameters.AddWithValue("@AccNum", accountNumber);
                                deleteCmd.ExecuteNonQuery();
                                deleteCmd.Parameters.Clear(); 

                                
                                deleteCmd.CommandText = "DELETE FROM records WHERE AccNum = @AccNum";
                                deleteCmd.Parameters.AddWithValue("@AccNum", accountNumber);
                                deleteCmd.ExecuteNonQuery();
                                deleteCmd.Parameters.Clear(); 

                               
                                deleteCmd.CommandText = "DELETE FROM transferhistory WHERE Sender = @Sender";
                                deleteCmd.Parameters.AddWithValue("@Sender", accountNumber);
                                deleteCmd.ExecuteNonQuery();
                            }

                            MessageBox.Show("Thank you for trusting our bank", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            
                            foreach (Form form in Application.OpenForms.Cast<Form>().ToArray())
                            {
                                form.Hide();
                            }

                            Login loginForm = new Login();
                            loginForm.StartPosition = FormStartPosition.CenterScreen;
                            loginForm.Show();
                        }
                        else
                        {
                          
                            foreach (Form form in Application.OpenForms.Cast<Form>().ToArray())
                            {
                                form.Hide();
                            }

                            Menu menuForm = new Menu();
                            menuForm.StartPosition = FormStartPosition.CenterScreen;
                            menuForm.Show();
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
    }

