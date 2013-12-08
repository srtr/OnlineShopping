using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShopping.WebUI.Controllers
{
    public class GeoLocationController : Controller
    {
        //
        // GET: /GeoLocation/

        public string mobileLogin(string userName=null,string password=null)
        {
            //return "success";
            return "{\"firstname\" : saran , \"lastname\" : kumar}";
        }
        public ViewResult storeLocator()
        {
            return View();
        }
    }
}


