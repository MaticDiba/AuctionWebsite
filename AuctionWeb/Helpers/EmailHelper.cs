using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Xml.Linq;

namespace AuctionWeb.Helpers
{
    public static class EmailHelper
    {
        static MailAddress fromAddress = new MailAddress(ConfigurationManager.AppSettings["fromAddress"], "Auction website");
        static MailAddress toAddress = new MailAddress("to@example.com", "To Name");
        static string fromPassword = ConfigurationManager.AppSettings["fromPassword"].ToString();
        const string subject = "Subject";
        const string body = "Body";

        public static SmtpClient GetMailClient()
        {
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            return smtp;
        }
        
    }
}