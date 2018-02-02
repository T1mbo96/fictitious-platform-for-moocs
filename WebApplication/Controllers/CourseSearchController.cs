using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class CourseSearchController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult SearchCourse()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SearchResult(string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                return View("SearchCourse");
            }
            string[] searchArr = searchString.Split(' ');
            var result = db.Courses.Where(course => course.Assignments.Any(tag => searchArr.Contains(tag.Tag.Name)));
            var courses = result.ToList();

            ViewBag.SearchString = searchString;

            return View(courses);
        }

        [Authorize]
        [HttpGet]
        public ActionResult SearchResultDetails(int? id, string searchString)
        {
            if (id == null)
            {
                return View("SearchCourse");
            }
            else
            {
                Course course = db.Courses.Find(id);

                enrollAuthenticatedUser(course);

                List<ContentGroup> groupedContentGroups = groupContentGroups((int) id);
                List<ContentGroup> sortedGroupedContentGroups = sortContentGroupsAndElements(groupedContentGroups);

                ViewBag.SearchString = searchString;
                ViewBag.CourseName = course.Title;

                return View(sortedGroupedContentGroups);
            }
        }

        // Gruppiert alle ContentGroups eines Course und alle ContentElements der jeweiligen ContentGroup.
        public List<ContentGroup> groupContentGroups(int id)
        {
            return db.ContentGroups.Where(contentGroup => contentGroup.CourseId == id).ToList();
        }

        // Behelfsmethode zur richtigen Sortierung der ContentGroups und deren ContentElements.
        public List<ContentGroup> sortContentGroupsAndElements(List<ContentGroup> groupedContentGroups)
        {
            groupedContentGroups = groupedContentGroups.OrderBy(order => order.Order).ToList();

            foreach (ContentGroup cg in groupedContentGroups)
            {
                cg.ContentElements = cg.ContentElements.OrderBy(order => order.Order).ToList();
            }

            return groupedContentGroups;
        }

        // Enrolled einen User in einen Course, falls dieser noch nicht enrolled ist.
        public void enrollAuthenticatedUser(Course course)
        {
            var userId = User.Identity.GetUserId();
            if (!db.Enrollments.Any(enrollment => enrollment.ApplicationUser.Id == userId && enrollment.CourseId == course.Id))
            {
                Enrollment enr = new Enrollment();

                enr.ApplicationUser = db.Users.Find(userId);
                enr.Course = course;
                enr.CourseId = course.Id;
                enr.Datetime = DateTime.Now;
                enr.Rating = Rating.None;

                db.Enrollments.Add(enr);
                db.SaveChanges();
            }
        }
    }
}