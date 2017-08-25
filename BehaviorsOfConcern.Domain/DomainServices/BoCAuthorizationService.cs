using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorsOfConcern.Domain.DomainServices.Abstract;
using BehaviorsOfConcern.Domain.Entities;
using BvsdSecurity.Service;
using Dapper;

namespace BehaviorsOfConcern.Domain.DomainServices {
    public class BoCAuthorizationService : RepositoryBase, IBoCAuthorizationService {
        const string bocTokenKeyPersonID = "personID", bocTokenKeySchoolID = "schoolID", bocTokenKeyUser = "user";
        const string icTokenKeyPersonID = "personID", icTokenKeyCalendarID = "calendarID", icTokenKeyUserID = "userID";
        private string _clearAuthenticationPrefix;
        private ICipher _icCipherService;  //note that BlowFish implementation of this service (interface) is not reentrant!  So, critical sections (here, lock() stmts) are required for each use.

        public BoCAuthorizationService(string connectionString, string clearAuthenticationPrefix, ICipher icCipherService) {
            _connString = connectionString;
            _clearAuthenticationPrefix = clearAuthenticationPrefix;
            _icCipherService = icCipherService;
        }


        public BVSDAdmin ExtractExternalUser(string externalToken) {
            BVSDAdmin user = new BVSDAdmin();
            if (!string.IsNullOrWhiteSpace(externalToken)) {
                if (!string.IsNullOrWhiteSpace(_clearAuthenticationPrefix) && (externalToken.IndexOf(_clearAuthenticationPrefix) == 0)) {
                    //split clear-text token
                    try {
                        string[] tokenParts = externalToken.Split(':')[1].Split(';');
                        user.ID = int.Parse(tokenParts[0]);
                        user.UserID = int.Parse(tokenParts[1]);
                        user.CalendarID = int.Parse(tokenParts[2]);
                    } catch { }
                } else {
                    try {
                        lock (_icCipherService) {
                            //decode/decrypt token  //TODO: use Convert.ToBase64String() instead of Web.HttpUtility..
                            System.Collections.Specialized.NameValueCollection tokenParts =
                                System.Web.HttpUtility.ParseQueryString(
                                _icCipherService.Decrypt_ECB(System.Web.HttpUtility.UrlDecode(externalToken)));

                            user.ID = int.Parse(tokenParts[icTokenKeyPersonID]);
                            user.UserID = int.Parse(tokenParts[icTokenKeyUserID]);
                            user.CalendarID = int.Parse(tokenParts[icTokenKeyCalendarID]);
                        }
                    } catch (Exception ex) {
                        //TODO: log exception
                        ;
                    }
                }

                if ((user.ID != 0) && (user.UserID != 0)) {
                    //call DB to flesh out user details
                    ValidateAdmin(user);
                }
            }
            return user;
        }


        public BVSDAdmin ExtractUser(string bocToken) {
            //TODO: use Convert.ToBase64String() instead of HtmlEncode()
            BVSDAdmin user = new BVSDAdmin { School = new School() };
            try {
                lock (_icCipherService) {
                    string clearToken = _icCipherService.Decrypt_ECB(System.Web.HttpUtility.UrlDecode(bocToken));
                    var paramsCollection = System.Web.HttpUtility.ParseQueryString(clearToken);

                    user.ID = int.Parse(paramsCollection[bocTokenKeyPersonID]);
                    user.School.ID = int.Parse(paramsCollection[bocTokenKeySchoolID]);
                    user.Name = paramsCollection[bocTokenKeyUser];
                }
            } catch (Exception ex) {
                //TODO: log exception
                ;
            }
            return user;
        }


        public string BuildToken(BVSDAdmin user) {
            //TODO: use Convert.ToBase64String() instead of HtmlEncode()
            if (((user?.ID ?? 0) == 0) || ((user?.School?.ID ?? 0) == 0) || (string.IsNullOrWhiteSpace(user?.Name))) {
                return null;
            } else {
                lock (_icCipherService) {
                    return System.Web.HttpUtility.HtmlEncode(_icCipherService.Encrypt_ECB(
                        string.Format("{0}={1}&{2}={3}&{4}={5}",
                        bocTokenKeyPersonID, user.ID, bocTokenKeySchoolID, user.School.ID, bocTokenKeyUser, user.Name)));
                }
            }
        }


        private void ValidateAdmin(BVSDAdmin candidateAdmin) {
            DynamicParameters sqlParams = new DynamicParameters(new {
                userID = candidateAdmin.UserID,
                calendarID = candidateAdmin.CalendarID
            });
            sqlParams.Add("districtAdminYN", null, DbType.Boolean, ParameterDirection.Output, null);
            sqlParams.Add("schoolID", null, DbType.Int32, ParameterDirection.Output, null);
            sqlParams.Add("userName", null, DbType.String, ParameterDirection.Output, 255);

            using (SqlConnection conn = new SqlConnection(_connString)) {
                conn.Open();
                conn.Execute("p_BVSD_BoC_ValidateBoCAdmin", sqlParams, commandType: CommandType.StoredProcedure);
            }

            candidateAdmin.School = new School { ID = sqlParams.Get<int?>("schoolID") ?? 0 };  //use 0 to represent a non-admin user (non school- & non district-admin)
            if (sqlParams.Get<bool?>("districtAdminYN") == true) candidateAdmin.School.ID = -1;  //use -1 to represent a district admin (which is not assoc. w/ any one school)
            candidateAdmin.Name = sqlParams.Get<string>("userName");
        }

    }
}
