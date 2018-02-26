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
        [Authorize]
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
            WebApplication.Models.Enrollment En = new WebApplication.Models.Enrollment();
            En.Datetime = DateTime.Now;
            En.CourseId = course.Id;
            En.Course = course;
            En.ApplicationUser = us.FindById(User.Identity.GetUserId());
            LinkedList<Enrollment> Enrollments = new LinkedList<Enrollment>();
            Enrollments.AddFirst(En);
            course.Enrollments = Enrollments;
            db.Enrollments.Add(En);

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

            return View(sortedGroupedContentGroups); ;
        }

        public ActionResult AddContent(String description, String title, int id, String[] header, int[] order)
        {
            updateCourse(description, title, id);

            if (!((header == null || order == null) || (header.Length == 0 || order.Length == 0)))
            {
                updateContentGroups(header, order, id);
            }

            CourseSearchController cs = new CourseSearchController();
            List<ContentGroup> sortedGroupedContentGroups = cs.processContentGroups(id);

            List<int> counterList = new List<int>();
            for (int i = 0; i < sortedGroupedContentGroups.Count(); i++)
            {
                counterList.Add(sortedGroupedContentGroups[i].ContentElements.Count());
            }

            ViewBag.CounterList = counterList;
            ViewBag.CourseId = id;
            ViewBag.CourseDescription = description;
            ViewBag.CourseTitle = title;

            return View(sortedGroupedContentGroups);
        }

        public void updateCourse(String description, String title, int id)
        {
            Course course = db.Courses.Find(id);

            course.Description = description;
            course.Title = title;

            db.SaveChanges();
        }

        public void updateContentGroups(String[] header, int[] order, int id)
        {
            List<ContentGroup> contentGroups = db.ContentGroups.Where(cg => cg.CourseId == id).ToList();
            foreach (var cg in contentGroups)
            {
                db.ContentGroups.Remove(cg);
            }

            for (int i = 0; i < header.Length; i++)
            {
                ContentGroup cg = new ContentGroup();
                Course course = db.Courses.Find(id);

                cg.Header = header[i];
                cg.Order = order[i];
                cg.CourseId = id;
                cg.Course = course;

                db.ContentGroups.Add(cg);
            }
            db.SaveChanges();
        }

        public ActionResult AddContentElements(int id)
        {
            ContentGroup cg = db.ContentGroups.Find(id);

            List<ContentElement> sortedContentElements = cg.ContentElements.OrderBy(order => order.Order).ToList();
            //Adrian
            ViewBag.CGid = id;
            //adrian
            return View(sortedContentElements);
        }

        public ActionResult SaveContentElement()
        {
            return View();
        }

        //Adrian
        [HttpPost]
        public ActionResult SaveContentElements(int CGId, String[] description, int[] order, String[] type, String[] url)
        {
            if (!((description.Length == 0 || url.Length == 0) || (type.Length == 0 || order.Length == 0)))
            {
                updateContentElements(CGId, description, url, order, type);
            }
            
            CourseSearchController cs = new CourseSearchController();
            List<ContentGroup> sortedGroupedContentGroups = cs.processContentGroups(db.Courses.OrderByDescending(p => p.Id).FirstOrDefault().Id);

            return View("~/Views/AddCourse/AddContent.cshtml",sortedGroupedContentGroups);
        }

        public void updateContentElements(int listCeId, String[] description, String[] url, int[] order, String[] type)
        {
            List<ContentElement> contentElements = db.ContentElements.Where(ce => ce.ContentGroupId == listCeId).ToList();
            foreach (var ce in contentElements)
            {
                db.ContentElements.Remove(ce);
            }


            for (int i = 0; i < description.Length; i++)
            {
                ContentElement ce = new ContentElement();
                Models.Type t = new Models.Type();
                //if(db.Types.Contains(type[i]))
                t.Name = type[i];

                //Course course = db.Courses.Find(id);

                ce.Description = description[i];
                ce.URL = url[i];
                ce.Order = order[i];
                ce.Type = t;
                ce.ContentGroup = db.ContentGroups.Find(listCeId);
                ce.ContentGroupId = listCeId;

                db.Types.Add(t);
                db.ContentElements.Add(ce);

            }
            db.SaveChanges();
        }
    }
}