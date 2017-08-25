using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Accountability {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }
        SqlConnection con;
        Airman lastAirman;
        // Add/Remove Airmen
        private void Button1_Click(object sender, EventArgs e) {
            Form2 addRemove = new Form2();
            addRemove.Show();
        }

        private void Form1_Load(object sender, EventArgs e) {
            // Connect to database
            con = new SqlConnection {
                ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = " + Application.StartupPath + @"\Accountability.mdf; Integrated Security = True"
            };
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            con.Close();
        }
        private string lastId = "";
        // Triggers the magic
        private void textBox1_Leave(object sender, EventArgs e) {

        }

        // Where the magic happens
        private void updateStuff() {
            string userId = textBox1.Text.ToUpper();
            lastId = userId;

            if (userId.Equals("")) {
                return;
            }

            //find Airman
            SqlCommand command = new SqlCommand("SELECT * FROM [dbo].[Airmen] WHERE [barcode] = '" + userId + "'", con);
            con.Open();

            SqlDataReader reader = command.ExecuteReader();
            try {
                string name = null;
                string room = null;
                string mtl = null;
                string barcode = null;
                string shift = null;
                bool boolIn = false;
                while (reader.Read()) {
                    name = reader["name"].ToString();
                    room = reader["room"].ToString();
                    mtl = reader["mtl"].ToString();
                    barcode = reader["barcode"].ToString();
                    shift = reader["shift"].ToString();
                    boolIn = bool.Parse(reader["inHouse"].ToString());
                }

                con.Close();
                if (name == null) {
                    //No airman found
                    return;
                }

                lastAirman = new Airman(name, room, mtl, barcode, shift, boolIn);
                //Airman was in state boolIn, now flip it in scans table
                bool currIn = !boolIn;

                // Update labels
                labelName.Text = name;
                labelRoom.Text = lastAirman.Room;
                labelShift.Text = lastAirman.Shift;

                string dateString = String.Format("{0:MM/dd/yy HH:mm}", DateTime.Now);  // 08/21/17 16:40
                
                int irow = -1;
                if(dataGridView1.Rows.Count > 0) {
                    DataGridViewCell bc = null;
                    for (int i = 0; i < dataGridView1.Rows.Count; i++) {
                        bc = dataGridView1.Rows[i].Cells[0];
                        if (bc.Value.ToString().ToUpper().Equals(barcode.ToUpper())) {
                            irow = i;
                            break;
                        }
                    }
                }
                if(irow < 0) {
                    //No row in the db, add it
                    // Add info to data grid view
                    DataGridViewRow row = new DataGridViewRow();
                    string inOut = (currIn ? "In" : "Out");
                    row.CreateCells(dataGridView1);
                    row.Cells[0].Value = barcode;
                    row.Cells[1].Value = name;                      //Name
                    row.Cells[2].Value = lastAirman.Room;           //Room
                    row.Cells[3].Value = lastAirman.Shift;     //Transition
                    row.Cells[4].Value = inOut;         //Status
                    row.Cells[5].Value = dateString;                //Time
                    dataGridView1.Rows.Insert(0, row);

                } else {
                    // Remove row from table, change values, and add it to the top
                    DataGridViewRow dgRow = dataGridView1.Rows[irow];
                    string inOut = (currIn ? "In" : "Out");
                    dgRow.Cells[4].Value = inOut;
                    dgRow.Cells[5].Value = dateString;
                    dataGridView1.Rows.RemoveAt(irow);
                    dataGridView1.Rows.Insert(0, dgRow);
                }


                // Now save info to database in scans table
                int saveIn = 0;
                if (currIn) {
                    saveIn = 1;
                }
                string statement = "INSERT INTO [dbo].[Scans] (barcode, inHouse, time) VALUES(";
                statement += "'" + userId + "', ";
                statement += saveIn + ", ";
                statement += "'" + dateString + "');";
                executeSQLCommand(statement);

                //Update in/out status in airmen table
                statement = "UPDATE [dbo].[Airmen] SET inHouse = " + saveIn + " WHERE barcode = '" + userId + "'; ";
                executeSQLCommand(statement);
            } finally {
                // Always call Close when done reading.
                reader.Close();
            }
            // Clear textbox
            textBox1.Text = "";
        }

        // Show out airmen
        private void button2_Click(object sender, EventArgs e) {
            outAirmen oA = new outAirmen();
            oA.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            if (textBox1.Text.Length >= 20) {
                if (textBox1.Text.EndsWith("\r\n")) {
                    textBox1.Text = textBox1.Text.Substring(0, textBox1.Text.Length - 2);
                }
                updateStuff();
                textBox1.Focus();
                textBox1.Text = "";
                /**
                 *name, room, afsc
                 * a= tech school
                 * b= bmt
                 * c= bmt
                 * d= bmt grad
                 * E = name
                 * f= rank
                 * g= gen
                 * H = afsc
                 * i= jqri
                 * j= tech squadron
                 * k= tech grad
                 * 
                 * 
                 * a= name
                 * b= ssan
                 * c= rank
                 * d= afsc
                 * e= tqri
                 * f= tech squadron
                 * g= techflight
                 * h= tech cou
                 * i= tech csd
                 */
            }
        }

        private void executeSQLCommand(string statement) {
            try {
                SqlCommand command = new SqlCommand(statement, con);
                con.Open();
                command.ExecuteNonQuery();
                con.Close();
            } catch (Exception) {
                MessageBox.Show("Could not run SQL query: " + statement, "SQL Error", MessageBoxButtons.OK);
                return;
            }
        }
    }
}
