﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UniversityRegistration.Models;

namespace UniversityRegistration.Controllers
{
    public class AdminController : Controller
    {
        //Note: 1 = Admin, 2 = Professor, 3 = Student, 4 = Advisor
        UniversityRegistrationContextContainer db = new UniversityRegistrationContextContainer();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult AddUser()
        {
            List <SelectListItem> advisors  = new List<SelectListItem>();
            advisors = (from m in db.Users
                                where m.userType == 4
                                select new SelectListItem {
                                    Text = m.Name, 
                                    Value = m.Name
                                }).ToList();
            advisors.Insert(0, new SelectListItem { Text = null, Value = null });
            ViewBag.Advisors = advisors;

            List<SelectListItem> majors = (from m in db.Majors
                                           select new SelectListItem
                                           {
                                               Text = m.majorName,
                                               Value = m.majorName
                                           }).ToList();
            majors.Insert(0, new SelectListItem { Text = null, Value = null });
            ViewBag.Majors = majors;
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(User input, String Major, String Advisor)
        {
            User check = (from m in db.Users
                          where m.Email == input.Email
                          select m).FirstOrDefault();

            if (check == null && input.Email != null) {
                input.registrationLink = "";
                input.Status = false;

                if (input.Gender != null && input.Gender.ToUpper().StartsWith("M"))
                    input.Gender = "M";
                else input.Gender = "F";

                db.Users.Add(input);

                db.SaveChanges();
                
                if (input.userType == 3) { 
                    Student newStudent = new Student();
                    newStudent.UserID = (from m in db.Users
                                         where m.Email == input.Email
                                         select m.Id).First();

                    newStudent.creditHours = 0;

                    Major = Major.ToUpper();

                    int? major = (from m in db.Majors
                                   where m.majorName == Major
                                   select m.majorId).FirstOrDefault();

                    if (major != null) 
                        newStudent.MajorID = major.Value;

                    if (Advisor != null)
                    {
                        int advisorId = (from m in db.Users
                                         where m.Name == Advisor
                                         select m.Id).FirstOrDefault();

                        newStudent.AdvisorID = advisorId;
                    }

                    db.Students.Add(newStudent);
                }

                db.SaveChanges();
            }
            else
            {
                TempData["error"] = "User exists or email is invalid. Please try again.";
                return RedirectToAction("AddUser");
            }

            return RedirectToAction("Index");
        }

        public ActionResult ChooseUserToEdit()
        {
            List<Models.User> UserList = new List<Models.User>();
            UserList = (from m in db.Users
                        select m).ToList();

            return View(UserList);
        }

        public ActionResult EditUserInfo(int id = -1, int studentId = -1)
        {
            ViewBag.Major = "";
            ViewBag.StudentAdvisor = "";
            ViewBag.Majors = new List<SelectListItem>();
            ViewBag.Advisors = new List<SelectListItem>();

            Models.User user = new Models.User();
            if (id != -1)
            {
                user = (from m in db.Users
                        where m.Id == id
                        select m).FirstOrDefault();
            }
            else if (studentId != -1)
            {
                int? userId = (from m in db.Students
                              where m.Id == studentId
                              select m.UserID).FirstOrDefault();

                if (userId != null)
                {
                    user = (from m in db.Users
                            where m.Id == userId
                            select m).FirstOrDefault();
                }
            }

            if (user.userType != null && user.userType == 3)
            {
                string majorHold = null;
                if (id > 0)
                {
                    int? majorID = (from m in db.Students
                                   where m.UserID == id
                                   select m.MajorID).FirstOrDefault();

                    majorHold = (from m in db.Majors
                                 where m.majorId == majorID
                                 select m.majorName).FirstOrDefault();
                    
                    int? advisorID = (from m in db.Students
                                      where id == m.UserID
                                      select m.AdvisorID).FirstOrDefault();

                    ViewBag.StudentAdvisor = (from m in db.Users
                                              where m.Id == advisorID &&
                                                m.userType == 4
                                              select m.Name).FirstOrDefault();
                }
                else
                {
                    majorHold = (from m in db.StudentInfoes
                                 where studentId == m.Id
                                 select m.majorName).FirstOrDefault();
                    
                    int? advisorID = (from m in db.StudentInfoes
                                      where studentId == m.Id
                                      select m.AdvisorID).FirstOrDefault();

                    ViewBag.StudentAdvisor = (from m in db.Users
                                              where m.Id == advisorID &&
                                                m.userType == 4
                                              select m.Name).FirstOrDefault();
                }

                ViewBag.Major = majorHold;

                List<SelectListItem> advisors = new List<SelectListItem>();
                advisors = (from m in db.Users
                            where m.userType == 4
                            select new SelectListItem
                            {
                                Text = m.Name,
                                Value = m.Name
                            }).ToList();
                advisors.Insert(0, new SelectListItem { Text = null, Value = null });
                ViewBag.Advisors = advisors;

                List<SelectListItem> majors = (from m in db.Majors
                                               select new SelectListItem
                                               {
                                                   Text = m.majorName,
                                                   Value = m.majorName
                                               }).ToList();
                majors.Insert(0, new SelectListItem { Text = null, Value = null });
                ViewBag.Majors = majors;
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult EditUserInfo(User input, String Major, String Advisor)
        {
            if (input.Email == null)
            {
                TempData["error"] = "Invalid email. Please try again";
                return RedirectToAction("Index");
            }
            Models.User user = new Models.User();
            user = (from m in db.Users
                    where m.Id == input.Id
                    select m).First();
            user.Name = input.Name.Trim();
            user.Password = input.Password.Trim();
            user.Email = input.Email.Trim();
            user.phoneNumber = input.phoneNumber.Trim();
            user.Address = input.Address.Trim();
            user.Status = input.Status;

            if (input.Gender.ToUpper().StartsWith("M"))
                user.Gender = "M";
            else user.Gender = "F";

            if (user.userType == 3)
            {
                Student editStudent = (from m in db.Students
                                     where m.UserID == user.Id
                                     select m).First();

                Major = Major.ToUpper();

                int? major = (from m in db.Majors
                              where m.majorName == Major
                              select m.majorId).FirstOrDefault();

                if (major != null)
                    editStudent.MajorID = major.Value;

                if (Advisor != null)
                {
                    int advisorId = (from m in db.Users
                                     where m.Name == Advisor
                                     select m.Id).FirstOrDefault();

                    editStudent.AdvisorID = advisorId;
                }
            }

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

            else
            {
                ViewBag.error = "Semester already exists.";
                return View();
            }

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

        public ActionResult ViewStudents(int AdvisorId = 0, String Major = "")
        {
            List<StudentInfo> list = new List<StudentInfo>();

            if (AdvisorId != 0)
            {
                list = (from m in db.StudentInfoes
                        where m.AdvisorID == AdvisorId
                        select m).ToList();
            }

            else if (Major != "")
            {
                int id = (from m in db.Majors
                          where m.majorName == Major
                          select m.majorId).First();

                list = (from m in db.StudentInfoes
                        where m.majorId == id
                        select m).ToList();
            }

            else
                list = (from m in db.StudentInfoes
                        select m).ToList();

            return View(list);
        }

        public ActionResult ChooseAdvisor()
        {
            List<User> list = (from m in db.Users
                               where m.userType == 4
                               select m).ToList();

            return View(list);
        }

        public ActionResult ChooseMajor()
        {
            List<Major> list = (from m in db.Majors
                                select m).ToList();

            return View(list);
        }

        public ActionResult ViewEnrollmentInfo()
        {
            int males = (from m in db.Users
                         where m.Gender == "M"
                         select m).Count();
            ViewBag.males = males;

            int females = (from m in db.Users
                           where m.Gender == "F"
                           select m).Count();
            ViewBag.females = females;

            List<String> majors = (from m in db.Majors
                                  select m.majorName).ToList();
            List<Major> majorList = new List<Major>();

            foreach (String n in majors)
            {
                int majorCount = (from m in db.StudentInfoes
                                    where m.majorName == n
                                    select m).Count();

                majorList.Add(new Major { majorId = majorCount, majorName = n });
            }

            return View(majorList);
        }

        public ActionResult ChooseClass()
        {
            List<Class> list = (from m in db.Classes
                                select m).ToList();

            return View(list);
        }

        public ActionResult AddBuilding()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddBuilding(Building input)
        {
            Building check = (from m in db.Buildings
                              where m.Name == input.Name
                              select m).FirstOrDefault();

            if (check == null)
            {
                db.Buildings.Add(input);
                db.SaveChanges();
            }

            else
            {
                ViewBag.error = "Building already exists.";
                return View();
            }
            return RedirectToAction("Index");
        }

        public ActionResult ChooseBuilding()
        {
            List<Building> list = new List<Building>();

            list = (from m in db.Buildings
                    select m).ToList();

            return View(list);
        }

        public ActionResult AddRoom(int id)
        {
            Building building = (from m in db.Buildings
                                 where m.Id == id
                                 select m).First();

            Room newRoom = new Room();
            newRoom.BuildingID = building.Id;

            return View(newRoom);
        }

        [HttpPost]
        public ActionResult AddRoom(Room input)
        {
            Room check = (from m in db.Rooms
                          where m.BuildingID == input.BuildingID &&
                            m.Name == input.Name
                          select m).FirstOrDefault();

            if (check == null)
            {
                db.Rooms.Add(input);
                db.SaveChanges();
            }

            else
            {
                ViewBag.error = "Room already exists in this building.";
                return View();
            }

            return RedirectToAction("Index");
        }

        public ActionResult ChooseProfessor(int id)
        {
            List<User> list = (from m in db.Users
                               where m.userType == 2
                               select m).ToList();

            ViewBag.Class = id;

            return View(list);
        }

        public ActionResult AssignProfessor(int userId, int classId)
        {
            Class editClass = (from m in db.Classes
                               where m.Id == classId
                               select m).First();

            editClass.ProfessorID = userId;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ChooseBuildingForClass(int classId)
        {
            List<Building> list = (from m in db.Buildings
                                   select m).ToList();

            ViewBag.Class = classId;

            return View(list);
        }

        public ActionResult ChooseRoomForClass(int buildingId, int classId)
        {
            List<Room> list = (from m in db.Rooms
                               where m.BuildingID == buildingId
                               select m).ToList();

            ViewBag.Class = classId;

            return View(list);
        }

        public ActionResult SetRoomForClass(int roomId, int classId)
        {
            Class edit = (from m in db.Classes
                          where m.Id == classId
                          select m).First();

            edit.RoomID = roomId;

            return RedirectToAction("Index");
        }

        public ActionResult AddClass()
        {
            List<SelectListItem> majors = (from m in db.Majors
                                           select new SelectListItem
                                           {
                                               Text = m.majorName,
                                               Value = m.majorId.ToString()
                                           }).ToList();

            List<SelectListItem> Semesters = (from m in db.Semesters
                                              select new SelectListItem
                                              {
                                                  Text = m.Name + " " + m.Year.ToString(),
                                                  Value = m.Id.ToString()
                                              }).ToList();

            ViewBag.majors = majors;
            ViewBag.semesters = Semesters;

            return View();
        }

        [HttpPost]
        public ActionResult AddClass(Class input)
        {
            try {
                db.Classes.Add(input);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return View();
            }

            return RedirectToAction("Index");
        }

        public ActionResult ChooseClassToRemove()
        {
            List<Class> list = (from m in db.Classes
                                select m).ToList();

            return View(list);
        }

        public ActionResult RemoveClass(int id)
        {
            Class remove = (from m in db.Classes
                            where m.Id == id
                            select m).First();

            db.Classes.Remove(remove);

            List<ClassStudent> entries = (from m in db.ClassStudents
                                          where m.ClassID == id
                                          select m).ToList();

            foreach (ClassStudent n in entries)
            {
                db.ClassStudents.Remove(n);
            }

            db.SaveChanges();

            return RedirectToAction("ChooseClassToRemove");
        }

        public ActionResult ChooseStudentForClass(int id)
        {
            List<StudentInfo> list = (from m in db.StudentInfoes
                                  select m).ToList();

            ViewBag.Class = id;

            return View(list);
        }

        public ActionResult AddStudentToClass(int studentId, int classId)
        {
            ClassStudent check = (from m in db.ClassStudents
                                  where m.ClassID == classId &&
                                    m.StudentID == studentId
                                  select m).FirstOrDefault();
            if (check == null)
            {
                ClassStudent add = new ClassStudent();
                add.ClassID = classId;
                add.StudentID = studentId;

                db.ClassStudents.Add(add);

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult ViewSchedule()
        {
            List<StudentInfo> students = db.StudentInfoes.ToList();
            SelectList sl = new SelectList(students, "Id", "Name");

            return View(sl);
        }

        [HttpPost]
        public ActionResult ViewSchedule(int id = 0)
        {
            return RedirectToAction("ViewSchedule", "Student", new { id = id });
        }
    }
}