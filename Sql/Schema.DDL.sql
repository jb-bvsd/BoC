drop table BoCIncident
go
drop table CodeSet
go


create table CodeSet (
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


create table BoCIncident (
    ID int not null identity(101, 1),

    IncidentDate date not null,
    SubmittedOn date not null,
    Submitter varchar(255) not null,
    SubmitterEmployeeID int null,
    Description varchar(max) not null,
    ReportingParty varchar(255) null,
    ReportingSchoolID int null,
    SpecificLocation varchar(2000) null,
    CategoryCD int null,
    SourceCD int null,
    StatusCD int null,

	UpdatedOn datetime default(getdate()), 
	UpdatedBy varchar(100) default(user),
    rowver rowversion not null

    constraint PK_BoCIncident primary key (ID),
    --constraint AK1_BoCIncident unique (Number)
    foreign key (ReportingSchoolID) references School(SchoolID)
)
go
