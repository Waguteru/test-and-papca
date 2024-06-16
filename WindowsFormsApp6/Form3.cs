using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp6
{
    public partial class Form3 : Form
    {

        private bool closed = false;

        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            if(closed)
            {
                return;
            }
            else if (CheckUser (username,password))
            {
                ShowRoleForm(username);
            }
        }

        string connectionstring = "Server = localhost; port = 5432; DataBase = bookshop; User Id = postgres; Password = 123";

        private bool CheckUser(string username, string password)
        {
            using(NpgsqlConnection connection = new NpgsqlConnection(connectionstring))
            {
                string query = $"select COUNT(*) from users where login_user = @username AND password_user = @password";
                using(NpgsqlCommand command = new NpgsqlCommand(query,connection)) 
                {
                    command.Parameters.AddWithValue("username", username);
                    command.Parameters.AddWithValue("password", password);

                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        private void ShowRoleForm(string username)
        {
            string role = GetRoleUser(username);

            if (role == "Менеджер")
            {
                Form1 form1 = new Form1();
                this.Hide();
                form1.ShowDialog();
                this.Close();
            }
            else if(role == "Пользователь")
            {
                Form2 form2 = new Form2();
                this.Hide();
                form2.ShowDialog(); this.Close();
            }

        }

        private string GetRoleUser(string username)
        {
            string role = "";
            using(NpgsqlConnection connection = new NpgsqlConnection(connectionstring))
            {
                string query = $"select role_user from qq where username = @username";
                using(NpgsqlCommand command = new NpgsqlCommand(query,connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if(result != null)
                        {
                            role = result.ToString();
                        }
                        else
                        {

                        }

                    }catch (Exception ex)
                    {

                    }
                }
            }
            return role;
        }
    }
}
