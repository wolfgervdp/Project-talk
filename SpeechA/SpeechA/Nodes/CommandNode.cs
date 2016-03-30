using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SpeechA.Properties;
/*
 * Todo: - set up internal datastructure
 * Todo: - Fill variables while looping
 * 
 * 
 */
// ReSharper disable InconsistentNaming
namespace SpeechA.Nodes
{
    internal class CommandNode
    {

        public readonly String speech;
        public String[] type;
        public readonly String[][] args;
        public readonly String[] action;


        Regex withPar = new Regex(@"[&%]+(.*)\((.*)\)\|(.*)%?");
        Regex withoutPar = new Regex(@"[&%]?(.*)\|(.*)%?");

        public CommandNode(String actionString)
        {

            speech = actionString.Split('&')[0];
            String[] commands = (new Regex(@"&.*$")).Match(actionString).Value.Split('%');
            action = new String[commands.Length];
            args = new String[commands.Length][];
            type = new String[commands.Length];
            
            for (int i = 0; i < commands.Length; i++)
            {
                if (commands[i].Contains("("))
                {
                    Match m = withPar.Match(commands[i]);
                    type[i] = m.Groups[1].Value;
                    args[i] = m.Groups[2].Value.Split(',');
                    action[i] = m.Groups[3].Value;
                    
                }
                else
                {
                    Match m = withoutPar.Match(commands[i]);
                    type[i] = m.Groups[1].Value;
                    action[i] = m.Groups[2].Value;
                }
                    
            }


        }

        public override string ToString()
        {
            String myargs = args.Aggregate("", (current, arg) => current + arg);
            return "Speech: " + speech + "\nType: " + type + "\nArgs: " + myargs + "\nAction: " + action;
        }

    }

}
        