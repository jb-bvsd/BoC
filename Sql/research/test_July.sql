select * from school

select dbo.getSchoolEndYear(0)

select * from calendar where endYear = dbo.getSchoolEndYear(0)
select distinct schoolID from calendar where endYear = dbo.getSchoolEndYear(0)


select * from school where schoolID in (select schoolID from calendar where endYear = dbo.getSchoolEndYear(0)) and address is null
order by name

select * from school where schoolID in (select schoolID from calendar where endYear = dbo.getSchoolEndYear(0)) and address is not null
order by name

select schoolID as ID, Name from school where schoolID in (select schoolID from calendar where endYear = dbo.getSchoolEndYear(0)) and address is not null
order by name



truncate table BoCIncident

insert into BoCIncident (ID, IncidentDate, SubmittedOn, Submitter, UpdatedBy, Description) values (next value for BoCIncidentSeq, '2017-6-6 13:13:13', getdate(), 'dummy x', 'dummy x', 'Pig shankle ham hock prosciutto, meatball ham beef ribs doner kevin burgdoggen flank pork tri-tip frankfurter. Strip steak ham hock short ribs jowl sausage frankfurter beef kevin ball tip. Rump jowl tenderloin turkey beef ribs porchetta picanha ribeye alcatra cupim brisket venison cow capicola pork loin.')
insert into BoCIncident (ID, IncidentDate, SubmittedOn, Submitter, Description) values (next value for BoCIncidentSeq, '2017-6-6 13:13:13', getdate(), 'dummy x', 'Pig shankle ham hock prosciutto, meatball ham beef ribs doner kevin burgdoggen flank pork tri-tip frankfurter. Strip steak ham hock short ribs jowl sausage frankfurter beef kevin ball tip. Rump jowl tenderloin turkey beef ribs porchetta picanha ribeye alcatra cupim brisket venison cow capicola pork loin.')




update BoCIncident set CorrectedReportingSchoolID = null where ID = 129


declare @nextID int = next value for BoCIncidentSeq; insert into BoCIncident (ID, IncidentDate, SubmittedOn, Submitter, Description) values (@nextID, '2017-6-6 13:13:13', getdate(), 'dummy x', 'Pig shankle ham hock prosciutto, meatball ham beef ribs doner kevin burgdoggen flank pork tri-tip frankfurter. Strip steak ham hock short ribs jowl sausage frankfurter beef kevin ball tip. Rump jowl tenderloin turkey beef ribs porchetta picanha ribeye alcatra cupim brisket venison cow capicola pork loin.'); select @nextID;

declare @nextID int = next value for BoCIncidentSeq; insert into BoCIncident (ID, IncidentDate, SubmittedOn, Submitter, UpdatedBy, Description) values (@nextID, '2017-6-6 13:13:13', getdate(), 'dummy x', 'dummy x', 'Pig shankle ham hock prosciutto, meatball ham beef ribs doner kevin burgdoggen flank pork tri-tip frankfurter. Strip steak ham hock short ribs jowl sausage frankfurter beef kevin ball tip. Rump jowl tenderloin turkey beef ribs porchetta picanha ribeye alcatra cupim brisket venison cow capicola pork loin.'); select @nextID;


select len(name), * from school order by 1



select ID, Description, IncidentDate, CorrectedIncidentDate, SubmittedOn, Submitter,
       SubmitterID, ReportingParty, ConcernedPartyID, ReportingSchoolID, CorrectedReportingSchoolID,
       SpecificLocation, CategoryCD, SourceCD, StatusCD, UpdatedOn, UpdatedBy, rowver
  from BoCIncident
 where ID = 129


select ID, Description,
       SubmitterID as [ID], Submitter as [Name]
  from BoCIncident
 where ID = 129


update BoCIncident set Description = 'Beef venison sirloin burgdoggen, ham hock spare ribs bacon kielbasa prosciutto. Pork chop t-bone bresaola ground round rump salami fatback landjaeger turkey, picanha sausage short ribs drumstick. Chicken' where ID = 129
update BoCIncident set ReportingParty = null where ID = 129



select * from BoCIncident
select * from BoCIncidentComment
