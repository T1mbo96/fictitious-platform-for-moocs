using Microsoft.AspNet.Identity;
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
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Courses
        public ActionResult Index()
        {
            return View(db.Courses.ToList());
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
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

        public ActionResult SearchCourse()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchResult(string searchString)
        {
            if(String.IsNullOrEmpty(searchString))
            {
                return View("SearchCourse");
            }
            string[] searchArr = searchString.Split(' ');
            var result = db.Courses.Where(course => course.Assignments.Any(tag => searchArr.Contains(tag.Tag.Name)));
            var courses = result.ToList();
            return View(courses);
        }

        [Authorize]
        public ActionResult SearchResultDetails(int id)
        {
            Course course = db.Courses.Find(id);

            enrollUser(course);

            List<ContentGroupViewModel> groupedContentGroups = groupContentGroupElements(id);
            List<ContentGroupViewModel> sortedGroupedContentGroups = sortContentGroupsAndElements(groupedContentGroups);

            return View(sortedGroupedContentGroups);
        }

        public ActionResult AddCourse()
        {
            return View();
        }

        public ActionResult MyCourses()
        {
            return View();
        }

        public void enrollUser(Course course)
        {
            Enrollment enr = new Enrollment();
            var currentUserId = User.Identity.GetUserId();
            var user = db.Users.Find(currentUserId);

            enr.ApplicationUser = user;
            enr.Course = course;
            enr.CourseId = course.Id;
            enr.Datetime = DateTime.Now;

            db.Enrollments.Add(enr);
            db.SaveChanges();
        }

        public List<ContentGroupViewModel> groupContentGroupElements(int id)
        {
            List<ContentGroup> contentGroups = db.ContentGroups.Where(contentGroup => contentGroup.CourseId == id).ToList();
            List<ContentGroupViewModel> groupedContentGroups = new List<ContentGroupViewModel>();

            while (contentGroups.Any())
            {
                ContentGroupViewModel cgvw = new ContentGroupViewModel();
                cgvw.Name = contentGroups.First().Header;
                cgvw.Order = contentGroups.First().Order;

                foreach (ContentGroup cg in contentGroups.Reverse<ContentGroup>())
                {
                    if (cgvw.Name == cg.Header)
                    {
                        cgvw.ContentElements.Add(db.ContentElements.Find(cg.ContentElement));
                        contentGroups.Remove(cg);
                    }
                }

                groupedContentGroups.Add(cgvw);
            }

            return groupedContentGroups;
        }

        public List<ContentGroupViewModel> sortContentGroupsAndElements(List<ContentGroupViewModel> groupedContentGroups)
        {
            groupedContentGroups.OrderBy(contentGroup => contentGroup.Order);
            foreach (ContentGroupViewModel cgvw in groupedContentGroups)
            {
                cgvw.ContentElements.OrderBy(contentElement => contentElement.Order);
            }

            return groupedContentGroups;
        }
    }
}
