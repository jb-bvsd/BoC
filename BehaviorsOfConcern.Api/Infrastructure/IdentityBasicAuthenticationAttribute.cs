using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ICCipher.Service;

namespace BehaviorsOfConcern.Api.Infrastructure {
    public class IdentityBasicAuthenticationAttribute : BasicAuthenticationAttribute {
        /*
        protected override async Task<IPrincipal> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken) {
            UserManager<IdentityUser> userManager = CreateUserManager();

            cancellationToken.ThrowIfCancellationRequested(); // Unfortunately, UserManager doesn't support CancellationTokens.
            IdentityUser user = await userManager.FindAsync(userName, password);

            if (user == null) {
                // No user with userName/password exists.
                return null;
            }

            // Create a ClaimsIdentity with all the claims for this user.
            cancellationToken.ThrowIfCancellationRequested(); // Unfortunately, IClaimsIdenityFactory doesn't support CancellationTokens.
            ClaimsIdentity identity = await userManager.ClaimsIdentityFactory.CreateAsync(userManager, user, "Basic");

            return new ClaimsPrincipal(identity);
        }

        private static UserManager<IdentityUser> CreateUserManager() {
            return new UserManager<IdentityUser>(new UserStore<IdentityUser>(new UsersDbContext()));
        }
        */


        /*
        protected override async Task<IPrincipal> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (userName != "testuser" || password != "Pass1word") {
                // No user with userName/password exists.
                return null;
            }

            // Create a ClaimsIdentity with all the claims for this user.
            Claim nameClaim = new Claim(ClaimTypes.Name, userName);
            List<Claim> claims = new List<Claim> { nameClaim };

            claims.Add(new Claim(ClaimTypes.Role, "testJBRole"));

            // important to set the identity this way, otherwise IsAuthenticated will be false
            // see: http://leastprivilege.com/2012/09/24/claimsidentity-isauthenticated-and-authenticationtype-in-net-4-5/
            ClaimsIdentity identity = new ClaimsIdentity(claims, AuthenticationTypes.Basic);

            var principal = new ClaimsPrincipal(identity);
            return principal;
        }
        */
        static readonly ICipher _icCipherService;

        static IdentityBasicAuthenticationAttribute() {
            //TODO:  _icCipherService should be instantiated via IoC Container!  Property injection will not work here, since C# Attribute subtypes are only allowed to receive simple types (int, double, string, etc.)
            _icCipherService = new BlowFish(System.Text.Encoding.ASCII.GetBytes(System.Web.Configuration.WebConfigurationManager.AppSettings["cipherSalt_IC"]));
        }

        protected override async Task<IPrincipal> AuthenticateAsync(string authorizationParameter, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            ClaimsPrincipal principal;
            try {
                var paramsCollection = HttpUtility.ParseQueryString(_icCipherService.Decrypt_ECB(HttpUtility.UrlDecode(authorizationParameter)));
                int personID = int.Parse(paramsCollection["personID"]);
                int schoolID = int.Parse(paramsCollection["schoolID"]);

                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, personID.ToString()));
                claims.Add(new Claim(ClaimTypes.UserData, schoolID.ToString()));
                claims.Add(new Claim(ClaimTypes.Role, (schoolID == -1) ? "DistrictAdmin" : (schoolID > 0) ? "SchoolAdmin" : "NotAnAdmin"));

                // important to set the identity this way, otherwise IsAuthenticated will be false
                // see: http://leastprivilege.com/2012/09/24/claimsidentity-isauthenticated-and-authenticationtype-in-net-4-5/
                ClaimsIdentity identity = new ClaimsIdentity(claims, AuthenticationTypes.Basic);

                principal = new ClaimsPrincipal(identity);
            } catch {
                principal = null;
            }

            return principal;
        }


    }
}
