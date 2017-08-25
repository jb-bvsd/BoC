drop proc p_BVSD_BoC_ReadIncidents
go
drop type BVSD_integerListTableType 
go

create type BVSD_integerListTableType  as table (n int not null primary key)
go
grant execute on type::dbo.BVSD_integerListTableType  to Reporting
go

create proc p_BVSD_BoC_ReadIncidents (
    @incidentID int,
    @pageSkip int,
    @pageTake int,
    @filterIncidentDateLow datetime = null,
    @filterIncidentDateHigh datetime = null,
    @filterStatus BVSD_integerListTableType  readonly,
    @filterCategory BVSD_integerListTableType  readonly,
    @filterOutcome BVSD_integerListTableType  readonly,
    @filterReportingSchoolID int = null,
    @filterMulti varchar(255) = null,
    @sortDefinition varchar(500) = null,
    @debug bit = 0,
    @filteredRecordsCount int out
) as

if (isnull(@incidentID, 0) != 0) begin
    
    -- note: it's important that the select clause here (names, number, & order) match that of the 
    --       cooresponding select clause in the 'else' path below  (clients expect this)
    select inc.ID, inc.Description, inc.IncidentDate, inc.CorrectedIncidentDate, inc.SpecificLocation,
           inc.StatusCD, inc.CategoryCD, inc.OutcomeCD, inc.SourceCD,
           inc.UpdatedOn, inc.UpdatedBy, inc.rowver as EntityVersion, inc.SubmittedOn,
           inc.SubmitterID, inc.Submitter,
           inc.ReportingParty,
           inc.ConcernedPartyID, per.StudentNumber, iden.IdentityID,
           iden.lastName + ', ' + iden.firstName + ' ' + iden.middleName as ConcernedParty,
           inc.ReportingSchoolID, osc.Name as ReportingSchool,
           inc.CorrectedReportingSchoolID, csc.Name as CorrectedReportingSchool,
           coalesce(csc.Name, osc.Name) as ReportingSchoolSort,
           coalesce(inc.CorrectedIncidentDate, inc.IncidentDate) as IncidentDateSort
      from BVSD_BoC_Incident inc inner join
           School osc
        on inc.ReportingSchoolID = osc.schoolID left outer join
           School csc
        on inc.CorrectedReportingSchoolID = csc.schoolID left outer join
           Person per
        on inc.ConcernedPartyID = per.personID left outer join
           [Identity] iden
        on per.currentIdentityID = iden.identityID
     where inc.ID = @incidentID
    ;
    select @filteredRecordsCount = @@ROWCOUNT
    ;
end else begin

    declare @filterStatusItemCount int, @filterCategoryItemCount int, @filterOutcomeItemCount int
    select @filterStatusItemCount = count(*) from @filterStatus
    select @filterCategoryItemCount = count(*) from @filterCategory
    select @filterOutcomeItemCount = count(*) from @filterOutcome
    if (len(isnull(ltrim(@sortDefinition), '')) < 1) select @sortDefinition = 'ID'

    select inc.ID, inc.Description, inc.IncidentDate, inc.CorrectedIncidentDate, inc.SpecificLocation,
           inc.StatusCD, inc.CategoryCD, inc.OutcomeCD, inc.SourceCD,
           inc.UpdatedOn, inc.UpdatedBy, inc.rowver as EntityVersion, inc.SubmittedOn,
           inc.SubmitterID, inc.Submitter,
           inc.ReportingParty,
           inc.ConcernedPartyID, per.StudentNumber, iden.IdentityID,
           iden.lastName + ', ' + iden.firstName + ' ' + iden.middleName as ConcernedParty,
           inc.ReportingSchoolID, osc.Name as ReportingSchool,
           inc.CorrectedReportingSchoolID, csc.Name as CorrectedReportingSchool,
           coalesce(csc.Name, osc.Name) as ReportingSchoolSort,
           coalesce(inc.CorrectedIncidentDate, inc.IncidentDate) as IncidentDateSort
      into #filteredBoCIncident
      from BVSD_BoC_Incident inc inner join
           School osc
        on inc.ReportingSchoolID = osc.schoolID left outer join
           School csc
        on inc.CorrectedReportingSchoolID = csc.schoolID left outer join
           Person per
        on inc.ConcernedPartyID = per.personID left outer join
           [Identity] iden
        on per.currentIdentityID = iden.identityID
     where ((inc.StatusCD in (select n from @filterStatus)) or (@filterStatusItemCount = 0))
       and ((inc.CategoryCD in (select n from @filterCategory)) or (@filterCategoryItemCount = 0))
       and ((inc.OutcomeCD in (select n from @filterOutcome)) or (@filterOutcomeItemCount = 0))
       and ((@filterReportingSchoolID = coalesce(inc.CorrectedReportingSchoolID, inc.ReportingSchoolID)) or (@filterReportingSchoolID is null))
       and coalesce(inc.CorrectedIncidentDate, inc.IncidentDate)
           between isnull(@filterIncidentDateLow, '1900-1-1') and isnull(@filterIncidentDateHigh, '2999-1-1')
       and ((inc.Description + '|' +  isnull(per.StudentNumber, '') + '|' + isnull(iden.lastName, '') + '|' +
           isnull(iden.firstName, '') + '|' + isnull(iden.middleName, '') like @filterMulti)
           or (@filterMulti is null))
    ;

    select @filteredRecordsCount = count(*) from #filteredBoCIncident
    ;
    declare @sqlSorted nvarchar(1000), @sqlParams nvarchar(100)
    select @sqlSorted = '
    select fin.*
      from #filteredBoCIncident fin left outer join
           (select row_number() over(order by isnull(SortOrder, 0), Label) as CategorySort, CD from BVSD_BoC_CodeSet where CategoryCD = 101) as cat
        on fin.CategoryCD = cat.CD left outer join
           (select row_number() over(order by isnull(SortOrder, 0), Label) as SourceSort, CD from BVSD_BoC_CodeSet where CategoryCD = 102) as src
        on fin.SourceCD = src.CD left outer join    
           (select row_number() over(order by isnull(SortOrder, 0), Label) as StatusSort, CD from BVSD_BoC_CodeSet where CategoryCD = 103) as sts
        on fin.StatusCD = sts.CD
     order by ' + @sortDefinition + '
    offset @pageSkip rows
     fetch next @pageTake rows only',
    @sqlParams = '@pageSkip int, @pageTake int'

    if @debug = 1 print @sqlSorted
    exec sp_executesql @sqlSorted, @sqlParams, @pageSkip, @pageTake
end
--end of proc, p_BVSD_BoC_ReadIncidents
go

grant execute on p_BVSD_BoC_ReadIncidents to Reporting



/*
declare @mylist BVSD_integerListTableType 
--insert into @mylist(n) values (9),(12),(27),(37)
insert into @mylist (n) values (3001),(3002),(3003)
exec p_BVSD_BoC_ReadIncidents 0, 10, @mylist


exec p_BVSD_BoC_ReadIncidents null, 38, '%, Socia%'

--1001 1010   2001 2006   3001 3004   4001 4011   6001 6006
exec p_BVSD_BoC_ReadIncidents null, 0, 10
exec p_BVSD_BoC_ReadIncidents null, 0, 10, '2017-7-1', '2017-7-7'
exec p_BVSD_BoC_ReadIncidents null, 0, 10, '2017-7-1', '2017-7-7', default, default, default, 28
exec p_BVSD_BoC_ReadIncidents null, 15, 30, default, default, default, default, default, 28

@pageSkip, @pageTake, @filterIncidentDateLow, @filterIncidentDateHigh, @filterStatus, @filterCategory, @filterOutcome, @filterReportingSchoolID, @filterMulti, @sortDefinition, @debug, @filteredRecordsCount
--CategorySort, SourceSort, StatusSort, SubmittedOn, ID, IncidentDateSort, ConcernedParty, ReportingSchoolSort

declare @recordCount int
exec p_BVSD_BoC_ReadIncidents null, 0, 2000, null, null, default, default, default, null, null, 'ReportingSchoolSort desc', 1, @recordCount out
select @recordCount

declare @recordCount int
exec p_BVSD_BoC_ReadIncidents null, 101, 100, null, null, default, default, default, null, null, '', 1, @recordCount out
select @recordCount

declare @recordCount int
exec p_BVSD_BoC_ReadIncidents 9144, 101, 100, null, null, default, default, default, null, null, '', 1, @recordCount out
select @recordCount
*/
