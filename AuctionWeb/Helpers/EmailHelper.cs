using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace AuctionWeb.Helpers
{
    public static class EmailHelper
    {
        static MailAddress fromAddress = new MailAddress("dzrjljubljana@gmail.com", "From Name");
        static MailAddress toAddress = new MailAddress("to@example.com", "To Name");
        const string fromPassword = "sovicaoka";
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