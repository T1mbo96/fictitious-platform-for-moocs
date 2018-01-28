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
using System.Collections.Generic;


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

        public class CourseCreationViewModel
        {
            public string Title { get; set; }
            public string Description { get; set; }
         
        }
        public ActionResult AddCourse()
        {
            return View("AddCourse");
        }
        [HttpPost]
        public ActionResult AddCourse(WebApplication.Models.Course cc, string Tags)
        {
            Course cou = new Course();
            cou.Description = cc.Description;
            cou.Title = cc.Title;
            char delimiter = ',';
            string[] tg = Tags.Split(delimiter);
           
            LinkedList<Assignment> AssigC = new LinkedList<Assignment>();
            
            
           
        
            
            foreach (var substring in tg)
            {   WebApplication.Models.Assignment assign = new WebApplication.Models.Assignment();
                    assign.CourseId = cou.Id;
                    assign.Course = cou;
                int count = db.Tags.Count();
             for (int i=0; i <= count; i++){
                   
                if (count!=0&&(db.Tags.Find(i+1).Name==substring)){


                        assign.Tag = db.Tags.Find(i);
                        assign.TagId = db.Tags.Find(i).Id;

                        db.Tags.Find(i).Assignments.Add(assign);
                        db.SaveChanges();
                        AssigC.AddFirst(assign);

                        db.Assignments.Add(assign);
                        db.SaveChanges();



                    }
                    else if (i==count)
                {

                        WebApplication.Models.Tag tag = new WebApplication.Models.Tag();
                        tag.Name = substring;
                        assign.Tag = tag;
                        assign.TagId = tag.Id;


                        AssigC.AddFirst(assign);
                        LinkedList<Assignment> AssignT = new LinkedList<Assignment>();
                        tag.Assignments = AssignT;

                        tag.Assignments.Add(assign);


                        db.Tags.Add(tag);
                        db.SaveChanges();
                        db.Assignments.Add(assign);
                        db.SaveChanges();


                     

                    }
            
           
       }
            }
            cou.Assignments = AssigC;
            db.Courses.Add(cou);
            db.SaveChanges();
            return RedirectToAction("Details", cou.Id);
        }

        

        public ActionResult MyCourses()
        {
            return View();
        }
    }
}
