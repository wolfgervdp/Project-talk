/*
 *  Todo:
 *          - Stop listening to self.
 *          - List with stuff that needst to be spoken, with priority queue.
 *          - GetVariable() method
 *          - TCP messages (instead of only typing in box) : Ok
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using SpeechA.Properties;
// ReSharper disable InconsistentNaming
namespace SpeechA
{
    public partial class SpeechA : Form
    {
        String GRAMPATH = @"C:\Users\Wolfger\Documents\Visual Studio 2013\Projects\SpeechA\SpeechA\SpeechA\Resources\gram.sshel";
        String EGPATH = @"C:\Program Files (x86)\EventGhost\EventGhost";
        char[] ENTERCHAR = {'\n', '\r'};

        SpeechRecognitionEngine sre;
        private NetworkConnector nc;
        private Thread ncThread;
        public Hashtable variables;
        Boolean isListening = true;
        SpeechSynthesizer synth;
        private Node root;

        public SpeechA()
        {
            
            InitializeComponent();
            variables = new Hashtable();
            //generateNumberFile();
            nc = new NetworkConnector(27120, this);
            synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            ncThread = new Thread(new ThreadStart(nc.start));
            ncThread.IsBackground = true;
            ncThread.Start();
            nc.NewMessage += nc_NewMessage;
            //MessageBox.Show(myNode.ToString());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            NodeBuilder nb = new NodeBuilder(variables);

            root = new Node("root", false);
            root = nb.getDataStructure(new StreamReader(GRAMPATH), root);
            startListening();
        }
       

        /*
         * Load grammar into RecognitionEngine, do the necessary stuff to get it working.
         */
        private void startListening(){
            GrammarParser gp = new GrammarParser();
            sre = new SpeechRecognitionEngine();
            sre.SetInputToDefaultAudioDevice();
            
            sre.LoadGrammar(gp.ParseGrammar(GRAMPATH));

            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
                Act(e.Result.Text);
        }
        private void nc_NewMessage(object sender, MessageReceivedEventArgs e)
        {
                Act(e.Message);
        }
        private void txtIn_TextChanged(object sender, EventArgs e)
        {
            
            String st = txtIn.Text;
            if (st.ToLower().Contains(ENTERCHAR[0]) | st.ToLower().Contains(ENTERCHAR[0]))
            {
                Act(st.ToLower().Split(ENTERCHAR)[0]);
                txtIn.Text = "";
            }
        }

        /*
         * Make distinction between Commands (internal, eventghost and external processes) and Speech. 
         */
        private void Act(String input)
        {
            CommandNode cmd = root.GetCommand(input);
            
            lblDebug.Text = cmd.speech;

            if (isListening)
            {
                Say(fillVars(cmd.speech));
                for (int i = 0; i < cmd.action.Length; i++)
                {
                    switch (cmd.type[i])
                    {
                        case "INT": INT_Execute(cmd.action[i]);
                            break;
                        case "PROC": PROC_Execute(cmd.action[i], cmd.args[i]);
                            break;
                        case "EG": EG_Execute(cmd.action[i]);
                            break;
                    }
                }
            }

            else
            {
                if ((cmd.type[0].Equals("INT")) && (cmd.action[0].Equals("START_LISTENING")))
                {
                    isListening = true;
                }
            }
        }

        /*
         * Make choices from variable in file (which is given with the grammar file).
         */
        public Choices varToChoices(String variable)
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
                if (mustAdd && nextLine.Contains(':') && !nextLine.TrimEnd(':').Equals(vartype) )
                {
                    mustLoop = false;
                }
            }
            return choices;
        }


        public void Say(String Speech)
        {
            Say(Speech, false);
        }
        public void Say(String Speech, bool async)
        {
            lblDebug.Text = Speech;
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            if (async)
                synth.SpeakAsync(Speech);
            else
                synth.Speak(Speech);
        }

        /*
         * Return the sentence with filled in variables.
         */
        private String fillVars(String inp)
        {
            String [] strarr = inp.Split();
            StringBuilder strb = new StringBuilder();
            foreach(String mystr in strarr){
                if (mystr.Contains('*'))
                {
                    strb.Append(variables[mystr.Trim('*')] + " ");
                }
                else
                {
                    strb.Append(mystr + " ");
                }
            }
            return strb.ToString();
        }
        /*
         * Internal commands: commands which must be executed by this application.
         */ 
        private void INT_Execute(String execution)
        {
            switch (execution)
            {
                case "EXIT": Application.Exit();
                    break;
                case "STOP_LISTENING": isListening = false;
                    break;
                case "START_LISTENING": isListening = true;
                    break;
            }
        }
        /*
         * Commands which are executed by another application. The output of the application is read by text to speech.
         * Todo: write a method of input (trough stdin, not trough parameters on cmd-line).
         */
        private void PROC_Execute(String execution, String[] args)
        {
            String[] tmp_args = new String[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains('*'))
                {
                    tmp_args[i] = variables[args[i].Trim('*')].ToString();
                }
                else
                {
                    tmp_args[i] = args[i];
                }
            }
            Process p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    FileName = execution
                }
            };
            foreach (String arg in tmp_args)
            {
                p.StartInfo.Arguments += arg + " ";
            }
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            String output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            Say(output, true);
        }
        /*
         * Execute an EG command trough event. 
         */
        private void EG_Execute(String EGevent)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = EGPATH;
                p.StartInfo.Arguments = " -h -e " + EGevent;
                p.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show(Resources.EG_not_installed);
            }
        }
    }
}
