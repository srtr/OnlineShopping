using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OnlineShopping.Domain.Entities
{
    public class Outlet
    {
        [HiddenInput(DisplayValue = false)]
        public int outletID { get; set; }
        public string owner { get; set; }
        public string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

    }
}
