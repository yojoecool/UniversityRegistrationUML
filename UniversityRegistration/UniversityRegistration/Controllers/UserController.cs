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

                // Admins
                if ((int)Session["Type"] == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }

                // Professors
                else if ((int)Session["Type"] == 2)
                {
                    return RedirectToAction("Index", "Professor");
                }

                // Students
                else if ((int)Session["Type"] == 3)
                {
                    return RedirectToAction("Index", "Student");
                }

                // Advisors
                else if ((int)Session["Type"] == 4)
                {
                    return RedirectToAction("Index", "Advisor");
                }
                
                else
                {
                    ViewBag.errorMessage = "Something went wrong; please contact your adminstrator.";
                    return View();
                }
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
                user.Name = user.Name.Trim();
                user.Address = user.Address.Trim();
                user.Email = user.Email.Trim();
                user.Gender = user.Gender.Trim();
                user.Password = user.Password.Trim();
                user.phoneNumber = user.phoneNumber.Trim();
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                
                // Come back and send them to the appropriate menu
                // Admins
                if ((int)Session["Type"] == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }

                // Professors
                else if ((int)Session["Type"] == 2)
                {
                    return RedirectToAction("Index", "Professor");
                }

                // Students
                else if ((int)Session["Type"] == 3)
                {
                    return RedirectToAction("Index", "Student");
                }

                // Advisors
                else if ((int)Session["Type"] == 4)
                {
                    return RedirectToAction("Index", "Advisor");
                }

                else
                {
                    ViewBag.errorMessage = "Something went wrong; please contact your adminstrator.";
                    return View();
                }
            }

            else
            {
                return View(user);
            }
        }

        public ActionResult RegisterForSystem(int userId)
        {
            User thisUser = db.Users.Find(userId);
            return View(thisUser);
        }

        [HttpPost]
        public ActionResult RegisterForSystem(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                Session["Type"] = user.userType;
                Session["User"] = user.Id;

                // Come back and send them to the appropriate menu
                // Admins
                if ((int)Session["Type"] == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }

                // Professors
                else if ((int)Session["Type"] == 2)
                {
                    return RedirectToAction("Index", "Professor");
                }

                // Students
                else if ((int)Session["Type"] == 3)
                {
                    return RedirectToAction("Index", "Student");
                }

                // Advisors
                else if ((int)Session["Type"] == 4)
                {
                    return RedirectToAction("Index", "Advisor");
                }

                else
                {
                    ViewBag.errorMessage = "Something went wrong; please contact your adminstrator.";
                    return View();
                }
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