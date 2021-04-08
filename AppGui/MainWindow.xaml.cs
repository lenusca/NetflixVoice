using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using mmisharp;
using Newtonsoft.Json;

namespace AppGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MmiCommunication mmiC;
        private NetflixService service;
        public MainWindow()
        {
            InitializeComponent();

            service = new NetflixService();

            mmiC = new MmiCommunication("localhost",8000, "User1", "GUI");
            mmiC.Message += MmiC_Message;
            mmiC.Start();

        }

        // write all movies
        private void writeAllMovies(List<String> all)
        {
            String outFile = "<?xml version=\"1.0\"?><grammar xml:lang=\"pt-PT\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/06/grammar\" tag-format= \"semantics/1.0\"><rule id=\"main\" scope=\"public\"><one-of><item><ruleref uri=\"#object\"/> </item></one-of></rule><rule id= \"object\"><one-of><item><tag>out.Categoria=\"MOVIES\"</tag><one-of><item><ruleref uri=\"#movies\"/><tag>out.movies=rules.movies.movies</tag></item></one-of></item></one-of></rule>";
            outFile += "<rule id=\"movies\"><item repeat=\"1\"><one-of> ";
            foreach (String item in all)
            {
                if (item != String.Empty)
                {
                    Regex reg = new Regex("[*'\",_&#^@]");
                    string temp = reg.Replace(item, " ");

                    outFile += "<item>" + temp + "<tag> out.movies =" + "\"" + temp + "\"" + "</tag></item>";
                }

            }
            outFile += "</one-of></item></rule></grammar>";
            Console.WriteLine("Writing to movies file");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter((Environment.CurrentDirectory + @"\..\..\..\..\speechModality\speechModality\all.grxml")))
            {
                file.WriteLine(outFile);
            }
        }

        // write all series
        private void writeAllSeries(List<String> all)
        {
            String outFile = "<?xml version=\"1.0\"?><grammar xml:lang=\"pt-PT\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/06/grammar\" tag-format= \"semantics/1.0\"><rule id=\"main\" scope=\"public\"><one-of><item><ruleref uri=\"#object\"/> </item></one-of></rule><rule id= \"object\"><one-of><item><tag>out.Categoria=\"SERIES\"</tag><one-of><item><ruleref uri=\"#series\"/><tag>out.series=rules.series.series</tag></item></one-of></item></one-of></rule>";
            outFile += "<rule id=\"series\"><item repeat=\"1\"><one-of> ";
            foreach (String item in all)
            {
                if (item != String.Empty)
                {
                    Regex reg = new Regex("[*'\",_&#^@]");
                    string temp = reg.Replace(item, " ");

                    outFile += "<item>" + temp + "<tag> out.series =" + "\"" + temp + "\"" + "</tag></item>";
                }

            }
            outFile += "</one-of></item></rule></grammar>";
            Console.WriteLine("Writing to series file");
            using (System.IO.StreamWriter file = new System.IO.StreamWriter((Environment.CurrentDirectory + @"\..\..\..\..\speechModality\speechModality\all.grxml")))
            {
                file.WriteLine(outFile);
            }
        }

        private void MmiC_Message(object sender, MmiEventArgs e)
        {
            Console.WriteLine(e.Message);
            var doc = XDocument.Parse(e.Message);
            var com = doc.Descendants("command").FirstOrDefault().Value;
            dynamic json = JsonConvert.DeserializeObject(com);

            List<String> all;
            
            Console.WriteLine((string)json.recognized.ToString());
            
            // profile, movie, serie, genero
            switch ((string)json.recognized[0].ToString())
            {
                case "PROFILE":
                    service.choosePerfil((string)json.recognized[1].ToString());
                    break;
                case "NAVBAR":
                    service.chooseNavBar((string)json.recognized[1].ToString());
                    // if i said movies, he write all movies in the file
                    if ((string)json.recognized[1].ToString() == "Filmes")
                    {
                        all = service.getAllMoviesSeries();
                        Console.WriteLine("Total of movies: " + all.Count());
                        if (all.Count() > 0)
                        {
                            writeAllMovies(all);
                        }
                    }

                    else if ((string)json.recognized[1].ToString() == "Séries") 
                    {
                        all = service.getAllMoviesSeries();
                        Console.WriteLine("Total of seres: " + all.Count());
                        if (all.Count() > 0)
                        {
                            writeAllSeries(all);
                        }
                    }
                    break;
                case "MOVIES":
                    service.watch((string)json.recognized[1].ToString());
                    break;
                case "SERIES":
                    service.watch((string)json.recognized[1].ToString());
                    break;
                case "PLAYERCONTROLS":
                    service.playerControls((string)json.recognized[1].ToString());
                    break;



            }

        }
    }
}
