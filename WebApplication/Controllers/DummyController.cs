using FlightSimulator.Model;
using FlightSimulator.Model.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        [HttpGet]
        public ActionResult loadAndDisplay(string path, int tempo)
        {
            string[] lines = readFromFile(path);
            foreach(string line in lines)
            {
                List<string> values = line.Split(',').ToList();
                Session["lon"] = values[0];
                Session["lat"] = values[0];
                Session["rudder"] = values[0];
                Session["throttle"] = values[0];
                System.Threading.Thread.Sleep(1000 * tempo);
            }

            return View();


            /*InfoModel.Instance.ip = ip;
            InfoModel.Instance.port = port.ToString();
            InfoModel.Instance.time = time;

            InfoModel.Instance.ReadData("Dor");

            


            return View();*/
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
