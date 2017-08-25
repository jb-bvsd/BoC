drop proc p_BVSD_BoC_SearchStudentsBySchool
go

create proc p_BVSD_BoC_SearchStudentsBySchool (
    @seekSchoolID int, @searchTerm varchar(100) = null, @seekYear smallint = null, @resultCountCap int = null
) as 

select @seekYear = isnull(@seekYear, dbo.getSchoolEndYear(0))
select @searchTerm = isnull(@searchTerm, '%')
select @resultCountCap = isnull(@resultCountCap, 100)

--select @seekSchoolID, @seekYear, @searchTerm, @resultCountCap

;with StudentList as (
    select per.personID, per.studentNumber, per.staffNumber,
           iden.identityID, iden.lastName, iden.firstName, iden.middleName, iden.suffix, iden.birthdate,
           enr.enrollmentID, enr.grade,
           '(#' + per.studentNumber + ')  (G '+ enr.grade + ') : ' + iden.lastName + ', ' + iden.firstName + ' ' + iden.middleName as searchTarget
      from Trial trl inner join
           ScheduleStructure sst  inner join
           Calendar cal inner join
           Enrollment enr inner join
           Person per inner join
           [Identity] iden
        on iden.identityID = per.currentIdentityID
        on enr.personID = per.personID
        on enr.calendarID = cal.calendarID
        on cal.calendarID = sst.calendarID
        on trl.structureID = sst.structureID
     where trl.active = 1
       and enr.endYear = @seekYear
       and enr.endDate is null
       and enr.serviceType = 'P'
       and cal.endYear = enr.endYear
       and cal.schoolID = @seekSchoolID
)
select top(@resultCountCap) personID, studentNumber, staffNumber, identityID, lastName, firstName, middleName, birthdate, grade, searchTarget as studentDescription
  from StudentList
 where searchTarget like @searchTerm
 --order by lastName, firstName, middleName, grade, studentNumber

--end of proc, p_BVSD_BoC_SearchStudentsBySchool
go

grant execute on p_BVSD_BoC_SearchStudentsBySchool to Reporting
go


/*
exec p_BVSD_BoC_SearchStudentsBySchool 38, '%: d%'
exec p_BVSD_BoC_SearchStudentsBySchool 38, '%: d%', 2016
exec p_BVSD_BoC_SearchStudentsBySchool 38, '%(g 10)%', null, 123
exec p_BVSD_BoC_SearchStudentsBySchool 38, '%(g 10)%: m%'
exec p_BVSD_BoC_SearchStudentsBySchool 38, '%, Socia%'
*/
