using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorsOfConcern.Domain.RepoServices.Abstract;

namespace BehaviorsOfConcern.Domain.RepoServices {
    public class BoCAuthorizationService : RepositoryBase, IBoCAuthorizationService {
        public BoCAuthorizationService(string connectionString) {
            _connString = connectionString;
        }

        public int? ReadBoCAdminID(int personID) {
            if (personID == 999101) return -1;
            if (personID == 999202) return 4321;
            if (personID == 999203) return -1234;
            if (personID == 999303) return null;
            return null;
        }
    }
}
