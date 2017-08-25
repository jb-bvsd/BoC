SELECT * from UserAccount where userID in 
    (SELECT userID from UserGroupMember where groupID in 
        (SELECT groupID from UserGroup where name in ('Elementary Principal', 'Secondary Principal')))
and userID in 
    (SELECT userID from UserGroupMember where groupID in 
        (SELECT groupID from UserGroup where name LIKE 'School Group%' and groupID in 
            (SELECT groupID from UserGroupSchoolYearRights where calendarID = 1278)))



sp_columns userAccount
select count(*) from userAccount  --68129
select min(calendarID), max(calendarID) from userAccount
select top 2000 * from userAccount order by 1
select top 2000 * from userAccount where username like '%busse%'
select username, count(*) from userAccount group by userName having count(*) > 1  --0
select userID, count(*) from userAccount group by userID having count(*) > 1  --0
select personID, count(*) from userAccount group by personID having count(*) > 1 order by 2  --376
select * from userAccount where personID in (14398, 45119, 51476, 56833, 62019, 79609, 91757, 102972, 271908, 316082, 316459, 360783, 395311) order by personID

sp_columns userGroup
select count(*) from userGroup  --429
select * from userGroup order by name

sp_columns userGroupMember
select count(*) from userGroupMember  --18235
select top 2000 * from userGroupMember
select memberID, count(*) from userGroupMember group by memberID having count(*) > 1

sp_columns userGroupSchoolYearRights
select count(*) from userGroupSchoolYearRights  --1459
select * from userGroupSchoolYearRights order by endYear
select * from userGroupSchoolYearRights order by endYear


select * from userGroup where name like '%Principal%'
select * from userGroup where name like '%School Group%' order by name
select * from userGroup where name like '%Admin%' order by name

select * from calendar where calendarID = 1278
select * from userGroupSchoolYearRights where calendarID = 1278
select * from userGroup where groupID in (select groupID from userGroupSchoolYearRights where calendarID = 1278)

select * from userGroupMember where groupID = 999
select * from userAccount where userID in (select userID from userGroupMember where groupID = 999)
