using FlightSimulator.Model;
using FlightSimulator.Model.Interface;
using System;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace WebApplication.Controllers
{
    public class DefaultController : Controller
    {
        private double latitude;
        private double longitude;

        private OrderedDictionary elements = new OrderedDictionary()
        {
            { "lon","/position/longitude-deg" },
            { "lat", "/position/latitude-deg" },
            { "rudder", "/controls/flight/rudder" },
            { "throttle", "/controls/engines/engine/throttle" }

        };

        // GET: Default
        public ActionResult Index()
        {
            return View();
        }


        private string getValuesFromServer(string ip, int port, List<string> vars)
        {
            IModel model = MyModel.Instance;
            model.connectClient(ip, port);
            ICollection<string> elemetsWithGet = addGetAndNewLineToStrings(vars);

            ITelnetClient c = model.getClient();
            string strings = c.read(elemetsWithGet);
            model.disconnectClient();
            return strings;
        }


        [HttpGet]
        public ActionResult display(string ip, int port)
        {

            List<string> vals = new List<string>();
            vals.Add((string)elements["lon"]);
            vals.Add((string)elements["lat"]);
            string strings = getValuesFromServer(ip,port,vals);

            List<string> values = strings.Split(',').ToList();
            Longitude = Double.Parse(extractDouble(values[0]));
            Latitude = Double.Parse(extractDouble(values[1]));
            Session["lat"] = Latitude;
            Session["lon"] = Longitude;

            return View();
        }


        string[] readFromFile(string path)
        {
            var dir = Server.MapPath("~\\files");
            var file = Path.Combine(dir, path);

            return System.IO.File.ReadAllLines(file);
        }

        public volatile static string[] lines;
        public volatile static int lineIndex;
        public static bool isMultiLine = false;

        private ActionResult setArgsAndDisplay(double lon, double lat, int tempo)
        {
            Session["time"] = tempo;
            Session["lat"] = lat;
            Session["lon"] = lon;

            return View("loadAndDisplay");
        }


        [HttpGet]
        public ActionResult loadAndDisplay(string path, int tempo)
        {
            var dir = Server.MapPath("~\\files");
            string file = Path.Combine(dir, path);
            lines = readFromFile(file);

            if (lines.Length >= lineIndex)
            {
                lineIndex = 0;
            }
            string line = lines[lineIndex];

            List<string> values = line.Split(',').ToList();
            double lon;
            double lat;
            if (values.Count >= 2)
            {
                lon = Double.Parse(values[0]);
                lat = Double.Parse(values[1]);
            }
            else
            {
                lon = 0;
                lat = 0;
            }
            isMultiLine = true;
            return setArgsAndDisplay(lon, lat, tempo);

        }

        [HttpPost]
        public string GetCourse()
        {
            if ((isMultiLine && lineIndex >= lines.GetLength(0))
                || lineIndex < 0)
            {
                lineIndex = 0;
                return "";
            }

            string line;
            if (isMultiLine)
            {
                line = lines[lineIndex];
            } else
            {
                line = lines[0];
            }
            List<string> values = line.Split(',').ToList();


            string rudder = values[2];
            string throttle = values[3];

            lineIndex++;
            return ToXml(Double.Parse(rudder), Double.Parse(throttle));
        }


        private string getValuesInBackgroundWithTime(string ip, int port, int tempo, List<string> args)
        {
            lines = new string[5];
            string strings = getValuesFromServer(ip, port, args);
            string line = getValuesAsString(strings);
            lines[0] = line;
            Task taskA = new Task(() => {
                while (true)
                {
                    strings = getValuesFromServer(ip, port, args);
                    lines[0] = getValuesAsString(strings);
                    lineIndex = 0;
                    System.Threading.Thread.Sleep(tempo * 1000);
                }
            });
            // Start the task.
            taskA.Start();
            return line;
        }


        [HttpGet]
        public ActionResult getArgsAndDisplay(string ip, int port, int tempo)

        {
            List<string> args = new List<string>();
            foreach (var e in elements.Values)
            {
                args.Add((string)e);
            }
            string line = getValuesInBackgroundWithTime(ip, port, tempo, args);
            List<string> values = line.Split(',').ToList();

            isMultiLine = false;
            return setArgsAndDisplay(Double.Parse(values[0]),
                                Double.Parse(values[1]), tempo);
            
        }

        

        private void toXml(XmlWriter writer, double rudder, double throttle)
        {
            writer.WriteStartElement("Location");
            writer.WriteElementString("rudder", rudder.ToString());
            writer.WriteElementString("throttle", throttle.ToString());
            writer.WriteEndElement();
        }

        private string ToXml(double rudder, double throttle)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Location");

            toXml(writer, rudder , throttle);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }


        [HttpPost]
        public string GetLocation()
        {
            /*****************************************************************
             * 
             * 
             * update lon & lat values with flight gear
             * 
             * 
             */
            Latitude = 0;
            Longitude = 0;

            return ToXml(Latitude, Longitude);
        }

        private void writeToFile(string path, string content)
        {
            var dir = Server.MapPath("~\\files");
            var file = Path.Combine(dir, path);

            Directory.CreateDirectory(dir);
            System.IO.File.AppendAllText(file, content + Environment.NewLine);

        }


        public static bool ExecuteWithTimeLimit(TimeSpan timeSpan, Action codeBlock)
        {
            try
            {
                Task task = Task.Factory.StartNew(() => codeBlock());
                task.Wait(timeSpan);
                return task.IsCompleted;
            }
            catch (AggregateException ae)
            {
                throw ae.InnerExceptions[0];
            }
        }


        private string extractDouble(string str)
        {
            List<string> values = str.Split(' ').ToList();
            string num = values.Find(
            delegate (string s)
            {
                return s.StartsWith("'") && s.EndsWith("'");
            });
            num = num.Replace("'", "");
            return num;

        }

        List<string> addGetAndNewLineToStrings(System.Collections.ICollection strings)
        {
            List<string> copyString = new List<string>();
            foreach (string str in strings)
            {
                copyString.Add("get " + str + Environment.NewLine);
            }
            return copyString;
        }

        private string getValuesAsString(string strings)
        {
            List<string> values = strings.Split(',').ToList();
            string properties = extractDouble(values[0]);

            int length = elements.Values.Count;
            for (int i = 1; i < length; ++i)
            {
                properties += "," + extractDouble(values[i]);
            }
            return properties;
        }


        [HttpGet]
        public ActionResult save(string ip, int port, int tempo, int duration, string fileName)
        {
            ActionResult action = getArgsAndDisplay(ip, port, tempo);

            ICollection<string> elemetsWithGet = addGetAndNewLineToStrings(elements.Values);
            IModel model = MyModel.Instance;
            ITelnetClient c = model.getClient();
            lines = new string[2];

            Task taskA = new Task(() => {
            //string str = "start";
                try
                {
                    
                    bool running = ExecuteWithTimeLimit(TimeSpan.FromSeconds(duration), () =>
                    {
                        do
                        {
                            model.connectClient(ip, port);
                            string strings = c.read(elemetsWithGet);
                            model.disconnectClient();
                            string properties =  getValuesAsString(strings);
                            lines[0] = properties;
                            writeToFile(fileName, properties);
                            System.Threading.Thread.Sleep(1000 * tempo);
                        } while (true);
                        
                    });
                    lineIndex = -1;
                   //     str = "success";
                }
                catch{
                //str = "fail";
                } 
                
            });
            // Start the task.
            taskA.Start();

            isMultiLine = false;
            return action;
        }


        public double Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
                //NotifyPropertyChanged("Latitude");
            }
        }

        public double Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;
                //NotifyPropertyChanged("Longitude");
            }
        }
    }
}
