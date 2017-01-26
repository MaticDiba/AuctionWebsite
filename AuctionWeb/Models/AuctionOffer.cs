using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AuctionWeb.Models
{
    public class AuctionOffer
    {
        public int AuctionOfferId { get; set; }
        public Double Amount { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email naslov je potreben!")]
        [EmailAddress(ErrorMessage = "Nepravilen email naslov!")]
        public string Email { get; set; }
        public int ImageId { get; set; }
        public string Guid { get; set; }
        public System.DateTime DateTime { get; set; }

        public virtual Image Image { get; set; }
    }
}