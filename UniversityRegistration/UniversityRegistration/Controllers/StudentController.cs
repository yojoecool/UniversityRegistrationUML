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

        public ActionResult ViewSchedule(int id = 0)
        {
            // If there's a paramater it means another user is looking up this person's schedule
            if (id == 0)
            {
                id = (int)Session["User"];
            }

            int studentID = (from m in db.Students
                             where m.UserID == id
                             select m.Id).First();

            List<ClassStudent> classStudents = db.ClassStudents.Where(m => m.StudentID == studentID).ToList();

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
            int num = (int)Session["User"];
            Student student = db.Students.FirstOrDefault(m => m.UserID == num);

            ar.StudentID = student.Id;
            ar.ClassID = classId;
            ar.Processed = false;

            db.AddRequests.Add(ar);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult SubmitDropRequest()
        {
            int num = (int)Session["User"];
            Student student = db.Students.FirstOrDefault(m => m.UserID == num);
            List<ClassStudent> classStudents = db.ClassStudents.Where(m => m.StudentID == student.Id).ToList();

            List<Class> classes = new List<Class>();
            foreach (ClassStudent cs in classStudents)
            {
                classes.Add(db.Classes.Find(cs.ClassID));
            }

            SelectList sl = new SelectList(classes, "Id", "Name");
            ViewBag.sl = sl;

            return View();
        }

        [HttpPost]
        public ActionResult SubmitDropRequest(int classId)
        {
            DropRequest dr = new DropRequest();
            int num = (int)Session["User"];
            Student student = db.Students.FirstOrDefault(m => m.UserID == num);

            dr.StudentID = student.Id;
            dr.ClassID = classId;
            dr.Processed = false;

            db.DropRequests.Add(dr);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}