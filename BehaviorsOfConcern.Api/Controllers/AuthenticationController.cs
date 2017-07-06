using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using BehaviorsOfConcern.Domain.RepoServices.Abstract;
using ICCipher.Service;

namespace BehaviorsOfConcern.Api.Controllers {
    public class AuthenticationController : ApiController {
        private ICipher _icCipherService;
        private IBoCAuthorizationService _bocAuthService;


        public AuthenticationController(ICipher icCipherService, IBoCAuthorizationService bocAuthService) {
            this._icCipherService = icCipherService;
            this._bocAuthService = bocAuthService;
        }


        [Route("api/auth/{icToken}")]
        [EnableCors(origins:"*", headers:"*", methods:"*")]
        [HttpGet]
        public IHttpActionResult Authenticate(string icToken) {
            string bocToken;
            try {
                int personID = ExtractPersonID(icToken);
                int? schoolID = _bocAuthService.ReadBoCAdminID(personID);
                bocToken = CreateToken(personID, schoolID);
            } catch (Exception ex) {
                bocToken = null;
            }

            if (bocToken == null)
                return StatusCode(HttpStatusCode.Unauthorized);
            else
                return Ok(new { authToken = bocToken });
        }

        private int ExtractPersonID(string icToken) {
            return int.Parse(HttpUtility.ParseQueryString(_icCipherService.Decrypt_ECB(
                HttpUtility.UrlDecode(icToken)))["personID"]);
        }

        private string CreateToken(int personID, int? schoolID) {
            if (schoolID == null) return null;
            else return //TODO: Convert.ToBase64String()
                    HttpUtility.HtmlEncode(_icCipherService.Encrypt_ECB(
                string.Format("personID={0}&schoolID={1}", personID, schoolID)));
        }

    }
}