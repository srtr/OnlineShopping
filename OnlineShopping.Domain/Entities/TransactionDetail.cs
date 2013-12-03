using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace OnlineShopping.Domain.Entities
{
    public class TransactionDetail
    {
        [Key, Column(Order = 0)]
        public int transactionID { get; set; }
        [Key, Column(Order = 2)]
        public string barcode { get; set; }
        public int unitSold { get; set; }
        public float cost { get; set; }
    }
}