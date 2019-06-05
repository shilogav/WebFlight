using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication.Controllers
{
    public class DummyController : ApiController
    {
         

        /*
        [HttpGet]
        public ActionResult loadAndDisplay(string ip, int port)
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


            return View();
        }*/
    }
}
