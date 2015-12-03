using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniversityRegistration.Models
{
    public class ViewRostersViewModel
    {
        public List<StudentInfo> students { get; set; }
        public List<Class> classes { get; set; }
    }
}