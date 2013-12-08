using ActionMailer.Net.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShopping.WebUI.Models;
using System.Net;

namespace OnlineShopping.WebUI.Controllers
{
    public class MailController : ActionMailer.Net.Mvc.MailerBase
    {
        public EmailResult SampleEmail( OnlineTransactionsViewModel model, string subject, string customerEmail)
        {
            To.Add(customerEmail);
            //Membership.GetUser(User.Identity.Name, true /* userIsOnline */).ProviderUserKey.ToString()
            From = "no-reply@pizza.com";
            // Subject = "Transaction details for transaction ID:11223344 dated : 10/11/2013";
            Subject = subject;
            return Email("SampleEmail", model);
        }
    }
}
