using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehaviorsOfConcern.Domain.Entities;

namespace BehaviorsOfConcern.Web.Models {
    public class IncidentListViewModel {
        public IEnumerable<School> SchoolList { get; set; }
    }
}