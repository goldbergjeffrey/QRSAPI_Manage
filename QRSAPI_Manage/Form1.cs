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
        QlikSdkDoStuff sdkConnect = new QlikSdkDoStuff();
        Boolean connected;
        private List<string> contentToUpload = new List<string>();
        public Form1()
        {
            InitializeComponent();
            if (QRS_Manage.Default.serverUrl != null)
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
                MessageBox.Show("Please enter the appropriate credentials in to the QRS Connect section.", "Alert", MessageBoxButtons.OK);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
                connectQRS();
        }

        private void btnBrowseUploadFileName_Click(object sender, EventArgs e)
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
                    txtUploadFileName.Text = openFileDialog1.FileName;
                }
            }

            if (txtUploadFileName.Text != null)
            {
                //this is where we need to access the app and find the content to upload
                string strFileNameWithPath = txtUploadFileName.Text;
                string[] strFileName = strFileNameWithPath.Split('\\');
                string appName = strFileName[strFileName.Length - 1].ToString();
                string result = "";
                bool connectSenseDesktop = sdkConnect.SenseDesktopConnect(); 
                if(connectSenseDesktop)
                {
                    result =  "Result from sdk connection: " + sdkConnect.connectResult + System.Environment.NewLine;
                    result += sdkConnect.getApp(appName) + System.Environment.NewLine;
                    sdkConnect.getImageList();
                    //Now get the content list from the Qlik Sense server to make sure there are no duplicates
                    if (!connected)
                    {
                        connectQRS();
                    }
                    textBox4.Text = "Content List for Default" + System.Environment.NewLine;
                    textBox4.Text += "********************************" + System.Environment.NewLine;
                    textBox4.Text += qrsConnect.getContentLibraryContents("Default") + System.Environment.NewLine;
                    textBox4.Text += "********************************" + System.Environment.NewLine;
                    List<string> contentItems = qrsConnect.contentFileNames(qrsConnect.contentItems);


                    foreach (string item in sdkConnect.imgList)
                    {
                        string[] strFilePath = item.Split('\\');
                        string itemName = strFilePath[strFilePath.Length - 1];

                        if (!contentItems.Contains(itemName))
                        {
                            result += "item: " + item + " will be uploaded to the server" + System.Environment.NewLine;
                            contentToUpload.Add(item);
                        }
                        else
                        {
                            result += "item: " + item + " will not be uploaded because it exists on the server" + System.Environment.NewLine;
                        }
                    }

                    txtUploadInfo.Text = result;
                }
                else
                {
                    result = sdkConnect.connectResult;
                    txtUploadInfo.Text = result;
                }
            }
        }

        private void btnUploadFile_Click(object sender, EventArgs e)
        {
            if (txtUploadFileName.Text != null)
            {
                //this is where we need to add the logic for properly uploading the app and the content.
               
                //Evaluate if app exists on server

                //Evaluate if content exists on server
                //Get the content from the server
                textBox4.Text = qrsConnect.uploadApp(txtUploadFileName.Text);
                foreach(string item in contentToUpload)
                {
                    textBox4.Text += qrsConnect.uploadFileToContentLibrary(item, "Default", true);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.InitialDirectory = Environment.SpecialFolder.MyDocuments + "\\Qlik\\Sense\\Content";
            openFileDialog2.Filter = "bmp files (*.bmp)|*.bmp|png files (*.png)|*.png|All files (*.*)|*.*";
            openFileDialog2.FilterIndex = 1;
            openFileDialog2.RestoreDirectory = false;

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog2.FileName != null)
                {
                    //textBox6.Text = openFileDialog2.FileName;
                }
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
  
        private void button6_Click(object sender, EventArgs e)
        {
            List<QRSDoStuff.stream> streams = qrsConnect.existingStreams;
            List<QRSDoStuff.app> unpublishedApps = qrsConnect.unpublishedApps;

            string selectedStreamID = streams.Find(x => x.name.Equals(comboBox1.Text)).ID;
            string selectedAppID = unpublishedApps.Find(x => x.name.Equals(comboBox2.Text)).ID;

            textBox4.Text = qrsConnect.publishApp(selectedAppID, selectedStreamID);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                label19.Show();
                textBox11.Show();
            }
            else
            {
                label19.Hide();
                textBox11.Hide();
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
            string result = "";
            for (int i = 0; i < selectedListBox.Items.Count; i++)
            {
                if (selectedListBox.GetSelected(i))
                {
                    result += i + ",";
                }
            }
            result = result.Substring(0, result.Length - 1);
            return result;
        }

        

        private void btnCreateTrigger_Click(object sender, EventArgs e)
        {
            string name = "";
            string schemaFilterDescription = "";
            string incrementDescription = "";
            int incrementOption;

            string selectedAppID = qrsConnect.allApps.Find(x => x.name.Equals(comboBox3.Text)).ID;

            
            switch (tabControl2.SelectedIndex)
            {
                case 0:
                    incrementOption = 0;
                    incrementDescription = "0 0 0 0";
                    schemaFilterDescription = "* * - * * * * *";
                    name = "Once";
                    textBox4.Text = qrsConnect.createTrigger(textBox12.Text, qrsConnect.getApp(selectedAppID), name, schemaFilterDescription, incrementDescription, incrementOption);
                    break;
                case 1:
                    incrementOption = 1;
                    incrementDescription = textBox8.Text + " " + textBox7.Text + " 0 0";
                    schemaFilterDescription = "* * - * * * * *";
                    name = "Hourly";
                    textBox4.Text = qrsConnect.createTrigger(textBox12.Text, qrsConnect.getApp(selectedAppID), name, schemaFilterDescription, incrementDescription, incrementOption);
                    break;
                case 2:
                    incrementOption = 2;
                    incrementDescription = "0 0 " + textBox10.Text + " 0";
                    schemaFilterDescription = "* * - * * * * *";
                    name = "Daily";
                    textBox4.Text = qrsConnect.createTrigger(textBox12.Text, qrsConnect.getApp(selectedAppID), name, schemaFilterDescription, incrementDescription, incrementOption);
                    break;
                case 3:
                    incrementOption = 3;
                    incrementDescription = "0 0 1 0";
                    schemaFilterDescription = "* * - " + numericString(listBox2) + " " + textBox9.Text + " * * *";
                    name = "Weekly";
                    textBox4.Text = qrsConnect.createTrigger(textBox12.Text, qrsConnect.getApp(selectedAppID), name, schemaFilterDescription, incrementDescription, incrementOption);
                    break;
                case 4:
                    incrementOption = 4;
                    incrementDescription = "0 0 1 0";
                    schemaFilterDescription = "* * - * * " + numericString(listBox3) + " * *";
                    name = "Monthly";
                    textBox4.Text = qrsConnect.createTrigger(textBox12.Text, qrsConnect.getApp(selectedAppID), name, schemaFilterDescription, incrementDescription, incrementOption);
                    break;
                default:
                    break;
            }
        }


        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    popPublishAppTab();
                    break;
                case 3:
                    popTaskTab();
                    break;
                default:
                    break;
            }

        }


        private void popPublishAppTab()
        {
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

            comboBox2.Items.Clear();
            comboBox1.Items.Clear();
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

        private void popTaskTab()
        {

            comboBox3.Items.Clear();


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

        private void button2_Click(object sender, EventArgs e)
        {
            if (!connected)
            {
                connectQRS();
            }

            textBox4.Text = "Content List for Default" + System.Environment.NewLine;
            textBox4.Text += "********************************" + System.Environment.NewLine;
            textBox4.Text += qrsConnect.getContentLibraryContents("Default") + System.Environment.NewLine;
            textBox4.Text += "********************************" + System.Environment.NewLine;

        }

 

    }

    #region commentedCode
        //UPLOAD TO SELECTED CONTENT LIBRARY
        /*           panel1.Hide();
                          panel2.Show();
                          panel3.Hide();
                          panelTasks.Hide();
                          if (!connected)
                          {
                              connectQRS();
                          }
                          cboContentLibrary.Items.Clear();
                          textBox4.Text = "Content Libraries" + System.Environment.NewLine;
                          textBox4.Text += "********************************" + System.Environment.NewLine;
                          textBox4.Text += qrsConnect.getContentLibraryList() + System.Environment.NewLine;
                          textBox4.Text += "********************************" + System.Environment.NewLine;
                          textBox4.Text += System.Environment.NewLine;

                          List<string> contentLibs = new List<string>();
                          foreach (QRSDoStuff.ContentLibrary item in qrsConnect.existingContentLibraries)
                          {
                              contentLibs.Add(item.name);
                          }
                          cboContentLibrary.Items.Add("Please select a content library to upload to");
                          cboContentLibrary.Items.AddRange(contentLibs.ToArray());
                          cboContentLibrary.SelectedIndex = 0;
        */


    #endregion


}

