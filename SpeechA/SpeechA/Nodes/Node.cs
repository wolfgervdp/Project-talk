using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
/*
 * Todo: get rid of the magic strings somehow
 */
// ReSharper disable InconsistentNaming
namespace SpeechA.Nodes
{
    class Node 
    {
        protected Hashtable children;
        protected readonly bool containsCommandNode;
        public String Text;
        public Node(String text, bool containsCommandNode)
        {
            children = new Hashtable();
            this.containsCommandNode = containsCommandNode;
            Text = text;
        }
        
        public CommandNode GetCommand(String searchString)
        {
            if (!containsCommandNode)
            {
                int index = searchString.IndexOf(' ');
                if (index < 0)
                {
                    index = searchString.Length;
                }
                String top = searchString.Substring(0, index);
                String nextSearchString = searchString.Substring(index).Trim();
                
                Node childNode = null;
                if (children.ContainsKey(top)) //If it contains our key, we know the next node
                {
                    childNode = (Node) children[top];
                }
                else if (children.ContainsKey("*"))
                    //If not, we know it's a variable node (given the datastructure is right)
                {
                    VariableNode tmp_vn = (VariableNode) children["*"];
                    tmp_vn.Value = top;
                    childNode = tmp_vn;
                }
                if (childNode != null)
                {
                    return childNode.GetCommand(nextSearchString);
                }
                else
                {
                    //return null;
                    throw new NodeNotFoundException("Returned null at node " + Text); //Should use learning here, to widen the vocabulary of Ella.
                }
            }
            else
            {
                return (CommandNode) children["$"];
            }
        }

        public void add(Node child)
        {
            children.Add(child.Text, child);
        }
        public void add(Node[] children)
        {
            foreach (Node child in children)
            {
                add(child);
            }
        }

        public void add(CommandNode child)
        {
            children.Add("$",child);
        }

        public override string ToString()
        {
            return Text;
        }
    }

}
