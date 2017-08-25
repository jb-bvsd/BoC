using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviorsOfConcern.Domain.Entities;
using BehaviorsOfConcern.Domain.DomainServices.Abstract;
using Dapper;
using System.Data;

namespace BehaviorsOfConcern.Domain.DomainServices {
    public class IncidentRepository : RepositoryBase, IIncidentRepository {
        public IncidentRepository(string connectionString) {
            _connString = connectionString;
        }


        public int CreateIncident(Incident newIncident) {
            DynamicParameters sqlParams = new DynamicParameters(new {
                Description = string.IsNullOrWhiteSpace(newIncident.Description) ? null : newIncident.Description,
                IncidentDate = newIncident.IncidentDate,
                ReportingSchoolID = (newIncident.ReportingSchool.ID == 0) ? null : (int?)newIncident.ReportingSchool.ID,
                CategoryCD = (newIncident.CategoryCD == 0) ? null : (int?)newIncident.CategoryCD,
                SourceCD = (newIncident.SourceCD == 0) ? null : (int?)newIncident.SourceCD,
                StatusCD = (newIncident.StatusCD == 0) ? null : (int?)newIncident.StatusCD,
                SubmitterID = (newIncident.Submitter.ID == 0) ? null : (int?)newIncident.Submitter.ID,
                SubmitterName = newIncident.Submitter.Name,
                UpdatedBy = newIncident.UpdatedBy
            });

            try {
                int incidentID;
                using (SqlConnection conn = new SqlConnection(_connString)) {
                    conn.Open();
                    incidentID = conn.QuerySingle<int>(@"
                        declare @nextID int = next value for BVSD_BoC_IncidentSeq ;
                        insert into BVSD_BoC_Incident (
                            ID, Description, IncidentDate, ReportingSchoolID, CategoryCD, SourceCD, StatusCD, SubmittedOn, SubmitterID, Submitter, UpdatedBy, UpdatedOn
                        ) values (
                            @nextID, @Description, @IncidentDate, @ReportingSchoolID, @CategoryCD, @SourceCD, @StatusCD, getdate(), @SubmitterID, @SubmitterName, @UpdatedBy, getdate()
                        );
                        select @nextID;", sqlParams);
                }
                return incidentID;
            } catch (SqlException ex) {
                //TODO:  Log error details here
                //wrap sql exception to keep Data.SqlClient namespace out of domain tier(s)
                throw new Invalid​Operation​Exception("Problem creating incident", ex);
            }
        }

        /*
                private string BuildIncidentQuerySQL(bool singleEntityYN) {
                    string sql = @"
                            select inc.ID, inc.Description, inc.IncidentDate, inc.CorrectedIncidentDate, inc.SpecificLocation,
                                   inc.CategoryCD, inc.SourceCD, inc.StatusCD, inc.OutcomeCD,
                                   inc.UpdatedOn, inc.UpdatedBy, inc.rowver as EntityVersion, inc.SubmittedOn,
                                   inc.SubmitterID as ID, inc.Submitter as Name,
                                   0 as ID, inc.ReportingParty as Name,
                                   inc.ConcernedPartyID as ID, per.StudentNumber, iden.IdentityID,
                                   iden.lastName + ', ' + iden.firstName + ' ' + iden.middleName as Name,
                                   inc.ReportingSchoolID as ID, osc.Name,
                                   inc.CorrectedReportingSchoolID as ID, csc.Name
                              from BoCIncident inc inner join
                                   School osc
                                on inc.ReportingSchoolID = osc.schoolID left outer join
                                   School csc
                                on inc.CorrectedReportingSchoolID = csc.schoolID left outer join
                                   Person per
                                on inc.ConcernedPartyID = per.personID left outer join
                                   [Identity] iden
                                on per.currentIdentityID = iden.identityID";
                    return sql + (singleEntityYN ? " where ID = @incidentID" : "");
                }


                public Incident ReadIncident(int incidentID) {
                    IEnumerable<Incident> incidents = null;
                    using (SqlConnection conn = new SqlConnection(_connString)) {
                        conn.Open();
                        incidents = conn.Query<Incident, Person, Person, Student, School, School, Incident>(
                            BuildIncidentQuerySQL(true),
                            (incident, submitter, reportingParty, concernedParty, school, correctedSchool) => {
                                incident.Submitter = submitter;
                                incident.ReportingParty = reportingParty;
                                incident.ConcernedParty = concernedParty;
                                incident.ReportingSchool = school;
                                incident.CorrectedReportingSchool = correctedSchool;
                                return incident;
                            }, new { incidentID }, null, true, "ID");
                    }
                    return incidents?.FirstOrDefault();
                }


                public IEnumerable<Incident> ReadIncidents() {
                    IEnumerable<Incident> incidents = null;
                    using (SqlConnection conn = new SqlConnection(_connString)) {
                        conn.Open();
                        incidents = conn.Query<Incident, Person, Person, Student, School, School, Incident>(
                            BuildIncidentQuerySQL(false),
                            (incident, submitter, reportingParty, concernedParty, school, correctedSchool) => {
                                incident.Submitter = submitter;
                                incident.ReportingParty = reportingParty;
                                incident.ConcernedParty = concernedParty;
                                incident.ReportingSchool = school;
                                incident.CorrectedReportingSchool = correctedSchool;
                                return incident;
                            }, splitOn: "ID");
                    }
                    return incidents;
                }
        */
        public Incident ReadIncident(int incidentID) {
            return ReadIncidents(incidentID, 0, 0, null, null, null, null, null, null, null, null)?.Item2?.FirstOrDefault();
        }


        public Tuple<int, IEnumerable<Incident>> ReadIncidents(int pageSkip, int pageTake,
            DateTime? filterIncidentDateLow, DateTime? filterIncidentDateHigh,
            IEnumerable<int> filterStatusCDs, IEnumerable<int> filterCategoryCDs, IEnumerable<int> filterOutcomeCDs,
            int? filterReportingSchoolID, string filterAdHoc, string sortDefinition) {

            return ReadIncidents(0, pageSkip, pageTake, filterIncidentDateLow, filterIncidentDateHigh, 
                filterStatusCDs, filterCategoryCDs, filterOutcomeCDs, filterReportingSchoolID, filterAdHoc, sortDefinition);
        }

        private Tuple<int, IEnumerable<Incident>> ReadIncidents(int incidentID, int pageSkip, int pageTake,
            DateTime? filterIncidentDateLow, DateTime? filterIncidentDateHigh,
            IEnumerable<int> filterStatusCDs, IEnumerable<int> filterCategoryCDs, IEnumerable<int> filterOutcomeCDs,
            int? filterReportingSchoolID, string filterAdHoc, string sortDefinition) {

            IEnumerable<Incident> incidents = null;
            int filteredRecordsCount = 0;

            DynamicParameters sqlParams = new DynamicParameters(new {
                incidentID,
                pageSkip,
                pageTake,
                filterIncidentDateLow,
                filterIncidentDateHigh,
                filterStatus = FillTableValuedParameter(filterStatusCDs),
                filterCategory = FillTableValuedParameter(filterCategoryCDs),
                filterOutcome = FillTableValuedParameter(filterOutcomeCDs),
                filterReportingSchoolID,
                filterMulti = filterAdHoc,
                sortDefinition
            });
            sqlParams.Add("filteredRecordsCount", filteredRecordsCount, DbType.Int32, ParameterDirection.Output, null);

            try {
                using (SqlConnection conn = new SqlConnection(_connString)) {
                    conn.Open();
                    incidents = conn.Query<Incident, dynamic, Incident>("p_BVSD_BoC_ReadIncidents",
                        PopulateAggregates, sqlParams, splitOn: "SubmitterID", commandType: CommandType.StoredProcedure);
                }
                //incidents = conn.Query<Incident, Person, Person, Student, School, School, Incident>  //nice, tidy syntax, but can't use it since we want to map the sproc column names to alternate property names
                //splitOn: "SubmitterID,ReportingParty,ConcernedPartyID,ReportingSchoolID,CorrectedReportingSchoolID",  //see note above

                return new Tuple<int, IEnumerable<Incident>>(sqlParams.Get<int>("filteredRecordsCount"), incidents);
            } catch (SqlException ex) {
                //TODO:  Log error details here
                //wrap sql exception to keep Data.SqlClient namespace out of domain tier(s)
                throw new Invalid​Operation​Exception("Problem reading incident(s)", ex);
            }
        }

        //h ttps://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql/table-valued-parameters
        private DataTable FillTableValuedParameter(IEnumerable<int> intSequence) {
            DataTable tvp = new DataTable();
            tvp.Columns.Add("dummy", typeof(int));
            if (intSequence != null) foreach (int n in intSequence) { tvp.Rows.Add(new object[] { n }); }
            return tvp;
        }

        private Incident PopulateAggregates(Incident incident, dynamic joinedFields) {
            incident.Submitter = new Person() { ID = joinedFields.SubmitterID, Name = joinedFields.Submitter };
            incident.ReportingSchool = new School() { ID = joinedFields.ReportingSchoolID, Name = joinedFields.ReportingSchool };

            if (joinedFields.CorrectedReportingSchool != null) incident.CorrectedReportingSchool = new School() {
                ID = joinedFields.CorrectedReportingSchoolID ?? 0,
                Name = joinedFields.CorrectedReportingSchool
            };

            if (joinedFields.ReportingParty != null) incident.ReportingParty = new Person() {
                Name = joinedFields.ReportingParty
            };

            if (joinedFields.ConcernedParty != null) incident.ConcernedParty = new Student() {
                ID = joinedFields.ConcernedPartyID ?? 0,
                StudentNumber = joinedFields.StudentNumber,
                IdentityID = joinedFields.IdentityID ?? 0,
                Name = joinedFields.ConcernedParty
            };

            return incident;
        }


        public int UpdateIncident(Incident incident) {
            DynamicParameters sqlParams = new DynamicParameters(new {
                incident.ID,
                ConcernedPartyID = incident.ConcernedParty?.ID ?? 0,
                ReportingParty = incident.ReportingParty?.Name,
                CorrectedIncidentDate = (incident.CorrectedIncidentDate == default(DateTime)) ? (DateTime?)null : incident.CorrectedIncidentDate,
                CorrectedReportingSchoolID = incident.CorrectedReportingSchool?.ID ?? 0,
                incident.SpecificLocation,
                incident.CategoryCD,
                incident.SourceCD,
                incident.StatusCD,
                incident.OutcomeCD,
                incident.UpdatedBy
            });

            int rowCount = 0;
            using (SqlConnection conn = new SqlConnection(_connString)) {
                conn.Open();
                rowCount = conn.Execute(@"
                    update BVSD_BoC_Incident set
                        ConcernedPartyID = coalesce(nullif(@ConcernedPartyID, 0), ConcernedPartyID),
                        ReportingParty = nullif(coalesce(@ReportingParty, ReportingParty), ''),
                        CorrectedIncidentDate = nullif(coalesce(@CorrectedIncidentDate, CorrectedIncidentDate), ''),
                        CorrectedReportingSchoolID = coalesce(nullif(@CorrectedReportingSchoolID, 0), CorrectedReportingSchoolID),
                        SpecificLocation = nullif(coalesce(@SpecificLocation, SpecificLocation), ''),
                        CategoryCD = coalesce(nullif(@CategoryCD, 0), CategoryCD),
                        SourceCD = coalesce(nullif(@SourceCD, 0), SourceCD),
                        StatusCD = coalesce(nullif(@StatusCD, 0), StatusCD),
                        OutcomeCD = coalesce(nullif(@OutcomeCD, 0), OutcomeCD),
                        UpdatedBy = @UpdatedBy,
	                    UpdatedOn = getdate()
                    where ID = @ID", sqlParams);
            }
            return rowCount;
        }



        public int CreateComment(Comment newComment) {
            DynamicParameters sqlParams = new DynamicParameters(new {
                Comment = string.IsNullOrWhiteSpace(newComment.Text) ? null : newComment.Text,
                BoCIncidentID = newComment.ParentID,
                UpdatedBy = newComment.UpdatedBy
            });

            int commentID;
            using (SqlConnection conn = new SqlConnection(_connString)) {
                conn.Open();
                commentID = conn.QuerySingle<int>(@"
                    declare @nextID int = next value for BVSD_BoC_IncidentCommentSeq;
                    insert into BVSD_BoC_IncidentComment (
                        ID, Comment, BoCIncidentID, UpdatedBy, UpdatedOn
                    ) values (
                        @nextID, @Comment, @BoCIncidentID, @UpdatedBy, getdate()
                    );
                    select @nextID;", sqlParams);
            }
            return commentID;
        }



        public IEnumerable<Comment> ReadComments(int incidentID) {
            IEnumerable<Comment> comments = null;
            using (SqlConnection conn = new SqlConnection(_connString)) {
                conn.Open();
                comments = conn.Query<Comment>(@"
                    select ID, BoCIncidentID as ParentID, Comment as [Text], UpdatedBy, UpdatedOn, rowver as EntityVersion
                      from BVSD_BoC_IncidentComment
                     where BoCIncidentID = @incidentID
                     order by UpdatedOn desc, ID desc", new { incidentID });
            }
            return comments;
        }

    }
}
