using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace ToDo_
{
    public partial class Form1 : Form
    {
        private SqlDataReader reader;
        SqlDataAdapter da = null;
        private DataTable table;
        private SqlConnection conn;
        SqlCommandBuilder cmd = null;
        DataSet set = null;
        List<Category> categories = new List <Category>();


        public Form1()
        {
            InitializeComponent();
            conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["MyNotes"].ConnectionString;
            Tables.Items.Add(new MyTable() { ShortName = "ToDo", SelectQuery = "Select * from ToDo" });
            Tables.Items.Add(new MyTable() { ShortName = "Category", SelectQuery = "Select * from Category" });
            Tables.SelectedIndex = 0;

        }

        class Category
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
        class MyTable
        {
            public string ShortName { get; set; }
            public string SelectQuery { get; set; }
            public override string ToString()
            {
                return ShortName;
            }
        }
        public void GetCategories()
        {
            categories.Clear();
            SqlDataReader rdr = null;
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Category", conn);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Category category = new Category();
                    category.ID = (int)rdr[0];
                    category.Name = (string)rdr[1];
                    categories.Add(category);
                }
            }
            finally
            {
                if (rdr != null)
                {
                    rdr.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        private void show_Click(object sender, EventArgs e)
        {
            try
            {
                GetCategories();
                SqlCommand comm = new SqlCommand();
                comm.CommandText = textBox1.Text;
                comm.Connection = conn;
                conn.Open();

                dataGridView1.DataSource = null;

                table = new DataTable();
                reader = comm.ExecuteReader();
                int line = 0;

                while (reader.Read())
                {
                    if (line == 0)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            table.Columns.Add(reader.GetName(i));
                        }
                        line++;
                    }
                    DataRow row = table.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (i == 6)
                        {
                            foreach (var item in categories)
                            {
                                if ((int)reader[i] == item.ID)
                                {
                                    row[i] = item.Name;
                                }
                            }
                        }
                        else row[i] = reader[i];
                    }
                    table.Rows.Add(row);
                }
                dataGridView1.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        private void DataSet(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.DataSource = null;

                set = new DataSet();
                string sql = textBox1.Text;
                da = new SqlDataAdapter(sql, conn);
                cmd = new SqlCommandBuilder(da);

                da.Fill(set, "ToDo");
                dataGridView1.DataSource = set.Tables["ToDo"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            da.Update(set, "ToDo");
        }

        private void Tables_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = ((MyTable)Tables.SelectedItem).SelectQuery;
        }
    }
}
