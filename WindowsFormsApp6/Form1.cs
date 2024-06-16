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
using System.Xml.Linq;

namespace WindowsFormsApp6
{
    public partial class Form1 : Form
    {

        DataBase dataBase = new DataBase();

        enum RowState 
        {
            New,
            Modfiend,
            MofFienNew,
            Deleted
        }

        int selectRow;
        public Form1()
        {
            InitializeComponent();

            button6.Visible = false;
        }

        public void CreateColumns()
        {
            dataGridView1.Columns.Add("id_book", "номер");
            dataGridView1.Columns.Add("name_book", "название");
            dataGridView1.Columns.Add("author", "автор");
            dataGridView1.Columns.Add("photobook", "фото");
            dataGridView1.Columns.Add("pricebook", "цена");
            dataGridView1.Columns.Add("IsNew", String.Empty);
            dataGridView1.Columns["id_book"].Visible = false;
            dataGridView1.Columns["IsNew"].Visible = false;
        }


        public void ReadSingleRow(DataGridView gridView,IDataRecord record)
        {
            gridView.Rows.Add(record.GetInt64(0),record.GetString(1),record.GetString(2),record.GetString(3),record.GetInt32(4),RowState.Modfiend);
        }

        public void RefreshData(DataGridView gridView)
        {
            gridView.Rows.Clear();

            dataBase.OpenConnection();

            string query = "select id_book,name_book,author,photobook,pricebook from books";

            NpgsqlCommand command = new NpgsqlCommand(query,dataBase.GetConnection());

           NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(gridView,reader);
            }
            reader.Close();
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshData(dataGridView1);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectRow = e.RowIndex;

            if(e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string path = row.Cells[3].Value.ToString();

                if(!string.IsNullOrEmpty(path) )
                {
                    pictureBox1.ImageLocation = path;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            if(e.RowIndex >= 0)
            {
                DataGridViewRow viewRow = dataGridView1.Rows[selectRow];

                textBox1.Text = viewRow.Cells[1].Value.ToString();
                textBox2.Text = viewRow.Cells[2].Value.ToString();
                textBox3.Text = viewRow.Cells[0].Value.ToString();
                textBox4.Text = viewRow.Cells[3].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            this.Hide();
            form2.ShowDialog();
            this.Close();
        }

        public void DeleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[4].Value = RowState.Deleted;
                return;
            }
            dataGridView1.Rows[index].Cells[4].Value = RowState.Deleted;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DeleteRow();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataBase.OpenConnection();

            var name = textBox1.Text;
            var author = textBox2.Text;

            var id = Convert.ToInt64(textBox3.Text);

            string query = $"delete from books where id_book = " + id;

            NpgsqlCommand command = new NpgsqlCommand(query,dataBase.GetConnection());

            command.ExecuteNonQuery();

            dataBase.CloseConnection();
        }

        public void UpdateRow()
        {
            int select = dataGridView1.CurrentCell.RowIndex;

            var name = textBox1.Text;
            var author = textBox2.Text;
            var id = textBox3.Text;

            if (dataGridView1.Rows[select].Cells[0].Value.ToString() != string.Empty)
            {
                dataGridView1.Rows[select].SetValues(id,name, author);
                dataGridView1.Rows[select].Cells[4].Value = RowState.MofFienNew;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            UpdateRow();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataBase.OpenConnection();

            var name = textBox1.Text;
            var author = textBox2.Text;
            var id = Convert.ToInt64(textBox3.Text);

            string query = $"update books set name_book = '{name}', author = '{author}' where id_book = " + id;

            NpgsqlCommand command = new NpgsqlCommand( query,dataBase.GetConnection());


            command.ExecuteNonQuery();

            dataBase.CloseConnection();
        }


        public void Search(DataGridView gridView)
        {
            gridView.Rows.Clear();

           

            string query = $"select id_book, name_book,author,photobook,pricebook from books where concat (name_book,author) like '%" + textBox5.Text + "%'";

            NpgsqlCommand command = new NpgsqlCommand(query,dataBase.GetConnection());

            dataBase.OpenConnection();

            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(gridView,reader);
            }
            reader.Close();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private int clikedIndex = -1; 

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                clikedIndex = e.RowIndex;

                button6.Visible = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if(clikedIndex >= 0 && clikedIndex < dataGridView1.Rows.Count)
            {
                string name = dataGridView1.Rows[clikedIndex].Cells[1].Value.ToString();
                string aurhor = dataGridView1.Rows[clikedIndex].Cells[2].Value.ToString();
                int price = Convert.ToInt32(dataGridView1.Rows[clikedIndex].Cells[4].Value);

                NpgsqlConnection connection = new NpgsqlConnection("Server = localhost;port = 5432; DataBase = bookshop; User Id = postgres; Password = 123");

                orders orders = new orders(name,aurhor,price,connection);

                this.Hide();

                orders.ShowDialog();

                this.Close();
            }
        }
    }
}
