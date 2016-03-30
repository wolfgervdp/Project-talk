using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Windows.Forms;

namespace SpeechA
{
    public delegate void NewMessageEventHandler(object sender, MessageReceivedEventArgs e);
    
    public class NetworkConnector
    {
        private const String IPADDRESS = "127.0.0.1";
        private TcpListener server;
        public event NewMessageEventHandler NewMessage;
        private Control parent;
        
        public NetworkConnector(int port, Control parent)
        {
            this.parent = parent;
            IPAddress ipAddress = IPAddress.Parse(IPADDRESS);
            server = new TcpListener(ipAddress, port);
        }

        public void start()
        {
            //MessageBox.Show("Network Connector started");
            try
            {
                Byte[] bytes = new Byte[256];
                String data = null;

                server.Start();
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();

                    NetworkStream stream = client.GetStream();
                    int i;
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        if (NewMessage != null)
                        {
                            //MessageBox.Show(Thread.CurrentThread.Name);
                            MessageReceivedEventArgs e = new MessageReceivedEventArgs(data,stream);
                            Console.WriteLine(data);
                            parent.BeginInvoke(NewMessage, new object[]{this, e});
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            
        }

    }
}
