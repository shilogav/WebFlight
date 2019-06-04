using FlightSimulator.Model;
using FlightSimulator.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

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
        public ActionResult display(int id, string ip, int port)
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
