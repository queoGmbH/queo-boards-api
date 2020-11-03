-- Löscht alle Daten aus der Datenbank außer den Nutzer und deren Rollen
begin transaction;

delete from tblCardNotification;
delete from tblCommentNotification;
delete from tblNotification;
delete from tblActivityBase;

delete from tblTask;
delete from tblChecklist;
delete from tblComment;

delete from tblCardAssignedUsers;
delete from tblLabelToCard;
delete from tblCard;
delete from tblList;

delete from tblDocument;

delete from tblLabel;

delete from tblBoardOwner;
delete from tblBoardMember;
delete from tblBoardTeams;
delete from tblBoard;
delete from tblTeamMember;
delete from tblTeam;


-- wenn wirklich löschen, dann Commit
rollback;