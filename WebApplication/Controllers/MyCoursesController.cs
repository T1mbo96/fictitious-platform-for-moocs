using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class MyCoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult MyCourses()
        {
            var userId = User.Identity.GetUserId();
            var enrResult = db.Enrollments.Where(enrollment => enrollment.ApplicationUser.Id == userId).ToList();
            var courses = new List<Course>();

            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var currentUser = userManager.FindById(userId);

            foreach (var enr in enrResult)
            {
                courses.Add(db.Courses.Find(enr.CourseId));
            }

            ViewBag.CurrentUser = currentUser.Id;

            return View(courses);
        }

        public ActionResult EditCourse(int? id)
        {
            if (id == null)
            {
                return View("~/Views/Home/Index.cshtml");
            } else
            {
                CourseSearchController cs = new CourseSearchController();
                Course course = db.Courses.Find(id);
                List<ContentGroup> sortedGroupedContentGroups = cs.processContentGroups((int)id);

                ViewBag.CourseName = course.Title;

                return View(sortedGroupedContentGroups);
            }
        }
        
    }
}