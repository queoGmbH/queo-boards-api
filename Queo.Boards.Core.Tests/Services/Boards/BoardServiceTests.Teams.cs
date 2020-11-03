using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Services.Impl;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services.Boards {

    /// <summary>
    /// Teil der BoardServiceTests-Klasse, der die Verwaltung der Teams an einem Board testet.
    /// </summary>
    public partial class BoardServiceTests : CreateBaseTest {


        /// <summary>
        ///     Testet das Hinzufügen eines Teams zu einem Board
        /// </summary>
        [TestMethod]
        public void TestAddTeam() {
            //Given: Ein Board und ein Team
            Board board = Create.Board();
            Team team = Create.Team();

            //When: Das Team dem Board hinzugefügt wird
            IList<Team> boardTeams = new BoardService(null, null, null, null, null).AddTeam(board, team, Create.User());

            //Then: Muss es in der Liste der Teams am Board enthalten sein
            boardTeams.Should().BeEquivalentTo(team);
            board.Teams.Should().BeEquivalentTo(team);
        }

        /// <summary>
        ///     Testet das beim Hinzufügen eines Teams zu einem archivierten Board eine Exception geworfen wird.
        /// </summary>
        [TestMethod]
        public void TestAddTeamToArchivedBoardShouldThrowException() {
            //Given: Ein archiviertes Board
            Board archivedBoard = Create.Board().ArchivedAt(DateTime.UtcNow);

            //When: Dem archivierten Board ein Team zugewiesen werden soll
            Action action = () => new BoardService(null, null, null, null, null).AddTeam(archivedBoard, Create.Team(), Create.User());

            //Then: Muss eine Exception geworfen werden
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        /// <summary>
        ///     Testet das beim Hinzufügen eines Teams zu einer Board-Vorlage eine Exception geworfen wird.
        /// </summary>
        [TestMethod]
        public void TestAddTeamToTemplateShouldThrowException() {
            //Given: Eine Board-Vorlage
            Board template = Create.Board().AsTemplate();

            //When: Der Vorlage ein Team zugewiesen werden soll
            Action action = () => new BoardService(null, null, null, null, null).AddTeam(template, Create.Team(), Create.User());

            //Then: Muss eine Exception geworfen werden
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        /// <summary>
        ///     Testet das erneute Hinzufügen eines Teams zu einem Board
        /// </summary>
        [TestMethod]
        public void TestAddTeamTwice() {
            //Given: Ein Board, dem ein Team zugewiesen ist 
            Team team = Create.Team();
            Board board = Create.Board().WithTeams(team);

            //When: Das Team erneut dem Board hinzugefügt wird
            IList<Team> boardTeams = new BoardService(null, null, null, null, null).AddTeam(board, team, Create.User());

            //Then: Darf nix passieren und das Team muss weiterhin dem Board zugewiesen sein.
            boardTeams.Should().BeEquivalentTo(team);
            board.Teams.Should().BeEquivalentTo(team);
        }

        /// <summary>
        ///     Testet das Nutzer eines Teams zu Board-Nutzern werden, wenn das Team dem Board hinzugefügt wird
        /// </summary>
        [TestMethod]
        public void TestAddTeamWithMembersShouldMakeTeamMembersBoardUsers() {
            //Given: Ein Team mit Mitgliedern und ein Board
            User teamMember1 = Create.User();
            User teamMember2 = Create.User();
            Team team = Create.Team().WithMembers(teamMember1, teamMember2);
            Board board = Create.Board();

            User addingUser = Create.User();
            Mock<IEmailNotificationService> emailNotificationMock = new Mock<IEmailNotificationService>();
            emailNotificationMock.Setup(s => s.NotifyUserAddedToBoard(teamMember1, board, addingUser));
            emailNotificationMock.Setup(s => s.NotifyUserAddedToBoard(teamMember2, board, addingUser));

            //When: Das Team dem Board hinzugefügt wird
            new BoardService(null, null, null, null, emailNotificationMock.Object).AddTeam(board, team, addingUser);

            //Then: Müssen die Mitglieder des Teams zu Nutzern des Boards werden
            board.GetBoardUsers().Should().Contain(teamMember1);
            board.GetBoardUsers().Should().Contain(teamMember2);

            //Then: Dürfen die Teammitglieder keine expliziten (separat zugewiesenen) Mitglieder des Boards werden
            board.Members.Should().BeEmpty();
        }

        /// <summary>
        ///     Testet, dass Mitglieder des Teams, die bereits Mitglied des Boards sind KEINE E-Mail über das neu verfügbare Board
        ///     erhalten.
        /// </summary>
        [TestMethod]
        public void TestAddTeamWithMembersShouldNotSendMailToExistingBoardMembersInTeam() {
            //Given: Ein Team mit einem Mitglied, dass bereits Mitglied des Boards ist, zu welchem das Team hinzugefügt wird.
            User teamAndBoardMember = Create.User();
            Team team = Create.Team().WithMembers(teamAndBoardMember);
            Board board = Create.Board().WithMembers(teamAndBoardMember);
            User addingUser = Create.User();

            Mock<IEmailNotificationService> emailNotificationMock = new Mock<IEmailNotificationService>();
            emailNotificationMock.Setup(s => s.NotifyUserAddedToBoard(teamAndBoardMember, board, addingUser));

            //When: Das Team dem Board hinzugefügt wird
            new BoardService(null, null, null, null, emailNotificationMock.Object).AddTeam(board, team, addingUser);

            //Then: Müssen die Mitglieder des Teams eine Benachrichtigung erhalten, dass sie jetzt Board-Nutzer sind
            emailNotificationMock.Verify(s => s.NotifyUserAddedToBoard(teamAndBoardMember, board, addingUser), Times.Never);
        }

        /// <summary>
        ///     Testet, dass neue Mitglieder aus dem Team eine E-Mail über das neu verfügbare Board erhalten.
        /// </summary>
        [TestMethod]
        public void TestAddTeamWithMembersShouldSendMailToNewBoardMembers() {
            //Given: Ein Team mit Mitgliedern, dass zu einem Board hinzugefügt wird.
            User teamMember1 = Create.User();
            User teamMember2 = Create.User();
            Team team = Create.Team().WithMembers(teamMember1, teamMember2);
            Board board = Create.Board();
            User addingUser = Create.User();

            Mock<IEmailNotificationService> emailNotificationMock = new Mock<IEmailNotificationService>();
            emailNotificationMock.Setup(s => s.NotifyUserAddedToBoard(teamMember1, board, addingUser));
            emailNotificationMock.Setup(s => s.NotifyUserAddedToBoard(teamMember2, board, addingUser));

            //When: Das Team dem Board hinzugefügt wird
            new BoardService(null, null, null, null, emailNotificationMock.Object).AddTeam(board, team, addingUser);

            //Then: Müssen die Mitglieder des Teams eine Benachrichtigung erhalten, dass sie jetzt Board-Nutzer sind
            emailNotificationMock.Verify(s => s.NotifyUserAddedToBoard(teamMember1, board, addingUser), Times.Once);
            emailNotificationMock.Verify(s => s.NotifyUserAddedToBoard(teamMember2, board, addingUser), Times.Once);
        }


        /// <summary>
        /// Testet das simple Löschen eines Teams ohne Mitglieder von einem Board 
        /// </summary>
        [TestMethod]
        public void TestRemoveTeam() {

            //Given: Ein Team ohne Mitglieder, das einem nicht archivierten Board zugewiesen ist.
            Team teamWithoutMembers = Create.Team();
            Board boardWithOneTeam = Create.Board();

            //When: Das Team vom Board entfernt werden soll
            IList<Team> remainingTeams = new BoardService(null, null, null, null, null).RemoveTeam(boardWithOneTeam, teamWithoutMembers);

            //Then: Darf es anschließend nicht mehr in der Liste der Teams am Board enthalten sein
            remainingTeams.Should().BeEmpty();
            boardWithOneTeam.Teams.Should().BeEmpty();

        }

        /// <summary>
        /// Testet, dass das entfernen eines Teams von einem archivierten Board eine Exception wirft.
        /// </summary>
        [TestMethod]
        public void TestRemoveTeamShouldThrowExceptionForArchivedBoard() {

            //Given: Ein archiviertes Board, dem ein Team zugewiesen ist.
            Team team = Create.Team();
            Board archivedBoardWithTeam = Create.Board().ArchivedAt(DateTime.UtcNow).WithTeams(team);

            //When: Das Team vom Board gelöscht werden soll
            Action action = () => new BoardService(null, null, null, null, null).RemoveTeam(archivedBoardWithTeam, team);

            //Then: Darf das nicht funktionieren
            action.ShouldThrow<ArgumentOutOfRangeException>();

        }

        /// <summary>
        /// Testet, dass das entfernen eines Teams von einer Board-Vorlage eine Exception wirft.
        /// </summary>
        [TestMethod]
        public void TestRemoveTeamShouldThrowExceptionForBoardTemplate() {

            //Given: Eine Board-Vorlage, dem ein Team zugewiesen ist.
            Team team = Create.Team();
            Board archivedBoardWithTeam = Create.Board().AsTemplate().WithTeams(team);

            //When: Das Team vom Board gelöscht werden soll
            Action action = () => new BoardService(null, null, null, null, null).RemoveTeam(archivedBoardWithTeam, team);

            //Then: Darf das nicht funktionieren
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        /// <summary>
        /// Testet das ein Nutzer, der durch die Mitgliedschaft im Team auch Mitglied der Karte war, von allen Karten entfernt wird, an denen er angemeldet war.
        /// </summary>
        [TestMethod]
        public void TestRemoveTeamShouldRemoveDeletedMembersFromCards() {

            //Given: Ein Nutzer in einem Team, welches einem Board zugewiesen ist.
            User user = Create.User();
            Team team = Create.Team().WithMembers(user);
            Board board = Create.Board().WithTeams(team);
            

            //Given: Der Nutzer ist außerdem zwei Karten des Boards angemeldet.
            List list = Create.List().OnBoard(board);
            Card card1 = Create.Card().OnList(list).WithAssignedUsers(user);
            Card card2 = Create.Card().OnList(list).WithAssignedUsers(user);

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(s => s.UnassignUsers(card1, user));
            cardServiceMock.Setup(s => s.UnassignUsers(card2, user));

            //When: Das Team vom Board entfernt wird

            new BoardService(null, null, null, cardServiceMock.Object, null).RemoveTeam(board, team);

            //Then: Muss der Nutzer von allen Karten entfernt werden
            cardServiceMock.Verify(s => s.UnassignUsers(card1, user), Times.Once);
            cardServiceMock.Verify(s => s.UnassignUsers(card2, user), Times.Once);
        }

        /// <summary>
        /// Testet das ein Nutzer, der durch die Mitgliedschaft im Team auch Mitglied der Karte war, von einer archivierten Karten entfernt wird, an der er angemeldet war.
        /// </summary>
        [TestMethod]
        public void TestRemoveTeamShouldRemoveDeletedMembersFromArchivedCard() {

            //Given: Ein Nutzer in einem Team, welches einem Board zugewiesen ist.
            User user = Create.User();
            Team team = Create.Team().WithMembers(user);
            Board board = Create.Board().WithTeams(team);


            //Given: Der Nutzer ist außerdem einer archivierten Karte des Boards angemeldet.
            List list = Create.List().OnBoard(board);
            Card archivedCard = Create.Card().OnList(list).ArchivedAt(DateTime.UtcNow).WithAssignedUsers(user);
            
            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(s => s.UnassignUsers(archivedCard, user));
            
            //When: Das Team vom Board entfernt wird

            new BoardService(null, null, null, cardServiceMock.Object, null).RemoveTeam(board, team);

            //Then: Muss der Nutzer auch von der archivierten Karte entfernt werden
            cardServiceMock.Verify(s => s.UnassignUsers(archivedCard, user), Times.Once);
        }

        /// <summary>
        /// Testet das ein Nutzer, der durch die Mitgliedschaft im Team auch Mitglied der Karte war, von einer Karte auf einer archivierten Liste entfernt wird, an der er angemeldet war.
        /// </summary>
        [TestMethod]
        public void TestRemoveTeamShouldRemoveDeletedMembersFromCardOnArchivedList() {

            //Given: Ein Nutzer in einem Team, welches einem Board zugewiesen ist.
            User user = Create.User();
            Team team = Create.Team().WithMembers(user);
            Board board = Create.Board().WithTeams(team);


            //Given: Der Nutzer ist außerdem an einer Karte auf einer archivierten Liste des Boards angemeldet.
            List archivedList = Create.List().OnBoard(board).ArchivedAt(DateTime.UtcNow);
            Card card = Create.Card().OnList(archivedList).WithAssignedUsers(user);

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(s => s.UnassignUsers(card, user));

            //When: Das Team vom Board entfernt wird
            new BoardService(null, null, null, cardServiceMock.Object, null).RemoveTeam(board, team);

            //Then: Muss der Nutzer auch von der Karte auf der archivierten Liste entfernt werden
            cardServiceMock.Verify(s => s.UnassignUsers(card, user), Times.Once);
        }

        /// <summary>
        /// Testet das der Besitzer eines Boards, der Mitglied des entfernten Teams ist, NICHT von den Karten des Boards entfernt wird. 
        /// </summary>
        [TestMethod]
        public void TestRemoveTeamShouldNotRemoveOwnerWhoIsMemberOfTheTeamFromCards() {

            //Given: Ein Nutzer der Besitzer eines Boards ist, dem auch ein Team zugewiesen ist, bei dem er Mitglied ist
            User user = Create.User();
            Team team = Create.Team().WithMembers(user);
            Board board = Create.Board().WithTeams(team).WithOwners(user);

            //Given: Der Nutzer ist außerdem an einer Karte auf des Boards angemeldet.
            List list = Create.List().OnBoard(board);
            Card card = Create.Card().OnList(list).WithAssignedUsers(user);

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(s => s.UnassignUsers(card, user));

            //When: Das Team vom Board entfernt wird
            new BoardService(null, null, null, cardServiceMock.Object, null).RemoveTeam(board, team);

            //Then: Darf der Besitzer des Boards nicht von den Karten entfernt werden.
            cardServiceMock.Verify(s => s.UnassignUsers(card, user), Times.Never);
        }

        /// <summary>
        /// Testet ein explizites Mitglied eines Boards, das Mitglied des entfernten Teams ist, NICHT von den Karten des Boards entfernt wird. 
        /// </summary>
        [TestMethod]
        public void TestRemoveTeamShouldNotRemoveMemberWhoIsMemberOfTheTeamFromCards() {

            //Given: Ein Nutzer der explizites Mitglied eines Boards ist, dem auch ein Team zugewiesen ist, bei dem er Mitglied ist
            User user = Create.User();
            Team team = Create.Team().WithMembers(user);
            Board board = Create.Board().WithTeams(team).WithMembers(user);

            //Given: Der Nutzer ist außerdem an einer Karte auf des Boards angemeldet.
            List list = Create.List().OnBoard(board);
            Card card = Create.Card().OnList(list).WithAssignedUsers(user);

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(s => s.UnassignUsers(card, user));

            //When: Das Team vom Board entfernt wird
            new BoardService(null, null, null, cardServiceMock.Object, null).RemoveTeam(board, team);

            //Then: Darf der Nutzer nicht von den Karten entfernt werden.
            cardServiceMock.Verify(s => s.UnassignUsers(card, user), Times.Never);
        }

        /// <summary>
        /// Testet, dass ein explizites Mitglied eines Boards, das Mitglied des entfernten Teams ist, NICHT vom Board entfernt wird. 
        /// </summary>
        [TestMethod]
        public void TestRemoveTeamShouldNotRemoveExplizitMemberWhoIsMemberOfTheTeam() {

            //Given: Ein Nutzer der explizites Mitglied eines Boards ist, dem auch ein Team zugewiesen ist, bei dem er Mitglied ist
            User user = Create.User();
            Team team = Create.Team().WithMembers(user);
            Board board = Create.Board().WithTeams(team).WithMembers(user);

            //Given: Der Nutzer ist außerdem an einer Karte auf des Boards angemeldet.
            List list = Create.List().OnBoard(board);
            Card card = Create.Card().OnList(list).WithAssignedUsers(user);

            //When: Das Team vom Board entfernt wird
            new BoardService(null, null, null, null, null).RemoveTeam(board, team);

            //Then: Muss der Nutzer weiterhin Mitglied und Nutzer des Boards sein.
            board.Members.Should().Contain(user);
            board.GetBoardUsers().Should().Contain(user);
        }
    }
}