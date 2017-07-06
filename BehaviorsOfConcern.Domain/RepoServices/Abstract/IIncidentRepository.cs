using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorsOfConcern.Domain.Entities;

namespace BehaviorsOfConcern.Domain.RepoServices.Abstract {
    public interface IIncidentRepository {
        int CreateIncident(Incident newIncident, string submitter);
        Incident ReadIncident(int incidentID);
    }
}
