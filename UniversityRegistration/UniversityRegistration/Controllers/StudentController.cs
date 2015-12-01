using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversityRegistration.Models;
using System.Web.Mvc;

namespace UniversityRegistration.Controllers
{
    public class StudentController : Controller
    {
        UniversityRegistrationContextContainer db = new UniversityRegistrationContextContainer();
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewSchedule()
        {
            List<ClassStudent> classStudents = db.ClassStudents.Where(m => m.StudentID == (int)Session["User"]).ToList();

            List<Class> classes = new List<Class>();

            foreach (ClassStudent cs in classStudents)
            {
                classes.Add(db.Classes.Find(cs.ClassID));
            }

            return View(classes);
        }

        public ActionResult SubmitAddRequest()
        {
            Semester activeSemester = db.Semesters.FirstOrDefault(m => (bool)m.Active);
            List<Class> classes = db.Classes.Where(m => m.SemesterID == activeSemester.Id).ToList();
            SelectList sl = new SelectList(classes, "Id", "Name");
            ViewBag.sl = sl;

            return View();
        }

        [HttpPost]
        public ActionResult SubmitAddRequest(int classId)
        {
            AddRequest ar = new AddRequest();
            ar.StudentID = (int)Session["User"];
            ar.ClassID = classId;
            ar.Processed = false;

            db.AddRequests.Add(ar);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult SubmitDropRequest()
        {
            List<ClassStudent> classStudents = db.ClassStudents.Where(m => m.StudentID == (int)Session["User"]).ToList();

            List<Class> classes = new List<Class>();
            foreach (ClassStudent cs in classStudents)
            {
                classes.Add(db.Classes.Find(cs.ClassID));
            }

            return View(classes);
        }

        [HttpPost]
        public ActionResult SubmitDropRequest(int classId)
        {
            DropRequest dr = new DropRequest();
            dr.StudentID = (int)Session["User"];
            dr.ClassID = classId;
            dr.Processed = false;

            db.DropRequests.Add(dr);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}