using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorsOfConcern.Domain.DomainServices.Abstract;
using BehaviorsOfConcern.Domain.Entities;
using Dapper;

namespace BehaviorsOfConcern.Domain.DomainServices {
    public class SchoolRepository : RepositoryBase, ISchoolRepository {
        public SchoolRepository(string connectionString) {
            _connString = connectionString;
        }

        public School ReadSchool(int schoolID) {
            throw new NotImplementedException();
        }


        public IEnumerable<School> ReadSchools() {
            IEnumerable<School> schoolList = null;
            using (SqlConnection conn = new SqlConnection(_connString)) {
                conn.Open();
                schoolList = conn.Query<School>(@"
                    select schoolID as ID, Name
                      from school
                     where schoolID in (select schoolID from calendar where endYear = dbo.getSchoolEndYear(0))
                       and address is not null"
                );
            }
            return schoolList;
        }


        public IEnumerable<Student> ReadStudentsBySchool(int schoolID, string filter) {
            IEnumerable<Student> students = null;
            using (SqlConnection conn = new SqlConnection(_connString)) {
                conn.Open();
                students = conn.Query("p_BVSD_BoC_SearchStudentsBySchool",
                    new { seekSchoolID = schoolID, searchTerm = filter, resultCountCap = 100 },
                    commandType: CommandType.StoredProcedure)
                    .Select(s => new Student {
                        ID = s.personID,
                        IdentityID = s.identityID,
                        StudentNumber = s.studentNumber,
                        //DOB = s.birthdate,
                        Name = s.lastName + ", " + s.firstName + ' ' + s.middleName,
                        Grade = s.grade,
                    });
            }
            return students;
        }
    }
}
