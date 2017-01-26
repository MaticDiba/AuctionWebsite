using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuctionWeb.Models
{
    public class Image
    {
        public Image()
        {
            this.AuctionOffers = new List<AuctionOffer>();
        }

        public int ImageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string ThumbnailPath { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDateTime { get; set; }
        public int GalleryId { get; set; }

        public virtual ICollection<AuctionOffer> AuctionOffers { get; set; }
        public virtual Gallery Gallery { get; set; }
    }
}