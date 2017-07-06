using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BehaviorsOfConcern.Domain.RepoServices.Abstract {
    public interface IBoCAuthorizationService {
        int? ReadBoCAdminID(int personID);
    }
}
