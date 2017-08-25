using System;

namespace BehaviorsOfConcern.Domain.Entities {
    public class Comment {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Text { get; set; }

        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Byte[] EntityVersion { get; set; }
    }
}