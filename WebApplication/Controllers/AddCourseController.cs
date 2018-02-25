using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class AddCourseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: AddCourse
        public ActionResult Index()
        {   
            return View();
        }


        [HttpPost]
        public ActionResult AddCourse(String CourseTitle, String Description, String Tags)
        {
            //new added
            Course course = new Course();

            course.Title = CourseTitle;
            course.Description = Description;
            Microsoft.AspNet.Identity.UserManager<ApplicationUser> us = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            course.Owner = us.FindById(User.Identity.GetUserId());

            //old
            char delimiter = ',';
            string[] tg = Tags.Split(delimiter);

            LinkedList<Assignment> AssigC = new LinkedList<Assignment>();
            List<Tag> AllTags = new List<Tag>();

            int count = 0;
            Tag Element = new Tag();

            foreach (var subString in tg)
            {
                string Temp = Regex.Replace(subString, @"(\s)", string.Empty);
                Assignment assign = new Assignment();

                assign.Course = course;
                assign.CourseId = course.Id;
                AllTags = db.Tags.ToList();

                count = db.Tags.Count();
                for (int i = 0; i <= count; i++)
                {

                    if (count != 0 && i != count)
                    {
                        Element = AllTags.ElementAt(i);
                    }

                    if (i != count && count != 0 && (Element.Name == Temp))
                    {
                        assign.Tag = Element;
                        assign.TagId = Element.Id;

                        db.Tags.Find(Element.Id).Assignments.Add(assign);

                        AssigC.AddFirst(assign);

                        db.Assignments.Add(assign);
                        db.SaveChanges();

                        break;
                    }
                    else if (i == count)
                    {

                        WebApplication.Models.Tag tag = new WebApplication.Models.Tag();
                        tag.Name = Temp;
                        assign.Tag = tag;
                        assign.TagId = tag.Id;

                        AssigC.AddFirst(assign);
                        LinkedList<Assignment> AssignT = new LinkedList<Assignment>();
                        tag.Assignments = AssignT;

                        tag.Assignments.Add(assign);

                        db.Tags.Add(tag);

                        db.Assignments.Add(assign);
                        db.SaveChanges();
                    }
                }
            }

            ViewBag.CourseTitle = CourseTitle;
            ViewBag.CourseDescription = Description;
            ViewBag.CourseId = course.Id;

            db.SaveChanges();

            CourseSearchController cs = new CourseSearchController();
            List<ContentGroup> sortedGroupedContentGroups = cs.processContentGroups(course.Id);

            return View(sortedGroupedContentGroups);
        }

        public ActionResult AddContent()
        {
            return View("~/Views/Home/Index.cshtml");
        }
    }
}