SELECT * from UserAccount where userID in 
    (SELECT userID from UserGroupMember where groupID in 
        (SELECT groupID from UserGroup where name in ('Elementary Principal', 'Secondary Principal')))


select * from UserGroupMember where groupID in (select groupID from userGroup where name in ('Elementary Principal', 'Secondary Principal')) 

select userID, count(*), min(groupID), max(groupID)
from UserGroupMember where groupID in (select groupID from userGroup where name in ('Elementary Principal', 'Secondary Principal'))
group by userID
order by 2 desc


select * from userGroup where name like '%BoC%Admin%'
select * from userAccount where personID in (14398, 45119, 51476, 56833, 62019, 79609, 91757, 102972, 271908, 316082, 316459, 360783, 395311) order by personID
select * from userAccount where personID in (401496, 1899258) order by personID
select * from userAccount where userID in (401496, 1899258) order by personID


select * from userAccount where userID in (1876372, 8916, 174406)
select * from userAccount where userName like '%busse%'
select * from userAccount where userName like '%belm%'

select * from calendar where calendarID in (1276, 777, 1446)




select ougm.*, (select count(*) from UserGroupMember i where i.userID = ougm.userID) as countGroup
from UserGroupMember ougm
where ougm.userID in (select userID from UserGroupMember where groupID in (select groupID from userGroup where name in ('Elementary Principal', 'Secondary Principal')))


select ougm.userID, ougm.groupID,
       (select count(*) from UserGroupMember i where i.userID = ougm.userID) as countGroup,
       (select count(*) from UserGroupMember im inner join UserGroup ig on im.groupID = ig.groupID where im.userID = ougm.userID and ig.name LIKE 'School Group%') as countSchoolGroup,
       (select min(ig.name) from UserGroupMember im inner join UserGroup ig on im.groupID = ig.groupID where im.userID = ougm.userID and ig.name LIKE 'School Group%') as minSchoolGroup,
       (select max(ig.name) from UserGroupMember im inner join UserGroup ig on im.groupID = ig.groupID where im.userID = ougm.userID and ig.name LIKE 'School Group%') as maxSchoolGroup,
       ougm.*, oua.*
  from UserGroupMember ougm inner join
       UserAccount oua
    on ougm.userID = oua.userID
 where ougm.groupID in (select groupID from userGroup where name in ('Elementary Principal', 'Secondary Principal'))
 order by countSchoolGroup desc, ougm.userID








select * from calendar where name like '%Monarch%'
select * from calendar where name like '%Centaurus%'
select * from calendar where name like '%Peak to Peak%'
select * from calendar where name like '%Louisville%'
select * from calendar where name like '%Boulder%'
select * from calendar where name like '%Angevine%'
select * from calendar where name like '%Columbine%'
select * from calendar where name like '%Arapahoe%'

userID   calendarID
1884175, 1328
1899697, 1132
1867068, 1491
1865629, 1479
196765, 1281
169329, 942
166251, 1452
7214, 1275
