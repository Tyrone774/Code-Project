using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Banking_Management_System
{
    public partial class TransactionHistory : Form
    {
        private DataTable originalDataTable;
        public TransactionHistory()
        {
            InitializeComponent();
            History();
            Search.SelectedItem = "All transactions";
        }

        private void History()
        {
            string accountNumber = Login.getAccNum;
            string connectionString = "server=127.0.0.1;user=root;database=bms;password=";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlCommand command = new MySqlCommand("SELECT ReferenceID, RunningBalance,Amount, Date, Time, Type, Reason FROM records WHERE AccNum = @AccNum", connection);
                    command.Parameters.AddWithValue("@AccNum", accountNumber);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    originalDataTable = new DataTable();
                    adapter.Fill(originalDataTable);

                    TransacHistory.DataSource = originalDataTable;
                    TransacHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    TransacHistory.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("An error occurred while fetching data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Search_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedType = (string)Search.SelectedItem;


            if (originalDataTable != null)
            {
                DataView dataView = new DataView(originalDataTable);
                string rowFilter = string.Empty;

                if (!selectedType.Equals("All transactions"))
                {
                    rowFilter = $"Type = '{selectedType}'";
                }

                dataView.RowFilter = rowFilter;
                TransacHistory.DataSource = dataView.ToTable();
            }
        }
        }
    }

