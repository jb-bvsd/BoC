using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using BehaviorsOfConcern.Domain.DomainServices.Abstract;
using BehaviorsOfConcern.Domain.Entities;
using BvsdSecurity.Service;

namespace BehaviorsOfConcern.Api.Controllers {
    public class AuthenticationController : ApiController {
        private IBoCAuthorizationService _bocAuthService;


        public AuthenticationController(IBoCAuthorizationService bocAuthService) {
            this._bocAuthService = bocAuthService;
        }


        [Route("api/auth")]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        public IHttpActionResult Authenticate(string externalToken) {
            string bocToken; BVSDAdmin user = null;
            try {
                //user = new BVSDAdmin { ID = 247006, Name = "EB A. Tester", School = new School { ID = -1 } };
                user = _bocAuthService.ExtractExternalUser(externalToken);
                bocToken = _bocAuthService.BuildToken(user);
            } catch (Exception ex) {
                //TODO: log exception
                bocToken = null;
            }

            if (bocToken == null)
                return StatusCode(HttpStatusCode.Unauthorized);
            else
                return Ok(new { authToken = bocToken, userName = user.Name, userSchoolID = user.School?.ID });
        }
    }
}