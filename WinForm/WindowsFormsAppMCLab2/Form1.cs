using System;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsAppMCLab9
{

    public partial class Form1 : Form
    {
        private const byte commandRead = 0xB1;
        private const byte commandWrite = 0xA1;
        private const byte slave_1_address = 0x43;
        private const byte slave_2_address = 0x45;
        private const int delay = 100;
        private int flag = 0;
        private static int countBytes = 0;
        private static int countBlocks = 0;
        private string message = "";
        private int countDistortionSlave1 = 0;
        private int countDistortionSlave2 = 0;
        private byte[] block = new byte[6];
        private readonly byte[] CRC_8_ROHS_table =  {
            0x0, 0x91, 0xe3, 0x72, 0x7, 0x96, 0xe4, 0x75, 0xe, 0x9f, 0xed, 0x7c, 0x9,
            0x98, 0xea, 0x7b, 0x1c, 0x8d, 0xff, 0x6e, 0x1b, 0x8a, 0xf8, 0x69, 0x12, 0x83, 0xf1, 0x60, 0x15,
            0x84, 0xf6, 0x67, 0x38, 0xa9, 0xdb, 0x4a, 0x3f, 0xae, 0xdc, 0x4d, 0x36, 0xa7, 0xd5, 0x44, 0x31,
            0xa0, 0xd2, 0x43, 0x24, 0xb5, 0xc7, 0x56, 0x23, 0xb2, 0xc0, 0x51, 0x2a, 0xbb, 0xc9, 0x58, 0x2d,
            0xbc, 0xce, 0x5f, 0x70, 0xe1, 0x93, 0x2, 0x77, 0xe6, 0x94, 0x5, 0x7e, 0xef, 0x9d, 0xc, 0x79,
            0xe8, 0x9a, 0xb, 0x6c, 0xfd, 0x8f, 0x1e, 0x6b, 0xfa, 0x88, 0x19, 0x62, 0xf3, 0x81, 0x10, 0x65,
            0xf4, 0x86, 0x17, 0x48, 0xd9, 0xab, 0x3a, 0x4f, 0xde, 0xac, 0x3d, 0x46, 0xd7, 0xa5, 0x34, 0x41,
            0xd0, 0xa2, 0x33, 0x54, 0xc5, 0xb7, 0x26, 0x53, 0xc2, 0xb0, 0x21, 0x5a, 0xcb, 0xb9, 0x28, 0x5d,
            0xcc, 0xbe, 0x2f, 0xe0, 0x71, 0x3, 0x92, 0xe7, 0x76, 0x4, 0x95, 0xee, 0x7f, 0xd, 0x9c, 0xe9, 0x78,
            0xa, 0x9b, 0xfc, 0x6d, 0x1f, 0x8e, 0xfb, 0x6a, 0x18, 0x89, 0xf2, 0x63, 0x11, 0x80, 0xf5, 0x64,
            0x16, 0x87, 0xd8, 0x49, 0x3b, 0xaa, 0xdf, 0x4e, 0x3c, 0xad, 0xd6, 0x47, 0x35, 0xa4, 0xd1, 0x40,
            0x32, 0xa3, 0xc4, 0x55, 0x27, 0xb6, 0xc3, 0x52, 0x20, 0xb1, 0xca, 0x5b, 0x29, 0xb8, 0xcd, 0x5c,
            0x2e, 0xbf, 0x90, 0x1, 0x73, 0xe2, 0x97, 0x6, 0x74, 0xe5, 0x9e, 0xf, 0x7d, 0xec, 0x99, 0x8, 0x7a,
            0xeb, 0x8c, 0x1d, 0x6f, 0xfe, 0x8b, 0x1a, 0x68, 0xf9, 0x82, 0x13, 0x61, 0xf0, 0x85, 0x14, 0x66,
            0xf7, 0xa8, 0x39, 0x4b, 0xda, 0xaf, 0x3e, 0x4c, 0xdd, 0xa6, 0x37, 0x45, 0xd4, 0xa1, 0x30, 0x42,
            0xd3, 0xb4, 0x25, 0x57, 0xc6, 0xb3, 0x22, 0x50, 0xc1, 0xba, 0x2b, 0x59, 0xc8, 0xbd, 0x2c, 0x5e, 0xcf
      };
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
                    textBoxMessageSlave1.Visible = true;
                    textBoxMessageSlave2.Visible = true;
                    labelSlave1.Visible = true;
                    labelSlave2.Visible = true;
                    countBytes = 0;
                    countBlocks = 0;
                    flag = 0;
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
                textBoxMessageSlave1.Visible = false;
                textBoxMessageSlave2.Visible = false;
                labelSlave1.Visible = false;
                labelSlave2.Visible = false;
            }
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(SerialPort.GetPortNames().ToArray());
        }

        private void buttonSlave1_Click(object sender, EventArgs e)
        {
            flag = 1;
            countBlocks = 0;
            byte[] b1 = new byte[1];//2
            b1[0] = slave_1_address;
            serialPort1.Write(b1, 0, 1);
            System.Threading.Thread.Sleep(delay);
            b1[0] = commandRead;
            serialPort1.Write(b1, 0, 1);

            ++countDistortionSlave1;
            labelSlave1.Text = countDistortionSlave1.ToString();
            if (countDistortionSlave1 == 3)
            {
                countDistortionSlave1 = 0;
            }

            if (textBoxSlave1.Text != "")
                textBoxSlave1.AppendText(Environment.NewLine);

            if (textBoxMessageSlave1.Text != "")
                textBoxMessageSlave1.AppendText(Environment.NewLine);
            textBoxMessageSlave1.Text = "";

            serialPort1.DiscardInBuffer();
  
            serialPort1.BaseStream.Flush();
        }

        private void buttonSlave2_Click(object sender, EventArgs e)
        {
            flag = 2;
            countBlocks = 0;
            byte[] b1 = new byte[1];
            b1[0] = slave_2_address;
            serialPort1.Write(b1, 0, 1);
            System.Threading.Thread.Sleep(delay);
            b1[0] = commandRead;
            serialPort1.Write(b1, 0, 1);
            ++countDistortionSlave2;
            labelSlave2.Text = countDistortionSlave2.ToString();
            if (countDistortionSlave2 == 3)
            {
                countDistortionSlave2 = 0;  
            }

            if (textBoxSlave2.Text != "")
                textBoxSlave2.AppendText(Environment.NewLine);
            if (textBoxMessageSlave2.Text != "")
                textBoxMessageSlave2.AppendText(Environment.NewLine);
            textBoxMessageSlave2.Text = "";

            serialPort1.DiscardInBuffer();
            serialPort1.BaseStream.Flush();  
        }

        private void buttonClear1_Click(object sender, EventArgs e)
        {
            textBoxSlave1.Text = "";
            textBoxMessageSlave1.Text = "";
        }

        private void buttonClear2_Click(object sender, EventArgs e)
        {
            textBoxSlave2.Text = "";
            textBoxMessageSlave2.Text = "";
        }
       
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (flag == 0)
                return;
            try
            {
                block[countBytes] = (byte)serialPort1.ReadByte();
                if (countBytes == 4)
                {
                    ++countBlocks;
                    if (block[4] != crc8_rohs())
                    {
                        message = "ER, b = " + countBlocks + ";  ";
                        if (flag == 1)
                            textBoxMessageSlave1.Text += message;
                        if (flag == 2)
                            textBoxMessageSlave2.Text += message;
                    }
                    else
                    {
                        message = "OK, b = " + countBlocks + ";  ";
                        if (flag == 1)
                            textBoxMessageSlave1.Text += message;
                        if (flag == 2)
                            textBoxMessageSlave2.Text += message;
                    }

                    for (int k = 0;k < 4; ++k)
                    {
                        if(flag == 1)
                        {
                            textBoxSlave1.Text += (char)block[k];
                        }
                        if (flag == 2)
                        {
                            textBoxSlave2.Text += (char)block[k];
                        }
                    }
                    countBytes = 0;
                    return;
                }
                ++countBytes;
                return;
            }

            catch
            {
                MessageBox.Show("Port " + comboBox1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private byte crc8_rohs()
        {
            byte crc = 0xFF;
            for(int k = 0;k < 4;++k){
                crc = CRC_8_ROHS_table[crc ^ block[k]];
            }
            return crc;
        }

        private void textBoxSlave2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
