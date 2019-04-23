using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
namespace Portfolio_Project
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".csv";
            ofd.Filter = "Comma Separated (*.csv)|*.csv";
            ofd.ShowDialog();
            textBox1.Text = ofd.FileName;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DataTable importData = GetDataFromFile();
            if (importData == null) return;

            SaveImportDataToDatabase(importData);
            MessageBox.Show("Portfolio file is imported");
        }
        private DataTable GetDataFromFile()
        {
            DataTable importedData = new DataTable();
            try
            {
                using (StreamReader sr = new StreamReader(textBox1.Text))
                {
                    string header = sr.ReadLine();
                    if (string.IsNullOrEmpty(header))
                    {
                        MessageBox.Show("No file data");
                        return null;

                    }
                    string[] headerColumns = header.Split(',');
                    foreach (string headerColumn in headerColumns)
                    {
                        importedData.Columns.Add(headerColumn);
                    }

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line)) continue;
                        string[] fields = line.Split(',');
                        DataRow importedRow = importedData.NewRow();
                        for (int i = 0; i < fields.Count(); i++)
                        {
                            importedRow[i] = fields[i];
                        }
                        importedData.Rows.Add(importedRow);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return importedData;

        }
        private void SaveImportDataToDatabase(DataTable importData)
        {
            string ConnectionString = "Data Source = DESKTOP-HTTI0I6\\sqlexpress; Initial Catalog = MyDatabase; Integrated Security = True";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                foreach (DataRow importRow in importData.Rows)
                {
                    SqlCommand cmd = new SqlCommand("insert into Portfolio_data(Security,Holding, Unit_Price) " +
                        "values(@security, @holding, @unitPrice)", conn);



                    cmd.Parameters.AddWithValue("@security", importRow["Security"]);
                    cmd.Parameters.AddWithValue("@holding", importRow["Holding"]);
                    cmd.Parameters.AddWithValue("@unitPrice", importRow["Unit_Price"]);
                    cmd.ExecuteNonQuery();
                    //conn.Close();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DataTable importData = GetDataFromDiffFile();
            if (importData == null) return;

            SaveImportDataToDatabase1(importData);
            MessageBox.Show("Benchmark file is imported");
        }
        private DataTable GetDataFromDiffFile()
        {
            DataTable importedData = new DataTable();
            try
            {
                using (StreamReader sr = new StreamReader(textBox1.Text))
                {
                    string header = sr.ReadLine();
                    if (string.IsNullOrEmpty(header))
                    {
                        MessageBox.Show("No file data");
                        return null;

                    }
                    string[] headerColumns = header.Split(',');
                    foreach (string headerColumn in headerColumns)
                    {
                        importedData.Columns.Add(headerColumn);
                    }

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line)) continue;
                        string[] fields = line.Split(',');
                        DataRow importedRow = importedData.NewRow();
                        for (int i = 0; i < fields.Count(); i++)
                        {
                            importedRow[i] = fields[i];
                        }
                        importedData.Rows.Add(importedRow);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return importedData;

        }
        private void SaveImportDataToDatabase1(DataTable importData)
        {
            string ConnectionString = "Data Source = DESKTOP-HTTI0I6\\sqlexpress; Initial Catalog = MyDatabase; Integrated Security = True";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                foreach (DataRow importRow in importData.Rows)
                {
                    SqlCommand cmd = new SqlCommand("insert into Benchmark_data(Security,Holding, Unit_Price) " +
                        "values(@security, @holding, @unitPrice)", conn);



                    cmd.Parameters.AddWithValue("@security", importRow["Security"]);
                    cmd.Parameters.AddWithValue("@holding", importRow["Holding"]);
                    cmd.Parameters.AddWithValue("@unitPrice", importRow["Unit_Price"]);
                    cmd.ExecuteNonQuery();

                }
            }
        }

       
            private void button5_Click(object sender, EventArgs e)
            {
                string str = "Data Source=DESKTOP-HTTI0I6\\sqlexpress;Initial Catalog=MyDatabase;Integrated Security=True";
                using (SqlConnection con = new SqlConnection(str))
                {
                    SqlCommand cmd = new SqlCommand("select * from Portfolio_data", con);

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Security");
                    dt.Columns.Add("Holding");
                    dt.Columns.Add("Unit_Price");
                    dt.Columns.Add("Market_Value");
                    dt.Columns.Add("Market_Val_Per");
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        DataRow dr = dt.NewRow();
                      dr["Security"] = rdr["Security"];
                        dr["Holding"] = rdr["Holding"];
                        dr["Unit_Price"] = rdr["Unit_Price"];
                    
                        dt.Rows.Add(dr);

                }
                con.Close();
                dataGridView1.DataSource = dt;
                
                
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
   
                    row.Cells[dataGridView1.Columns["Market_Value"].Index].Value=
                        (Convert.ToDouble(row.Cells[dataGridView1.Columns["Holding"].Index].Value) *
                        Convert.ToDouble(row.Cells[dataGridView1.Columns["Unit_Price"].Index].Value));
                    }
                double sum = 0;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    sum += Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value);
                }
               // MessageBox.Show(sum.ToString());

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {

                    row.Cells[dataGridView1.Columns["Market_Val_Per"].Index].Value =
                        ((Convert.ToDouble(row.Cells[dataGridView1.Columns["Market_Value"].Index].Value))/ sum)*100;
                        
                }
            }



            }

        //private void button6_Click(object sender, EventArgs e)
        //{
            
        //}

        

        private void button6_Click(object sender, EventArgs e)
        {
            string str = "Data Source=DESKTOP-HTTI0I6\\sqlexpress;Initial Catalog=MyDatabase;Integrated Security=True";
            using (SqlConnection con = new SqlConnection(str))
            {
                SqlCommand cmd = new SqlCommand("select * from Benchmark_data", con);

                DataTable dt = new DataTable();
                dt.Columns.Add("Security");
                dt.Columns.Add("Holding");
                dt.Columns.Add("Unit_Price");
                dt.Columns.Add("Market_Value");
                dt.Columns.Add("Market_Val_Per");
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DataRow dr = dt.NewRow();
                    dr["Security"] = rdr["Security"];
                    dr["Holding"] = rdr["Holding"];
                    dr["Unit_Price"] = rdr["Unit_Price"];
                    
                    dt.Rows.Add(dr);

                }
                con.Close();
                dataGridView2.DataSource = dt;


                foreach (DataGridViewRow row in dataGridView2.Rows)
                {

                    row.Cells[dataGridView2.Columns["Market_Value"].Index].Value =
                        (Convert.ToDouble(row.Cells[dataGridView2.Columns["Holding"].Index].Value) *
                        Convert.ToDouble(row.Cells[dataGridView2.Columns["Unit_Price"].Index].Value));
                }
                double sum = 0;
                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    sum += Convert.ToDouble(dataGridView2.Rows[i].Cells[3].Value);
                }
                //MessageBox.Show(sum.ToString());

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {

                    row.Cells[dataGridView2.Columns["Market_Val_Per"].Index].Value =
                        ((Convert.ToDouble(row.Cells[dataGridView2.Columns["Market_Value"].Index].Value)) / sum) * 100;

                }

            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
           // foreach (DataGridViewRow item in dataGridView1)
           // {
           //if(Convert.ToDouble())
           // }   
            


            
            //foreach (DataGridViewRow row in dataGridView1.Rows)
            //{
            

            //    if ((Convert.ToInt32(row.Cells[dataGridView1.Columns["Holding"].Index].Value)) >1000)
            //        Convert.ToDouble(dataGridView1.Rows[i].Cells[0].Value
            //    {
                    
            //        MessageBox.Show("done");
            //        //row.Cells[dataGridView3.Columns["Security"].Index].Value = Convert.ToInt32(row.Cells[dataGridView1.Columns["Holding"].Index].Value);
            //        //row.Cells[dataGridView3.Columns["Main"].Index].Value = Convert.ToDouble(row.Cells[dataGridView1.Columns["Holding"].Index].Value);
            //    }


            //    else
            //    {
            //        MessageBox.Show("n");
            //    }
            //}
        }



       private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
        //    int n = 0;
        //    foreach (DataGridViewRow row in dataGridView1.Rows)
        //    {
        //        if (dataGridView1.Rows.Count != n + 1)
        //        {

        //            dataGridView3.Rows.Add();
        //            dataGridView3.Rows[n].Cells[0].Value = row.Cells[1].Value.ToString();
        //        }
        //        n += 1;
        //    }

        //    int i = 0;
        //    foreach (DataGridViewRow row in dataGridView2.Rows)
        //    {
        //        if (dataGridView2.Rows.Count != i + 1)
        //        {

        //            dataGridView3.Rows.Add();
        //            dataGridView3.Rows[i].Cells[1].Value = row.Cells[1].Value.ToString();
        //        }
        //        i += 1;
        //    }
            

        //}

        //private void button9_Click(object sender, EventArgs e)
        //{
        //    int i = 0;
        //    foreach (DataGridViewRow row in dataGridView3.Rows)
        //    {
        //        if (dataGridView2.Rows.Count != i + 1)
        //        {
        //            if (Convert.ToInt64(dataGridView3.Rows[i].Cells[0].Value)== (Convert.ToInt64(dataGridView3.Rows[i].Cells[1].Value)))
        //            {
        //                row.Cells[dataGridView3.Columns["Trade"].Index].Value = 100;
        //            }
        //            else
        //            {
        //                MessageBox.Show("not e");
        //            }
        //        }i += 1;

        //        //row.Cells[dataGridView3.Columns["Trade"].Index].Value =
        //        //(Convert.ToString(row.Cells[dataGridView3.Columns["Security1"].Index].Value) *
        //        //Convert.ToDouble(row.Cells[dataGridView3.Columns["Security2"].Index].Value));
        //    }

            //foreach (DataGridViewRow row in dataGridView3.Rows)
            //{

            //    if ((row.Cells[dataGridView3.Columns["Security1"].Index].Value).Equals((row.Cells[dataGridView3.Columns["Security2"].Index].Value)))
            //    {
            //        MessageBox.Show("dhjd");
            //    }
            //    //dataGridView1.CurrentRow.Cells["ProductName"].Value.Equals("Diamond")

            //    //row.Cells[dataGridView3.Columns["Trade"].Index].Value =
            //    //    (Convert.ToDouble(row.Cells[dataGridView2.Columns["Holding"].Index].Value) *
            //    //    Convert.ToDouble(row.Cells[dataGridView2.Columns["Unit_Price"].Index].Value));
            //}


        }





















        //private void button6_Click_1(object sender, EventArgs e)
        //{


        //        }





    }
    }
    


