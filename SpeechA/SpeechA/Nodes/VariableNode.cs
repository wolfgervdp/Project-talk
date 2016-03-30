using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechA
{
    class VariableNode:Node
    {
        public String Value;
        public VariableNode(bool containsCommandNode) : base("*", containsCommandNode){}
        public override string ToString()
        {
            return Value;
        }
    }
}
