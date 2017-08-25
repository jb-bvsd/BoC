
drop table BVSD_BoC_IncidentComment
go
drop table BVSD_BoC_Incident
go
drop table BVSD_BoC_CodeSet
go


create table BVSD_BoC_CodeSet (
    CD int not null,

    CategoryCD int not null,
    Label varchar(50) not null,
    Description varchar(255) null,
    DefaultYN bit not null default(0),
    ActiveYN bit not null default(1),
    SortOrder smallint null,
    ProgrammaticEnum varchar(100) null,  --"enum" name which should never change, as app code may depend on the value

    constraint CodeSet_PK primary key (CD),
    constraint CodeSet_AK1 unique (CategoryCD, Label)
    --constraint CodeSet_U_ProgrammaticEnum unique (ProgrammaticEnum) --not avail (on null column) in SS
)
go


create table BVSD_BoC_Incident (
    ID int not null,

    Description varchar(max) not null,
    IncidentDate datetime2(0) not null,
    CorrectedIncidentDate datetime2(0) null,
    SubmittedOn datetime2(0) not null,
    Submitter varchar(255) not null,
    SubmitterID int not null,
    ReportingParty varchar(255) null,
    ConcernedPartyID int null,
    ReportingSchoolID int not null,
    CorrectedReportingSchoolID int null,
    SpecificLocation varchar(2000) null,
    CategoryCD int null,
    SourceCD int null,
    StatusCD int null,
    OutcomeCD int null,

    UpdatedOn datetime2(0) not null default(getdate()),
    --UpdatedBy varchar(100) default(user),
    UpdatedBy varchar(100) not null,
    rowver rowversion not null,

    constraint PK_BoCIncident primary key (ID),
    --constraint AK1_BoCIncident unique (Number),
    foreign key (ConcernedPartyID) references Person(PersonID),
    foreign key (ReportingSchoolID) references School(SchoolID),
    foreign key (CorrectedReportingSchoolID) references School(SchoolID),
)
go

drop sequence BVSD_BoC_IncidentSeq
go
create sequence BVSD_BoC_IncidentSeq as int start with 101 increment by 1
go



create table BVSD_BoC_IncidentComment (
    ID int not null,

    Comment varchar(max) not null,
    BoCIncidentID int not null,

	UpdatedOn datetime2(0) not null default(getdate()),
    UpdatedBy varchar(100) not null,
    rowver rowversion not null,

    constraint PK_BoCIncidentComment primary key (ID),
    foreign key (BoCIncidentID) references BVSD_BoC_Incident(ID)
)
go

drop sequence BVSD_BoC_IncidentCommentSeq 
go
create sequence BVSD_BoC_IncidentCommentSeq  as int start with 101 increment by 1
go



grant delete, insert, references, select, update on BVSD_BoC_CodeSet to Reporting
grant delete, insert, references, select, update on BVSD_BoC_Incident to Reporting
grant delete, insert, references, select, update on BVSD_BoC_IncidentComment to Reporting

grant references, update on BVSD_BoC_IncidentSeq to Reporting
grant references, update on BVSD_BoC_IncidentCommentSeq to Reporting
