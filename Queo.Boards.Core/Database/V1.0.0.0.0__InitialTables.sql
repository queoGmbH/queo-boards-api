
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Activity_Creator_User]') AND parent_object_id = OBJECT_ID('tblActivityBase'))
alter table tblActivityBase  drop constraint FK_Activity_Creator_User


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Activity_Board]') AND parent_object_id = OBJECT_ID('tblActivityBase'))
alter table tblActivityBase  drop constraint FK_Activity_Board


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_BoardOwner_User]') AND parent_object_id = OBJECT_ID('tblBoardOwner'))
alter table tblBoardOwner  drop constraint FK_BoardOwner_User


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_BoardOwner_Board]') AND parent_object_id = OBJECT_ID('tblBoardOwner'))
alter table tblBoardOwner  drop constraint FK_BoardOwner_Board


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_BoardMember_User]') AND parent_object_id = OBJECT_ID('tblBoardMember'))
alter table tblBoardMember  drop constraint FK_BoardMember_User


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_BoardMember_Board]') AND parent_object_id = OBJECT_ID('tblBoardMember'))
alter table tblBoardMember  drop constraint FK_BoardMember_Board


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_BoardTeams_Team]') AND parent_object_id = OBJECT_ID('tblBoardTeams'))
alter table tblBoardTeams  drop constraint FK_BoardTeams_Team


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_BoardTeams_Board]') AND parent_object_id = OBJECT_ID('tblBoardTeams'))
alter table tblBoardTeams  drop constraint FK_BoardTeams_Board


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Card_Creator]') AND parent_object_id = OBJECT_ID('tblCard'))
alter table tblCard  drop constraint FK_Card_Creator


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Card_List]') AND parent_object_id = OBJECT_ID('tblCard'))
alter table tblCard  drop constraint FK_Card_List


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_LabelToCard_Label]') AND parent_object_id = OBJECT_ID('tblLabelToCard'))
alter table tblLabelToCard  drop constraint FK_LabelToCard_Label


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_LabelToCard_Card]') AND parent_object_id = OBJECT_ID('tblLabelToCard'))
alter table tblLabelToCard  drop constraint FK_LabelToCard_Card


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_USER_ASSIGNED_TO_CARD]') AND parent_object_id = OBJECT_ID('tblCardAssignedUsers'))
alter table tblCardAssignedUsers  drop constraint FK_USER_ASSIGNED_TO_CARD


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_CARD_WITH_ASSIGNED_USERS]') AND parent_object_id = OBJECT_ID('tblCardAssignedUsers'))
alter table tblCardAssignedUsers  drop constraint FK_CARD_WITH_ASSIGNED_USERS


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Checklist_Card]') AND parent_object_id = OBJECT_ID('tblChecklist'))
alter table tblChecklist  drop constraint FK_Checklist_Card


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Comment_User_Creator]') AND parent_object_id = OBJECT_ID('tblComment'))
alter table tblComment  drop constraint FK_Comment_User_Creator


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Comment_Card]') AND parent_object_id = OBJECT_ID('tblComment'))
alter table tblComment  drop constraint FK_Comment_Card


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Document_Card]') AND parent_object_id = OBJECT_ID('tblDocument'))
alter table tblDocument  drop constraint FK_Document_Card


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Label_Board]') AND parent_object_id = OBJECT_ID('tblLabel'))
alter table tblLabel  drop constraint FK_Label_Board


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_List_Board]') AND parent_object_id = OBJECT_ID('tblList'))
alter table tblList  drop constraint FK_List_Board


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_NOTIFICATION_FOR_USER]') AND parent_object_id = OBJECT_ID('tblNotification'))
alter table tblNotification  drop constraint FK_NOTIFICATION_FOR_USER


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK5ADC1E36320F9C7B]') AND parent_object_id = OBJECT_ID('tblCardNotification'))
alter table tblCardNotification  drop constraint FK5ADC1E36320F9C7B


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_NOTIFICATION_FOR_CARD]') AND parent_object_id = OBJECT_ID('tblCardNotification'))
alter table tblCardNotification  drop constraint FK_NOTIFICATION_FOR_CARD


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK3384D3A0D9B82148]') AND parent_object_id = OBJECT_ID('tblCommentNotification'))
alter table tblCommentNotification  drop constraint FK3384D3A0D9B82148


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_NOTIFICATION_FOR_COMMENT]') AND parent_object_id = OBJECT_ID('tblCommentNotification'))
alter table tblCommentNotification  drop constraint FK_NOTIFICATION_FOR_COMMENT


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_Task_Checklist]') AND parent_object_id = OBJECT_ID('tblTask'))
alter table tblTask  drop constraint FK_Task_Checklist


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_TeamMember_User]') AND parent_object_id = OBJECT_ID('tblTeamMember'))
alter table tblTeamMember  drop constraint FK_TeamMember_User


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_TeamMember_Team]') AND parent_object_id = OBJECT_ID('tblTeamMember'))
alter table tblTeamMember  drop constraint FK_TeamMember_Team


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'[FK_ROLE_TO_USER]') AND parent_object_id = OBJECT_ID('tblUserRoles'))
alter table tblUserRoles  drop constraint FK_ROLE_TO_USER


    if exists (select * from dbo.sysobjects where id = object_id(N'tblActivityBase') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblActivityBase

    if exists (select * from dbo.sysobjects where id = object_id(N'tblBoard') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblBoard

    if exists (select * from dbo.sysobjects where id = object_id(N'tblBoardOwner') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblBoardOwner

    if exists (select * from dbo.sysobjects where id = object_id(N'tblBoardMember') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblBoardMember

    if exists (select * from dbo.sysobjects where id = object_id(N'tblBoardTeams') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblBoardTeams

    if exists (select * from dbo.sysobjects where id = object_id(N'tblCard') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblCard

    if exists (select * from dbo.sysobjects where id = object_id(N'tblLabelToCard') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblLabelToCard

    if exists (select * from dbo.sysobjects where id = object_id(N'tblCardAssignedUsers') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblCardAssignedUsers

    if exists (select * from dbo.sysobjects where id = object_id(N'tblChecklist') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblChecklist

    if exists (select * from dbo.sysobjects where id = object_id(N'tblComment') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblComment

    if exists (select * from dbo.sysobjects where id = object_id(N'tblDocument') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblDocument

    if exists (select * from dbo.sysobjects where id = object_id(N'tblLabel') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblLabel

    if exists (select * from dbo.sysobjects where id = object_id(N'tblList') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblList

    if exists (select * from dbo.sysobjects where id = object_id(N'tblNotification') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblNotification

    if exists (select * from dbo.sysobjects where id = object_id(N'tblCardNotification') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblCardNotification

    if exists (select * from dbo.sysobjects where id = object_id(N'tblCommentNotification') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblCommentNotification

    if exists (select * from dbo.sysobjects where id = object_id(N'tblTask') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblTask

    if exists (select * from dbo.sysobjects where id = object_id(N'tblTeam') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblTeam

    if exists (select * from dbo.sysobjects where id = object_id(N'tblTeamMember') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblTeamMember

    if exists (select * from dbo.sysobjects where id = object_id(N'tblUser') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblUser

    if exists (select * from dbo.sysobjects where id = object_id(N'tblUserRoles') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table tblUserRoles

    create table tblActivityBase (
        Id INT IDENTITY NOT NULL,
       ActivityType NVARCHAR(255) not null,
       BusinessId UNIQUEIDENTIFIER not null unique,
       CreationDate DATETIME not null,
       Text NVARCHAR(255) not null,
       Creator_Id INT not null,
       Board_Id INT not null,
       primary key (Id)
    )

    create table tblBoard (
        Id INT IDENTITY NOT NULL,
       BusinessId UNIQUEIDENTIFIER not null unique,
       Accessibility NVARCHAR(255) not null,
       Title NVARCHAR(255) null,
       ColorScheme NVARCHAR(255) null,
       IsArchived BIT default 0  not null,
       IsTemplate BIT default 0  not null,
       ArchivedAt DATETIME null,
       CreatedAt DATETIME null,
       Creator_Id INT not null,
       primary key (Id)
    )

    create table tblBoardOwner (
        Board_Id INT not null,
       User_Id INT not null,
       primary key (Board_Id, User_Id)
    )

    create table tblBoardMember (
        Board_Id INT not null,
       User_Id INT not null,
       primary key (Board_Id, User_Id)
    )

    create table tblBoardTeams (
        Board_Id INT not null,
       Team_Id INT not null,
       primary key (Board_Id, Team_Id)
    )

    create table tblCard (
        Id INT IDENTITY NOT NULL,
       BusinessId UNIQUEIDENTIFIER not null unique,
       Title NVARCHAR(255) not null,
       Description NVARCHAR(MAX) null,
       Due DATETIME null,
       DueExpirationNotificationCreated BIT not null,
       DueExpirationNotificationCreatedAt DATETIME null,
       IsArchived BIT default 0  not null,
       ArchivedAt DATETIME null,
       CreatedAt DATETIME not null,
       CreatedBy_Id INT not null,
       List_Id INT not null,
       PositionInList INT null,
       primary key (Id)
    )

    create table tblLabelToCard (
        Card_Id INT not null,
       Label_Id INT not null
    )

    create table tblCardAssignedUsers (
        Card_Id INT not null,
       User_Id INT not null
    )

    create table tblChecklist (
        Id INT IDENTITY NOT NULL,
       BusinessId UNIQUEIDENTIFIER not null unique,
       Title NVARCHAR(255) not null,
       Card_Id INT not null,
       primary key (Id)
    )

    create table tblComment (
        Id INT IDENTITY NOT NULL,
       BusinessId UNIQUEIDENTIFIER not null unique,
       Text NVARCHAR(MAX) null,
       CreationDate DATETIME not null,
       IsDeleted BIT not null,
       Creator_Id INT not null,
       Card_Id INT not null,
       primary key (Id)
    )

    create table tblDocument (
        Id INT IDENTITY NOT NULL,
       BusinessId UNIQUEIDENTIFIER not null unique,
       Name NVARCHAR(255) not null,
       Card_Id INT not null,
       primary key (Id)
    )

    create table tblLabel (
        Id INT IDENTITY NOT NULL,
       BusinessId UNIQUEIDENTIFIER not null unique,
       Name NVARCHAR(255) not null,
       Color NVARCHAR(255) not null,
       Board_Id INT not null,
       primary key (Id)
    )

    create table tblList (
        Id INT IDENTITY NOT NULL,
       BusinessId UNIQUEIDENTIFIER not null unique,
       Title NVARCHAR(255) not null,
       IsArchived BIT default 0  not null,
       ArchivedAt DATETIME null,
       Board_Id INT not null,
       PositionOnBoard INT null,
       primary key (Id)
    )

    create table tblNotification (
        Id INT IDENTITY NOT NULL,
       NotificationCategory nvarchar(100) not null,
       BusinessId UNIQUEIDENTIFIER not null unique,
       CreationDateTime DATETIME not null,
       EmailSendAt DATETIME null,
       EmailSend BIT not null,
       IsMarkedAsRead BIT not null,
       NotificationFor_Id INT not null,
       primary key (Id)
    )

    create table tblCardNotification (
        Id INT not null,
       NotificationReason nvarchar(100) not null,
       Card_Id INT not null,
       primary key (Id)
    )

    create table tblCommentNotification (
        Id INT not null,
       NotificationReason nvarchar(100) not null,
       Comment_Id INT not null,
       primary key (Id)
    )

    create table tblTask (
        Id INT IDENTITY NOT NULL,
       BusinessId UNIQUEIDENTIFIER not null unique,
       Title NVARCHAR(255) null,
       IsDone BIT default 0  not null,
       Checklist_Id INT not null,
       primary key (Id)
    )

    create table tblTeam (
        Id INT IDENTITY NOT NULL,
       BusinessId UNIQUEIDENTIFIER not null unique,
       Name NVARCHAR(75) not null,
       Description nvarchar(max) null,
       CreatedAt DATETIME null,
       CreatedBy_Id INT not null,
       primary key (Id)
    )

    create table tblTeamMember (
        Team_Id INT not null,
       User_Id INT not null,
       primary key (Team_Id, User_Id)
    )

    create table tblUser (
        Id INT IDENTITY NOT NULL,
       BusinessId UNIQUEIDENTIFIER not null unique,
       UserName NVARCHAR(100) not null unique,
       PasswordHash NVARCHAR(255) null,
       UserCategory NVARCHAR(255) default 'Local'  not null,
       Firstname NVARCHAR(200) null,
       Lastname NVARCHAR(200) null,
       Email NVARCHAR(200) null,
       Phone NVARCHAR(200) null,
       Company NVARCHAR(200) null,
       Department NVARCHAR(200) null,
       IsEnabled BIT not null,
       PasswordResetRequestId UNIQUEIDENTIFIER null,
       PasswordResetRequestValidTo DATETIME null,
       primary key (Id)
    )

    create table tblUserRoles (
        User_Id INT not null,
       Role NVARCHAR(255) null
    )

    alter table tblActivityBase 
        add constraint FK_Activity_Creator_User 
        foreign key (Creator_Id) 
        references tblUser

    alter table tblActivityBase 
        add constraint FK_Activity_Board 
        foreign key (Board_Id) 
        references tblBoard

    alter table tblBoardOwner 
        add constraint FK_BoardOwner_User 
        foreign key (User_Id) 
        references tblUser

    alter table tblBoardOwner 
        add constraint FK_BoardOwner_Board 
        foreign key (Board_Id) 
        references tblBoard

    alter table tblBoardMember 
        add constraint FK_BoardMember_User 
        foreign key (User_Id) 
        references tblUser

    alter table tblBoardMember 
        add constraint FK_BoardMember_Board 
        foreign key (Board_Id) 
        references tblBoard

    alter table tblBoardTeams 
        add constraint FK_BoardTeams_Team 
        foreign key (Team_Id) 
        references tblTeam

    alter table tblBoardTeams 
        add constraint FK_BoardTeams_Board 
        foreign key (Board_Id) 
        references tblBoard

    alter table tblCard 
        add constraint FK_Card_Creator 
        foreign key (CreatedBy_Id) 
        references tblUser

    alter table tblCard 
        add constraint FK_Card_List 
        foreign key (List_Id) 
        references tblList

    alter table tblLabelToCard 
        add constraint FK_LabelToCard_Label 
        foreign key (Label_Id) 
        references tblLabel

    alter table tblLabelToCard 
        add constraint FK_LabelToCard_Card 
        foreign key (Card_Id) 
        references tblCard

    alter table tblCardAssignedUsers 
        add constraint FK_USER_ASSIGNED_TO_CARD 
        foreign key (User_Id) 
        references tblUser

    alter table tblCardAssignedUsers 
        add constraint FK_CARD_WITH_ASSIGNED_USERS 
        foreign key (Card_Id) 
        references tblCard

    alter table tblChecklist 
        add constraint FK_Checklist_Card 
        foreign key (Card_Id) 
        references tblCard

    alter table tblComment 
        add constraint FK_Comment_User_Creator 
        foreign key (Creator_Id) 
        references tblUser

    alter table tblComment 
        add constraint FK_Comment_Card 
        foreign key (Card_Id) 
        references tblCard

    alter table tblDocument 
        add constraint FK_Document_Card 
        foreign key (Card_Id) 
        references tblCard

    alter table tblLabel 
        add constraint FK_Label_Board 
        foreign key (Board_Id) 
        references tblBoard

    alter table tblList 
        add constraint FK_List_Board 
        foreign key (Board_Id) 
        references tblBoard

    alter table tblNotification 
        add constraint FK_NOTIFICATION_FOR_USER 
        foreign key (NotificationFor_Id) 
        references tblUser

    alter table tblCardNotification 
        add constraint FK5ADC1E36320F9C7B 
        foreign key (Id) 
        references tblNotification

    alter table tblCardNotification 
        add constraint FK_NOTIFICATION_FOR_CARD 
        foreign key (Card_Id) 
        references tblCard

    alter table tblCommentNotification 
        add constraint FK3384D3A0D9B82148 
        foreign key (Id) 
        references tblNotification

    alter table tblCommentNotification 
        add constraint FK_NOTIFICATION_FOR_COMMENT 
        foreign key (Comment_Id) 
        references tblComment

    alter table tblTask 
        add constraint FK_Task_Checklist 
        foreign key (Checklist_Id) 
        references tblChecklist

    alter table tblTeamMember 
        add constraint FK_TeamMember_User 
        foreign key (User_Id) 
        references tblUser

    alter table tblTeamMember 
        add constraint FK_TeamMember_Team 
        foreign key (Team_Id) 
        references tblTeam

    alter table tblUserRoles 
        add constraint FK_ROLE_TO_USER 
        foreign key (User_Id) 
        references tblUser
