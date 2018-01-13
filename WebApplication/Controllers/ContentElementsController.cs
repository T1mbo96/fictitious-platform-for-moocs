using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class ContentElementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ContentElements
        public ActionResult Index()
        {
            var contentElements = db.ContentElements.Include(c => c.Type);
            return View(contentElements.ToList());
        }

        // GET: ContentElements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentElement contentElement = db.ContentElements.Find(id);
            if (contentElement == null)
            {
                return HttpNotFound();
            }
            return View(contentElement);
        }

        // GET: ContentElements/Create
        public ActionResult Create()
        {
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name");
            return View();
        }

        // POST: ContentElements/Create
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,URL,Order,TypeId")] ContentElement contentElement)
        {
            if (ModelState.IsValid)
            {
                db.ContentElements.Add(contentElement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name", contentElement.TypeId);
            return View(contentElement);
        }

        // GET: ContentElements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentElement contentElement = db.ContentElements.Find(id);
            if (contentElement == null)
            {
                return HttpNotFound();
            }
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name", contentElement.TypeId);
            return View(contentElement);
        }

        // POST: ContentElements/Edit/5
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,URL,Order,TypeId")] ContentElement contentElement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contentElement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TypeId = new SelectList(db.Types, "Id", "Name", contentElement.TypeId);
            return View(contentElement);
        }

        // GET: ContentElements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentElement contentElement = db.ContentElements.Find(id);
            if (contentElement == null)
            {
                return HttpNotFound();
            }
            return View(contentElement);
        }

        // POST: ContentElements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContentElement contentElement = db.ContentElements.Find(id);
            db.ContentElements.Remove(contentElement);
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
