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
namespace SpeechA
{
    internal class CommandNode
    {

        public readonly String speech;
        public String[] type;
        public readonly String[][] args;
        public readonly String[] action;

        public CommandNode(String actionString)
        {

            speech = actionString.Split('&')[0].Trim('&');
            String[] commands = (new Regex(@"&.*$")).Match(actionString).Value.Trim('$').Split('%');
            action = new String[commands.Length];
            args = new String[commands.Length][];
            type = new String[commands.Length];
            for (int i = 0; i < commands.Length; i++)
            {
                String temp = (new Regex(@"^.*\|")).Match(commands[i]).Value.Trim(new char[] {'&', '|'}); //Get value in between & and |, and trim & and |
                type[i] = temp.Split('(')[0];
                if (temp.Split('(').Length > 1)
                {
                    args[i] = temp.Split('(')[1].Trim(new char[] {'(', ')'}).Split(',');
                }
                temp = (new Regex(@"\|.*$")).Match(commands[i]).Value;
                action[i] = temp.Trim('|');
            }


        }

        public override string ToString()
        {
            String myargs = args.Aggregate("", (current, arg) => current + arg);
            return "Speech: " + speech + "\nType: " + type + "\nArgs: " + myargs + "\nAction: " + action;
        }

    }

}
        