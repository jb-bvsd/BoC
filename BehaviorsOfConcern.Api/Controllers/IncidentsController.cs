using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using BehaviorsOfConcern.Api.Infrastructure;
using BehaviorsOfConcern.Api.Models;
using BehaviorsOfConcern.Domain.DomainServices;
using BehaviorsOfConcern.Domain.DomainServices.Abstract;
using BehaviorsOfConcern.Domain.Entities;
using BvsdSecurity.Service;

namespace BehaviorsOfConcern.Api.Controllers {
    public class IncidentsController : ApiController {
        private IIncidentRepository _incidentRepository;
        private ILookupCodeCache _lookupCodeCache;


        public IncidentsController(IIncidentRepository incidentRepository, ILookupCodeCache lookupCodeCache) {
            this._incidentRepository = incidentRepository;
            this._lookupCodeCache = lookupCodeCache;
        }


        //note: a more "pure" restful implementation would use a route of "api/incidents", with an HTTP verb of GET.
        //      But since the datatables.net plug-in will send a potentially *very long* request query string
        //      - & additionally, we wouldn't want to expose this to the user, and have them possibly bookmark it -  
        //      an HTTP POST is much preferred.
        //      And since we want the typical "api/incidents" + POST, to indicate "create a new Incident", we
        //      arrive at a route of "api/incidents/search" + POST, to retrieve our list of Incidents.
        [Route("api/incidents/search")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [IdentityBasicAuthenticationAttribute(Realm = "none")]
        [Authorize(Roles = BvsdRoles.DistrictAdmin + "," + BvsdRoles.SchoolAdmin)]
        [HttpPost]
        public IHttpActionResult Search(SearchRequest request) {
            Tuple<int, IEnumerable<Incident>> foundIncidents;
            if (request == null) return NotFound();

            try {
                IEnumerable<int> filterStatusCDs = request.FilterStatus?.Select(f => f.Value);
                IEnumerable<int> filterCategoryCDs = request.FilterCategory?.Select(f => f.Value);
                IEnumerable<int> filterOutcomeCDs = request.FilterOutcome?.Select(f => f.Value);
                Tuple<DateTime?, DateTime?> range = DetermineIncidentDateSearchRange(request.FilterRecency, _lookupCodeCache);
                string filterAdHoc = string.IsNullOrWhiteSpace(request.Search.Value) ? null : "%" + request.Search.Value.Trim() + "%";
                int? filterSchool = User.IsInRole(BvsdRoles.DistrictAdmin) ? request.FilterSchool :
                    int.Parse(ExtractClaimValue(ClaimTypes.UserData) ?? "0");

                foundIncidents = _incidentRepository.ReadIncidents(
                    request.Start, request.Length, range.Item1, range.Item2,
                    filterStatusCDs, filterCategoryCDs, filterOutcomeCDs,
                    filterSchool, filterAdHoc, BuildSortDefinition(request));
            } catch (Invalid​Operation​Exception ex) {
                //TODO: log exception
                return NotFound();
            } catch (SystemException ex) {
                //TODO: log exception
                return NotFound();
            }

            if (foundIncidents.Item2 == null) {
                return NotFound();
            } else {
                SearchResponse<Incident> response = new SearchResponse<Incident> {
                    Draw = request.Draw,
                    RecordsTotal = -1,
                    RecordsFiltered = foundIncidents.Item1,
                    Data = foundIncidents.Item2.ToList()
                };
                return Ok(response);
            }
        }

        private Tuple<DateTime?, DateTime?> DetermineIncidentDateSearchRange(int? recencyCode, ILookupCodeCache lookups) {
            DateTime? rangeLow = null; DateTime? rangeHigh = null;
            if (recencyCode != null) {
                try {
                    LookupCode luc = lookups.CodeDictionary[(int)recencyCode];
                    switch (luc.ProgrammaticEnum.ToUpper()) {
                        case "RECENCYINTERVAL_TODAY": rangeLow = DateTime.Today; rangeHigh = DateTime.Today.AddDays(1); break;
                        case "RECENCYINTERVAL_LAST48HOURS": rangeLow = DateTime.Now.AddHours(-48); rangeHigh = DateTime.Now.AddHours(1); break;
                        case "RECENCYINTERVAL_LAST7DAYS": rangeLow = DateTime.Today.AddDays(-7); rangeHigh = DateTime.Today.AddDays(1); break;
                        case "RECENCYINTERVAL_LAST30DAYS": rangeLow = DateTime.Today.AddDays(-30); rangeHigh = DateTime.Today.AddDays(1); break;
                        case "RECENCYINTERVAL_THISYEAR": rangeLow = new DateTime(DateTime.Today.Year, 1, 1); rangeHigh = new DateTime(DateTime.Today.Year + 1, 1, 1); break;
                    }
                } catch { }
            }
            return new Tuple<DateTime?, DateTime?>(rangeLow, rangeHigh);
        }

        private string BuildSortDefinition(SearchRequest request) {
            const string candidateSortItems = "|ID|IncidentDateSort|ConcernedParty|ReportingSchoolSort|CategorySort|SourceSort|StatusSort|SubmittedOn|";
            StringBuilder sortDefinition = new StringBuilder();
            bool userIDSort = false;

            if (request.Order != null) {
                foreach (var ori in request.Order) {
                    string sortColumnName = request.Columns[ori.Column].Name;

                    if (candidateSortItems.Contains("|" + sortColumnName + "|")) {
                        sortDefinition.Append(",");
                        sortDefinition.Append(sortColumnName);
                        if (ori.Dir.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                            sortDefinition.Append(" desc");
                        userIDSort = userIDSort || sortColumnName.Equals("ID", StringComparison.InvariantCultureIgnoreCase);
                    }
                }
            }
            if (!userIDSort) sortDefinition.Append(",ID");
            return sortDefinition.ToString(1, sortDefinition.Length - 1);
        }


        [Route("api/incidents")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [IdentityBasicAuthenticationAttribute(Realm = "none")]
        [Authorize(Roles = BvsdRoles.DistrictAdmin + "," + BvsdRoles.SchoolAdmin + "," + BvsdRoles.BvsdEmployee)]
        [HttpPost]
        public IHttpActionResult Create(Incident incident) {
            try {
                incident.StatusCD = _lookupCodeCache.CodeEnumDictionary["Status_Submitted"];

                if (User.IsInRole(BvsdRoles.DistrictAdmin)) {
                    if (incident.Submitter == null) incident.Submitter = new BVSDAdmin();
                    incident.Submitter.Name = ExtractClaimValue(ClaimTypes.Name);
                    incident.Submitter.ID = int.Parse(ExtractClaimValue(ClaimTypes.NameIdentifier) ?? "0");
                } else if (User.IsInRole(BvsdRoles.SchoolAdmin)) {
                    if (incident.Submitter == null) incident.Submitter = new BVSDAdmin();
                    if (incident.ReportingSchool == null) incident.ReportingSchool = new School();
                    incident.Submitter.Name = ExtractClaimValue(ClaimTypes.Name);
                    incident.Submitter.ID = int.Parse(ExtractClaimValue(ClaimTypes.NameIdentifier) ?? "0");
                    incident.ReportingSchool.ID = int.Parse(ExtractClaimValue(ClaimTypes.UserData) ?? "0");
                    incident.ReportingSchool.Name = "";
                } else if (User.IsInRole(BvsdRoles.BvsdEmployee)) {
                    //no prior identity assumed - submitter parameters must be supplied in the call to this api endpoint
                    incident.SourceCD = _lookupCodeCache.CodeEnumDictionary["Source_Website"];
                }

                incident.UpdatedBy = string.Format("{0}: {1}", incident.Submitter.ID, incident.Submitter.Name);
            } catch (SystemException ex) {
                //TODO: log exception
                return BadRequest("Problem creating incident: can't verify user credentials");
            }

            try {
                IList<string> validationFailures;
                if (incident.IsValid(out validationFailures)) {
                    incident.ID = _incidentRepository.CreateIncident(incident);
                    //return CreatedAtRoute()
                    return Ok(incident.ID);
                } else {
                    //string validationMessage = string.Format("{0}:\n{1}", "Problem creating incident",
                    //    string.Join("\n", validationFailures));
                    string validationMessage = string.Join("\n", validationFailures);
                    return BadRequest(validationMessage);
                }
            } catch (Invalid​Operation​Exception ex) {
                //TODO: log exception
                return BadRequest("Problem creating incident: data storage issue");
            }
        }


        [Route("api/incidents/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [IdentityBasicAuthenticationAttribute(Realm = "none")]
        [Authorize(Roles = BvsdRoles.DistrictAdmin + "," + BvsdRoles.SchoolAdmin)]
        [HttpGet]
        public IHttpActionResult Read(int id) {
            try {
                Incident incident = _incidentRepository.ReadIncident(id);
                if (incident == null) return NotFound(); else return Ok(incident);
            } catch (SystemException ex) {
                //TODO: log exception
                return NotFound();
            }
        }


        [Route("api/incidents/{id}")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [IdentityBasicAuthenticationAttribute(Realm = "none")]
        [Authorize(Roles = BvsdRoles.DistrictAdmin + "," + BvsdRoles.SchoolAdmin)]
        [HttpPut]
        public IHttpActionResult Update(int id, Incident incident) {
            //public IHttpActionResult UpdateIncidentAttribute(int id, System.Net.Http.Formatting.FormDataCollection fdc) {
            try {
                int updateCount = 0;
                if (incident != null) {
                    incident.ID = id;
                    incident.UpdatedBy = string.Format("{0}: {1}", ExtractClaimValue(ClaimTypes.NameIdentifier), ExtractClaimValue(ClaimTypes.Name));

                    //work around to get around NULL ambiguity (webApi model binder interprets empty field as a NULL value - we want the empty string, look for backspace char instead)
                    const string emptyMarker = "\b";
                    if (incident.ReportingParty?.Name == emptyMarker) incident.ReportingParty.Name = "";
                    if (incident.SpecificLocation == emptyMarker) incident.SpecificLocation = "";

                    updateCount = _incidentRepository.UpdateIncident(incident);
                }
                return Ok(updateCount);
            } catch (System.Data.SqlClient.SqlException ex) {
                return BadRequest("can't update incident msg here/n" + ex.Message);
            }
        }


        private string ExtractClaimValue(string claimType) {
            ClaimsIdentity userClaimsIdentity = (ClaimsIdentity)User.Identity;
            return userClaimsIdentity.FindFirst(claimType)?.Value;
        }



        [Route("api/incidents/{incidentID}/comments")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [IdentityBasicAuthenticationAttribute(Realm = "none")]
        [Authorize(Roles = BvsdRoles.DistrictAdmin + "," + BvsdRoles.SchoolAdmin)]
        [HttpPost]
        public IHttpActionResult AddComment(int incidentID, Comment comment) {
            if ((comment == null) || string.IsNullOrWhiteSpace(comment.Text)) return BadRequest("empty comment (msg here)");
            comment.ParentID = incidentID;
            comment.UpdatedBy = string.Format("{0}: {1}", ExtractClaimValue(ClaimTypes.NameIdentifier), ExtractClaimValue(ClaimTypes.Name));

            try {
                comment.ID = _incidentRepository.CreateComment(comment);
                //return CreatedAtRoute()
                return Ok(comment.ID);
            } catch (System.Data.SqlClient.SqlException ex) {
                return BadRequest("can't create comment (msg here)/n" + ex.Message);
            }
        }


        [Route("api/incidents/{incidentID}/comments")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [IdentityBasicAuthenticationAttribute(Realm = "none")]
        [Authorize(Roles = BvsdRoles.DistrictAdmin + "," + BvsdRoles.SchoolAdmin)]
        [HttpGet]
        public IHttpActionResult ReadComments(int incidentID) {
            IEnumerable<Comment> comments = _incidentRepository.ReadComments(incidentID);
            if (comments == null) return NotFound(); else return Ok(comments);
        }
    }
}
