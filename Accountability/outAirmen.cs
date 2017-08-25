using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Accountability
{
    public partial class outAirmen : Form
    {
        public outAirmen()
        {
            InitializeComponent();
        }
        SqlConnection con;
        private void outAirmen_Load(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            // Connect to database
            con = new SqlConnection {
                ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = " + Application.StartupPath + @"\Accountability.mdf; Integrated Security = True"
            };

            SqlCommand command = new SqlCommand("SELECT * FROM [dbo].[Airmen] WHERE [inHouse] = 'False' ORDER BY [room] ASC;", con);
            con.Open();

            SqlDataReader reader = command.ExecuteReader();
            int swingsOut = 0;
            int daysOut = 0;
            int midsOut = 0;
            try
            {
                string name = null;
                string room = null;
                string mtl = null;
                string shift = null;
                while (reader.Read())
                {
                    name = reader["name"].ToString();
                    room = reader["room"].ToString();
                    mtl = reader["mtl"].ToString();
                    shift = reader["shift"].ToString();
                    if (shift.ToUpper().Trim(' ').Equals("MIDS"))
                        midsOut++;
                    if (shift.ToUpper().Trim(' ').Equals("DAYS"))
                        daysOut++;
                    if (shift.ToUpper().Trim(' ').Equals("SWINGS"))
                        swingsOut++;
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dataGridView1);
                    row.Cells[0].Value = name;                      //Name
                    row.Cells[1].Value = room;                      //Room
                    row.Cells[2].Value = shift;                //Shift
                    row.Cells[3].Value = mtl;                       //MTL
                    row.Cells[4].Value = "N/A";

                    /* TODO: 
                     * figure out joins to get the last scan of a user with a certain barcode
                     * (Requires joining on the right columns and possibly orderby)
                     * row.Cells[4].Value = dateString;                //Last Scan Time
                     */


                    dataGridView1.Rows.Add(row);
                }
                con.Close();
                lblMidsStatus.Text = "Mids: " + midsOut;
                lblDaysStatus.Text = "Days: " + daysOut;
                lblSwingsStatus.Text = "Swings: " + swingsOut;
            }
            finally
            {
                reader.Close();
            }
        }
    }
}
