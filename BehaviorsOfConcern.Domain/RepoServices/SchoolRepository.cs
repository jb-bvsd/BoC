using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorsOfConcern.Domain.Entities;
using BehaviorsOfConcern.Domain.RepoServices.Abstract;

namespace BehaviorsOfConcern.Domain.RepoServices {
    public class SchoolRepository : RepositoryBase, ISchoolRepository {
        public SchoolRepository(string connectionString) {
            _connString = connectionString;
        }


        public School ReadSchool(int schoolID) {
            throw new NotImplementedException();
        }

        public IEnumerable<School> ReadSchools() {
            throw new NotImplementedException();
        }
    }
}
