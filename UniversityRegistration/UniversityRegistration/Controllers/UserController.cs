using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversityRegistration.Models;

namespace UniversityRegistration.Controllers
{
    public class UserController : Controller
    {
        UniversityRegistrationContextContainer db = new UniversityRegistrationContextContainer();
        // GET: User
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(string username, string password)
        {
            List<User> users = db.Users.Where(m => m.Email.Equals(username) && m.Password.Equals(password)).ToList();

            // If there's a match
            if (users.Count == 1)
            {
                Session["Type"] = users.First().userType;
                Session["User"] = users.First().Id;
                return RedirectToAction("something");
            }

            else
            {
                ViewBag.errorMessage = "We couldn't find that username/password combination. Please try again.";
                return View();
            }
        }

        public ActionResult LogOut()
        {
            Session["Type"] = null;
            return RedirectToAction("LogIn");
        }

        public ActionResult EditInfo()
        {
            User thisUser = db.Users.Find(Session["User"]);
            return View(thisUser);
        }

        [HttpPost]
        public ActionResult EditInfo(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                // Come back and send them to the appropriate menu
                return RedirectToAction("");
            }

            else
            {
                return View(user);
            }
        }

        public ActionResult RegisterForSystem()
        {
            User thisUser = db.Users.Find(Session["User"]);
            return View(thisUser);
        }

        [HttpPost]
        public ActionResult RegisterForSystem(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                // Come back and send them to the appropriate menu
                return RedirectToAction("");
            }

            else
            {
                return View(user);
            }
        }

        public ActionResult ViewClasses()
        {
            int semester = getCurrentSemester();

            List<Class> classes = db.Classes.Where(m => m.SemesterID == semester).ToList();
            return View(classes);
        }

        public int getCurrentSemester()
        {
            Semester semester = db.Semesters.FirstOrDefault(m => (bool)m.Active);
            return semester.Id;
        }

    }
}