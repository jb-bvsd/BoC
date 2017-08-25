using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BehaviorsOfConcern.Web.Models {
    public class AuthOverrideViewModel {
        public int? personID { get; set; }
        public int? userID { get; set; }
        public int? calendarID { get; set; }

        public string BuildToken(string prefix) {
            if (string.IsNullOrWhiteSpace(prefix) || (personID == null) || (userID == null)) return null;
            else return string.Format("{0}:{1};{2};{3}", prefix, personID, userID, calendarID);
        }
    }
}