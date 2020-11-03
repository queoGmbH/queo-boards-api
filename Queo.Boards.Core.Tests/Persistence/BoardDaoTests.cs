using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Persistence.Impl;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Persistence {
    [TestClass]
    public class BoardDaoTests : PersistenceBaseTest {

        public BoardDao BoardDao { get; set; }

        public TeamDao TeamDao { get; set; }

        /// <summary>
        ///     Testet, dass beim Suchen der Boards für einen Nutzer, ein Board gefunden wird, zu dessen Mitgliedern der Nutzer
        ///     gehört
        /// </summary>
        [TestMethod]
        public void TestFindBoardForUserAsMember() {
            //Given: Ein Nutzer der Mitglied eines eingeschränkten Boards ist 
            User user = Create.User();
            Board boardWithUserAsMember = Create.Board().Restricted().WithMembers(user).Build();

            //When: Die Boards für den Nutzer gesucht werden sollen 
            IPage<Board> foundBoards = BoardDao.FindBoardsForUser(PageRequest.All, user);

            //Then: Muss das Board gefunden werden.
            foundBoards.Should().Contain(boardWithUserAsMember);
        }

        /// <summary>
        ///     Testet, dass beim Suchen der Boards für einen Nutzer, ein Board gefunden wird, zu dessen Team-Mitgliedern der Nutzer
        ///     gehört
        /// </summary>
        [TestMethod]
        public void TestFindBoardForUserAsTeamMember() {
            //Given: Ein Nutzer der Mitglied eines eingeschränkten Boards ist, da er Mitglied eines Teams ist. 
            User user = Create.User();
            Team team = Create.Team().WithMembers(user);
            
            Board boardWithUserAsTeamMember = Create.Board().Restricted().WithTeams(team).Build();

            //When: Die Boards für den Nutzer gesucht werden sollen 
            IPage<Board> foundBoards = BoardDao.FindBoardsForUser(PageRequest.All, user);

            //Then: Muss das Board gefunden werden.
            foundBoards.Should().Contain(boardWithUserAsTeamMember);
        }

        /// <summary>
        ///     Testet, dass beim Suchen der Boards für einen Nutzer, ein Board gefunden wird, dessen Eigentümer der Nutzer ist
        /// </summary>
        [TestMethod]
        public void TestFindBoardForUserOwner() {
            //Given: Ein Nutzer der Eigentümer eines Boards ist 
            User user = Create.User();
            Board boardWithUserAsOwner = Create.Board().Restricted().WithOwners(user).Build();

            //When: Die Boards für den Nutzer gesucht werden sollen 
            IPage<Board> foundBoards = BoardDao.FindBoardsForUser(PageRequest.All, user);

            //Then: Muss das Board gefunden werden.
            foundBoards.Should().Contain(boardWithUserAsOwner);
        }

        /// <summary>
        ///     Testet, dass beim Suchen der Boards für einen Nutzer, keine Board-Vorlagen gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindBoardForUserShouldNotFindTemplates() {
            //Given: Ein Nutzer der Eigentümer einer Vorlage ist 
            User user = Create.User();
            Board boardTemplateWithUserAsOwner = Create.Board().WithOwners(user).AsTemplate().Build();

            //When: Die Boards für den Nutzer gesucht werden sollen 
            IPage<Board> foundBoards = BoardDao.FindBoardsForUser(PageRequest.All, user);

            //Then: Darf die Vorlage NICHT gefunden werden.
            foundBoards.Should().NotContain(boardTemplateWithUserAsOwner);
        }

        /// <summary>
        ///     Testet, dass beim Suchen der Boards für einen Nutzer, ein Board gefunden wird, das öffentlich zugänglich ist
        /// </summary>
        [TestMethod]
        public void TestFindBoardForUserWhenPublic() {
            //Given: Ein Nutzer der Mitglied eines Boards ist 
            User user = Create.User();
            Board publicBoard = Create.Board().Public().Build();

            //When: Die Boards für den Nutzer gesucht werden sollen 
            IPage<Board> foundBoards = BoardDao.FindBoardsForUser(PageRequest.All, user);

            //Then: Muss das Board gefunden werden.
            foundBoards.Should().Contain(publicBoard);
        }

        /// <summary>
        /// Ein eingeschränktes Board, dem ein Team zugeordnet ist, welchem der Nutzer nicht zugeordnet ist.
        /// </summary>
        [TestMethod]
        public void TestFindBoardForUserShouldNotFindRestrictedBoards()
        {

            //Given: Ein Team mit Nutzern, das einem restricted Board zugeordnet ist
            User otherUserInTeam = Create.User();
            Team assignedTeam = Create.Team().WithMembers(otherUserInTeam);
            Board boardWithOtherTeam = Create.Board().Restricted().WithTeams(assignedTeam);

            User user = Create.User();
            Team unassignedTeam = Create.Team().WithMembers(user);

            //When: Für einen Nutzer der nicht Mitglied des Teams ist und auch anderweitig nicht dem Board zugewiesen ist nach Boards gesucht wird
            IPage<Board> foundBoards = BoardDao.FindBoardsForUser(PageRequest.All, user);

            //Then: Darf das Board NICHT gefunden werden.
            foundBoards.Should().NotContain(boardWithOtherTeam);

        }

        /// <summary>
        ///     Testet das Suchen nach einem Board, anhand einer Zeichenfolge, die sich in der Mitte des Namens befindet, sich
        ///     allerdings in Groß- und Kleinschreibung unterscheidet.
        /// </summary>
        [TestMethod]
        public void TestFindPublicBoardByWithContainingUpperAndLower() {
            //Given: Ein öffentliches Board, welches die Suchzeichenfolge in der Mitte des Namens enthält, allerdings mit unterschiedlicher Groß- und Kleinschreibung
            User user = Create.User();
            const string SEARCH_TERM = "erfiNDER";
            Board board = Create.Board().Public().WithTitle("Ich bin der Erfinder der Board-Suche").Build();
            Board boardWithoutSearchTerm = Create.Board().Public().WithTitle("Anderer Titel").Build();

            //When: Nach Boards anhand der Suchzeichenfolge gesucht wird
            IPage<Board> found = BoardDao.FindBoardsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss das Board gefunden werden.
            found.Should().Contain(board);
            found.Should().NotContain(boardWithoutSearchTerm);
        }

        /// <summary>
        ///     Testet das Suchen nach einem Board, anhand einer Zeichenfolge, die sich in der Mitte des Namens befindet.
        /// </summary>
        [TestMethod]
        public void TestFindPublicBoardByWithNameContaining() {
            //Given: Ein öffentliches Board, welches die Suchzeichenfolge in der Mitte des Namens enthält
            User user = Create.User();
            const string SEARCH_TERM = "finde";
            Board board = Create.Board().Public().WithTitle("Wer mich " + SEARCH_TERM + "t, darf mich behalten").Build();
            Board boardWithoutSearchTerm = Create.Board().Public().WithTitle("Anderer Titel").Build();

            //When: Nach Boards anhand der Suchzeichenfolge gesucht wird
            IPage<Board> found = BoardDao.FindBoardsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss das Board gefunden werden.
            found.Should().Contain(board);
            found.Should().NotContain(boardWithoutSearchTerm);
        }

        /// <summary>
        ///     Testet das Suchen nach einem Board, anhand einer Zeichenfolge, die sich in der Mitte des Namens befindet, wenn die
        ///     Suche vorne und hinten Leerzeichen enthält.
        /// </summary>
        [TestMethod]
        public void TestFindPublicBoardByWithNameContainingTrim() {
            //Given: Ein öffentliches Board, welches die Suchzeichenfolge ohne die Leerzeichen in der Mitte des Namens enthält
            User user = Create.User();
            const string SEARCH_TERM = " finde ";
            Board board = Create.Board().Public().WithTitle("Erfinder der Board-Suche").Build();
            Board boardWithoutSearchTerm = Create.Board().Public().WithTitle("Anderer Titel").Build();

            //When: Nach Boards anhand der Suchzeichenfolge gesucht wird
            IPage<Board> found = BoardDao.FindBoardsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss das Board gefunden werden.
            found.Should().Contain(board);
            found.Should().NotContain(boardWithoutSearchTerm);
        }

        /// <summary>
        ///     Testet das Suchen nach einem Board, anhand einer Zeichenfolge, die sich am Ende des Namens befindet.
        /// </summary>
        [TestMethod]
        public void TestFindPublicBoardByWithNameEnding() {
            //Given: Ein öffentliches Board, welches die Suchzeichenfolge am Ende des Namens enthält
            User user = Create.User();
            const string SEARCH_TERM = "finde";
            Board board = Create.Board().Public().WithTitle("Ich wette, dass ich dich " + SEARCH_TERM).Build();
            Board boardWithoutSearchTerm = Create.Board().Public().WithTitle("Anderer Titel").Build();

            //When: Nach Boards anhand der Suchzeichenfolge gesucht wird
            IPage<Board> found = BoardDao.FindBoardsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss das Board gefunden werden.
            found.Should().Contain(board);
            found.Should().NotContain(boardWithoutSearchTerm);
        }

        /// <summary>
        ///     Testet das Suchen nach einem Board, anhand einer Zeichenfolge, die sich am Anfang des Board-Titels befindet.
        /// </summary>
        [TestMethod]
        public void TestFindPublicBoardWithNameStarting() {
            //Given: Ein öffentliches Board, welches die Suchzeichenfolge am Anfang des Namens enthält
            User user = Create.User();
            const string SEARCH_TERM = "Finde";
            Board board = Create.Board().Public().WithTitle(SEARCH_TERM + " mich").Build();
            Board boardWithoutSearchTerm = Create.Board().Public().WithTitle("Anderer Titel").Build();

            //When: Nach Boards anhand der Suchzeichenfolge gesucht wird
            IPage<Board> found = BoardDao.FindBoardsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss das Board gefunden werden.
            found.Should().Contain(board);
            found.Should().NotContain(boardWithoutSearchTerm);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestGetBoardWithLists() {
            // Given: 
            List list = Create.List().Build();

            // When: 
            Board board = BoardDao.Get(list.Board.Id);

            // Then: 
            Assert.AreEqual(1, board.Lists.Count);
            Assert.AreEqual(list, board.Lists[0]);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestSaveGet() {
            // Given: 
            User creator = Create.User().Build();
            Board board = Create.Board().ArchivedAt(DateTime.UtcNow).Creator(creator).Build();

            // When: 
            Board reloaded = BoardDao.Get(board.Id);

            // Then: 
            Assert.AreEqual(board, reloaded);
            Assert.IsTrue(reloaded.IsArchived);
            reloaded.IsTemplate.Should().BeFalse();
            reloaded.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
        }



        /// <summary>
        /// Testet das beim Abrufen von archivierten Board keine nicht archivierten Boards mitgeliefert werden
        /// </summary>
        [TestMethod]
        public void TestFindArchivedBoardsShouldNotReturnUnarchivedBoards() {

            //Given: Ein archiviertes und ein nicht archiviertes Board
            User user = Create.User();
            Board archivedBoard = Create.Board().ArchivedAt(DateTime.UtcNow).WithOwners(user).Build();
            Board notArchivedBoard = Create.Board().WithOwners(user).Build();

            //When: alle archivierten Boards abgerufen werden sollen
            IPage<Board> foundBoards = BoardDao.FindArchivedBoards(PageRequest.All, user);

            //Then: Darf das nicht archivierte Board nicht mit abgerufen werden
            foundBoards.Should().Contain(archivedBoard);
            foundBoards.Should().NotContain(notArchivedBoard);
        }

        /// <summary>
        /// Testet das beim Abrufen von archivierten Board keine nicht archivierten Boards mitgeliefert werden
        /// </summary>
        [TestMethod]
        public void TestFindArchivedBoardsShouldNotReturnTemplates() {

            //Given: Ein archiviertes Board, eine archivierte Board-Vorlage und eine nicht archivierte Board-Vorlage
            User user = Create.User();
            Board archivedBoard = Create.Board().ArchivedAt(DateTime.UtcNow).WithOwners(user).Build();
            Board template = Create.Board().AsTemplate().WithOwners(user).Build();
            Board archivedTemplate = Create.Board().ArchivedAt(DateTime.UtcNow).AsTemplate().WithOwners(user).Build();

            //When: alle archivierten Boards abgerufen werden sollen
            IPage<Board> foundBoards = BoardDao.FindArchivedBoards(PageRequest.All, user);

            //Then: Darf die Board-Vorlage nicht mit abgerufen werden
            foundBoards.Should().Contain(archivedBoard);
            foundBoards.Should().NotContain(template);
            foundBoards.Should().NotContain(archivedTemplate);
        }


        /// <summary>
        /// Testet das beim Abrufen von archivierten Board das Ergebnis nach Archivierungsdatum sortiert ist.
        /// </summary>
        [TestMethod]
        public void TestFindArchivedBoardsShouldReturnArchivedBoardsOrderedByArchivedAtDesc() {

            //Given: Ein archiviertes und ein nicht archiviertes Board
            User user = Create.User();
            Board archivedBoard20170810 = Create.Board().ArchivedAt(new DateTime(2017, 08, 10)).WithOwners(user).Build();
            Board archivedBoard20170809 = Create.Board().ArchivedAt(new DateTime(2017, 08, 09)).WithOwners(user).Build();
            Board archivedBoard20170811 = Create.Board().ArchivedAt(new DateTime(2017, 08, 11)).WithOwners(user).Build();

            //When: alle archivierten Boards abgerufen werden sollen
            IPage<Board> foundBoards = BoardDao.FindArchivedBoards(PageRequest.All, user);

            //Then: Darf das nicht archivierte Board nicht mit abgerufen werden
            //foundBoards.Should().BeInDescendingOrder(board => board.ArchivedAt);
            foundBoards.Should().ContainInOrder(archivedBoard20170811, archivedBoard20170810, archivedBoard20170809);
        }

        /// <summary>
        /// Testet, dass beim Suchen nach archivierten Boards, keine Boards gefunden werden, bei denen der Nutzer (ohne Admin-Recht) nicht Besitzer ist, auch wenn sie öffentlich sind.
        /// </summary>
        [TestMethod]
        public void TestFindArchivedBoardsForUserShouldOnlyFindBoardsWhereUserWithoutAdminRoleIsOwner() {

            //Given: Ein archiviertes Board, bei dem ein Nutzer ohne Admin-Rechte kein Besitzer ist
            User userWithoutAdminRole = Create.User().WithRoles(UserRole.USER);

            Board archivedBoardWhereUserisOwner = Create.Board().Public().ArchivedAt(DateTime.UtcNow).WithOwners(userWithoutAdminRole).Build();
            Board archivedBoardWhereUserisMember = Create.Board().Public().ArchivedAt(DateTime.UtcNow).WithMembers(userWithoutAdminRole).Build();
            Board archivedBoardWhereUserisNothing = Create.Board().Public().ArchivedAt(DateTime.UtcNow).Build();

            //When: Archivierte Boards für den Nutzer abgerufen werden
            IPage<Board> foundBoards = BoardDao.FindArchivedBoards(PageRequest.All, userWithoutAdminRole);

            //Then: Dürfen keine Boards gefunden werden, bei denen der Nutzer kein Besitzer ist.
            foundBoards.Should().Contain(archivedBoardWhereUserisOwner);
            foundBoards.Should().NotContain(archivedBoardWhereUserisMember);
            foundBoards.Should().NotContain(archivedBoardWhereUserisNothing);
        }

        /// <summary>
        /// Testet, dass beim Suchen nach archivierten Boards, auch Boards gefunden werden, bei denen der Nutzer (ein Admin) nicht Besitzer ist, auch wenn sie restricted sind.
        /// </summary>
        [TestMethod]
        public void TestFindArchivedBoardsForUserShouldAlsoFindBoardsWhereUserWithAdminRoleIsNoOwner() {

            //Given: Ein Admin Nutzer
            User adminUser = Create.User().WithRoles(UserRole.ADMINISTRATOR);
            
            //Given: Ein Board bei dem der Nutzer Owner ist
            Board archivedBoardWhereUserisOwner = Create.Board().Restricted().ArchivedAt(DateTime.UtcNow).WithOwners(adminUser).Build();
            //Given: Ein Board bei dem der Nutzer Member ist
            Board archivedBoardWhereUserisMember = Create.Board().Restricted().ArchivedAt(DateTime.UtcNow).WithMembers(adminUser).Build();

            //Given: Ein Board bei dem der Nutzer nichts ist
            Board archivedBoardWhereUserisNothing = Create.Board().Restricted().ArchivedAt(DateTime.UtcNow).Build();

            //When: Archivierte Boards für den Nutzer abgerufen werden
            IPage<Board> foundBoards = BoardDao.FindArchivedBoards(PageRequest.All, adminUser);

            //Then: Müssen alle archivierten Boards gefunden werden, unabhängig davon, ob der Nutzer Admin ist oder nicht.
            foundBoards.Should().Contain(archivedBoardWhereUserisOwner);
            foundBoards.Should().Contain(archivedBoardWhereUserisMember);
            foundBoards.Should().Contain(archivedBoardWhereUserisNothing);
        }

        /// <summary>
        /// Testet das Speichern und Laden eines Boards, das als Vorlage markiert ist
        /// </summary>
        [TestMethod]
        public void TestSaveAndLoadTemplate() {

            //Given: Ein Board, das als Vorlage markiert ist
            Board board = new Board(new BoardDto("Board-Vorlage", Accessibility.Restricted, "Vorlage"), new EntityCreatedDto(Create.User(), DateTime.UtcNow), true);
            
            //When: Das Board gespeichert und anschließend wieder geladen wird
            BoardDao.Save(board);
            BoardDao.FlushAndClear();
            Board reloadedTemplate = BoardDao.GetByBusinessId(board.BusinessId);

            //Then: Muss es korrekt als Template markiert sein.
            reloadedTemplate.IsTemplate.Should().BeTrue();

        }

        /// <summary>
        /// Testet das beim Speichern des Board neu zugeordnete Teams gespeichert werden
        /// </summary>
        [TestMethod]
        public void TestSaveBoardShouldCascadeTeams() {

            //Given: Ein Board, dem keine Teams zugewiesen sind
            Board board = Create.Board();
            Team team1 = Create.Team();
            Team team2 = Create.Team();

            //When: Dem Board Teams zugewiesen werden und das Board gespeichert wird
            board.AddTeam(team1);
            board.AddTeam(team2);
            BoardDao.Save(board);
            BoardDao.FlushAndClear();
            Board reloaded = BoardDao.Get(board.Id);

            //Then: Müssen die Teams nach dem Neuladen des Boards diesem weiterhin zugeordnet sein
            reloaded.Teams.Should().BeEquivalentTo(team1, team2);

        }


        /// <summary>
        /// Testet das beim Speichern des Boards, die nicht mehr zugewiesenen Teams vom Board entfernt werden
        /// </summary>
        [TestMethod]
        public void TestSaveBoardShouldRemoveTeams() {

            //Given: Ein Board, dem zwei Teams zugewiesen sind
            Team team1 = Create.Team();
            Team team2 = Create.Team();
            Board boardWithTeams = Create.Board().WithTeams(team1, team2);

            //When: Dem Board Teams entfernt werden und das Board gespeichert wird
            boardWithTeams.RemoveTeam(team1);
            BoardDao.Save(boardWithTeams);
            BoardDao.FlushAndClear();
            Board reloaded = BoardDao.Get(boardWithTeams.Id);

            //Then: Müssen das gelöschte Team nach dem Neuladen vom Board entfernt werden und das andere Team weiterhin zugeordnet sein.
            reloaded.Teams.Should().BeEquivalentTo(team2);

            //Then: Darüber hinaus, darf das vom Board entfernte Team nicht komplett gelöscht werden
            TeamDao.GetByBusinessId(team1.BusinessId).Should().Be(team1);

        }

        /// <summary>
        /// Testet das Suchen nach Boards denen ein Team zugewiesen ist
        /// </summary>
        [TestMethod]
        public void TestFindBoardsWithTeam() {

            //Given: Mehrere Boards und ein Team, welches einigen der Boards zugewiesen ist
            Team team = Create.Team();

            Board boardWithTeam = Create.Board().WithTeams(team);
            Board archivedBoardWithTeam = Create.Board().WithTeams(team);
            Board boardWithoutTeams = Create.Board();
            Board boardWithOtherTeam = Create.Board().WithTeams(Create.Team());

            //When: Nach Boards für das Team gesucht wird
            IPage<Board> foundBoards = BoardDao.FindBoardsWithTeam(PageRequest.All, team);

            //Then: Dürfen nur die Boards gefunden werden, denen das Team zugewiesen ist.
            foundBoards.Should().BeEquivalentTo(boardWithTeam, archivedBoardWithTeam);

        }

        /// <summary>
        /// Testet, dass ein Administrator bei der Suche nach Boards alle Boards angezeigt bekommt, unabhängig davon, ob er zugewiesen ist oder nicht
        /// </summary>
        [TestMethod]
        public void TestFindBoardForUserShouldReturnAllBoardWhenSearchingAndUserIsAdmin() {
            //Given: Ein Nutzer in der Rolle Admin 
            User userWithAdminRole = Create.User().WithRoles(UserRole.ADMINISTRATOR);

            //Given: Boards, denen der Nutzer nicht zugewiesen ist
            Board publicBoard = Create.Board().Public().WithTitle("Öffentlich auch für Admins");
            Board restrictedBoard = Create.Board().Restricted().WithTitle("Öffentlich auch für Admins");

            //When: Boards für den Nutzer abgerufen werden
            IPage<Board> foundBoards = BoardDao.FindBoardsForUser(PageRequest.All, userWithAdminRole, "Admins");

            //Then: Müssen alle Boards kommen
            foundBoards.Should().Contain(new[] { publicBoard, restrictedBoard });
        }
    }
}