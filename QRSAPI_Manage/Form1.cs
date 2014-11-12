using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace QRSAPI_Manage
{
    public partial class Form1 : Form
    {
        QRSDoStuff qrsConnect = new QRSDoStuff();
        Boolean connected;
        public Form1()
        {
            InitializeComponent();
            if(QRS_Manage.Default.serverUrl !=null)
            {
                textBox1.Text = QRS_Manage.Default.serverUrl;
                textBox2.Text = QRS_Manage.Default.serverHeaderName;
                textBox3.Text = QRS_Manage.Default.serverHeaderValue;
            }
            
        }

        private void connectQRS()
        {
            if (textBox1.Text != null && textBox2.Text != null && textBox3.Text != null)
            {
                string foo = qrsConnect.connectQrs(textBox1.Text, textBox2.Text, textBox3.Text);
                textBox4.Text = foo;
                connected = qrsConnect.qrsConnected;
            }
            else
            {
                MessageBox.Show("Please enter the appropriate credentials in to the QRS Connect section.","Alert", MessageBoxButtons.OK);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connectQRS();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.SpecialFolder.MyDocuments + "\\Qlik\\Sense\\Apps";
            openFileDialog1.Filter = "qvf files (*.qvf)|*.qvf|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName != null)
                {
                    textBox5.Text = openFileDialog1.FileName;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox5.Text != null)
            {

                textBox4.Text = qrsConnect.uploadApp(textBox5.Text);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                QRS_Manage.Default.serverUrl = textBox1.Text;
                QRS_Manage.Default.serverHeaderName = textBox2.Text;
                QRS_Manage.Default.serverHeaderValue = textBox3.Text;
                QRS_Manage.Default.Save();
            }

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            panel1.Show();
            panel2.Hide();
            panel3.Hide();
            panelTasks.Hide();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            panel1.Hide();
            panel2.Show();
            panel3.Hide();
            panelTasks.Hide();
            if (!connected)
            {
                connectQRS();
            } 

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            panel1.Hide();
            panel2.Hide();
            panel3.Show();
            panelTasks.Hide();


            if (!connected)
            {
                connectQRS();
            }
            textBox4.Text = "UNPUBLISHED APPS" + System.Environment.NewLine;
            textBox4.Text += "********************************" + System.Environment.NewLine;
            textBox4.Text += qrsConnect.getUnpublishedApps() + System.Environment.NewLine;
            textBox4.Text += "********************************" + System.Environment.NewLine;
            textBox4.Text += System.Environment.NewLine;
            textBox4.Text += "AVAILABLE STREAMS" + System.Environment.NewLine;
            textBox4.Text += "********************************" + System.Environment.NewLine;
            textBox4.Text += qrsConnect.getListOfStreams() + System.Environment.NewLine;
            textBox4.Text += "********************************" + System.Environment.NewLine;

            List<string> appsToLoad = new List<string>();
            foreach (QRSDoStuff.app item in qrsConnect.unpublishedApps)
            {
                appsToLoad.Add(item.name);
            }
            comboBox2.Items.Add("Please select an app to publish");
            comboBox2.Items.AddRange(appsToLoad.ToArray());
            comboBox2.SelectedIndex = 0;

            List<string> streamsToLoad = new List<string>();
            foreach (QRSDoStuff.stream item in qrsConnect.existingStreams)
            {
                streamsToLoad.Add(item.name);
            }
            comboBox1.Items.Add("Please select a stream to publish to");
            comboBox1.Items.AddRange(streamsToLoad.ToArray());
            comboBox1.SelectedIndex = 0;


        }

        private void button6_Click(object sender, EventArgs e)
        {
            List<QRSDoStuff.stream> streams = qrsConnect.existingStreams;
            List<QRSDoStuff.app> unpublishedApps = qrsConnect.unpublishedApps;

            string selectedStreamID = streams.Find(x=> x.name.Equals(comboBox1.Text)).ID;
            string selectedAppID = unpublishedApps.Find(x=> x.name.Equals(comboBox2.Text)).ID;

            textBox4.Text = qrsConnect.publishApp(selectedAppID,selectedStreamID);

            
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            panel1.Hide();
            panel2.Hide();
            panel3.Hide();
            panelTasks.Show();
            label9.Show();
            label20.Show();
            textBox12.Show();
            btnCreateTask.Show();
            checkBox3.Show();

            if (!connected)
            {
                connectQRS();
            }

            textBox4.Text = "All APPS" + System.Environment.NewLine;
            textBox4.Text += "********************************" + System.Environment.NewLine;
            textBox4.Text += qrsConnect.getAllApps() + System.Environment.NewLine;
            textBox4.Text += "********************************" + System.Environment.NewLine;

            List<string> appsToLoad = new List<string>();
            foreach (QRSDoStuff.app item in qrsConnect.allApps)
            {
                appsToLoad.Add(item.name);
            }
            
            comboBox3.Items.Add("Please select an app to create a task");
            comboBox3.Items.AddRange(appsToLoad.ToArray());
            comboBox3.SelectedIndex = 0;
            comboBox3.Show();
            
            

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                label19.Show();
                textBox11.Show();
                listTriggerType.Show();
            }
            else
            {
                label19.Hide();
                textBox11.Hide();
                listTriggerType.Hide();
            }
        }

        private void btnCreateTask_Click(object sender, EventArgs e)
        {
            QRSDoStuff.app selectedApp = qrsConnect.getApp(qrsConnect.allApps.Find(x => x.name.Equals(comboBox3.Text)).ID);
            string newTaskName = textBox12.Text;
            textBox4.Text = qrsConnect.createTask(newTaskName, selectedApp);
        }

        private string numericString(ListBox selectedListBox)
        {
            string result ="";
            for(int i =0;i<selectedListBox.Items.Count;i++)
            {
                if (selectedListBox.GetSelected(i))
                {
                    result += i + ",";
                }
            }
            result = result.Substring(0,result.Length-1);
            return result;
        }

        private void listTriggerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string valSelected = listTriggerType.SelectedItem.ToString();
            if (valSelected != null)
            {
                switch (valSelected)
                {
                    case "Once":
                        panelMonthly.Hide();
                        panelWeekly.Hide();
                        panelDaily.Hide();
                        panelHourly.Hide();
                        PanelOnce.Show();
                        break;
                    case "Hourly":
                        panelMonthly.Hide();
                        panelWeekly.Hide();
                        panelDaily.Hide();
                        panelHourly.Show();
                        PanelOnce.Hide();
                        break;
                    case "Daily":
                        PanelOnce.Hide();
                        panelWeekly.Hide();
                        panelDaily.Show();
                        panelHourly.Hide();
                        panelMonthly.Hide();
                        break;
                    case "Weekly":
                        PanelOnce.Hide();
                        panelWeekly.Show();
                        panelDaily.Hide();
                        panelHourly.Hide();
                        panelMonthly.Hide();
                        break;
                    case "Monthly":
                        PanelOnce.Hide();
                        panelWeekly.Hide();
                        panelDaily.Hide();
                        panelHourly.Hide();
                        panelMonthly.Show();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (listTriggerType.Visible)
                {
                    MessageBox.Show("Please select a trigger interval", "Alert", MessageBoxButtons.OK);
                }
            }
        }

        private void btnCreateTrigger_Click(object sender, EventArgs e)
        {
            string valSelected = listTriggerType.SelectedItem.ToString();
            string name = "";
            string schemaFilterDescription = "";
            string incrementDescription = "";
            int incrementOption;

            string selectedAppID = qrsConnect.allApps.Find(x => x.name.Equals(comboBox3.Text)).ID;

            if (valSelected != null)
            {
                switch (valSelected)
                {
                    case "Once":
                        incrementOption = 0;
                        incrementDescription = "0 0 0 0";
                        schemaFilterDescription = "* * - * * * * *";
                        name = "Once";
                        textBox4.Text = qrsConnect.createTrigger(textBox12.Text, qrsConnect.getApp(selectedAppID), name, schemaFilterDescription, incrementDescription, incrementOption);
                        break;
                    case "Hourly":
                        incrementOption = 1;
                        incrementDescription = textBox8.Text + " " + textBox7.Text + " 0 0";
                        schemaFilterDescription = "* * - * * * * *";
                        name = "Hourly";
                        textBox4.Text = qrsConnect.createTrigger(textBox12.Text, qrsConnect.getApp(selectedAppID), name, schemaFilterDescription, incrementDescription, incrementOption);
                        break;
                    case "Daily":
                        incrementOption = 2;
                        incrementDescription = "0 0 " + textBox10.Text + " 0";
                        schemaFilterDescription = "* * - * * * * *";
                        name = "Daily";
                        textBox4.Text = qrsConnect.createTrigger(textBox12.Text, qrsConnect.getApp(selectedAppID), name, schemaFilterDescription, incrementDescription, incrementOption);
                        break;
                    case "Weekly":
                        incrementOption = 3;
                        incrementDescription = "0 0 1 0";
                        schemaFilterDescription = "* * - " + numericString(listBox2) + " " + textBox9.Text + " * * *";
                        name = "Weekly";
                        textBox4.Text = qrsConnect.createTrigger(textBox12.Text, qrsConnect.getApp(selectedAppID), name, schemaFilterDescription, incrementDescription, incrementOption);
                        break;
                    case "Monthly":
                        incrementOption = 4;
                        incrementDescription = "0 0 1 0";
                        schemaFilterDescription = "* * - * * " + numericString(listBox3) + " * *";
                        name = "Monthly";
                        textBox4.Text = qrsConnect.createTrigger(textBox12.Text, qrsConnect.getApp(selectedAppID), name, schemaFilterDescription, incrementDescription, incrementOption);
                        break;
                    default:
                        break;

                }
                qrsConnect.createdTask = null;
            }            

        }
    }
    
}
