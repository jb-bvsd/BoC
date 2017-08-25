using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using BehaviorsOfConcern.Domain.DomainServices.Abstract;
using BehaviorsOfConcern.Domain.Entities;
using BvsdSecurity.Service;
using BehaviorsOfConcern.Api.Infrastructure;

namespace BehaviorsOfConcern.Api.Controllers {
    public class SchoolController : ApiController {
        private ISchoolRepository _schoolRepository;


        public SchoolController(ISchoolRepository schoolRepository) {
            this._schoolRepository = schoolRepository;
        }


        [Route("api/schools")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [IdentityBasicAuthenticationAttribute(Realm = "none")]
        [Authorize(Roles = BvsdRoles.DistrictAdmin + "," + BvsdRoles.SchoolAdmin + "," + BvsdRoles.BvsdEmployee)]
        [HttpGet]
        public IHttpActionResult ReadAll() {
            IEnumerable<School> schools = _schoolRepository.ReadSchools();
            if (schools == null) return NotFound(); else return Ok(schools.OrderBy(s => s.Name));
        }


        [Route("api/schools/{schoolID}/students")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [IdentityBasicAuthenticationAttribute(Realm = "none")]
        [Authorize(Roles = BvsdRoles.DistrictAdmin + "," + BvsdRoles.SchoolAdmin)]
        [HttpGet]
        public IHttpActionResult ReadStudentsBySchool(int schoolID, string searchTerm) {
            var students = from student in _schoolRepository.ReadStudentsBySchool(schoolID, searchTerm)
                           orderby student.Name, student.Grade, student.StudentNumber
                           select student;
            if ((students == null) || (students.Count() < 1)) return NotFound(); else return Ok(students);
        }

    }
}
