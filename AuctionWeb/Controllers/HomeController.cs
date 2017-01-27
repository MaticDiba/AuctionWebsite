using AuctionWeb.DAL;
using AuctionWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuctionWeb.Controllers
{
    public class HomeController : Controller
    {
        private AuctionContext db = new AuctionContext();
        public ActionResult Index()
        {
            List<Auction> auctions = db.Auctions.OrderByDescending(x => x.CreatedDateTime).Take(5).ToList();
            return View(auctions);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}