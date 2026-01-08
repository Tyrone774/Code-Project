using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Banking_Management_System
{
    public partial class AccountDetails : Form
    {
        public AccountDetails()
        {
            InitializeComponent();
        }
        private void AccountDetails_Load(object sender, EventArgs e)
        {
            FetchName();
            FetchAddress();
            FetchAccNumber();
            FetchAccType();
            FetchAccBalance();
            FetchPINPass();
            FetchBirthday();
        }

        private void FetchName()
        {
            string accountNumber = Login.getAccNum;
            try
            {
                string query = "SELECT FirstName, LastName FROM accounts WHERE AccNum = @AccNum";

                string connectionString = "server=127.0.0.1;user=root;database=bms;password=";

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AccNum", accountNumber);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string firstName = reader["FirstName"].ToString();
                                string lastName = reader["LastName"].ToString();
                                string fullName = firstName + " " + lastName;
                                UserName.Text = fullName;
                            }
                            else
                            {
                                UserName.Text = "No data found";
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error fetching name: {ex.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FetchAddress()
        {
            string accountNumber = Login.getAccNum;
            try
            {
                string query = "SELECT Address FROM accounts WHERE AccNum = @AccNum";

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
                                string address = reader["Address"].ToString();
                                Address.Text = address; // Assuming 'Address' is the correct TextBox control name
                            }
                            else
                            {
                                Address.Text = "No Address found"; // Assuming 'Address' is the correct TextBox control name
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error fetching address: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FetchAccNumber()
        {
            try
            {
                string accountNumber = Login.getAccNum;

                string query = "SELECT AccNum FROM accounts WHERE AccNum = @AccNum";

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
                                string accNum = reader["AccNum"].ToString();
                                AccNum.Text = accNum; // Assuming 'AccNum' is the correct TextBox control name
                            }
                            else
                            {
                                AccNum.Text = "No data found"; // Assuming 'AccNum' is the correct TextBox control name
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error fetching account number: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FetchAccType()
        {
            try
            {
                string accountNumber = Login.getAccNum;

                string query = "SELECT Type FROM accounts WHERE AccNum = @AccNum";

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
                                string accType = reader["Type"].ToString();
                                AccType.Text = accType; // Assuming 'AccType' is the correct TextBox control name
                            }
                            else
                            {
                                AccType.Text = "No data found"; // Assuming 'AccType' is the correct TextBox control name
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error fetching account type: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                                AccBalance.Text = balance.ToString();
                            }
                            else
                            {
                                AccBalance.Text = "0"; 
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

        private void FetchPINPass()
        {
            try
            {
                string accountNumber = Login.getAccNum;

                string query = "SELECT PINPass FROM accounts WHERE AccNum = @AccNum";

                using (MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user = root; database = bms; password ="))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AccNum", accountNumber);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int pinPass = Convert.ToInt32(reader["PINPass"]);
                                Pin.Text = pinPass.ToString();
                            }
                            else
                            {
                                Pin.Text = "No data found";
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error fetching PINPass: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FetchBirthday()
        {
            try
            {
                string accountNumber = Login.getAccNum;

                string query = "SELECT Birthday FROM accounts WHERE AccNum = @AccNum";

                using (MySqlConnection connection = new MySqlConnection("server = 127.0.0.1; user = root; database = bms; password ="))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AccNum", accountNumber);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime birthday = Convert.ToDateTime(reader["Birthday"]);
                                Birthday.Text = birthday.ToString("yyyy-MM-dd");  
                            }
                            else
                            {
                                Birthday.Text = "No data found";
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Error fetching birthday: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Edit();
        }
        private void Edit()
        {
            if (EditButton.Text.Equals("Edit"))
            {
               
                Address.ReadOnly = false;

                EditButton.Text = "Save";
            }
            else if (EditButton.Text.Equals("Save"))
            {
                string accountNumber = Login.getAccNum;

                try
                {
                    using (MySqlConnection conn = new MySqlConnection("server = 127.0.0.1; user = root; database = bms; password ="))
                    {
                        conn.Open();
                        string newAddress = Address.Text;

                        string updateQuery = "UPDATE accounts  SET Address = @Address WHERE AccNum = @AccNum";

                        using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@Address", newAddress);
                            updateCmd.Parameters.AddWithValue("@AccNum", accountNumber);

                            int rowsAffected = updateCmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Changes saved successfully.");
                            }
                            else
                            {
                                MessageBox.Show("Failed to change data.");
                            }
                        }
                    }
                }
                catch (MySqlException)
                {
                    MessageBox.Show("Error in editing. Please try again later.");
                }
                Address.ReadOnly = true;
                EditButton.Text = "Edit";
            }
        }

        private bool isHidden = true;
        private void HidePass()
        {
            if (isHidden)
            {
                Pin.PasswordChar = '\0';
                isHidden = false;
            }
            else
            {
                Pin.PasswordChar = '*';
                isHidden = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            HidePass();
        }
    }
}
