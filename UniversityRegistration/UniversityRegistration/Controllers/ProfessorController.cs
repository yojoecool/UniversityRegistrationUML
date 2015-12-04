using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UniversityRegistration.Models;
using System.Web.Mvc;

namespace UniversityRegistration.Controllers
{
    public class ProfessorController : Controller
    {
        UniversityRegistrationContextContainer db = new UniversityRegistrationContextContainer();
        // GET: Professor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewStudents()
        {
            List<StudentInfo> students = new List<StudentInfo>();
            int num = (int)Session["User"];
            List<Class> classes = db.Classes.Where(m => m.ProfessorID == num).ToList();

            List<ClassStudent> cs = new List<ClassStudent>();
            foreach (Class c in classes)
            {
                cs.AddRange(db.ClassStudents.Where(m => m.ClassID == c.Id));
            }

            foreach (ClassStudent it in cs)
            {
                students.Add(db.StudentInfoes.Find(it.StudentID));
            }
            

            return View(students);
        }

        public ActionResult ViewStudentInfo(int studentId)
        {
            
            StudentInfo student = db.StudentInfoes.FirstOrDefault(m => m.Id == studentId);
            return View(student);
        }

        public ActionResult ViewClassRosters()
        {
            int id = (int)Session["User"];
            List<Class> classes = db.Classes.Where(m => m.ProfessorID == id).ToList();

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

            ViewBag.classSize = classSize;
            ViewRostersViewModel model = new ViewRostersViewModel();
            model.classes = classes;
            model.students = students;

            return View(model);
        }
    }
}