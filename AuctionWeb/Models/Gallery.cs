using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuctionWeb.Models
{
    public class Gallery
    {
        public Gallery()
        {
            this.Images = new List<Image>();
        }

        public int GalleryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDateTime { get; set; }

        public int AuctionId { get; set; }
        public virtual Auction Auction { get; set; }
        public virtual ICollection<Image> Images { get; set; }
    }
}