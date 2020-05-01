using System;
using System.IO;
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

namespace tcpConnection
{
    public partial class Form1 : Form
    {
        IPAddress localAddress;
        Int32 serverPort;
        TcpListener server;
        Socket client;
        NetworkStream clientStream;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            localAddress = IPAddress.Parse("127.0.0.1");
            serverPort = Int32.Parse(textBox2.Text);
            server = new TcpListener(IPAddress.Any, serverPort);
            server.Start();
            textBox3.Text = "Waiting for a connection...";
            Thread serverThread = new Thread(listen);
            serverThread.Start();
        }
        private void listen()
        {
            while (true)
            {
                client = server.AcceptSocket();
                if (server.Pending())
                {
                    textBox3.Text = "Waiting for a connection...";
                }
                var t = new Thread(new ParameterizedThreadStart(getMessage));
                t.Start(client);
            }
        }
        private void getMessage(object clientObj)
        {
            textBox3.Text = "Connected to " + client.RemoteEndPoint;
            client = clientObj as Socket;
            clientStream = new NetworkStream(client);
            byte[] sentData = Encoding.UTF8.GetBytes("Connected to server!");
            clientStream.Write(sentData, 0, sentData.Length);

            while (true)
            {
                try
                {
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    clientStream.Read(buffer, 0, buffer.Length);

                    textBox4.Text = Encoding.UTF8.GetString(buffer);
                    //clientStream.Flush();
                    sentData = Encoding.UTF8.GetBytes("Server was received!");
                    clientStream.Write(sentData, 0, sentData.Length);
                    if (textBox4.Text == "EXIT")
                    {
                        sentData = Encoding.UTF8.GetBytes("bye");
                        clientStream.Write(sentData, 0, sentData.Length);
                        textBox3.Text = "Closed connection to " + client.RemoteEndPoint;
                        break;
                    }
                } catch
                {
                    
                }
            }
                clientStream.Close();
                client.Close();
        }
    }
}
