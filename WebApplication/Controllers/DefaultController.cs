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
            { "lat", "/position/latitude-deg" }
        };

        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult display(string ip, int port)
        {
            IModel model = MyModel.Instance;
            model.connectClient(ip, port);
            List<string> vals = new List<string>();
            string a = (string)elements["lon"];
            string b = (string)elements["lat"];
            vals.Add(a);
            vals.Add(b);
            ICollection<string> elemetsWithGet = addGetAndNewLineToStrings(vals);

            ITelnetClient c = model.getClient();
            string strings = c.read(elemetsWithGet);
            List<string> values = strings.Split(',').ToList();

            Longitude = Double.Parse(extractDouble(values[0]));
            Latitude  = Double.Parse(extractDouble(values[1]));

            Session["lat"] = Latitude;
            Session["lon"] = Longitude;


            return View();


            /*InfoModel.Instance.ip = ip;
            InfoModel.Instance.port = port.ToString();
            InfoModel.Instance.time = time;

            InfoModel.Instance.ReadData("Dor");

            Session["time"] = time;


            return View();*/
        }

        

        private void toXml(XmlWriter writer)
        {
            writer.WriteStartElement("Location");
            writer.WriteElementString("Latitude", this.Latitude.ToString());
            writer.WriteElementString("Longitude", this.Longitude.ToString());
            writer.WriteEndElement();
        }

        private string ToXml(double lon, double lat)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Location");

            toXml(writer);

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
            Latitude = 100;
            Longitude = 150;

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


        [HttpGet]
        public string save(string ip, int port, int tempo, int duration, string fileName)
        {
            string msg = fileName + " added";
            ICollection<string> elemetsWithGet = addGetAndNewLineToStrings(elements.Values);
            IModel model = MyModel.Instance;
            model.connectClient(ip, port);

            try
            {
                ITelnetClient c = model.getClient();

                bool Completed = ExecuteWithTimeLimit(TimeSpan.FromSeconds(duration), () =>
                {
                    do
                    {
                        string strings = c.read(elemetsWithGet);
                        List<string> values = strings.Split(',').ToList();
                        string properties = extractDouble(values[0]);

                        int length = elements.Values.Count;
                        for (int i = 1; i < length; ++i)
                        {
                            properties += "," + extractDouble(values[i]);
                        }

                        writeToFile(fileName, properties);
                        System.Threading.Thread.Sleep(1000 * tempo);
                    } while (c.isConnected());
                });
            }
            catch
            {
                msg = "there was a problem";
            }

            model.disconnectClient();
            return msg;
        }


        public double Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value + 90;
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
                longitude = value + 180;
                //NotifyPropertyChanged("Longitude");
            }
        }
    }
}
