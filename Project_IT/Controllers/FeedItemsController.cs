using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using log4net;
using Project_IT.Models;
using Project_IT.Services;

namespace Project_IT.Controllers
{
    [Authorize]
    public class FeedItemsController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FeedItemsController));
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FeedItems
        public ActionResult Index()
        {
            try
            {
                var feedItems = db.FeedItems.OrderByDescending(f => f.CreatedOn).ToList();
                return View(feedItems);
            }
            catch (Exception ex)
            {
                log.Error("Error in Index: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: FeedItems/Details/5
        public ActionResult Details(int? id)
        {
            try
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
            catch (Exception ex)
            {
                log.Error("Error in Details: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: FeedItems/Create
        public ActionResult Create()
        {
            try
            {
                return View(new FeedItem { IsPublished = true });
            }
            catch (Exception ex)
            {
                log.Error("Error in Create: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: FeedItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FeedItem feedItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.FeedItems.Add(feedItem);
                    db.SaveChanges();

                    SitemapGenerator.RegenerateSitemap();

                    return RedirectToAction("Index");
                }

                return View(feedItem);
            }
            catch (Exception ex)
            {
                log.Error("Error in Create [POST]: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: FeedItems/Edit/5
        public ActionResult Edit(int? id)
        {
            try
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
            catch (Exception ex)
            {
                log.Error("Error in Edit: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: FeedItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FeedItem feedItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(feedItem).State = EntityState.Modified;
                    db.SaveChanges();

                    SitemapGenerator.RegenerateSitemap();

                    return RedirectToAction("Index");
                }
                return View(feedItem);
            }
            catch (Exception ex)
            {
                log.Error("Error in Edit [POST]: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: FeedItems/Delete/5
        public ActionResult Delete(int? id)
        {
            try
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
            catch (Exception ex)
            {
                log.Error("Error in Delete: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: FeedItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                FeedItem feedItem = db.FeedItems.Find(id);
                db.FeedItems.Remove(feedItem);
                db.SaveChanges();

                SitemapGenerator.RegenerateSitemap();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Error in DeleteConfirmed: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
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
