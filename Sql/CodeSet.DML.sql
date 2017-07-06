--truncate table CodeSet

--code set category definitions:
insert into CodeSet (CategoryCD, CD, Label) values (0,101,'Incident Category')
insert into CodeSet (CategoryCD, CD, Label) values (0,102,'Incident Source')
insert into CodeSet (CategoryCD, CD, Label) values (0,103,'Incident Status')
insert into CodeSet (CategoryCD, CD, Label) values (0,104,'Incident Outcome')
insert into CodeSet (CategoryCD, CD, Label) values (0,105,'Involved Party Type')

--code sets:
insert into CodeSet (CategoryCD, CD, Label) values (101,1001,'Concerning Behavior')
insert into CodeSet (CategoryCD, CD, Label) values (101,1002,'Self-Harm Concern')
insert into CodeSet (CategoryCD, CD, Label) values (101,1003,'General Conduct')
insert into CodeSet (CategoryCD, CD, Label) values (101,1004,'Threatening Behavior')
insert into CodeSet (CategoryCD, CD, Label) values (101,1005,'Familiy Crisis')
insert into CodeSet (CategoryCD, CD, Label) values (101,1006,'Physical Health')
insert into CodeSet (CategoryCD, CD, Label) values (101,1007,'Mental Health')
insert into CodeSet (CategoryCD, CD, Label) values (101,1008,'Academic')
insert into CodeSet (CategoryCD, CD, Label) values (101,1009,'Abuse/Neglect')
insert into CodeSet (CategoryCD, CD, Label) values (101,1010,'Other')

insert into CodeSet (CategoryCD, CD, Label) values (102,2001,'Website')
insert into CodeSet (CategoryCD, CD, Label) values (102,2002,'Safe2Tell')
insert into CodeSet (CategoryCD, CD, Label) values (102,2003,'Threat Assessment')
insert into CodeSet (CategoryCD, CD, Label) values (102,2004,'Suicide Assessment')
insert into CodeSet (CategoryCD, CD, Label) values (102,2005,'Paper Report')
insert into CodeSet (CategoryCD, CD, Label) values (102,2006,'Other')

insert into CodeSet (CategoryCD, CD, Label) values (103,3001,'Submitted')
insert into CodeSet (CategoryCD, CD, Label) values (103,3002,'Received')
insert into CodeSet (CategoryCD, CD, Label) values (103,3003,'In Progress')
insert into CodeSet (CategoryCD, CD, Label) values (103,3004,'Closed')

insert into CodeSet (CategoryCD, CD, Label) values (104,4001,'Dismissed/Unsubstantiated')
insert into CodeSet (CategoryCD, CD, Label) values (104,4002,'Threat Assessment')
insert into CodeSet (CategoryCD, CD, Label) values (104,4003,'Suicide Assessment')
insert into CodeSet (CategoryCD, CD, Label) values (104,4004,'Refer to Counselor')
insert into CodeSet (CategoryCD, CD, Label) values (104,4005,'Refer to Law Enforcement')
insert into CodeSet (CategoryCD, CD, Label) values (104,4006,'Report to Outside Agency')
insert into CodeSet (CategoryCD, CD, Label) values (104,4007,'Discipline')
insert into CodeSet (CategoryCD, CD, Label) values (104,4008,'Parent Conference')
insert into CodeSet (CategoryCD, CD, Label) values (104,4009,'Student Conference')
insert into CodeSet (CategoryCD, CD, Label) values (104,4010,'School Safety Plan')
insert into CodeSet (CategoryCD, CD, Label) values (104,4011,'Other')

insert into CodeSet (CategoryCD, CD, Label) values (105,5001,'Offender')
insert into CodeSet (CategoryCD, CD, Label) values (105,5002,'Victim / Targeted Party')
insert into CodeSet (CategoryCD, CD, Label) values (105,5003,'Witness')

/*
select * from CodeSet order by CategoryCD, CD
select @@ROWCOUNT
*/





--truncate table CodeSet
insert into CodeSet (CategoryCD, CD, Label) values (0,101,'Project Type')
insert into CodeSet (CategoryCD, CD, Label) values (0,102,'Project Step/Status')
insert into CodeSet (CategoryCD, CD, Label) values (0,103,'Project Section')
insert into CodeSet (CategoryCD, CD, Label) values (0,104,'Territory')
insert into CodeSet (CategoryCD, CD, Label) values (0,105,'Member Block')
insert into CodeSet (CategoryCD, CD, Label) values (0,106,'Sister Block')
insert into CodeSet (CategoryCD, CD, Label) values (0,107,'Business Vertical Type')
insert into CodeSet (CategoryCD, CD, Label) values (0,108,'Time Filter Group')
insert into CodeSet (CategoryCD, CD, Label) values (0,109,'Reco Status')
insert into CodeSet (CategoryCD, CD, Label) values (0,110,'Step Initialization')
insert into CodeSet (CategoryCD, CD, Label) values (0,111,'Variable Isolation Type')
insert into CodeSet (CategoryCD, CD, Label) values (0,112,'Sample View')
insert into CodeSet (CategoryCD, CD, Label) values (0,113,'Model Type')
insert into CodeSet (CategoryCD, CD, Label) values (0,114,'Shipping Method')
insert into CodeSet (CategoryCD, CD, Label) values (0,115,'Contact Type')
insert into CodeSet (CategoryCD, CD, Label) values (0,116,'Contact Role')
insert into CodeSet (CategoryCD, CD, Label) values (0,117,'Reco MetaModel Type')
insert into CodeSet (CategoryCD, CD, Label) values (0,118,'Reco Model Step Status')

insert into CodeSet (CategoryCD, CD, Label) values (101,1001,'Prospect')
insert into CodeSet (CategoryCD, CD, Label) values (101,1002,'House File')
insert into CodeSet (CategoryCD, CD, Label) values (101,1003,'Straight Select')
insert into CodeSet (CategoryCD, CD, Label) values (101,1004,'Data Append')
insert into CodeSet (CategoryCD, CD, Label) values (101,1005,'Rescore')
insert into CodeSet (CategoryCD, CD, Label) values (101,1006,'Refulfillment')
insert into CodeSet (CategoryCD, CD, Label) values (101,1007,'Special Report')
insert into CodeSet (CategoryCD, CD, Label) values (101,1008,'Analysis')

insert into CodeSet (CategoryCD, CD, Label) values (102,2001,'Analysis')
insert into CodeSet (CategoryCD, CD, Label) values (102,2002,'Bill')
insert into CodeSet (CategoryCD, CD, Label) values (102,2003,'Conversion')
insert into CodeSet (CategoryCD, CD, Label) values (102,2004,'Data Append')
insert into CodeSet (CategoryCD, CD, Label) values (102,2005,'Final Select')
insert into CodeSet (CategoryCD, CD, Label) values (102,2006,'Final Spec')
insert into CodeSet (CategoryCD, CD, Label) values (102,2007,'Model')
insert into CodeSet (CategoryCD, CD, Label) values (102,2008,'Report')
insert into CodeSet (CategoryCD, CD, Label) values (102,2009,'Score')
insert into CodeSet (CategoryCD, CD, Label) values (102,2010,'Ship')
insert into CodeSet (CategoryCD, CD, Label) values (102,2011,'Special Sample')
insert into CodeSet (CategoryCD, CD, Label) values (102,2012,'Standard Sample')
insert into CodeSet (CategoryCD, CD, Label) values (102,2013,'Straight Select')
insert into CodeSet (CategoryCD, CD, Label) values (102,2014,'Suppression')
insert into CodeSet (CategoryCD, CD, Label) values (102,2901,'Unknown')
insert into CodeSet (CategoryCD, CD, Label) values (102,2902,'Completed')

insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (103,3001,'Data Append', 'ProjectSection_DataAppend')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (103,3002,'Files', 'ProjectSection_Files')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (103,3003,'Fulfillment', 'ProjectSection_Fulfillment')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (103,3004,'Model Details', 'ProjectSection_ModelDetails')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (103,3005,'Report', 'ProjectSection_Report')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (103,3006,'Rescore', 'ProjectSection_Rescore')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (103,3007,'Shipping', 'ProjectSection_Shipping')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (103,3008,'Straight Select', 'ProjectSection_StraightSelect')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (103,3901,'Header', 'ProjectSection_Header')
update CodeSet set ActiveYN = 0 where CD = 3901

insert into CodeSet (CategoryCD, CD, Label) values (104,4001,'USA')
insert into CodeSet (CategoryCD, CD, Label) values (104,4002,'N America')
insert into CodeSet (CategoryCD, CD, Label) values (104,4003,'World')
update CodeSet set DefaultYN = 1 where CD = 4001

insert into CodeSet (CategoryCD, CD, Label) values (105,5001,'Block All')
insert into CodeSet (CategoryCD, CD, Label) values (105,5002,'Do Not Block')
insert into CodeSet (CategoryCD, CD, Label) values (105,5003,'Block - Special')

insert into CodeSet (CategoryCD, CD, Label) values (106,6001,'Block All')
insert into CodeSet (CategoryCD, CD, Label) values (106,6002,'Do Not Block')
insert into CodeSet (CategoryCD, CD, Label) values (106,6003,'Block - Special')

insert into CodeSet (CategoryCD, CD, Label) values (107,7001,'Core')
insert into CodeSet (CategoryCD, CD, Label) values (107,7002,'IBI')
insert into CodeSet (CategoryCD, CD, Label) values (107,7003,'Mobile')
update CodeSet set DefaultYN = 1 where CD = 7001

insert into CodeSet (CategoryCD, CD, Label, SortOrder) values (108,8001,'Overdue',1)
insert into CodeSet (CategoryCD, CD, Label, SortOrder) values (108,8002,'As Soon As Possible',2)
insert into CodeSet (CategoryCD, CD, Label, SortOrder) values (108,8003,'Today',3)
insert into CodeSet (CategoryCD, CD, Label, SortOrder) values (108,8004,'Tomorrow',4)
insert into CodeSet (CategoryCD, CD, Label, SortOrder) values (108,8005,'This Week',5)
insert into CodeSet (CategoryCD, CD, Label, SortOrder) values (108,8006,'Next Week',6)
insert into CodeSet (CategoryCD, CD, Label, SortOrder) values (108,8007,'Sometime Later',7)
insert into CodeSet (CategoryCD, CD, Label, SortOrder) values (108,8901,'Unknown',99)
--insert into CodeSet (CategoryCD, CD, Label, SortOrder) values (108,8000,'Unknown',99)
update CodeSet set ActiveYN = 0 where CD = 8002

insert into CodeSet (CategoryCD, CD, Label, SortOrder, DefaultYN) values (109,9001,'Planned',1,1)
insert into CodeSet (CategoryCD, CD, Label, SortOrder) values (109,9002,'Started',2)
insert into CodeSet (CategoryCD, CD, Label, SortOrder) values (109,9003,'On Hold',3)
insert into CodeSet (CategoryCD, CD, Label, SortOrder) values (109,9004,'Called Off',4)
insert into CodeSet (CategoryCD, CD, Label, SortOrder) values (109,9005,'Other',5)

insert into CodeSet (CategoryCD, CD, Label) values (110,10001,'Required')
insert into CodeSet (CategoryCD, CD, Label) values (110,10002,'Optional')
insert into CodeSet (CategoryCD, CD, Label) values (110,10003,'Optional, initially selected')

insert into CodeSet (CategoryCD, CD, Label) values (111,11001,'No VI')
insert into CodeSet (CategoryCD, CD, Label) values (111,11002,'VI NS')
insert into CodeSet (CategoryCD, CD, Label) values (111,11003,'VI')

insert into CodeSet (CategoryCD, CD, Label) values (112,12001,'12')
insert into CodeSet (CategoryCD, CD, Label) values (112,12002,'48')


insert into CodeSet (CategoryCD, CD, Label) values (113,13001,'model type A')
insert into CodeSet (CategoryCD, CD, Label) values (113,13002,'model type B')
insert into CodeSet (CategoryCD, CD, Label) values (113,13003,'model type C')
insert into CodeSet (CategoryCD, CD, Label) values (113,13004,'model type D')


insert into CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (114,14001,'Email',1,'ShippingMethod_Email')
insert into CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (114,14002,'FTP',2,'ShippingMethod_FTP')
insert into CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (114,14003,'Wamnet',3,'ShippingMethod_Wamnet')
insert into CodeSet (CategoryCD, CD, Label, SortOrder, ProgrammaticEnum) values (114,14004,'Other',4,'ShippingMethod_Other')

insert into CodeSet (CategoryCD, CD, Label) values (115,15001,'Ad hoc contact')
insert into CodeSet (CategoryCD, CD, Label) values (115,15002,'CRM user')
insert into CodeSet (CategoryCD, CD, Label) values (115,15003,'AD user')

insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (116,16001,'Personnel','ContactType_Personnel')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (116,16002,'Presenter','ContactType_Presenter')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (116,16003,'Ship To','ContactType_ShipTo')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (116,16004,'Service Bureau','ContactType_ServiceBureau')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (116,16005,'Member','ContactType_Member')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (116,16006,'Proposal','ContactType_Proposal')

insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (117,17001,'Acquisition','RecoMetaModelType_Acquisition')
insert into CodeSet (CategoryCD, CD, Label, ProgrammaticEnum) values (117,17002,'Retention','RecoMetaModelType_Retention')

insert into CodeSet (CategoryCD, CD, Label) values (118,18001,'Cont')
insert into CodeSet (CategoryCD, CD, Label) values (118,18002,'Test')
insert into CodeSet (CategoryCD, CD, Label) values (118,18003,'Re-Test')
