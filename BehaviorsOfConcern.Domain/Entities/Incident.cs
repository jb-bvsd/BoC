using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorsOfConcern.Domain.Entities {
    public class Incident {
        public int ID { get; set; }
        public DateTime OccurredOn { get; set; }
        public DateTime SubmissionDate { get; set; }
        public Person Complainant { get; set; }  //free-form name, correct?  If so, do we want to capture contact info (full name, phone, e-mail)?
        public Person Preparer { get; set; }  //does this have to be an IC user?
        public string Description { get; set; }
        public School Location { get; set; }  //can "off-site" or "off premises" be an option?
        public string SpecificLocation { get; set; }
        public IList<Person> InvolvedParties { get; set; }  // offender / victim / witness
        public int CategoryCD { get; set; }  //required, to accept?  required, to reject?
        public int SourceCD { get; set; }  //required, to accept?  required, to reject?
        public int StatusCD { get; set; }  //required, to accept?  required, to reject?
        public IList<Comment> Comments { get; set; }
    }
}
