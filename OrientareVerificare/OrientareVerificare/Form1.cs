using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace OrientareVerificare
{
    public partial class Form1 : Form
    {
        string cardUID = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbSerialPorts.DataSource = SerialPort.GetPortNames();
            Form1.CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cmbSerialPorts.SelectedIndex > -1)
            {
                //MessageBox.Show(String.Format("You selected port '{0}'", cmbSerialPorts.SelectedItem));
                serialPort1.PortName = cmbSerialPorts.SelectedItem.ToString();
                
                try
                {
                    serialPort1.Open();
                }
                catch { Exception ex; }
                finally
                {
                    if (serialPort1 != null)
                    {
                        if (serialPort1.IsOpen)
                        {
                            richTextBox1.AppendText(serialPort1.PortName + " port opened.\n");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a port first");
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string s = serialPort1.ReadLine();
            if(s.IndexOf("$") == 0)
            {
                s = s.Replace("$", "");
                s = s.Replace("\r", "");
                using (System.IO.StreamWriter file =
                               new System.IO.StreamWriter(@"D:\Stafeta\log.csv", true))
                {
                    file.WriteLine(s);
                }
                Console.WriteLine(s);
                string[] split = s.Split(',');
                //foreach (string item in split)
                //{
                    dataGridView1.Rows.Add(split);
                //}
            } else
            {
                if(s.IndexOf("Card UID: ") > -1)
                {
                    cardUID = s.Substring(10, 8);
                    using (System.IO.StreamWriter file =
                               new System.IO.StreamWriter(@"D:\Stafeta\" + cardUID + ".txt", false)) ;
                }
                if (cardUID != "")
                {
                    using (System.IO.StreamWriter file =
                               new System.IO.StreamWriter(@"D:\Stafeta\" + cardUID + ".txt", true))
                    {
                        file.Write(s);
                    }
                }
                richTextBox1.AppendText(s);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            serialPort1.Dispose();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialPort1.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string filename = "";
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV (*.csv)|*.csv";
            sfd.FileName = "Output.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Data will be exported and you will be notified when it is ready.");
                if (File.Exists(filename))
                {
                    try
                    {
                        File.Delete(filename);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                    }
                }
                int columnCount = dataGridView1.ColumnCount;
                string columnNames = "";
                string[] output = new string[dataGridView1.RowCount + 1];
                for (int i = 0; i < columnCount; i++)
                {
                    columnNames += dataGridView1.Columns[i].Name.ToString() + ",";
                }
                output[0] += columnNames;
                for (int i = 1; i  < dataGridView1.RowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        output[i] += dataGridView1.Rows[i - 1].Cells[j].Value.ToString() + ",";
                    }
                }
                System.IO.File.WriteAllLines(sfd.FileName, output, System.Text.Encoding.UTF8);
                MessageBox.Show("Your file was generated and its ready for use.");
            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("F1,"+textBox1.Text);
            System.Threading.Thread.Sleep(2000);
            serialPort1.Close();
            serialPort1.Open();
        }
    }
}
