using FlightSimulator.Model;
using FlightSimulator.Model.Interface;
using System;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication.Controllers
{
    public class DefaultController : Controller
    {
        private double latitude;
        private double longitude;

        // GET: Default
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult display(string ip, int port)
        {
            IModel model = MyModel.Instance;
            model.openServer(ip, port);
            
            Longitude = model.Longitude;
            Latitude = model.Latitude;

            Session["time"] = 0;
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


        [HttpGet]
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
            System.IO.File.AppendAllText(file, content+Environment.NewLine);
            
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
            string num = values.FindLast(
            delegate (string s)
            {
                return s.StartsWith("'") && str.EndsWith("'");
            });
            num = num.Replace("'", "");
            return num;
        }

        [HttpGet]
        public string save(string ip, int port, int tempo, int duration, string fileName)
        {


            IModel model = MyModel.Instance;
            

            string[] elements = {"get /position/longitude-deg" + Environment.NewLine ,
                                 "get /position/latitude-deg" + Environment.NewLine,
                                 "get /instrumentation/airspeed-indicator/indicated-speed-kt" + Environment.NewLine,
                                 "get /instrumentation/altimeter/indicated-altitude-ft" + Environment.NewLine };
            // TODO: add direction *******************************************************



            do
            {
                model.connectClient(ip, port);
                try
                {
                    string strings = model.getClient().read(elements);
                    List<string> values = strings.Split(',').ToList();

                    string properties = extractDouble(values[0]);
                    for (int i = 1; i < elements.GetLength(0); ++i)
                    {
                        properties += "," + extractDouble(values[i]);
                    }

                    writeToFile(fileName, properties);
                    model.disconnectClient();
                }
                catch { }

                System.Threading.Thread.Sleep(1000 / tempo);
            } while (model.isClientConnected());





            // /save/127.0.0.1/5400/4/10/flight1
            model.disconnectClient();
            return fileName + " added";
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
