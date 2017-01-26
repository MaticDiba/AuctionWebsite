using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AuctionWeb.DAL;
using AuctionWeb.Models;
using AuctionWeb.Helpers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace AuctionWeb.Controllers
{
    public class AuctionOffersController : Controller
    {
        private AuctionContext db = new AuctionContext();

        // GET: AuctionOffers
        [Authorize]
        public ActionResult Index()
        {
            var auctionOffera = db.AuctionOffera.Include(a => a.Image);
            return View(auctionOffera.ToList());
        }

        // GET: AuctionOffers/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuctionOffer auctionOffer = db.AuctionOffera.Find(id);
            if (auctionOffer == null)
            {
                return HttpNotFound();
            }
            return View(auctionOffer);
        }

        // GET: AuctionOffers/Create
        public ActionResult Create(int? imageId)
        {
            ViewBag.ImageId = new SelectList(db.Images, "ImageId", "Name");
            Image im = db.Images.Find(imageId);

            AuctionOffer offer = new AuctionOffer() { ImageId = imageId.GetValueOrDefault() };
            if (im != null && im.AuctionOffers.Count(of => string.IsNullOrEmpty(of.Guid)) > 0)
            {
                offer.Amount = im.AuctionOffers.Where(of => string.IsNullOrEmpty(of.Guid)).Max(of => of.Amount)+1;
            }
            else
            {
                offer.Amount = 20.00;
            }
            return PartialView(offer);
        }

        // POST: AuctionOffers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include = "AuctionOfferId,Amount,ImageId,DateTime,Email,ImageId")] AuctionOffer auctionOffer)
        {
            System.Net.Mail.MailAddress addr = null;
            try
            {
                try
                {
                    addr = new System.Net.Mail.MailAddress(auctionOffer.Email);
                }
                catch
                {
                    return Json(new { message = "Vaš email ni v pravilni obliki!", value = auctionOffer.Amount, close = false }, JsonRequestBehavior.AllowGet);
                }
                var validOffers = db.AuctionOffera.Where(of => of.ImageId == auctionOffer.ImageId && string.IsNullOrEmpty(of.Guid));
                double maxValueForImage = validOffers.Count() > 0 ? validOffers.Max(of => of.Amount) : 0.0;
                if (auctionOffer.Amount <= maxValueForImage)
                {
                    return Json(new { message = "Obstaja višja ponudba, prosimo ponudite večji znesek od: " + maxValueForImage, value = maxValueForImage + 1, close = false }, JsonRequestBehavior.AllowGet);
                }
                if (ModelState.IsValid && addr.Address == auctionOffer.Email)
                {
                    if (db.AuctionOffera.Count(au => au.Email == auctionOffer.Email && au.Amount == auctionOffer.Amount && au.ImageId == auctionOffer.ImageId) > 0)
                    {
                        return Json(new { message = "Ponudba s tem email naslovom in to vrednostjo že obstaja" , value = maxValueForImage + 1, close = false }, JsonRequestBehavior.AllowGet);
                    }
                    auctionOffer.AuctionOfferId = db.AuctionOffera.Count() > 0 ? db.AuctionOffera.Max(au => au.AuctionOfferId) : 1;
                    auctionOffer.DateTime = DateTime.Now;
                    auctionOffer.Guid = Guid.NewGuid().ToString().Replace("-", "_");
                    db.AuctionOffera.Add(auctionOffer);
                    db.SaveChanges();
                    CheckIfUserExistThenCreate(auctionOffer);
                    try
                    {
                        SendMail(auctionOffer);
                        return Json(new { message = "OK", close = true }, JsonRequestBehavior.AllowGet);
                    }catch(Exception ex)
                    {
                        db.AuctionOffera.Remove(auctionOffer);
                        throw ex;
                    }
                    //return RedirectToAction("Index");
                }

                ViewBag.ImageId = new SelectList(db.Images, "ImageId", "Name", auctionOffer.ImageId);
                return Json(new { message = "NotOK", value = auctionOffer.Amount, close = false }, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                return Json(new { message = ex.Message, value = 0, close = false }, JsonRequestBehavior.AllowGet);
            }
            //return View(auctionOffer);
        }

        private void CheckIfUserExistThenCreate(AuctionOffer auctionOffer)
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<App_Start.ApplicationUserManager>();
            var user = userManager.FindByEmail(auctionOffer.Email);
            if (user == null)
            {
                var newUser = new ApplicationUser { UserName = auctionOffer.Email, Email = auctionOffer.Email };
                var result = userManager.CreateAsync(newUser);
            }
       }

        private void SendMail(AuctionOffer auctionOffer)
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.To.Add(auctionOffer.Email);
            msg.IsBodyHtml = true;
            msg.From = new System.Net.Mail.MailAddress("matic.batista@Ius-software.si");
            msg.Subject = "Confirm your offer";
            System.Text.StringBuilder bodi = new System.Text.StringBuilder();
            bodi.Append("<html><body>");

            bodi.Append("Confirm your auction offer by cluicking on link <a href=\"http://dzrjlauction.azurewebsites.net/AuctionOffers/Confirm/?guid=" + auctionOffer.Guid+ "\">confirm</a><br/>");
            bodi.Append("</html></body>");
            msg.Body = bodi.ToString();
            System.Net.Mail.SmtpClient smtp = EmailHelper.GetMailClient();
            smtp.Send(msg);
        }

        public ActionResult Confirm(string guid)
        {
            if(db.AuctionOffera.Count(au => au.Guid.Equals(guid)) > 0){
                AuctionOffer offer = db.AuctionOffera.First(au => au.Guid.Equals(guid));
                if(offer.Image.AuctionOffers.Count(of => string.IsNullOrEmpty(of.Guid) && of.Amount >= offer.Amount) > 0)
                {
                    Session["message"] = "Obstaja ponudba, ki je višja od vaše.";
                    return RedirectToAction("Details", "Auctions", new { id = offer.Image.Gallery.Auction.AuctionId});
                }
                //if(db.AuctionOffera.Count(of => of.ImageId == offer.ImageId && au.gui))
                offer.Guid = string.Empty;
                db.Entry(offer).State = EntityState.Modified;
                db.SaveChanges();
                Session["message"] = "Vaša ponudba je bila zabeležena.";
                return RedirectToAction("Details", "Auctions", new { id = offer.Image.Gallery.Auction.AuctionId });
            }
            else
            {
                Session["message"] = "Vaša ponudba ni bila zabeležena.";
            }
            return RedirectToAction("Index", "Home");
        }

        public void ClearMessage()
        {
            Session["message"] = null;
        }
        // GET: AuctionOffers/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuctionOffer auctionOffer = db.AuctionOffera.Find(id);
            if (auctionOffer == null)
            {
                return HttpNotFound();
            }
            ViewBag.ImageId = new SelectList(db.Images, "ImageId", "Name", auctionOffer.ImageId);
            return View(auctionOffer);
        }

        // POST: AuctionOffers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "AuctionOfferId,Amount,ImageId,DateTime")] AuctionOffer auctionOffer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auctionOffer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ImageId = new SelectList(db.Images, "ImageId", "Name", auctionOffer.ImageId);
            return View(auctionOffer);
        }

        // GET: AuctionOffers/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuctionOffer auctionOffer = db.AuctionOffera.Find(id);
            if (auctionOffer == null)
            {
                return HttpNotFound();
            }
            return View(auctionOffer);
        }

        // POST: AuctionOffers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            AuctionOffer auctionOffer = db.AuctionOffera.Find(id);
            db.AuctionOffera.Remove(auctionOffer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
