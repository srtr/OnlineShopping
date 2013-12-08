using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Domain.Entities
{
    public class ShippingDetails
    {
        [Required(ErrorMessage = "Please enter the address")]
        public string shippingAddress { get; set; }
    }
}
