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
            Models.User user = new Models.User();
            user = (from m in db.Users
                    where m.Name != null
                    select m).FirstOrDefault();

            if (user != null)
            {
                ViewBag.Name = user.Name;
            }
            return View();
        }
    }
}