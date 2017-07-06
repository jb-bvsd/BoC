using BehaviorsOfConcern.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BehaviorsOfConcern.Api.Infrastructure;
using System.Web.Http.Cors;
using BehaviorsOfConcern.Api.Models;
using ICCipher.Service;

namespace BehaviorsOfConcern.Api.Controllers {
    //[IdentityBasicAuthenticationAttribute(Realm = "non-null-JB")]
    //[Authorize(Roles = "Admin")]
    //[Authorize(Users = "testuser")]
    public class IncidentsController : ApiController {
        private ICipher _icCipherService;

        public IncidentsController(ICipher icCipherService) {
            this._icCipherService = icCipherService;
        }

        Incident[] incidents = new Incident[] {
            new Incident { ID = 1, Description = "Tomato Soup", SpecificLocation = "Groceries", StatusCD = 1 },
            new Incident { ID = 2, Description = "Yo-yo", SpecificLocation = "Toys", StatusCD = 2 },
            new Incident { ID = 3, Description = "Hammer", SpecificLocation = "Hardware", StatusCD = 3 }
        };

        //note: a more "pure" restful implementation would use a route of "api/incidents", with an HTTP verb of GET.
        //      But since the datatables.net plug-in will send a potentially *very long* request query string
        //      - and separately, we wouldn't want to expose this to the user, and have them possibly bookmark it -  
        //      an HTTP POST is much preferred.
        //      And since we want the typical "api/incidents" + POST, to indicate "create a new Incident", we
        //      arrive at a route of "api/incidents/search" + POST, to retrieve our list of Incidents.
        [Route("api/incidents/search")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [IdentityBasicAuthenticationAttribute(Realm = "none")]
        [Authorize(Roles = "DistrictAdmin, SchoolAdmin")]
        [HttpPost]
        public IHttpActionResult SearchIncidents(SearchRequest request) {
            var ci = (System.Security.Claims.ClaimsIdentity)User.Identity;
            string schoolIDasString = ci.FindFirst(System.Security.Claims.ClaimTypes.UserData).Value;
            return Ok(incidents);
        }



        [Route("api/incidents")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        public IHttpActionResult CreateIncident(Incident incident, string SpecificOther = "none") {
            //if DistrictAdmin then incident.ReportingSchool.ID must be supplied
            //if SchoolAdmin then ignore any incident.ReportingSchool.ID and use ClaimsIdentity..UserData (schoolID)
            //if BVSDEmployee then ??? (validate API key in the request header?) (incident.ReportingSchool.ID must be supplied)


            //return CreatedAtRoute()
            return Ok();
        }



        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult GetIncident(int id) {
            var incident = incidents.FirstOrDefault((i) => i.ID == id);
            if (incident == null) {
                return NotFound();
            }
            return Ok(incident);
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public IHttpActionResult PostIncident(int id, int draw, int start, int length) {
            string sample = OrthogonalSample(@"C:\JBWork\BVSD\BehaviorsOfConcern-GIT\BehaviorsOfConcern.Api\_sampleData\orthogonal_working.txt")
                .Replace("[draw_value]", draw.ToString());

            var jss = new System.Web.Script.Serialization.JavaScriptSerializer();
            var obj = jss.Deserialize<object>(sample);

            var payload = (Dictionary<string, object>)obj;
            payload["data"] = ((object[])payload["data"])
                .Skip(start)
                .Take(length)
                .ToArray();

            return Ok(incidents);
        }

        private string OrthogonalSample(string sampleFilePath) {
            return System.IO.File
                .ReadAllText(sampleFilePath)
                .Replace("\r", "")
                .Replace("\n", "");
        }


    }
}
