using OnlineShopping.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShopping.WebUI.Models
{
    public class OnlineTransactionsViewModel
    {
        public IEnumerable<OnlineTransaction> onlineTransactions { get; set; }
        public IEnumerable<OnlineTransactionDetail> onlineTransactionDetails{ get; set; }
        public IEnumerable<Product> products { get; set; }
    }
}