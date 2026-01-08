using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace Banking_Management_System
{
    public partial class Transfer : Form
    {
        MySqlConnection conn;

        public Transfer()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TransferAmount.Text))
            {
                MessageBox.Show("Please input an amount", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string TransferAmountToReturn = TransferAmount.Text.Trim();
            int amountInt = int.Parse(TransferAmountToReturn);
            if (amountInt == 0)
            {
                MessageBox.Show("The amount must be greater than zero", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }

            DateTime selectedDate = DP3.Value;
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

            if (string.IsNullOrWhiteSpace(Receiver.Text))
            {
                MessageBox.Show("Please enter the receiver's account number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }

            string accountNumber = Login.getAccNum;
            string transactionType = "Transfer";
            string receiverAccountNumber = Receiver.Text;
            string reason = (string)Rason.SelectedItem;
            int senderBalance;
            
            try
            {
                using (conn = new MySqlConnection("server=127.0.0.1;user=root;database=bms;password="))
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

                    query = "SELECT Amount FROM accounts WHERE AccNum = @AccountNumber";
                    using (var checkSenderPstmt = new MySqlCommand(query, conn))
                    {
                        checkSenderPstmt.Parameters.AddWithValue("@AccountNumber", accountNumber);
                        using (var senderResult = checkSenderPstmt.ExecuteReader())
                        {
                            if (senderResult.Read())
                            {
                                senderBalance = senderResult.GetInt32("Amount");

                                if (amountInt > senderBalance)
                                {
                                    MessageBox.Show("Insufficient balance to transfer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                } 
                            }
                            else
                            {
                                MessageBox.Show("Sender account not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                           
                        }
                        
                    }

                    query = "UPDATE accounts SET Amount = Amount - @Amount WHERE AccNum = @AccountNumber";
                    using (var updateSenderBalancePstmt = new MySqlCommand(query, conn))
                    {
                        updateSenderBalancePstmt.Parameters.AddWithValue("@Amount", TransferAmountToReturn);
                        updateSenderBalancePstmt.Parameters.AddWithValue("@AccountNumber", accountNumber);
                        updateSenderBalancePstmt.ExecuteNonQuery();
                    }

                    query = "UPDATE accounts SET Amount = Amount + @Amount WHERE AccNum = @AccountNumber";
                    using (var updateReceiverBalancePstmt = new MySqlCommand(query, conn))
                    {
                        updateReceiverBalancePstmt.Parameters.AddWithValue("@Amount", TransferAmountToReturn);
                        updateReceiverBalancePstmt.Parameters.AddWithValue("@AccountNumber", receiverAccountNumber);
                        updateReceiverBalancePstmt.ExecuteNonQuery();
                    }

                    query = "INSERT INTO transferhistory (Sender, AccNum, Amount, Date, Time) VALUES (@Sender, @AccNum, @Amount, @Date, @Time)";
                    using (var updateTransferHistoryPstmt = new MySqlCommand(query, conn))
                    {
                        updateTransferHistoryPstmt.Parameters.AddWithValue("@Sender", accountNumber);
                        updateTransferHistoryPstmt.Parameters.AddWithValue("@AccNum", receiverAccountNumber);
                        updateTransferHistoryPstmt.Parameters.AddWithValue("@Amount", TransferAmountToReturn);
                        updateTransferHistoryPstmt.Parameters.AddWithValue("@Date", DateTime.Now.Date);
                        updateTransferHistoryPstmt.Parameters.AddWithValue("@Time", DateTime.Now.TimeOfDay);
                        updateTransferHistoryPstmt.ExecuteNonQuery();
                    }

                    string insertQuery = "INSERT INTO records (AccNum, RunningBalance, Amount, Date, Time, Type, Reason) VALUES (@AccNum, @RunningBalance, @Amount, @Date, @Time, @Type, @Reason)";
                    using (MySqlCommand insertRecordCmd = new MySqlCommand(insertQuery, conn))
                    {
                        insertRecordCmd.Parameters.AddWithValue("@AccNum", accountNumber);
                        insertRecordCmd.Parameters.AddWithValue("@RunningBalance", senderBalance);
                        insertRecordCmd.Parameters.AddWithValue("@Amount", amountInt);
                        insertRecordCmd.Parameters.AddWithValue("@Date", DateTime.Now.Date);
                        insertRecordCmd.Parameters.AddWithValue("@Time", DateTime.Now.TimeOfDay);
                        insertRecordCmd.Parameters.AddWithValue("@Type", transactionType);
                        insertRecordCmd.Parameters.AddWithValue("@Reason", reason);
                        insertRecordCmd.ExecuteNonQuery();
                    }

                    DialogResult option = MessageBox.Show("Withdraw Successfully.\n\nDo you want to print the receipt?", "Print Receipt", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (option == DialogResult.Yes)
                    {
                        Receipt receiptFrame = new Receipt(accountNumber, "Transfer", TransferAmountToReturn + "," + receiverAccountNumber);
                        receiptFrame.Show();
                    }
                    TransferAmount.Text = "";
                    Receiver.Text = "";
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void TransferAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            string input = TransferAmount.Text + e.KeyChar;
            foreach (char c in input)
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
                {
                    e.Handled = true;
                    Error2.Text = "Must be digits only!";
                    return;
                }
            }
            Error2.Text = "";
        }
    }
}



