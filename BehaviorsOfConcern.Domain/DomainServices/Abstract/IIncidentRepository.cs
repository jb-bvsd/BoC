using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorsOfConcern.Domain.Entities;

namespace BehaviorsOfConcern.Domain.DomainServices.Abstract {
    public interface IIncidentRepository {
        int CreateIncident(Incident newIncident);
        Incident ReadIncident(int incidentID);
        Tuple<int, IEnumerable<Incident>> ReadIncidents(int pageSkip, int pageTake,
            DateTime? filterIncidentDateLow, DateTime? filterIncidentDateHigh,
            IEnumerable<int> filterStatusCDs, IEnumerable<int> filterCategoryCDs, IEnumerable<int> filterOutcomeCDs,
            int? filterReportingSchoolID, string filterAdHoc, string sortDefinition);
        int UpdateIncident(Incident incident);
        int CreateComment(Comment newComment);
        IEnumerable<Comment> ReadComments(int incidentID);
    }
}
