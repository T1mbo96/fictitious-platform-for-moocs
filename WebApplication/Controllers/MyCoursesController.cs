using Microsoft.AspNet.Identity;
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

            foreach(var enr in enrResult)
            {
                courses.Add(db.Courses.Find(enr.CourseId));
            }

            return View(courses);
        }
        
    }
}