using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechA.Nodes
{
    /*
     * Helperclass to create the node structure.
     */
    class NodeBuilder
    {
        public Hashtable variables;

        public NodeBuilder(Hashtable variables)
        {
            this.variables = variables;
        }

        public Node getDataStructure(StreamReader sr, Node baseNode)
        {
            Node lastNode = baseNode;
            String nextLine;
            while (sr.Peek() > -1)
            {
                nextLine = sr.ReadLine();
                if (nextLine.Contains('>'))
                {
                    if (nextLine.Contains('$'))
                    {
                        Node tempNode = new VariableNode(true);
                        variables.Add(nextLine.Split('>')[0].Split(',')[1], tempNode); //Add variablenode to variables, key=variablename, needed to vars in later
                        tempNode.add(new CommandNode(nextLine.Split('>')[1]));
                        lastNode.add(tempNode);
                    }
                    else
                    {
                        Node tempNode = new Node(nextLine.Split('>')[0].Trim(), true);
                        tempNode.add(new CommandNode(nextLine.Split('>')[1]));
                        lastNode.add(tempNode);
                    }
                }
                if (nextLine.Contains('{'))
                {
                    if (nextLine.Contains('$'))
                    {
                        Node tempNode = new VariableNode(false);
                        variables.Add(nextLine.Split('>')[0].Split(',')[1], tempNode); //Add variablenode to variables, key=variablename, needed to vars in later
                        lastNode.add(getDataStructure(sr, tempNode));
                    }
                    else
                    {
                        lastNode.add(getDataStructure(sr, new Node(nextLine.Split('{')[0].Trim(), false)));
                    }
                }
                if (nextLine.Contains('}'))
                {
                    return lastNode;
                }
            }
            sr.Close();
            return lastNode;
        }

    }
}
