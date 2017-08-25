--truncate table BVSD_BoC_CodeSet

--code set category definitions:
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label) values (0,101,'Incident Category')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label) values (0,102,'Incident Source')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label) values (0,103,'Incident Status')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label) values (0,104,'Incident Outcome')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label) values (0,105,'Involved Party Type')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label) values (0,106,'Recency Interval')

--code sets:
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (101,1001,'Concerning Behavior',30)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (101,1002,'Self-Harm Concern',80)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (101,1003,'General Conduct',50)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (101,1004,'Threatening Behavior',90)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (101,1005,'Familiy Crisis',40)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (101,1006,'Physical Health',70)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (101,1007,'Mental Health',60)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (101,1008,'Academic',20)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (101,1009,'Abuse/Neglect',10)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (101,1010,'Other',100)

insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (102,2001,'Website',50,'Source_Website')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (102,2002,'Safe2Tell',20,'Source_Safe2Tell')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (102,2003,'Threat Assessment',40,'Source_ThreatAssessment')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (102,2004,'Suicide Assessment',30,'Source_SuicideAssessment')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (102,2005,'Paper Report',10,'Source_PaperReport')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (102,2006,'Other',60,'Source_Other')

insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (103,3001,'Submitted',10,'Status_Submitted')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (103,3002,'Received',20,'Status_Received')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (103,3003,'In Progress',30,'Status_InProgress')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (103,3004,'Closed',40,'Status_Closed')

insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (104,4001,'Dismissed/Unsubstantiated',20)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (104,4002,'Threat Assessment',100)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (104,4003,'Suicide Assessment',90)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (104,4004,'Refer to Counselor',40)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (104,4005,'Refer to Law Enforcement',50)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (104,4006,'Report to Outside Agency',60)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (104,4007,'Discipline',10)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (104,4008,'Parent Conference',30)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (104,4009,'Student Conference',80)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (104,4010,'School Safety Plan',70)
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder) values (104,4011,'Other',110)

insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label) values (105,5001,'Offender')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label) values (105,5002,'Victim / Targeted Party')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label) values (105,5003,'Witness')

insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (106,6001,'Today',10,'RecencyInterval_Today')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (106,6002,'Last 48 hours',20,'RecencyInterval_Last48Hours')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (106,6003,'Last 7 days',30,'RecencyInterval_Last7Days')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (106,6004,'Last 30 days',40,'RecencyInterval_Last30Days')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (106,6005,'This year',50,'RecencyInterval_ThisYear')
insert into BVSD_BoC_CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (106,6006,'All history',60,'RecencyInterval_AllHistory')

/*
select * from BVSD_BoC_CodeSet order by CategoryCD, CD
select * from BVSD_BoC_CodeSet order by CategoryCD, Label
select * from BVSD_BoC_CodeSet order by CategoryCD, SortOrder, CD
*/
