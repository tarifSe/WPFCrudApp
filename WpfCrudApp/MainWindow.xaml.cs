using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCrudApp.Models;

namespace WpfCrudApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection con;
        SqlCommand cmd;
        private int customerId;
        private string query;
        private Customer aCustomer;
        public MainWindow()
        {
            InitializeComponent();
            con = new SqlConnection(@$"Server={Environment.MachineName}; Database=CustomerCrudDb; Integrated Security=True");
            LoadDataGrid();
        }

        public void LoadDataGrid()
        {
            cmd = new SqlCommand(@"SELECT * FROM Customers", con);

            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(sdr);
            con.Close();

            if (dt.Rows.Count > 0)
            {
                displayDataGrid.ItemsSource = dt.DefaultView;
            }
        }

        public void TextBoxClear()
        {
            nameTextBox.Clear();
            addressTextBox.Clear();
            emailTextBox.Clear();
            phoneTextBox.Clear();
        }

        public bool IsValid()
        {
            if (nameTextBox.Text == string.Empty)
            {
                MessageBox.Show("Name is required.");
                return false;
            }
            if (phoneTextBox.Text == string.Empty)
            {
                MessageBox.Show("Phone Number is required.");
                return false;
            }
            if (emailTextBox.Text == string.Empty)
            {
                MessageBox.Show("Email is required.");
                return false;
            }
            if (addressTextBox.Text == string.Empty)
            {
                MessageBox.Show("Address is required.");
                return false;
            }
            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            aCustomer = new Customer()
            {
                Name = nameTextBox.Text,
                Phone = phoneTextBox.Text,
                Email = emailTextBox.Text,
                Address = addressTextBox.Text
            };

            if (IsValid())
            {
                query = @"INSERT INTO Customers VALUES(@Name, @Phone, @Email, @Address)";
                cmd = new SqlCommand(query, con);

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Name", aCustomer.Name);
                cmd.Parameters.AddWithValue("@Phone", aCustomer.Phone);
                cmd.Parameters.AddWithValue("@Email", aCustomer.Email);
                cmd.Parameters.AddWithValue("@Address", aCustomer.Address);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TextBoxClear();
                MessageBox.Show("Save successfull.");
                LoadDataGrid();
            }
        }

        private void displayDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SaveButton.IsEnabled = false;
            if (displayDataGrid.Items.Count > 0)
            {
                customerId = Convert.ToInt32(((DataRowView)displayDataGrid.SelectedItem).Row["Id"]);
                nameTextBox.Text = ((DataRowView)displayDataGrid.SelectedItem).Row["Name"].ToString();
                phoneTextBox.Text = ((DataRowView)displayDataGrid.SelectedItem).Row["Phone"].ToString();
                emailTextBox.Text = ((DataRowView)displayDataGrid.SelectedItem).Row["Email"].ToString();
                addressTextBox.Text = ((DataRowView)displayDataGrid.SelectedItem).Row["Address"].ToString();
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            aCustomer = new Customer()
            {
                Name = nameTextBox.Text,
                Phone = phoneTextBox.Text,
                Email = emailTextBox.Text,
                Address = addressTextBox.Text
            };

            if (IsValid())
            {
                query = @$"UPDATE Customers SET Name=@Name, Phone=@Phone, Email=@Email, Address=@Address WHERE Id= {customerId}";
                cmd = new SqlCommand(query, con);

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Name", aCustomer.Name);
                cmd.Parameters.AddWithValue("@Phone", aCustomer.Phone);
                cmd.Parameters.AddWithValue("@Email", aCustomer.Email);
                cmd.Parameters.AddWithValue("@Address", aCustomer.Address);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TextBoxClear();
                MessageBox.Show("Update successfull.");
                LoadDataGrid();
                SaveButton.IsEnabled = true;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid())
            {
                query = @$"DELETE FROM Customers WHERE Id={customerId}";
                cmd = new SqlCommand(query, con);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TextBoxClear();
                MessageBox.Show("Delete successfull.");
                LoadDataGrid();
                SaveButton.IsEnabled = true;
            }
        }
    }
}
