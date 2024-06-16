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
    public partial class Form2 : Form
    {

        DataBase dataBase = new DataBase();

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataBase.OpenConnection();

            var name = textBox1.Text;
            var author = textBox2.Text;
            var photo = textBox3.Text;
            var ptice = Convert.ToInt32(textBox4.Text);

            string query = $"insert into books(name_book,author,photobook,pricebook) values ('{name}','{author}','{photo}','{ptice}') ";

            NpgsqlCommand command = new NpgsqlCommand(query,dataBase.GetConnection());

            command.ExecuteNonQuery();

            MessageBox.Show("Товар добавлен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

            dataBase.CloseConnection();

            Form1 form1 = new Form1();
            this.Hide();
            form1.ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                textBox3.Text = openFileDialog1.FileName;
        }
    }
}
