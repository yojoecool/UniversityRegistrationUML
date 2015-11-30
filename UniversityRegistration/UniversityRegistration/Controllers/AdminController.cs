using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversityRegistration.Models;

namespace UniversityRegistration.Controllers
{
    public class AdminController : Controller
    {
        UniversityRegistrationContextContainer db = new UniversityRegistrationContextContainer();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(User input)
        {
            User check = (from m in db.Users
                          where m.Email == input.Email
                          select m).FirstOrDefault();

            if (check == null) {
                input.registrationLink = "";
                db.Users.Add(input);
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ChooseUserToEdit()
        {
            List<Models.User> UserList = new List<Models.User>();
            UserList = (from m in db.Users
                        select m).ToList();

            return View(UserList);
        }

        public ActionResult EditUserInfo(int id)
        {
            Models.User user = new Models.User();
            user = (from m in db.Users
                    where m.Id == id
                    select m).FirstOrDefault();

            return View(user);
        }

        [HttpPost]
        public ActionResult EditUserInfo(User input)
        {
            Models.User user = new Models.User();
            user = (from m in db.Users
                    where m.Id == input.Id
                    select m).First();
            user.Name = input.Name;
            user.Password = input.Password;
            user.Email = input.Email;
            user.Gender = input.Gender;
            user.phoneNumber = input.phoneNumber;
            user.Address = input.Address;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult CreateSemester()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateSemester(Semester input)
        {
            if (input.Active == null)
                input.Active = false;

            Semester check = (from m in db.Semesters
                              where m.Name == input.Name &&
                                m.Year == input.Year
                              select m).FirstOrDefault();
            if (check == null)
                db.Semesters.Add(input);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ChooseSemester()
        {
            List<Semester> list = new List<Semester>();

            list = (from m in db.Semesters
                    select m).ToList();

            return View(list);
        }

        public ActionResult Activate_DeactivateSemester(int id)
        {
            Semester chosen = (from m in db.Semesters
                               where m.Id == id
                               select m).FirstOrDefault();

            return View(chosen);
        }

        [HttpPost]
        public ActionResult Activate_DeactivateSemester(Semester input)
        {
            Semester change = (from m in db.Semesters
                               where m.Id == input.Id
                               select m).First();

            change.Active = input.Active;

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}