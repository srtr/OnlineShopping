using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Domain.Entities
{
    public class Manufacturer
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int manufacturerID { get; set; }
        public string manufacturerName { get; set; }
    }
}
