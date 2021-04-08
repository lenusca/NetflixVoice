using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mmisharp;
using Microsoft.Speech.Recognition;
using System.IO;

namespace speechModality
{
    public class SpeechMod
    {
        private SpeechRecognitionEngine sre;
        private Grammar gr;
        private Grammar toLoad;
        private Grammar loaded;
        private bool done;
        private bool grammarLoaded;
        public event EventHandler<SpeechEventArg> Recognized;
        protected virtual void onRecognized(SpeechEventArg msg)
        {
            EventHandler<SpeechEventArg> handler = Recognized;
            if (handler != null)
            {
                handler(this, msg);
            }
        }

        private LifeCycleEvents lce;
        private MmiCommunication mmic;

        public SpeechMod()
        {
            //init LifeCycleEvents..
            lce = new LifeCycleEvents("ASR", "FUSION","speech-1", "acoustic", "command"); // LifeCycleEvents(string source, string target, string id, string medium, string mode)
            //mmic = new MmiCommunication("localhost",9876,"User1", "ASR");  //PORT TO FUSION - uncomment this line to work with fusion later
            mmic = new MmiCommunication("localhost", 8000, "User1", "ASR"); // MmiCommunication(string IMhost, int portIM, string UserOD, string thisModalityName)

            mmic.Send(lce.NewContextRequest());

            //load pt recognizer
            sre = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("pt-PT"));
            gr = new Grammar(Environment.CurrentDirectory + "\\ptG.grxml", "rootRule");
            sre.LoadGrammar(gr);

            grammarLoaded = false;

            sre.SetInputToDefaultAudioDevice();
            sre.RecognizeAsync(RecognizeMode.Multiple);
            sre.SpeechRecognized += Sre_SpeechRecognized;
            sre.SpeechHypothesized += Sre_SpeechHypothesized;

            //////////////////NEW////////////////////////
            sre.RecognizerUpdateReached += new EventHandler<RecognizerUpdateReachedEventArgs>(recognizer_RecognizerUpdateReached);

            var fileSystemWatcher = new FileSystemWatcher();

            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Path = Environment.CurrentDirectory + @"\..\..";

            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void recognizer_RecognizerUpdateReached(object sender, RecognizerUpdateReachedEventArgs e)
        {
            if (done == false)
            {
                if (grammarLoaded == true)
                {
                    try
                    {
                        sre.UnloadGrammar(loaded);
                        Console.WriteLine("Grammar unloaded");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                }
                sre.LoadGrammar(toLoad);
                Console.WriteLine("Grammar loaded");
                grammarLoaded = true;
                done = true;
                loaded = toLoad;

            }
        }

        private void Sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            onRecognized(new SpeechEventArg() { Text = e.Result.Text, Confidence = e.Result.Confidence, Final = false });
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            onRecognized(new SpeechEventArg(){Text = e.Result.Text, Confidence = e.Result.Confidence, Final = true});
            
            //SEND
            // IMPORTANT TO KEEP THE FORMAT {"recognized":["SHAPE","COLOR"]}
            string json = "{ \"recognized\": [";
            foreach (var resultSemantic in e.Result.Semantics)
            {
                json+= "\"" + resultSemantic.Value.Value +"\", ";
            }
            json = json.Substring(0, json.Length - 2);
            json += "] }";

            var exNot = lce.ExtensionNotification(e.Result.Audio.StartTime+"", e.Result.Audio.StartTime.Add(e.Result.Audio.Duration)+"",e.Result.Confidence, json);
            mmic.Send(exNot);
        }
        ///NEW////
        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("WRITTING ");
            GrammarBuilder builder = new GrammarBuilder();
            builder.AppendRuleReference(Environment.CurrentDirectory + @"\..\..\all.grxml", "main");
            toLoad = new Grammar(builder);
            done = false;
            sre.RequestRecognizerUpdate();
        }
    }
}
