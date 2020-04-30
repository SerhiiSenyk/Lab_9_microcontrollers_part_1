using System;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
//17:48
namespace WindowsFormsAppMCLab9
{

    public partial class Form1 : Form
    {
        private const byte commandRead = 0xB1;
        private const byte commandWrite = 0xA1;
        private const byte slave_1_address = 0x43;
        private const byte slave_2_address = 0x45;
        private const int delay = 100;
        public Form1()
        {
            InitializeComponent();
            TextBox.CheckForIllegalCrossThreadCalls = false;

        }

        private void buttonOpenPort_Click(object sender, EventArgs e)
        {

            if (!serialPort1.IsOpen)
                try
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.Open();
                    buttonOpenPort.Text = "Close";
                    comboBox1.Enabled = false;
                    buttonSlave1.Visible = true;
                    buttonSlave2.Visible = true;
                    textBoxSlave1.Visible = true;
                    textBoxSlave2.Visible = true;
                    buttonClear1.Visible = true;
                    buttonClear2.Visible = true;
                }
                catch
                {
                    MessageBox.Show("Port " + comboBox1.Text + " is invalid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            else
            {
                serialPort1.Close();
                buttonOpenPort.Text = "Open";
                comboBox1.Enabled = true;
                buttonSlave1.Visible = false;
                buttonSlave2.Visible = false;
                textBoxSlave1.Visible = false;
                textBoxSlave2.Visible = false;
                buttonClear1.Visible = false;
                buttonClear2.Visible = false;
            }
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(SerialPort.GetPortNames().ToArray());
        }



        private void buttonSlave1_Click(object sender, EventArgs e)
        {
            byte[] b1 = new byte[1];//2
            b1[0] = slave_1_address;
            serialPort1.Write(b1, 0, 1);
            System.Threading.Thread.Sleep(delay);
            b1[0] = commandRead;
            serialPort1.Write(b1, 0, 1);

            textBoxSlave1.Text = serialPort1.ReadExisting();
            
            
            serialPort1.DiscardInBuffer();
            
            serialPort1.BaseStream.Flush();
        }

        private void buttonSlave2_Click(object sender, EventArgs e)
        {
            byte[] b1 = new byte[1];
            b1[0] = slave_2_address;
            serialPort1.Write(b1, 0, 1);
            System.Threading.Thread.Sleep(delay);
            b1[0] = commandRead;
            serialPort1.Write(b1, 0, 1);

            textBoxSlave2.Text = serialPort1.ReadExisting();

            serialPort1.DiscardInBuffer();
            serialPort1.BaseStream.Flush();  
        }

        private void buttonClear1_Click(object sender, EventArgs e)
        {
            textBoxSlave1.Text = "";
        }

        private void buttonClear2_Click(object sender, EventArgs e)
        {
            textBoxSlave2.Text = "";
        }
    }
}
