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
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Xml;
using System.Xml.Linq;

namespace Practica_M06
{

    public partial class Form1 : Form
    {
        DataView ds;
        DataTable dt;

        practicam6Entities entity;
        /**
         * Funcions inicials
         **/
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

        }

        /**
         * Omple el ComboBox amb els noms de les taules de la base de dades
         **/
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
                    for (int i = 0; i < reader.FieldCount; i++)
                        comboBox.Items.Add(reader.GetValue(i).ToString());
                }
                reader.Close();
            }
        }

        /**
         * S'executa quen es selecciona un element de la comboBox. 
         * Llavors carrega al dataGridView Tota al informació pertanyent a la
         * taula seleccionada.
         **/
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            entity = new practicam6Entities();
            this.dataGridView1.AutoGenerateColumns = true;
            this.dataGridView1.Columns.Clear();
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
                    break;
                case "factura_detall":
                    dataGridView1.DataSource = entity.factura_detall;
                    break;
                case "clients":
                    dataGridView1.DataSource = entity.clients;
                    break;
            }
        }

        /**
         * Actualitzaa la bd els canvis realitzats al datagridview.
         **/
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            entity.SaveChanges();
        }

        /**
         * Exporta la factura a un PDF. Es pregunta ón es desitja guardar-ho.
         **/
        private void exportButton_Click(object sender, EventArgs e)
        {
            PdfPTable pdfTable = new PdfPTable(dataGridView1.ColumnCount);
            pdfTable.DefaultCell.Padding = 3;
            pdfTable.WidthPercentage = 30;
            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfTable.DefaultCell.BorderWidth = 1;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                cell.BackgroundColor = new BaseColor(240, 240, 240);
                pdfTable.AddCell(cell);
            }

            for (int x = 0; x < dataGridView1.Rows.Count - 1; x++)
            {
                foreach (DataGridViewCell cell in dataGridView1.Rows[x].Cells)
                {
                    pdfTable.AddCell(cell.Value.ToString());
                }
            }

            var fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string folderPath = fbd.SelectedPath;
                using (FileStream stream = new FileStream(folderPath + "\\DataGridViewExport.pdf", FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
                    PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    pdfDoc.Add(pdfTable);
                    pdfDoc.Close();
                    stream.Close();
                }
            }
        }

        /**
         * S'executa quen es selecciona un element de la comboBox. 
         * Llavors carrega al dataGridView Tota al informació pertanyent a la
         * taula seleccionada.
         **/
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            entity = new practicam6Entities();
            this.dataGridView2.AutoGenerateColumns = true;
            this.dataGridView2.Columns.Clear();
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

        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            DataTable dT = GetDataTableFromDGV(dataGridView2);
            DataSet dS = new DataSet(comboBox1.Text);
            dS.Tables.Add(dT);



            var fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string folderPath = fbd.SelectedPath;
                Console.WriteLine(folderPath);
                System.IO.FileStream streamWrite = new System.IO.FileStream(folderPath + @"\" + comboBox1.Text + ".xml", System.IO.FileMode.Create);

                dS.WriteXml(streamWrite);
                streamWrite.Close();


            }



        }

        private DataTable GetDataTableFromDGV(DataGridView dgv)
        {
            var dt = new DataTable();
            dt.TableName = "Row";
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if (column.Visible)
                {
                    // You could potentially name the column based on the DGV column name (beware of dupes)
                    // or assign a type based on the data type of the data bound to this DGV column.
                    dt.Columns.Add();
                    dt.Columns[column.Index].ColumnName = column.Name;
                }
            }

            object[] cellValues = new object[dgv.Columns.Count];
            foreach (DataGridViewRow row in dgv.Rows)
            {
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    cellValues[i] = row.Cells[i].Value;
                }
                dt.Rows.Add(cellValues);
            }

            return dt;
        }



        /**
        * Filtra el que escrius al TextBox.
        **/
        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "productes":
                    dataGridView2.DataSource = entity.productes;
                    break;
                case "factura":
                    dataGridView2.DataSource = entity.factura;
                    break;
                case "factura_detall":
                    dataGridView2.DataSource = entity.factura_detall;
                    break;
                case "clients":
                    dataGridView2.DataSource = entity.clients;
                    break;


            }
            dt = new DataTable();
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

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "XML Files|*.xml";
            dlg.Title = "Select a XML File";

            dlg.ShowDialog();
            XDocument xDoc = XDocument.Load(dlg.FileName);
            //List<productes> pr = xDoc.Descendants("productes").
            //        Select(Row =>
            //            new productes
            //            {
            //                id_Producte = Convert.ToInt32(Row.Element("id_Producte").Value),
            //                Producte = Row.Element("Producte").Value,
            //                Preu = Convert.ToDecimal(Row.Element("Preu").Value)
            //            }).ToList();

           

            
            
            

            //foreach (productes p in pr)
            //{
            //    entity.productes.AddObject(p);
            //}
            //entity.SaveChanges();
            //} catch (Exception) { Console.WriteLine("Error"); }
        }
    }
}
