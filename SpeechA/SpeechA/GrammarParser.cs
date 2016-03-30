using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpeechA
{
    class GrammarParser
    {
        /*
         * Parses the file recursively into a grammarfile.
        */
        public Grammar ParseGrammar(String path)
        {
            GrammarBuilder gb;
            try
            {
                StreamReader sr = new StreamReader(path);
                gb = buildGrammar(sr);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                gb = new GrammarBuilder("");
            }
            return new Grammar(gb);
        }
        private GrammarBuilder buildGrammar(StreamReader sr)
        {
            char[] parseSymbol = new char[] { '{', '>', '$', '\t' };
            Choices choices = new Choices();
            String nextLine;

            while (sr.Peek() > -1)
            {
                nextLine = sr.ReadLine();
                if (nextLine.Contains('>'))
                {
                    String text = nextLine.Split('>')[0];
                    if (nextLine.Contains('$'))
                    {
                        choices.Add(varToChoices(text.Trim(parseSymbol)));
                    }
                    else
                    {
                        choices.Add(text);
                    }
                }
                if (nextLine.Contains('{'))
                {
                    if (nextLine != null && nextLine.Contains('$'))
                    {
                        GrammarBuilder gb = new GrammarBuilder(varToChoices(nextLine.Trim(parseSymbol)));
                        gb.Append(buildGrammar(sr));
                        choices.Add(gb);
                    }
                    else
                    {

                        GrammarBuilder gb = new GrammarBuilder(nextLine.Split('{')[0]);
                        gb.Append(buildGrammar(sr));
                        choices.Add(gb);
                    }
                }
                if (nextLine.Contains('}'))
                {
                    return new GrammarBuilder(choices);
                }
            }
            sr.Close();
            return new GrammarBuilder(choices);

        }
        /*
         * Make choices from variable in file (which is given with the grammar file).
         */
        private Choices varToChoices(String variable)
        {
            String varpath = variable.Split(',')[2];
            String varname = variable.Split(',')[1];
            String vartype = variable.Split(',')[0];
            Choices choices = new Choices();
            StreamReader sr = new StreamReader(varpath);
            String nextLine;
            bool mustLoop = true;
            bool mustAdd = false;
            while ((sr.Peek() > -1) && mustLoop)
            {
                nextLine = sr.ReadLine();
                if (nextLine.Contains(':') && nextLine.TrimEnd(':').Equals(vartype))
                {
                    mustAdd = true;
                }
                if (mustAdd)
                {
                    choices.Add(nextLine);
                }
                if (mustAdd && nextLine.Contains(':') && !nextLine.TrimEnd(':').Equals(vartype))
                {
                    mustLoop = false;
                }
            }
            return choices;
        }
    }
}
