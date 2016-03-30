using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace SpeechA
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(String data, NetworkStream stream)
        {
            String[] dataArray= data.Split('\n');
            foreach (String str in dataArray)
            {
                if (str.Contains("message"))
                {
                    this.Message = str.Split(':')[1];
                }
                if (str.Contains("location"))
                {
                    this.Location = str.Split(':')[1];
                }
            }
            this.stream = stream;
        }
        public String Location
        {
            get;
            private set;
        }
        public String Message
        {
            get;
            private set;
        }
        public NetworkStream stream
        {
            get;
            private set;
        }
   
    }

}

