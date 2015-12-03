using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversityRegistration.Models;
using System.Web.Mvc;

namespace UniversityRegistration.Controllers
{

    public class AdvisorController : Controller
    {
        UniversityRegistrationContextContainer db = new UniversityRegistrationContextContainer();
        // GET: Advisor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApproveDropRequest()
        {
            List<DropRequest> drqs = db.DropRequests.Where(m => (bool)!m.Processed).ToList();
            List<Student> students = db.Students.Where(m => m.AdvisorID == (int)Session["User"]).ToList();

            List<DropRequest> dropRequests = new List<DropRequest>();
            foreach (Student s in students)
            {
                dropRequests.AddRange(drqs.Where(m => m.StudentID == s.Id));
            }

            return View(dropRequests);
        }

        [HttpPost]
        public ActionResult ApproveDropRequest(int id)
        {
            DropRequest dr = db.DropRequests.Find(id);
            dr.Processed = true;

            ClassStudent cs = db.ClassStudents.FirstOrDefault(m => m.ClassID == dr.ClassID && m.StudentID == dr.StudentID);
            db.ClassStudents.Remove(cs);
            db.Entry(dr).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ApproveDropRequest");
        }

        public ActionResult ApproveAddRequest()
        {
            List<AddRequest> arqs = db.AddRequests.Where(m => (bool)!m.Processed).ToList();
            List<Student> students = db.Students.Where(m => m.AdvisorID == (int)Session["User"]).ToList();

            List<AddRequest> AddRequests = new List<AddRequest>();
            foreach (Student s in students)
            {
                AddRequests.AddRange(arqs.Where(m => m.StudentID == s.Id));
            }

            return View(AddRequests);
        }

        [HttpPost]
        public ActionResult ApproveAddRequest(int id)
        {
            AddRequest ar = db.AddRequests.Find(id);
            ar.Processed = true;

            ClassStudent cs = new ClassStudent();
            cs.ClassID = ar.ClassID;
            cs.StudentID = ar.StudentID;
            db.ClassStudents.Add(cs);

            db.Entry(ar).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ApproveAddRequest");
        }

        public ActionResult ViewClassRosters()
        {
            int id = db.Semesters.FirstOrDefault(m => (bool)m.Active).Id;
            List<Class> classes = db.Classes.Where(m => m.SemesterID == id).ToList();
            List<int> classSize = new List<int>();

            List<ClassStudent> Cstudents = new List<ClassStudent>();

            foreach (Class c in classes)
            {
                IEnumerable<ClassStudent> these = db.ClassStudents.Where(m => m.ClassID == c.Id);
                classSize.Add(these.Count());
                Cstudents.AddRange(these);
            }

            List<StudentInfo> students = new List<StudentInfo>();
            foreach (ClassStudent cs in Cstudents)
            {
                students.Add(db.StudentInfoes.Find(cs.StudentID));
            }

            ViewRostersViewModel model = new ViewRostersViewModel();
            model.classes = classes;
            model.students = students;
            ViewBag.classSize = classSize;

            return View(model);
        }

        public ActionResult ViewStudents()
        {
            List<StudentInfo> students = db.StudentInfoes.Where(m => m.AdvisorID == (int)Session["User"]).ToList();
            return View(students);
        }

        public ActionResult ViewStudentInfo(int studentId)
        {
            StudentInfo student = db.StudentInfoes.FirstOrDefault(m => m.Id == studentId);
            return View(student);
        }

        public ActionResult AddStudentToClass()
        {
            List<StudentInfo> students = db.StudentInfoes.Where(m => m.AdvisorID == (int)Session["User"]).ToList();
            int id = db.Semesters.FirstOrDefault(m => (bool)m.Active).Id;
            List<Class> classes = db.Classes.Where(m => m.SemesterID == id).ToList();

            ViewBag.classesSL = new SelectList(classes, "Id", "Name");
            ViewBag.studentsSL = new SelectList(students, "Id", "Name");

            ViewRostersViewModel model = new ViewRostersViewModel();
            model.classes = classes;
            model.students = students;

            return View(model);
        }

        [HttpPost]
        public ActionResult AddStudentToClass(int studentId, int classId)
        {
            ClassStudent cs = new ClassStudent();
            cs.StudentID = studentId;
            cs.ClassID = classId;

            db.ClassStudents.Add(cs);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}