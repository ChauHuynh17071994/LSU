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

namespace LSU_Chau_Huynh
{
    public partial class Form1 : Form
    {
        private string dataOut;
        private string dataIn;
        public Form1()
        {
            InitializeComponent();

            // using array of ports incase we want to add more than 1 port
            string[] ports = SerialPort.GetPortNames();
            cbComPort.Items.AddRange(ports);

            btnClose.Enabled = false;
            btnClearOut.Enabled = false;
            btnSend.Enabled = false;
            btnReadVersion.Enabled = false;
            btnReadStatus.Enabled = false;
            btnClearDataIn.Enabled = false;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                // Initializing the serial port
                serialPort1.PortName = cbComPort.Text;
                serialPort1.BaudRate = Convert.ToInt32(cbBaudRate.Text);
                serialPort1.DataBits = Convert.ToInt32(cbDataBits.Text);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cbStopBits.Text);
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), cbParityBits.Text);
                serialPort1.Handshake = (Handshake)Enum.Parse(typeof(Handshake), cbFlowControl.Text);
                
                // Set the read/write timeouts  
                serialPort1.ReadTimeout = 500;
                serialPort1.WriteTimeout = 500;

                serialPort1.Open();
                progressBar1.Value = 100;
                btnSend.Enabled = true;
                btnOpen.Enabled = false;
                btnClose.Enabled = true;
                btnReadStatus.Enabled = true;
                btnReadVersion.Enabled = true;
                lblPortStatus.Text = "ON";
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error: ", MessageBoxButtons.OK, MessageBoxIcon.Error);

                btnOpen.Enabled = true;
                btnClose.Enabled = false;
                lblPortStatus.Text = "OFF";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                    progressBar1.Value = 0;

                    btnClose.Enabled = false;
                    lblPortStatus.Text = "OFF";
                    if (!btnOpen.Enabled)
                        btnOpen.Enabled = true;
                    if (btnSend.Enabled)
                        btnSend.Enabled = false;
                    if (btnReadStatus.Enabled)
                        btnReadStatus.Enabled = false;
                    if (btnReadVersion.Enabled)
                        btnReadVersion.Enabled = false;
                }
            } catch(Exception err)
            {
                MessageBox.Show(err.Message, "Error: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                dataOut = txtData.Text;
                serialPort1.Write(dataOut);
                lblSendStatus.Text = "Success";
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if(txtData.Text != "")
            {
                txtData.Text = "";
                btnClearOut.Enabled = false;
            }
        }

        private void txtData_TextChanged(object sender, EventArgs e)
        {
            int dataLength = txtData.TextLength;
            // this code keep the length 2 digits.
            lblDataOutLength.Text = string.Format("{0:00}", dataLength);
            btnClearOut.Enabled = true;
            lblSendStatus.Text = "";
        }

        private void btnReadVersion_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    // send a request to LSU with syntax [103,from,to,crc16]
                    //[103,from,to,crc16]
                    dataIn = serialPort1.ReadExisting();

                    this.Invoke(new EventHandler(ShowData));
                    lblReceiveStatus.Text = "Success";
                }
                catch(Exception err)
                {
                    MessageBox.Show(err.Message, "Error: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowData(object sender, EventArgs e)
        {
            txtDataIn.Text = dataIn;
        }
        private void btnReadStatus_Click(object sender, EventArgs e)
        {
            // take the four element in the response array version: [13,from,to,orig_req,'version',crc16]
            try
            {
                dataIn = serialPort1.ReadExisting();

                this.Invoke(new EventHandler(ShowData));
                lblReceiveStatus.Text = "Success";
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearDataIn_Click(object sender, EventArgs e)
        {
            if(txtDataIn.Text != "")
            {
                txtDataIn.Text = "";
            }
        }

        private void txtDataIn_TextChanged(object sender, EventArgs e)
        {
            int dataInLength = txtDataIn.TextLength;
            // this code keep the length 2 digits.
            lblDataInLength.Text = string.Format("{0:00}", dataInLength);
            btnClearDataIn.Enabled = true;
        }
    }
}
