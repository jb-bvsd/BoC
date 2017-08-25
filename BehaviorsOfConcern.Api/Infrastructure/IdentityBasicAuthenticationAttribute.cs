using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using BehaviorsOfConcern.Domain.DomainServices;
using BehaviorsOfConcern.Domain.DomainServices.Abstract;
using BehaviorsOfConcern.Domain.Entities;
using BvsdSecurity.Service;

namespace BehaviorsOfConcern.Api.Infrastructure {
    //TODO: cite Microsoft ref. & author here

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

        static readonly string _simpleApiKey;
        static readonly IBoCAuthorizationService _bocAuthorizationService;


        static IdentityBasicAuthenticationAttribute() {
            //TODO:  _bocAuthorizationService should be instantiated via IoC Container!  Property injection will not work here since C# Attribute (sub)types are only allowed to receive simple types (int, double, string, etc.)
            _bocAuthorizationService = new BoCAuthorizationService(null, null,
                new BlowFish(System.Text.Encoding.ASCII.GetBytes(System.Web.Configuration.WebConfigurationManager.AppSettings["cipherSalt_IC"])));

            //TODO:  _simpleApiKey should be injected via IoC Container (requires jumping through a number of hoops (due to this being a .net 'Attribute' subclass) )
            _simpleApiKey = System.Web.Configuration.WebConfigurationManager.AppSettings["simpleApiKey"];
        }


        protected override async Task<IPrincipal> AuthenticateAsync(string authorizationParameter, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            ClaimsPrincipal principal;
            List<Claim> claims = new List<Claim>();
            try {
                if (!string.IsNullOrWhiteSpace(_simpleApiKey) && (_simpleApiKey == HttpUtility.UrlDecode(authorizationParameter))) {
                    claims.Add(new Claim(ClaimTypes.Role, BvsdRoles.BvsdEmployee));
                } else {
                    BVSDAdmin sessionUser = _bocAuthorizationService.ExtractUser(authorizationParameter);

                    claims.Add(new Claim(ClaimTypes.Name, sessionUser.Name));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, sessionUser.ID.ToString()));
                    claims.Add(new Claim(ClaimTypes.UserData, sessionUser.School.ID.ToString()));
                    claims.Add(new Claim(ClaimTypes.Role, (sessionUser.School.ID == -1) ? BvsdRoles.DistrictAdmin :
                        (sessionUser.School.ID > 0) ? BvsdRoles.SchoolAdmin : BvsdRoles.NotAnAdmin));
                }
                // important to set the identity this way, otherwise IsAuthenticated will be false
                // see: ht tp://leastprivilege.com/2012/09/24/claimsidentity-isauthenticated-and-authenticationtype-in-net-4-5/
                ClaimsIdentity identity = new ClaimsIdentity(claims, AuthenticationTypes.Basic);

                principal = new ClaimsPrincipal(identity);
            } catch (Exception ex) {
                //TODO: log ex
                principal = null;
            }

            return principal;
        }
    }
}
