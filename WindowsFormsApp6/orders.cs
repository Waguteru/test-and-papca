using Npgsql;
using NpgsqlTypes;
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
    public partial class orders : Form
    {

        private string name;
        private string author;
        private NpgsqlConnection conn;
        private int price;

        private Random random1 = new Random();

        public orders(string name, string author,int price,Npgsql.NpgsqlConnection connection)
        {
            InitializeComponent();
            this.name = name;
            this.author = author;
            conn = connection;
            this.price = price;

            richTextBox1.Text = $"название книги: {this.name}\n\n" +
                                $"автор: {this.author}\n\n" +
                                $"price: {this.price}\n\n" +
                                $"date: {DateTime.Now:dd/MM/yyyy}\n\n"+
                                $"пункт выдачи: {comboBox1.SelectedItem}";

            numericUpDown1.Value = 1;
        }


        private string GenerateCod()
        {
            return random1.Next(100,999).ToString();
        }

        private void orders_Load(object sender, EventArgs e)
        {

        }

        private void UpdateRichBox()
        {
            decimal quality = numericUpDown1.Value;

            if(quality <= 0)
            {
                MessageBox.Show("заказ не может быть пустым","ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);

                Form1 form1 = new Form1();

                this.Hide();

                form1.ShowDialog();

                this.Close();
            }
            else
            {

                decimal totalCount = (this.price * quality);

                richTextBox1.Text = $"название книги: {this.name}\n\n" +
                               $"автор: {this.author}\n\n" +
                               $"price: {this.price} \n\n" +
                               $"date: {DateTime.Now:dd/MM/yyyy} \n\n" +
                               $"summa: {totalCount:C}\n\n"+
                               $"cod orders: {GenerateCod()} \n\n"+
                               $"пункт выдачи: {comboBox1.SelectedItem}";

                textBox1 .Text = totalCount.ToString();
            }
        } 

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateRichBox();

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

            decimal quality = numericUpDown1.Value;

            UpdateRichBox();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string namebook = this.name;
            string author = this.author;
            decimal totalprice = decimal.Parse(textBox1.Text);
            string codeorders = GenerateCod();

            using(NpgsqlCommand cmd = new NpgsqlCommand())
            {
                conn.Open();
                cmd.Connection = conn;

                string query = "INSERT INTO test (namebook,author,totalprice,dateorders,codeorders)" + "VALUES (@Namebook,@Author,@Totalprice,@Dateorders,@Codeorders)";

                cmd.CommandText = query;

               

                cmd.Parameters.AddWithValue("@Namebook", namebook);
                cmd.Parameters.AddWithValue("@Author", author);
                cmd.Parameters.AddWithValue("@Totalprice", totalprice);
                cmd.Parameters.AddWithValue("@Codeorders", int.Parse(codeorders));
                cmd.Parameters.Add(new NpgsqlParameter("@Dateorders", NpgsqlDbType.Date)).Value = DateTime.Now.Date;

                cmd.ExecuteNonQuery();
            }
        }
    }
}
