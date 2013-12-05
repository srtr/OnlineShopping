using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace OnlineShopping.Domain.Entities
{
    public class OnlineTransaction
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int transactionID { get; set; }
        public DateTime date { get; set; }
        public int cashierID { get; set; }
        public string userKey { get; set; }
    }
}