using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Project_IT.Models;

namespace Project_IT.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FeedItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FeedItems
        public ActionResult Index()
        {
            var feedItems = db.FeedItems.OrderByDescending(f => f.CreatedOn).ToList();
            return View(feedItems);
        }

        // GET: FeedItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedItem feedItem = db.FeedItems.Find(id);
            if (feedItem == null)
            {
                return HttpNotFound();
            }
            return View(feedItem);
        }

        // GET: FeedItems/Create
        public ActionResult Create()
        {
            return View(new FeedItem { CreatedOn = DateTime.UtcNow, IsPublished = true });
        }

        // POST: FeedItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Subtitle,Description,ImageUrl,Category,LabelBadge,OfferText,ButtonText,ActionUrl,ValidTo,IsFeatured,IsPublished,CreatedOn")] FeedItem feedItem)
        {
            if (ModelState.IsValid)
            {
                if (feedItem.CreatedOn == default(DateTime))
                {
                    feedItem.CreatedOn = DateTime.UtcNow;
                }
                db.FeedItems.Add(feedItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(feedItem);
        }

        // GET: FeedItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedItem feedItem = db.FeedItems.Find(id);
            if (feedItem == null)
            {
                return HttpNotFound();
            }
            return View(feedItem);
        }

        // POST: FeedItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Subtitle,Description,ImageUrl,Category,LabelBadge,OfferText,ButtonText,ActionUrl,ValidTo,IsFeatured,IsPublished,CreatedOn")] FeedItem feedItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(feedItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(feedItem);
        }

        // GET: FeedItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FeedItem feedItem = db.FeedItems.Find(id);
            if (feedItem == null)
            {
                return HttpNotFound();
            }
            return View(feedItem);
        }

        // POST: FeedItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FeedItem feedItem = db.FeedItems.Find(id);
            db.FeedItems.Remove(feedItem);
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
