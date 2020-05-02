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
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;

namespace tcpConnection
{
    public partial class SERVER : Form
    {
        IPAddress localAddress;
        Int32 serverPort;
        TcpListener server;
        Socket client;
        NetworkStream clientStream;
        private object require;

        public SERVER()
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
            status.Text = "Waiting for a connection...";
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
                    status.Text = "Waiting for a connection...";
                }
                var t = new Thread(new ParameterizedThreadStart(getMessage));
                t.Start(client);
            }
        }
        private void getMessage(object clientObj)
        {
            status.Text = "Connected to " + client.RemoteEndPoint;
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

                    data.Text = Encoding.UTF8.GetString(buffer);
                    //clientStream.Flush();
                    sentData = Encoding.UTF8.GetBytes("Server was received!");
                    clientStream.Write(sentData, 0, sentData.Length);

                    if (data.Text == "EXIT")
                    {
                        sentData = Encoding.UTF8.GetBytes("bye");
                        clientStream.Write(sentData, 0, sentData.Length);
                        status.Text = "Closed connection to " + client.RemoteEndPoint;
                        break;
                    }

                    string url = parse(data.Text);
                    
                    webBrowser1.Navigate(data.Text);
                    if (check(url))
                    {
                        client = clientObj as Socket;
                        clientStream = new NetworkStream(client);
                        byte[] noti = Encoding.UTF8.GetBytes("Successfull");
                        clientStream.Write(noti, 0, noti.Length);

                       
                    }
                    else
                    {
                        client = clientObj as Socket;
                        clientStream = new NetworkStream(client);
                        byte[] noti = Encoding.UTF8.GetBytes("Unsuccessfull");
                        clientStream.Write(noti, 0, noti.Length);
                    }


                    
                } catch
                {
                    
                }
            }
                clientStream.Close();
                client.Close();
        }
                
        private string parse(string url)
        {
            string head = "http://";
            if (head.Contains(url))
                return url;
            else
                return head + url;
        }

        private bool check(string url)
        {
            try
            {
                //Creating the HttpWebRequest 
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too. 
                request.Method = "HEAD";
                //Getting the Web Response. 
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Returns TRUE if the Status code == 200 
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                //Any exception will returns false. 
                return false;
            }
        }

    }
}
