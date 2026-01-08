using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Banking_Management_System
{
    public partial class DeleteAccount : Form
    {

        public DeleteAccount()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string enteredPin = EnteredPIN.Text;
            string accountNumber = Login.getAccNum;

            try
            {
                string connectionString = "Server=localhost;Database=bms;User Id=root;Password=;";
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();

                    string selectQuery = "SELECT PINPass FROM accounts WHERE AccNum = @AccNum";
                    using (MySqlCommand selectCmd = new MySqlCommand(selectQuery, con))
                    {
                        selectCmd.Parameters.AddWithValue("@AccNum", accountNumber);

                        using (MySqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string pinFromDatabase = reader["PINPass"].ToString();

                                if (enteredPin == pinFromDatabase)
                                {
                                    reader.Close();

                                    string selectBalanceQuery = "SELECT Amount FROM accounts WHERE AccNum = @AccNum";
                                    using (MySqlCommand selectBalanceCmd = new MySqlCommand(selectBalanceQuery, con))
                                    {
                                        selectBalanceCmd.Parameters.AddWithValue("@AccNum", accountNumber);

                                        using (MySqlDataReader balanceReader = selectBalanceCmd.ExecuteReader())
                                        {
                                            if (balanceReader.Read())
                                            {
                                                double balance = Convert.ToDouble(balanceReader["Amount"]);

                                                if (balance > 0)
                                                {
                                                    DeleteOption option = new DeleteOption();
                                                    option.MdiParent = this.MdiParent;                        
                                                    option.Show();
                                                    EnteredPIN.Text = "";
                                                }
                                                else
                                                {
                                                    DialogResult confirm = MessageBox.Show("Are you sure you want to delete?", "Confirm Deletion", MessageBoxButtons.YesNo);

                                                    if (confirm == DialogResult.Yes)
                                                    {
                                                        balanceReader.Close(); // Close reader before executing another query

                                                        using (MySqlCommand deleteCmd = new MySqlCommand())
                                                        {
                                                            deleteCmd.Connection = con;
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
                                                        
                                                        DialogResult result = MessageBox.Show("Thank you for trusting our bank", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                        if (result == DialogResult.OK)
                                                        {
                                                 
                                                            foreach (Form form in System.Windows.Forms.Application.OpenForms.Cast<Form>().ToArray())
    {
                                                                form.Hide();
                                                            }

                                                            Login loginForm = new Login();
                                                            loginForm.StartPosition = FormStartPosition.CenterScreen; // Center the Login form
                                                            loginForm.Show();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Account balance not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Incorrect PIN. Deletion aborted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Account not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error accessing database: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnteredPIN_KeyPress(object sender, KeyPressEventArgs e)
        {
            string input = EnteredPIN.Text + e.KeyChar;
            foreach (char c in input)
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
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

