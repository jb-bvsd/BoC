using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorsOfConcern.Domain.Entities {
    public class Person {
        public int ID { get; set; }  //personID
        public string Name { get; set; }
        public string StaffNumber { get; set; }
    }

    public class BVSDAdmin : Person {
        public School School { get; set; }
        public int UserID { get; set; }
        public int CalendarID { get; set; }
    }

    public class Student : Person {
        public int IdentityID { get; set; }
        public string StudentNumber { get; set; }
        public DateTime DOB { get; set; }
        public string Grade { get; set; }
    }
}
