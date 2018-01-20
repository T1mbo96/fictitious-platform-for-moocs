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

        // Gruppiert alle ContentGroups eines Course und alle ContentElements der jeweiligen ContentGroup.
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

        // Behelfsmethode zur richtigen Sortierung der ContentGroups und deren ContentElements.
        public List<ContentGroupViewModel> sortContentGroupsAndElements(List<ContentGroupViewModel> groupedContentGroups)
        {
            List<ContentGroupViewModel> orderedContentGroups = orderContentGroups(groupedContentGroups);

            foreach(ContentGroupViewModel cgvw in orderedContentGroups)
            {
                cgvw.ContentElements = orderContentElements((List<ContentElement>) cgvw.ContentElements);
            }

            return orderedContentGroups;
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

        // BubbleSort-Algorithmus zur Sortierung der ContentGroups über das ContentGroupViewModel nach der Order.
        public List<ContentGroupViewModel> orderContentGroups(List<ContentGroupViewModel> groupedContentGroups)
        {
            ContentGroupViewModel[] groupArr = groupedContentGroups.ToArray();

            for(int i = 0; i < groupArr.Length; i++)
            {
                for(int j = i; j < groupArr.Length; j++)
                {
                    if(groupArr[i].Order > groupArr[j].Order)
                    {
                        ContentGroupViewModel cgvw = groupArr[i];
                        groupArr[i] = groupArr[j];
                        groupArr[j] = cgvw;
                    }
                }
            }

            return groupArr.ToList();
        }

        // BubbleSort-Algorithmus zur Sortierung der ContentElements nach der Order.
        public List<ContentElement> orderContentElements(List<ContentElement> groupedContentElements)
        {
            ContentElement[] elementArr = groupedContentElements.ToArray();

            for(int i = 0; i < elementArr.Length; i++)
            {
                for(int j = i; j < elementArr.Length; j++)
                {
                    if(elementArr[i].Order > elementArr[j].Order)
                    {
                        ContentElement ce = elementArr[i];
                        elementArr[i] = elementArr[j];
                        elementArr[j] = ce;
                    }
                }
            }

            return elementArr.ToList();
        }
    }
}