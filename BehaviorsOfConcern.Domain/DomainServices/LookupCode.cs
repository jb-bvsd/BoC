using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorsOfConcern.Domain.DomainServices.Abstract;
using Dapper;

namespace BehaviorsOfConcern.Domain.DomainServices {
    public enum LookupCategories { MetaCategory = 0, IncidentCategory = 101, Source, Status, Outcome, InvolvedPartyType, RecencyInterval };

    public class LookupCode {
        public int Value { get; set; }
        public LookupCategories Category { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public bool DefaultYN { get; set; }
        public bool ActiveYN { get; set; }
        public int SortOrder { get; set; }
        public string ProgrammaticEnum { get; set; }
    }
}
