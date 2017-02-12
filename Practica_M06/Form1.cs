using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Practica_M06
{
    
    public partial class Form1 : Form
    {

        practicam6Entities entity;
        string table = "";
        public Form1()
        {
            InitializeComponent();
            fillCombo(comboBox);
            fillCombo(comboBox1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.dataGridView1.Columns.Clear();
            exportButton.Visible = false;
            Console.WriteLine("Test");


        }
        private void fillCombo(ComboBox comboBox)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = "PracticaM6";
            if (dbCon.IsConnect())
            {

                string query = "SHOW TABLES";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string row = "";
                    for (int i = 0; i < reader.FieldCount; i++)
                        comboBox.Items.Add(reader.GetValue(i).ToString());


                }
                reader.Close();
            }
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("##### " + comboBox.Text);
            entity = new practicam6Entities();
            this.dataGridView1.AutoGenerateColumns = true;
            this.dataGridView1.Columns.Clear();
            //dataGridView1.DataSource = entity.factura;
            if (comboBox.Text == "factura")
            {
                exportButton.Visible = true;
                
            } else
            {
                exportButton.Visible = false;
            }

            switch (comboBox.Text)
            {
                case "productes":
                    dataGridView1.DataSource = entity.productes;

                    break;
                case "factura":
                    dataGridView1.DataSource = entity.factura;
                    //dataGridView1.Columns.RemoveAt(5);
                    break;
                case "factura_detall":
                    dataGridView1.DataSource = entity.factura_detall;
                    break;
                case "clients":
                    dataGridView1.DataSource = entity.clients;
                    break;


            }
            
            
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            entity.SaveChanges();
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            //Creating iTextSharp Table from the DataTable data
            PdfPTable pdfTable = new PdfPTable(dataGridView1.ColumnCount);
            pdfTable.DefaultCell.Padding = 3;
            pdfTable.WidthPercentage = 30;
            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTable.DefaultCell.BorderWidth = 1;

            //Adding Header row
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                cell.BackgroundColor = new BaseColor(240, 240, 240);
                pdfTable.AddCell(cell);
            }

            //Adding DataRow
            for(int x = 0; x<dataGridView1.Rows.Count - 1; x++)
            {
                foreach (DataGridViewCell cell in dataGridView1.Rows[x].Cells)
                {
                    pdfTable.AddCell(cell.Value.ToString());
                }
            }

            //Exporting to PDF
            string folderPath = "C:\\PDFs\\";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            using (FileStream stream = new FileStream(folderPath + "DataGridViewExport.pdf", FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                pdfDoc.Add(pdfTable);
                pdfDoc.Close();
                stream.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("##### " + comboBox1.Text);
            entity = new practicam6Entities();
            this.dataGridView2.AutoGenerateColumns = true;
            this.dataGridView2.Columns.Clear();
            //dataGridView1.DataSource = entity.factura;
            if (comboBox1.Text == "factura")
            {
                exportButton.Visible = true;

            }
            else
            {
                exportButton.Visible = false;
            }

            switch (comboBox1.Text)
            {
                case "productes":
                    dataGridView2.DataSource = entity.productes;
                    break;
                case "factura":
                    dataGridView2.DataSource = entity.factura;
                    //dataGridView2.Columns.RemoveAt(5);
                    break;
                case "factura_detall":
                    dataGridView2.DataSource = entity.factura_detall;
                    break;
                case "clients":
                    dataGridView2.DataSource = entity.clients;
                    break;


            }

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "productes":
                    dataGridView2.DataSource = entity.productes;
                    break;
                case "factura":
                    dataGridView2.DataSource = entity.factura;
                    //dataGridView2.Columns.RemoveAt(5);
                    break;
                case "factura_detall":
                    dataGridView2.DataSource = entity.factura_detall;
                    break;
                case "clients":
                    dataGridView2.DataSource = entity.clients;
                    break;


            }
            DataTable dt = new DataTable();
            foreach (DataGridViewColumn col in dataGridView2.Columns)
            {
                dt.Columns.Add(col.HeaderText);
            }

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                DataRow dRow = dt.NewRow();
                foreach (DataGridViewCell cell in row.Cells)
                {
                    dRow[cell.ColumnIndex] = cell.Value;
                    Console.WriteLine(cell.Value);
                }
                dt.Rows.Add(dRow);
            }

            DataView ds = new DataView(dt);
            string outputInfo = "";
            string[] keyWords = textBox1.Text.Split(' ');
            bool first = false;
            foreach (string word in keyWords)
            {
                dataGridView2.DataSource = ds;
                if (outputInfo.Length == 0)
                {
                     
                    outputInfo += "(";
                    foreach(DataGridViewColumn col in dataGridView2.Columns)
                    {
                        if (first)
                        {
                            outputInfo += "OR ";
                        }
                        outputInfo += col.HeaderText + " LIKE '%" + word + "%' ";
                        first = true;
                    }
                    
                    outputInfo += ")";
                }
                else
                {
                    outputInfo += " AND (";
                    foreach (DataGridViewColumn col in dataGridView2.Columns)
                    {
                        if (first)
                        {
                            outputInfo += "OR ";
                        }
                        outputInfo += col.HeaderText + " LIKE '%" + word + "%' ";
                        first = true;
                    }

                    outputInfo += ")";
                }
            }
            ds.RowFilter = outputInfo;
        }
    }
}
