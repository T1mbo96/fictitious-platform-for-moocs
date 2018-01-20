using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

                List<ContentGroupViewModel> groupedContentGroups = groupContentGroupElements((int) id);
                List<ContentGroupViewModel> sortedGroupedContentGroups = sortContentGroupsAndElements(groupedContentGroups);

                ViewBag.SearchString = searchString;
                ViewBag.CourseName = course.Title;

                return View(sortedGroupedContentGroups);
            }
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
                        cgvw.ContentElements.Add(db.ContentElements.Find(cg.ContentId));
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

                db.Enrollments.Add(enr);
                db.SaveChanges();
            }
        }
    }
}