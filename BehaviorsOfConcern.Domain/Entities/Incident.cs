using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorsOfConcern.Domain.Entities {
    public class Incident {
        //public Incident() {
        //    Submitter = new Person();
        //    CorrectedReportingSchool = new School();
        //}

        public int ID { get; set; }
        public string Description { get; set; }
        public DateTime IncidentDate { get; set; }
        public DateTime CorrectedIncidentDate { get; set; }
        public DateTime SubmittedOn { get; set; }
        public Person Submitter { get; set; }  //does this have to be an IC user? -> no
        
        public Person ReportingParty { get; set; }
        public Student ConcernedParty { get; set; }

        public School ReportingSchool { get; set; }
        public School CorrectedReportingSchool { get; set; }

        public string SpecificLocation { get; set; }
        //public IList<Person> InvolvedParties { get; set; }  // offender / victim / witness
        public int CategoryCD { get; set; }
        public int SourceCD { get; set; }
        public int StatusCD { get; set; }
        public int OutcomeCD { get; set; }
        public IList<Comment> Comments { get; set; }

        public string UpdatedBy { get; set; }
        public Byte[] EntityVersion { get; set; }


        public bool IsValid(){
            IList<string> dummy;
            return IsValid(out dummy);
        }

        public bool IsValid(out IList<string> failureMessages) {
            failureMessages = new List<string>();
            if (IncidentDate.Year < 1900) failureMessages.Add("Bad incident date");
            if (string.IsNullOrWhiteSpace(Description)) failureMessages.Add("Incident description is required");
            if (string.IsNullOrWhiteSpace(Submitter?.Name)) failureMessages.Add("Incident submitter's user name is required");
            if ((ReportingSchool?.ID == null) || (ReportingSchool.ID == 0)) failureMessages.Add("Reporting school must be specified");
            if (StatusCD==0) failureMessages.Add("Incident status must be specified");
            return (failureMessages.Count == 0);
        }
    }
}
