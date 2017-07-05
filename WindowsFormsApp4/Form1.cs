using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        //Fix this!
        String cons;
        String unitUpdate;
        String unitFill = @"SELECT DISTINCT Unit FROM InvList";
        String customerFill = @"SELECT DISTINCT Customer FROM InvList WHERE
               Project NOT LIKE '' AND Customer NOT LIKE '';";
        String projectFill = @"SELECT DISTINCT Project FROM InvList";
        String unitCond = "Unit";
        string cond = "";
        String project = "";
        String customer = "";
        String unit = "";

        public Form1()
        {
            InitializeComponent();
            DataClass data = new DataClass();

            // Fill the comboboxes initially
            data.comboFill(unitFill, comboBox1);
            data.comboFill(customerFill, comboBox2);
            data.comboFill(projectFill, comboBox3);

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        public class DataClass
        {
            private SQLiteConnection sqlite;

            public DataClass()
            {
                sqlite = new SQLiteConnection("Data Source=C:/sqlite/NSM20.sqlite;Version=3");
            }

            public DataSet selectQuery(String query, String unit, String customer, String project)
            {
                SQLiteDataAdapter ad;
                DataSet dt = new DataSet();

                SQLiteCommand cmd;
                sqlite.Open();  //Initiate connection to the db
                cmd = sqlite.CreateCommand();
                
                //Adding SQL parameters
                cmd.Parameters.AddWithValue("@VarUnit", unit);
                cmd.Parameters.AddWithValue("@VarCustomer", customer);
                cmd.Parameters.AddWithValue("@VarProject", project);
                cmd.CommandText = query;  //set the passed query

                ad = new SQLiteDataAdapter(cmd);
                ad.Fill(dt); //fill the datasource
                //MessageBox.Show(dt.Tables[0].Select("Unit is not NULL").Length.ToString());

                sqlite.Close();
                return dt;
            }
            public void comboFill(String query, ComboBox comboBox)
            {
                sqlite.Open();
                SQLiteCommand cmd = new SQLiteCommand(query, sqlite);
                SQLiteDataReader DR = cmd.ExecuteReader();

                while (DR.Read())
                {
                    comboBox.Items.Add(DR[0]);
                }
                sqlite.Close();
            }
            public void comboConnect(String query, String unit, String customer, String project, ComboBox comboBox)
            {
                sqlite.Open();
                SQLiteCommand cmd;
                cmd = sqlite.CreateCommand();

                comboBox.Items.Clear();

                cmd = new SQLiteCommand(query, sqlite);
                //Adding SQL parameters
                cmd.Parameters.AddWithValue("@varUnit", unit);
                cmd.Parameters.AddWithValue("@varCustomer", customer);
                cmd.Parameters.AddWithValue("@varProject", project);
                cmd.CommandText = query;  //set the passed query

               SQLiteDataReader DR = cmd.ExecuteReader();

                while (DR.Read())
                {
                    comboBox.Items.Add(DR[0]);
                }
                sqlite.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            project = "";
            customer = "";
            unit = "";

            if(comboBox1.SelectedItem != null)
            {
                unit = comboBox1.Text;
            }

            if (comboBox2.SelectedItem != null)
            {
                customer = comboBox2.Text;
            }

            if (comboBox3.SelectedItem != null)
            {
                project = comboBox3.Text;
            }

            cons = @"SELECT * FROM InvList WHERE Unit LIKE @varUnit || '%' AND Customer LIKE @varCustomer || '%' AND Project LIKE @varProject || '%';";
            
            //string addText = textBox1.Text;
            //comboBox1.Items.Add(addText);
            DataClass test1 = new DataClass();

            dataGridView1.DataSource = test1.selectQuery(cons, unit, customer, project).Tables[0];

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                DataClass combo = new DataClass();

                project = comboBox3.Text;
                customer = comboBox2.Text;
                unit = comboBox1.Text;
                String query1 = "";
                String query2 = "";

                if (comboBox2.SelectedItem == null && comboBox3.SelectedItem == null)
                {
                    query1 = @"SELECT Project FROM Console WHERE ConUnitNo = @varUnit UNION SELECT Project FROM MainUnit WHERE MainUnitNo = @varUnit
                                UNION SELECT Project FROM SubUnit WHERE SubUnitNo = @varUnit;";
                    query2 = @"SELECT Customer FROM Console WHERE ConUnitNo = @varUnit UNION SELECT Customer FROM MainUnit WHERE MainUnitNo = @varUnit
                                UNION SELECT Customer FROM SubUnit WHERE SubUnitNo = @varUnit;";
                    combo.comboConnect(query2, unit, customer, project, comboBox2);
                    combo.comboConnect(query1, unit, customer, project, comboBox3);
                }
                else if(comboBox2.SelectedItem != null && comboBox3.SelectedItem == null)
                {
                    query1 = @"SELECT Project FROM Console WHERE ConUnitNo = @varUnit AND Customer = @varCustomer 
                                    UNION SELECT Project FROM MainUnit WHERE MainUnitNo = @varUnit AND Customer = @varCustomer
                                    UNION SELECT Project FROM SubUnit WHERE SubUnitNo = @varUnit AND Customer = @varCustomer;";
                    combo.comboConnect(query1, unit, customer, project, comboBox3);
                }
                else if(comboBox3.SelectedItem != null && comboBox2.SelectedItem == null)
                {
                    query1 = @"SELECT Customer FROM Console WHERE ConUnitNo = @varUnit AND Project = @varProject 
                                    UNION SELECT Customer FROM MainUnit WHERE MainUnitNo = @varUnit AND Project = @varProject
                                    UNION SELECT Customer FROM SubUnit WHERE SubUnitNo = @varUnit AND Project = @varProject;";
                    combo.comboConnect(query1, unit, customer, project, comboBox2);
                }

            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
            {
                DataClass combo = new DataClass();

                project = comboBox3.Text;
                customer = comboBox2.Text;
                unit = comboBox1.Text;
                String query1 = "";
                String query2 = "";
                string cond = "";

                if (comboBox1.SelectedItem == null && comboBox3.SelectedItem == null)
                {
                    cond = "Customer";
                    if (unitCond != "Unit")
                    {
                        query1 = unitBuild(/*cond*/);
                    }
                    else
                    {
                        query1 = @"SELECT ConUnitNo FROM Console WHERE Customer = @varCustomer UNION SELECT MainUnitNo FROM MainUnit WHERE Customer = @varCustomer
                                UNION SELECT SubUnitNo FROM SubUnit WHERE Customer = @varCustomer;";
                    }
                    query2 = @"SELECT Project FROM Console WHERE Customer = @varCustomer UNION SELECT Project FROM MainUnit WHERE Customer = @varCustomer
                                UNION SELECT Project FROM SubUnit WHERE Customer = @varCustomer;";
                    combo.comboConnect(query2, unit, customer, project, comboBox3);
                    combo.comboConnect(query1, unit, customer, project, comboBox1);
                }
                else if (comboBox1.SelectedItem != null && comboBox3.SelectedItem == null)
                {
                    query1 = @"SELECT Project FROM Console WHERE ConUnitNo = @varUnit AND Customer = @varCustomer 
                                    UNION SELECT Project FROM MainUnit WHERE MainUnitNo = @varUnit AND Customer = @varCustomer
                                    UNION SELECT Project FROM SubUnit WHERE SubUnitNo = @varUnit AND Customer = @varCustomer;";
                    combo.comboConnect(query1, unit, customer, project, comboBox3);
                }
                else if (comboBox3.SelectedItem != null && comboBox1.SelectedItem == null)
                { 
                    cond = "Project,Customer";
                    if (unitCond != "Unit")
                    {
                        query1 = unitBuild(/*cond*/);
                    }
                    else
                    {
                        query1 = @"SELECT ConUnitNo FROM Console WHERE Customer = @varCustomer AND Project = @varProject 
                                    UNION SELECT MainUnitNo FROM MainUnit WHERE Customer = @varCustomer AND Project = @varProject
                                    UNION SELECT SubUnitNo FROM SubUnit WHERE Customer = @varCustomer AND Project = @varProject;";
                    }
                    combo.comboConnect(query1, unit, customer, project, comboBox1);
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem != null)
            {
                DataClass combo = new DataClass();

                String project = comboBox3.Text;
                String customer = comboBox2.Text;
                String unit = comboBox1.Text;
                String query1 = "";
                String query2 = "";
                //string cond = "";

                if (comboBox1.SelectedItem == null && comboBox2.SelectedItem == null)
                {
                    cond = "Project";
                    if (unitCond != "Unit")
                    {
                        query1 = unitBuild(/*cond*/);
                    }
                    else
                    {
                        query1 = @"SELECT ConUnitNo FROM Console WHERE Project = @varProject UNION SELECT MainUnitNo FROM MainUnit WHERE Project = @varProject
                                UNION SELECT SubUnitNo FROM SubUnit WHERE Project = @varProject;";
                    }
                    query2 = @"SELECT Customer FROM Console WHERE Project = @varProject UNION SELECT Customer FROM MainUnit WHERE Project = @varProject
                                UNION SELECT Customer FROM SubUnit WHERE Project = @varProject;";
                    combo.comboConnect(query2, unit, customer, project, comboBox2);
                    combo.comboConnect(query1, unit, customer, project, comboBox1);
                }
                else if (comboBox1.SelectedItem != null && comboBox2.SelectedItem == null)
                {
                    query1 = @"SELECT Customer FROM Console WHERE ConUnitNo = @varUnit AND Project = varProject 
                                    UNION SELECT Customer FROM MainUnit WHERE MainUnitNo = @varUnit AND Project = varProject
                                    UNION SELECT Customer FROM SubUnit WHERE SubUnitNo = @varUnit AND Project = varProject;";
                    combo.comboConnect(query1, unit, customer, project, comboBox2);
                }
                else if (comboBox2.SelectedItem != null && comboBox1.SelectedItem == null)
                {

                    if (unitCond != "Unit")
                    {
                        query1 = unitBuild(/*cond*/);
                    }
                    else
                    {
                        query1 = @"SELECT ConUnitNo FROM Console WHERE Customer = @varCustomer AND Project = @varProject 
                                    UNION SELECT MainUnitNo FROM MainUnit WHERE Customer = @varCustomer AND Project = @varProject
                                    UNION SELECT SubUnitNo FROM SubUnit WHERE Customer = varCustomer AND Project = @varProject;";
                    }
                    combo.comboConnect(query1, unit, customer, project, comboBox1);
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {                           
            if (radioButton1.Checked)
            {
                unitCond = "Unit";
                unitUpdate = @"SELECT ConUnitNo FROM Console UNION SELECT MainUnitNo FROM MainUnit
                               UNION SELECT SubUnitNo FROM SubUnit;"; // Må også innom unitBuilder!
                updateCombo(unitUpdate);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            DataClass combo = new DataClass();
            project = comboBox3.Text;
            customer = comboBox2.Text;
            unit = comboBox1.Text;
            if (radioButton2.Checked)
            {
                unitCond = "ConUnitNo";
                unitUpdate = unitBuild();
                //unitUpdate = @"SELECT DISTINCT ConUnitNo FROM Console";
                combo.comboConnect(unitUpdate, unit, customer, project, comboBox1);
                //updateCombo(unitUpdate);
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            DataClass combo = new DataClass();
            project = comboBox3.Text;
            customer = comboBox2.Text;
            unit = comboBox1.Text;

            if (radioButton3.Checked)
            {
                unitCond = "MainUnitNo";
                unitUpdate = unitBuild();
                //unitUpdate = @"SELECT DISTINCT MainUnitNo FROM MainUnit";
                //updateCombo(unitUpdate);
                combo.comboConnect(unitUpdate, unit, customer, project, comboBox1);
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            DataClass combo = new DataClass();
            project = comboBox3.Text;
            customer = comboBox2.Text;
            unit = comboBox1.Text;

            if (radioButton4.Checked)
            {
                unitCond = "SubUnitNo";
                unitUpdate = unitBuild();
                //unitUpdate = @"SELECT DISTINCT SubUnitNo FROM SubUnit";
                //updateCombo(unitUpdate);               
                combo.comboConnect(unitUpdate, unit, customer, project, comboBox1);
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        public void updateCombo(String query)
        {
            DataClass data = new DataClass();
            unitFill = query;
            comboBox1.Items.Clear();
            data.comboFill(unitFill, comboBox1);
            //MessageBox.Show(query);
        }

        public String unitBuild(/*string cond*/)
        {
            string par1 = "";
            string par2 = "";
            string unitSql = "";
            string[] sqlParts = cond.Split(',');

            if(sqlParts.Length < 2)
            {
                par1 = sqlParts[0];

                if (!unitCond.Equals("ConUnitNo"))
                {
                    unitSql = @"SELECT " + unitCond + " FROM " + unitCond.Replace("No", string.Empty) + " WHERE " + par1 + " = @var" + par1 + ";";
                }
                else
                {
                    unitSql = @"SELECT " + unitCond + " FROM Console WHERE " + par1 + " = @var" + par1 + ";";
                }
            }
            else
            {
                par1 = sqlParts[0];
                par2 = sqlParts[1];

                if (!unitCond.Equals("ConUnitNo"))
                {
                    unitSql = @"SELECT " + unitCond + " FROM " + unitCond.Replace("No", string.Empty) + " WHERE " + par1 + " = @var" + par1 + " AND " + par2 + " = @var" + par2 + ";";
                }
                else
                {
                    unitSql = @"SELECT " + unitCond + " FROM Console WHERE " + par1 + " = @var" + par1 + " AND " + par2 + " = @var" + par2 + ";";
                }
            }
            if(sqlParts[0] == "")
            {
                if (!unitCond.Equals("ConUnitNo"))
                {
                    unitSql = @"SELECT " + unitCond + " FROM " + unitCond.Replace("No", string.Empty) + "";
                }
                else
                {
                    unitSql = @"SELECT " + unitCond + " FROM Console";
                }
            }
            return unitSql;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
