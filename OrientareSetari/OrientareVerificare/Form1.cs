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
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbSerialPorts.DataSource = SerialPort.GetPortNames();
            Form1.CheckForIllegalCrossThreadCalls = false;
            toolTip1.SetToolTip(this.textBox1, "Numarul postului\n0 - Start\n1 - PC1\n....\nMaxim 13");
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
                toolStripStatusLabel1.Text = s.Replace("$","").Replace("\r","");
            } else
            {
                richTextBox1.AppendText(s);
                richTextBox1.ScrollToCaret();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            serialPort1.Dispose();
            richTextBox1.Text = "";

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialPort1.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            

            Int32 unixTimestamp = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            serialPort1.WriteLine("T" + unixTimestamp);
            //serialPort1.WriteLine("D" + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            serialPort1.WriteLine("F0,"+textBox1.Text);
            System.Threading.Thread.Sleep(2000);
            serialPort1.Close();
            serialPort1.Open();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("R");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.WriteLine("r");
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.WriteLine("E");
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.WriteLine("e");
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine(textBox2.Text);
           // System.Threading.Thread.Sleep(2000);
           // serialPort1.Close();
           // serialPort1.Open();

        }
    }
}
