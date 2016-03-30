using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechA.Nodes
{
    class NodeNotFoundException : ApplicationException
    {
        public String NodeMessage { get; private set; }
        public NodeNotFoundException(String nodeMessage)
        {
            this.NodeMessage = nodeMessage;
        }
    }
}
