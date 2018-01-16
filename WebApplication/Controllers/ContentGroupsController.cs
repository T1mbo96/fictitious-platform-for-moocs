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
    public class ContentGroupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ContentGroups
        public ActionResult Index()
        {
            var contentGroups = db.ContentGroups.Include(c => c.Course);
            return View(contentGroups.ToList());
        }

        // GET: ContentGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentGroup contentGroup = db.ContentGroups.Find(id);
            if (contentGroup == null)
            {
                return HttpNotFound();
            }
            return View(contentGroup);
        }

        // GET: ContentGroups/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Title");
            ViewBag.ContentId = new SelectList(db.ContentElements, "Id", "Description");
            return View();
        }

        // POST: ContentGroups/Create
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Order,Header,CourseId,ContentId")] ContentGroup contentGroup)
        {
            if (ModelState.IsValid)
            {
                db.ContentGroups.Add(contentGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Title", contentGroup.CourseId);
            ViewBag.TagId = new SelectList(db.ContentElements, "Id", "Description", contentGroup.ContentId);
            return View(contentGroup);
        }

        // GET: ContentGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentGroup contentGroup = db.ContentGroups.Find(id);
            if (contentGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Title", contentGroup.CourseId);
            return View(contentGroup);
        }

        // POST: ContentGroups/Edit/5
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Order,Header,CourseId,ContentId")] ContentGroup contentGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contentGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "Id", "Title", contentGroup.CourseId);
            return View(contentGroup);
        }

        // GET: ContentGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentGroup contentGroup = db.ContentGroups.Find(id);
            if (contentGroup == null)
            {
                return HttpNotFound();
            }
            return View(contentGroup);
        }

        // POST: ContentGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContentGroup contentGroup = db.ContentGroups.Find(id);
            db.ContentGroups.Remove(contentGroup);
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
