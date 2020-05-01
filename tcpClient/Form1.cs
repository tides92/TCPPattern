using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace tcpClient
{
    public partial class Form1 : Form
    {
        TcpClient client = new TcpClient();
        NetworkStream serverStream;
        string readData = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] outstream = Encoding.UTF8.GetBytes(textBox4.Text);
            serverStream.Write(outstream, 0, outstream.Length);
            serverStream.Flush();
            textBox4.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                client.Connect(textBox1.Text, Int32.Parse(textBox2.Text));
            } catch
            {
                textBox3.Text = "This port already connected!";
            }
            Thread ctThread = new Thread(getMessage);
            ctThread.Start();

        }

        private void getMessage()
        {
            while (true)
            {
                serverStream = client.GetStream();
                var bufferSize = client.ReceiveBufferSize;
                byte[] instream = new byte[bufferSize];

                serverStream.Read(instream, 0, bufferSize);
                readData = Encoding.UTF8.GetString(instream);
                if (readData == "bye") { this.Close(); }
                msg();

            }
        }
        private void msg()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(msg));
            }
            else
            {
                textBox3.Text = readData;
            }
        }
    }
}
