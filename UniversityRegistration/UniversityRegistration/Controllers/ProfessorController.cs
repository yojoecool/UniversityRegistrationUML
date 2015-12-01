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
            List<Student> students = new List<Student>();
            List<Class> classes = db.Classes.Where(m => m.ProfessorID == (int)Session["User"]).ToList();

            List<ClassStudent> cs = new List<ClassStudent>();
            foreach (Class c in classes)
            {
                cs.AddRange(db.ClassStudents.Where(m => m.ClassID == c.Id));
            }

            foreach (ClassStudent it in cs)
            {
                students.Add(db.Students.Find(it.StudentID));
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

            List<ClassStudent> Cstudents = new List<ClassStudent>();
            
            foreach (Class c in classes)
            {
                Cstudents.AddRange(db.ClassStudents.Where(m => m.ClassID == c.Id));
            }

            List<Student> students = new List<Student>();
            foreach (ClassStudent cs in Cstudents)
            {
                students.Add(db.Students.Find(cs.StudentID));
            }

            ViewRostersViewModel model = new ViewRostersViewModel();
            model.classes = classes;
            model.students = students;

            return View(model);
        }
    }
}