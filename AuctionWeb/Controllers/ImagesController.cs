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
using System.IO;
using System.Web.Hosting;

namespace AuctionWeb.Controllers
{
    public class ImagesController : Controller
    {
        private AuctionContext db = new AuctionContext();
        FilesHelper filesHelper;
        String tempPath = "~/somefiles/";
        String serverMapPath = "~/Files/";
        private string StorageRoot
        {
            get { return Path.Combine(HostingEnvironment.MapPath(serverMapPath)); }
        }
        private string UrlBase = "/Files/";
        String DeleteURL = "/Images/Delete/";
        String DeleteType = "GET";

        public ImagesController()
        {
            filesHelper = new FilesHelper(DeleteURL, DeleteType, StorageRoot, UrlBase, tempPath, serverMapPath);
        }
        // GET: Images
        [Authorize]
        public ActionResult Index()
        {
            var images = db.Images.Include(i => i.Gallery);
            return View(images.ToList());
        }

        // GET: Images/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // GET: Images/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.GalleryId = new SelectList(db.Galleries, "GalleryId", "Name");
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ImageId,Name,Description,Path,ThumbnailPath,CreatedBy,CreatedDateTime,GalleryId")] Image image)
        {
            if (ModelState.IsValid)
            {
                db.Images.Add(image);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GalleryId = new SelectList(db.Galleries, "GalleryId", "Name", image.GalleryId);
            return View(image);
        }

        // GET: Images/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            ViewBag.GalleryId = new SelectList(db.Galleries, "GalleryId", "Name", image.GalleryId);
            return View(image);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ImageId,Name,Description,Path,ThumbnailPath,CreatedBy,CreatedDateTime,GalleryId")] Image image)
        {
            if (ModelState.IsValid)
            {
                db.Entry(image).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GalleryId = new SelectList(db.Galleries, "GalleryId", "Name", image.GalleryId);
            return View(image);
        }

        // GET: Images/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
            Image image = db.Images.Find(id);
            if (image == null)
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }

            db.Images.Remove(image);
            db.SaveChanges();

            return Json("OK", JsonRequestBehavior.AllowGet);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Image image = db.Images.Find(id);
            db.Images.Remove(image);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public JsonResult Upload(int GalleryId)
        {
            var resultList = new List<ViewDataUploadFilesResult>();

            var CurrentContext = HttpContext;

            filesHelper.UploadAndShowResults(CurrentContext, resultList);
            JsonFiles files = new JsonFiles(resultList);

            bool isEmpty = !resultList.Any();
            if (isEmpty)
            {
                return Json("Error ");
            }
            else
            {
                foreach(var file in files.files)
                {
                    int iMaxImageId = db.Images.Count()> 0 ? db.Images.Max(im => im.ImageId) : 1;
                    Image newImage = new Image() { ImageId = iMaxImageId+1, Name = file.name, Path = file.url, ThumbnailPath = file.thumbnailUrl, GalleryId = GalleryId, CreatedDateTime = DateTime.Now };

                    db.Images.Add(newImage);
                    db.SaveChanges();
                    file.ImageId = newImage.ImageId;
                }
                return Json(files);
            }
        }

        [Authorize]
        public JsonResult GetImages4Gallery(int GalleryId)
        {
            Gallery gallery = db.Galleries.Find(GalleryId);

            List<Image> images = gallery.Images.ToList();

            var list = filesHelper.GetFileList(images);
            return Json(list, JsonRequestBehavior.AllowGet);
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
