using System;
using System.Net;
using System.Text.RegularExpressions;

/*
    Searchfight for Cignium Programming Challenge
    Code by: Julio Rodriguez
*/

namespace searchfight
{

    //Class to determine what Search Engine to use
    class SearchEngine
    {

        //URL as Constants
        private const string URLGoogle = "https://www.google.com.pe/#q=";
        private const string URLBing = "https://www.bing.com/search?q=";
        private const string URLYahoo = "https://search.yahoo.com/search?p=";
        private const string URLExalead = "http://www.exalead.com/search/web/results/?q=";
        private const string URLBaidu = "http://www.baidu.com/s?wd=";

        //Returns the full URL
        public string ReturnURL(string engine, string item)
        {

            //Turning the query into URL friendly
            item = System.Web.HttpUtility.UrlEncode(item);

            switch (engine.ToLower())
            {
                case "google":
                    return URLGoogle + item;
                case "bing":
                    return URLBing + item;
                case "yahoo":
                    return URLYahoo + item;
                case "exalead":
                    return URLExalead + item;
                case "baidu":
                    return URLBaidu + item;
                default:
                    Console.WriteLine("ERROR: \"" + engine + "\" is not a supported web search engine or is an incorrect name.");
                    Environment.Exit(0);
                    return "ERROR";
            }

        }

    }

    //Scraper to obtain the results of the webpages
    class WebScraper
    {

        //Main Scraper method

        public long Scraper(string URL, string engine)
        {
            WebClient webclient = new WebClient();
            string webpage = webclient.DownloadString(URL);
            string resultNumber = "";
            long parsedResult = 0;
            
            switch (engine.ToLower())
            {
                case "google":
                    Console.WriteLine("ERROR: \"" + engine + "\" is not implemented at this moment.");
                    Environment.Exit(0);
                    resultNumber = GoogleScraper(webpage);
                    break;
                case "bing":
                    resultNumber = BingScraper(webpage);
                    parsedResult = Int64.Parse(resultNumber.Replace(".", ""));
                    break;
                case "yahoo":
                    resultNumber = YahooScraper(webpage);
                    parsedResult = Int64.Parse(resultNumber.Replace(",", ""));
                    break;
                case "exalead":
                    resultNumber = ExaleadScraper(webpage);
                    parsedResult = Int64.Parse(resultNumber.Replace(",", ""));
                    break;
                case "baidu":
                    resultNumber = BaiduScraper(webpage);
                    parsedResult = Int64.Parse(resultNumber.Replace(",", ""));
                    break;
                default:
                    Console.WriteLine("ERROR: \"" + engine + "\" is not scrapable at this moment.");
                    Environment.Exit(0);
                    break;
            }

            return parsedResult;

        }

        //Scrape and Regex depending on which engine is used:
        //===================================================

        //-Google
        private string GoogleScraper(string webpage)
        {
            throw new NotImplementedException();
        }
        //-Bing
        private string BingScraper(string webpage)
        {
            Regex regex = new Regex("\"sb_count\">[\\d+(?:.)]+ result");
            Match match = regex.Match(webpage);
            Regex regexToNumber = new Regex("[\\d+(?:.)]+");
            Match matchToNumber = regexToNumber.Match(match.Value);
            return matchToNumber.Value;
        }
        //-Yahoo
        private string YahooScraper(string webpage)
        {
            Regex regex = new Regex("span>[\\d+(?:,)]+ results");
            Match match = regex.Match(webpage);
            Regex regexToNumber = new Regex("[\\d+(?:,)]+");
            Match matchToNumber = regexToNumber.Match(match.Value);
            return matchToNumber.Value;
        }
        //-Exalead
        private string ExaleadScraper(string webpage)
        {
            Regex regex = new Regex("\"pull-right\">[\\d+(?:,)]+ results");
            Match match = regex.Match(webpage);
            Regex regexToNumber = new Regex("[\\d+(?:,)]+");
            Match matchToNumber = regexToNumber.Match(match.Value);
            return matchToNumber.Value;
        }
        //-Baidu
        private string BaiduScraper(string webpage)
        {
            Regex regex = new Regex("div>.................................[\\d+(?:,)]+...</div");
            Match match = regex.Match(webpage);
            Regex regexToNumber = new Regex("[\\d+(?:,)]+");
            Match matchToNumber = regexToNumber.Match(match.Value);
            return matchToNumber.Value;
        }

    }

    class Searchfight
    {

        private const int numberOfSearchEngines = 4;

        static void Main(string[] args)
        {

            if (args.Length == 0) //Checking for input Error
            {
                Console.WriteLine("Please enter the programming languages to query.");
                Environment.Exit(0);
            }
            else
            {
                SearchEngine searchengine = new SearchEngine();
                WebScraper webscraper = new WebScraper();
                long[][] totalresults = new long[args.Length][];
                for (int i=0; i < args.Length; i++)
                {
                    totalresults[i] = LangResults(args[i], searchengine, webscraper); //Scraping results
                }

                //Search for the individual Engine Search winners:
                String searchwinner = "";
                String searchengineName = "";
                bool tieEngine = false;
                for (int i=0; i < numberOfSearchEngines; i++)
                {
                    long biggerNumber = 0;
                    switch (i)
                    {
                        case 0:
                            searchengineName = "Bing";
                            break;
                        case 1:
                            searchengineName = "Yahoo";
                            break;
                        case 2:
                            searchengineName = "Exalead";
                            break;
                        case 3:
                            searchengineName = "Baidu";
                            break;
                        default:
                            Console.WriteLine("ERROR: Array of scraping out of bounds");
                            Environment.Exit(0);
                            break;
                    }
                    for (int j=0; j < args.Length; j++)
                    {
                        if (totalresults[j][i] > biggerNumber)
                        {
                            biggerNumber = totalresults[j][i];
                            searchwinner = args[j];
                            tieEngine = false;
                        } else if (totalresults[j][i] == biggerNumber)
                        {
                            tieEngine = true;
                        }
                    }
                    if (!tieEngine) //Write the Engine Search Winner
                    {
                        Console.WriteLine(searchengineName + " winner: " + searchwinner); 
                    } else
                    {
                        Console.WriteLine(searchengineName + " winner: It's a Tie!");
                    }
                }

                //Search for the Total winner:
                long biggerTotal = 0;
                string winnerTotal = "";
                bool tieTotal = false;
                for (int i=0; i < args.Length; i++)
                {
                    long argSum = 0;
                    for (int j=0; j < numberOfSearchEngines; j++)
                    {
                        argSum += totalresults[i][j];
                    }

                    if (argSum > biggerTotal)
                    {
                        biggerTotal = argSum;
                        winnerTotal = args[i];
                        tieTotal = false;
                    } else if (argSum == biggerTotal)
                    {
                        tieTotal = true;
                    }
                }

                if (!tieTotal) //Write the Total Web Search Engine Winner
                {
                    Console.WriteLine("Total Web Search Engine Winner: " + winnerTotal);
                } else
                {
                    Console.WriteLine("Total Web Search Engine Winner: It's a Tie!"); 
                }

                //Console.ReadLine(); //Debugging purposes

            }

        }

        //Get the results of a certain language in all Search Engines
        static long[] LangResults(string language, SearchEngine searchengine, WebScraper webscraper)
        {

            long[] langresultArray = new long[4];

            Console.Write(language + ": ");

            string searchengineName = "";

            for (int i=0; i < numberOfSearchEngines; i++)
            {
                switch (i)
                {
                    case 0:
                        searchengineName = "Bing";                       
                        break;
                    case 1:
                        searchengineName = "Yahoo";
                        break;
                    case 2:
                        searchengineName = "Exalead";
                        break;
                    case 3:
                        searchengineName = "Baidu";
                        break;
                    default:
                        Console.WriteLine("ERROR: Array of scraping out of bounds");
                        Environment.Exit(0);
                        break;
                }

                string url = searchengine.ReturnURL(searchengineName, language);
                long result = webscraper.Scraper(url, searchengineName);
                langresultArray[i] = result;

                Console.Write(searchengineName + ": " + result);
                if (i != 3)
                {
                    Console.Write(" ");
                }
                else
                {
                    Console.Write("\n");
                }

            }

            return langresultArray;

        }

    }    

}