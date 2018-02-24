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
        public ActionResult SearchResultDetails(int? id, string searchString, bool b)
        {
            if (id == null)
            {
                return View("SearchCourse");
            }
            else if (b == true)
            {
                Course course = db.Courses.Find(id);
                List<ContentGroup> sortedGroupedContentGroups = processContentGroups((int)id);

                if (checkIfRated((int) id))
                {
                    ViewBag.AlreadyRated = true;
                } else
                {
                    ViewBag.AlreadyRated = false;
                }

                ViewBag.SearchString = searchString;
                ViewBag.CourseName = course.Title;
                ViewBag.EnrId = getEnrollmentId(course);

                return View("~/Views/MyCourses/MyCoursesDetails.cshtml", sortedGroupedContentGroups);
            }
            else
            {
                Course course = db.Courses.Find(id);
                List<ContentGroup> sortedGroupedContentGroups = processContentGroups((int)id);

                enrollAuthenticatedUser(course);

                if (checkIfRated((int)id))
                {
                    ViewBag.AlreadyRated = true;
                }
                else
                {
                    ViewBag.AlreadyRated = false;
                }

                ViewBag.SearchString = searchString;
                ViewBag.CourseName = course.Title;
                ViewBag.EnrId = getEnrollmentId(course);

                return View(sortedGroupedContentGroups);
            }
        }

        private bool checkIfRated(int id)
        {
            bool rated = false;
            var userId = User.Identity.GetUserId();

            if (!db.Enrollments.Where(enrollment => enrollment.ApplicationUser.Id == userId && enrollment.CourseId == id).ToList().First().Rating.Equals(Rating.None)) {
                rated = true;
            }

            return rated;
        }

        // Stellt die fertig sortierte Liste der ContentGroups für die Weiterverarbeitung zusammen.
        private List<ContentGroup> processContentGroups(int id)
        {
            return sortContentGroupsAndElements(groupContentGroups((int)id));
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

        public int getEnrollmentId(Course course)
        {
            var userId = User.Identity.GetUserId();

            return db.Enrollments.Where(enrollment => enrollment.ApplicationUser.Id == userId && enrollment.CourseId == course.Id).ToArray()[0].Id;
        }

        public JsonResult SetRating(int enrId, int rating)
        {
            setRatingForEnrollment(enrId, rating);
            return Json(averageRating(enrId), JsonRequestBehavior.AllowGet);
        }

        public void setRatingForEnrollment(int enrId, int rating)
        {
            var enr = db.Enrollments.Find(enrId);

            enr.Rating = (Rating) rating;

            db.SaveChanges();
        }

        public double averageRating(int enrId)
        {
            Enrollment enr = db.Enrollments.Find(enrId);

            var ratingList = db.Enrollments.Where(enrollment => enrollment.CourseId == enr.CourseId && ((int) enrollment.Rating) != ((int) Rating.None)).ToList();

            double sumRating = 0.0;
            int counter = 0;

            foreach(var item in ratingList)
            {
                sumRating += (double) item.Rating;
                counter++;
            }

            return (sumRating / counter);
        }

        public JsonResult GetTags(String text)
        {
            return Json(db.Tags.Where(tag => tag.Name.StartsWith(text)).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult AlreadyRated(int enrId)
        {
            var enr = db.Enrollments.Find(enrId);
            bool alreadyRated = false;

            if (enr.Rating != Rating.None)
            {
                alreadyRated = true;
            }

            return Json(alreadyRated, JsonRequestBehavior.AllowGet);
        }

    }
}