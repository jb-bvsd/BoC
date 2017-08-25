using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehaviorsOfConcern.Domain.DomainServices;
using BehaviorsOfConcern.Domain.Entities;

namespace BehaviorsOfConcern.Web.Models {
    public class IncidentListViewModel {
        public IEnumerable<School> SchoolList { get; set; }
        public IEnumerable<LookupCode> SourceLookups { get; set; }
        public IEnumerable<LookupCode> IncidentCategoryLookups { get; set; }
        public IEnumerable<LookupCode> StatusLookups { get; set; }
        public IEnumerable<LookupCode> OutcomeLookups { get; set; }
        public IEnumerable<LookupCode> RecencyIntervalLookups { get; set; }
    }
}
