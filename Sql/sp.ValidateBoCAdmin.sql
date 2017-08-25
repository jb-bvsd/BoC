drop proc p_BVSD_BoC_ValidateBoCAdmin
go

create proc p_BVSD_BoC_ValidateBoCAdmin (
    @userID int, @calendarID int,
    @districtAdminYN bit out, @schoolID int out, @userName varchar(255) out
) as
--given: userID, calendarID  (personID wouldn't work since for a given personID, there are multiple userIDs present in table userAccount)
--return: districtAdminYN, schoolID, userName
--check if userID is member of BoCAdmin or ICAdmin groups, if so set schoolID=null, districtAdmin=true, lookup userName based on userID
--otherwise check if userID is member of a 'Principal' group, if so set schoolID based on calendarID lookup, userName based on userID
--otherwise return null userName, null schoolID, districtAdmin=false

select @districtAdminYN = 0, @schoolID = null, @userName = null

select @districtAdminYN = 1, --cast(count(*) as bit),
       @schoolID = null,
       @userName = iden.lastName + ', ' + iden.firstName  --read the final name in the list (thus order by clause is important)  (there can be more than one [identity] row for a given personID)
  from userGroup grp inner join
       userGroupMember mem inner join
       userAccount usr inner join
       [identity] iden
    on iden.personID = usr.personID
    on mem.userID = usr.userID
    on mem.groupID = grp.groupID
 where mem.userID = @userID
   and ((grp.name like 'BoC % Admin%') or (grp.name like 'Admin Group: IC Admin%'))
 order by iden.effectiveDate, iden.identityID

if (@districtAdminYN = 0) begin
    select @userName = iden.lastName + ', ' + iden.firstName  --read the final name in the list (thus order by clause is important)  (there can be more than one [identity] row for a given personID)
      from userGroup grp inner join
           userGroupMember mem inner join
           userAccount usr inner join
           [identity] iden
        on iden.personID = usr.personID
        on mem.userID = usr.userID
        on mem.groupID = grp.groupID
     where mem.userID = @userID
       and grp.name in ('Elementary Principal', 'Secondary Principal')
     order by iden.effectiveDate, iden.identityID
    
    if (@userName is not null)
        select @schoolID = schoolID from calendar where calendarID = @calendarID
end

--end of proc, p_BVSD_BoC_ValidateBoCAdmin
go

grant execute on p_BVSD_BoC_ValidateBoCAdmin to Reporting


/*
declare @districtAdminYN bit, @schoolID int, @userName varchar(255)
exec p_BVSD_BoC_ValidateBoCAdmin 1899258, 1277, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 1884175, 1277, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 1899697, 1277, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 1867068, -1, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 1865629, 1277, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 196765,  null, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 169329,  1277, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 166251,  1277, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 7214,    0, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
*/
/*
1884175 School Group: Monarch K-8 School
1899697 School Group: Centaurus High School
1867068	School Group: Peak to Peak Charter
1865629	School Group: Louisville Middle School
196765	School Group: Boulder High
169329	School Group: Angevine Middle
166251	School Group: Columbine Elementary School
7214	School Group: Arapahoe Ridge High School
*/
/*
declare @districtAdminYN bit, @schoolID int, @userName varchar(255)
exec p_BVSD_BoC_ValidateBoCAdmin 1237214, 1275, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 1855154, 9876, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 1899258, null, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName

exec p_BVSD_BoC_ValidateBoCAdmin 1884175, 1328, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 1899697, 1132, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 1867068, 1491, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 1865629, 1479, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 196765,  1281, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 169329,  942,  @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 166251,  1452, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
exec p_BVSD_BoC_ValidateBoCAdmin 7214,    1275, @districtAdminYN out, @schoolID out, @userName out; select @districtAdminYN, @schoolID, @userName
*/
