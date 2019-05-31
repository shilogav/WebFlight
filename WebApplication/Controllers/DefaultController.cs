﻿using FlightSimulator.Model;
using FlightSimulator.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

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


            return View();

            
            /*InfoModel.Instance.ip = ip;
            InfoModel.Instance.port = port.ToString();
            InfoModel.Instance.time = time;

            InfoModel.Instance.ReadData("Dor");

            Session["time"] = time;


            return View();*/
        }

        private ActionResult displayLocation(string ip, string port)
        {

            return View();
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