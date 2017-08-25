using Excel=Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Accountability
{
    public partial class Form2 : Form
    {
        OleDbConnection con;
        public Form2()
        {
            InitializeComponent();
            con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Application.StartupPath + "/Accountability.accdb;Persist Security Info=False;");
        }
        //SqlConnection con;
        
        private void Button2_Click(object sender, EventArgs e)
        {
            //con = new SqlConnection
            //{
            //    ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = " + Application.StartupPath + @"\Accountability.mdf; Integrated Security = True"
            //};
           

            //barcode, lastName, firstName, room, shift, afsc, mtl, inHouse

            string statement = "INSERT INTO [dbo].[Airmen] (";
            string values = ") VALUES(";
            if(!txtBarcode.Text.Equals("")) {
                statement += "barcode, ";
                values += "'" + txtBarcode.Text.ToUpper() + "', ";
            }
            statement += "name";
            values += "'" + txtlName.Text + ", ";
            values += txtfName.Text + " " + txtMI.Text + "'";
            if (!txtRoom.Text.Equals("")) {
                statement += ", room";
                values += ", '" + txtRoom.Text + "'";
            }
            if (!cmbShift.Text.Equals("")) {
                statement += ", shift";
                values += ", '" + cmbShift.Text + "'";
            }
            if (!txtAfsc.Text.Equals("")) {
                statement += ", afsc";
                values += ", '" + txtAfsc.Text + "'";
            }
            if (!cmbMtl.Text.Equals("")) {
                statement += ", mtl";
                values += ", '" + cmbMtl.Text + "'";
            }
            statement += ", inHouse";
            values += ", 1)";
            string fullStatement = statement + values;
            OleDbCommand command = new OleDbCommand(fullStatement, con);
            con.Open();
            command.ExecuteNonQuery();
            con.Close();

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OleDbCommand command;
            openFileDialog1.Filter = "Excel Files|*.xls;*.xlsx";
            openFileDialog1.Title = "Select Airmen DB";
            
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            { 
                string fileName = openFileDialog1.FileName;
                
                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(fileName);
                Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                Excel.Range xlRange = xlWorksheet.UsedRange;

                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;

                    /*
                     * SAMPLE STRING
                     * "1TOSC8HX1DPLFK0AFW, 'Hackert, Kerry D', A311, Days, 3E831, SSgt Rudloff"
                     * barcode, name, room, shift, afsc, mtl
                     */
                for (int i = 2; i <= rowCount; i++) {
                    string statement = "INSERT INTO [dbo].[Airmen] (";
                    string values = ") VALUES (";
                    
                    for (int j = 1; j <= colCount; j++) {
                        if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null) {
                            statement += xlRange.Cells[1, j].Value2.ToString();
                            if (j <= 5)
                                statement += ", ";
                            values += "'" + xlRange.Cells[i, j].Value2.ToString() + "'";
                            if (j <= 5)
                                values += ", ";
                            else
                                values += ")";
                        }
                    }
                    string fullStatement = statement + values;
                    command = new OleDbCommand(fullStatement, con);
                    con.Open();
                    command.ExecuteNonQuery();
                    con.Close();
                }
                
                string[] airmen = File.ReadAllLines(fileName);
                foreach(string airman in airmen)
                {
                    
                    string[] fields = airman.Split(',');
                }
            }
        }
    }
}
