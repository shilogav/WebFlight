using FlightSimulator.Model;
using FlightSimulator.Model.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WebApplication.Controllers;

namespace WebApplication.Controllers
{
    public class dummyController : Controller
    {
        private double latitude;
        private double longitude;

        // GET: dummy
        public ActionResult Index()
        {
            return View();
        }

        string[] readFromFile(string path)
        {
            var dir = Server.MapPath("~\\files");
            var file = Path.Combine(dir, path);
            
            return System.IO.File.ReadAllLines(file);
        }

        private string[]lines;
        private int lineIndex;

        [HttpGet]
        public ActionResult loadAndDisplay(string path, int tempo)
        {
            var dir = Server.MapPath("~\\files");
            string file = Path.Combine(dir, path);
            lines = readFromFile(file);

            string line = lines[lineIndex];

            Session["time"] = tempo;
            Session["path"] = path;
            List<string> values = line.Split(',').ToList();

            Session["lon"] = values[0];
            Session["lat"] = values[1];

            return View();
            


            /*InfoModel.Instance.ip = ip;
            InfoModel.Instance.port = port.ToString();
            InfoModel.Instance.time = time;

            InfoModel.Instance.ReadData("Dor");

            


            return View();*/
        }

        [HttpPost]
        public string GetCourse()
        {
            
            if (lineIndex >= lines.GetLength(0))
            {
                // TODO: add if and alert when the lines end. **********************************************************
            }

            string line = lines[lineIndex];
            List<string> values = line.Split(',').ToList();


            string rudder = values[2];
            string throttle = values[3];

            lineIndex++;

            return ToXml(Double.Parse(rudder), Double.Parse(throttle));
        }


        private void toXml(XmlWriter writer, double rudder, double throttle)
        {
            writer.WriteStartElement("Diection");
            writer.WriteElementString("throttle", throttle.ToString());
            writer.WriteElementString("rudder", rudder.ToString());
            writer.WriteEndElement();
        }

        private string ToXml(double rudder, double throttle)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Diection");

            toXml(writer, rudder, throttle);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
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
