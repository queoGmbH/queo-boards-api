using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Services.Impl;
using Queo.Boards.Core.Tests.Infrastructure;
using Queo.Boards.Core.Validators.Teams;

namespace Queo.Boards.Core.Tests.Services {
    [TestClass]
    public class TeamServiceTest : ServiceBaseTest {
        /// <summary>
        ///     Testet das Erstellen eines Teams
        /// </summary>
        [TestMethod]
        public void TestCreate() {
            //Given: Daten für ein neues Team

            User createdBy = Create.User();
            DateTime createdAt = new DateTime(2015, 05, 06, 07, 08, 09, DateTimeKind.Utc);
            EntityCreatedDto entityCreatedDto = new EntityCreatedDto(createdBy, createdAt);
            string teamname = "Team zum Testen der Create-Methode";
            string description = "Beschreibung des Teams zum Testen der Create-Methode.";
            IList<User> members = new List<User> {
                Create.User(),
                Create.User()
            };

            Mock<ITeamDao> teamDaoMock = new Mock<ITeamDao>();
            teamDaoMock.Setup(d => d.Save(It.IsAny<Team>()));
            
            TeamService teamService = new TeamService(teamDaoMock.Object, null, null, new TeamNameValidator());

            //When: Das Team erstellt werden soll
            Team createdTeam = teamService.Create(new TeamDto(teamname, description), members, entityCreatedDto);

            //Then: Muss ein neues Team geliefert werden, welches über den Dao gespeichert wurde
            teamDaoMock.Verify(d => d.Save(createdTeam), Times.Once());

            createdTeam.Name.Should().Be(teamname);
            createdTeam.Description.Should().Be(description);
            createdTeam.Members.Should().BeEquivalentTo(members);
            createdTeam.CreatedBy.Should().Be(createdBy);
            createdTeam.CreatedAt.Should().Be(createdAt);
        }

        /// <summary>
        ///     Testet das Erstellen eines Teams, mit der maximal erlaubten Länge für den Teamnamen
        /// </summary>
        [TestMethod]
        public void TestCreateTeamWithMaxLength() {
            //Given: Ein Teamname der die maximale Länge besitzt
            string longName = new string('t', TeamNameValidator.MAX_LENGTH);
            Mock<ITeamDao> teamDaoMock = new Mock<ITeamDao>();
            teamDaoMock.Setup(d => d.Save(It.Is<Team>(t => t.Name == longName)));
            TeamService teamService = new TeamService(teamDaoMock.Object, null, null, new TeamNameValidator());

            //When: Ein Team mit diesem Namen erstellt werden soll
            Team createdTeam = teamService.Create(new TeamDto(longName, ""), new List<User>(), new EntityCreatedDto(Create.User(), DateTime.UtcNow));

            //Then: Muss das funktionieren.
            teamDaoMock.Verify(d => d.Save(createdTeam), Times.Once());
        }

        /// <summary>
        ///     Testet, dass wenn ein Team mit einem Namen länger als <see cref="TeamNameValidator.MAX_LENGTH" /> Zeichen erstellt
        ///     werden soll, eine Exception geworfen wird
        /// </summary>
        [TestMethod]
        public void TestCreateTeamWithNameLongerThan75CharsShouldThrowArgumentOutOfRangeException() {
            //Given: Ein Teamname, der 1 Zeichen länger als die zugelassene Länge ist  

            Mock<ITeamDao> teamDaoMock = new Mock<ITeamDao>();
            teamDaoMock.Setup(d => d.Save(It.IsAny<Team>()));
            TeamService teamService = new TeamService(teamDaoMock.Object, null, null, new TeamNameValidator());
            string toLongName = new string('t', TeamNameValidator.MAX_LENGTH + 1);

            //When: Ein Team mit zu langem Namen erstellt werden soll
            Action action = () => teamService.Create(new TeamDto(toLongName, ""), new List<User>(), new EntityCreatedDto(Create.User(), DateTime.UtcNow));

            //Then: Muss eine Exception geworfen werden und die Dao-Methode zum Speichern darf nicht aufgerufen werden
            action.ShouldThrow<ArgumentOutOfRangeException>();
            teamDaoMock.Verify(d => d.Save(It.IsAny<Team>()), Times.Never());
        }


        /// <summary>
        /// Testet das Ändern eines Teams
        /// </summary>
        [TestMethod]
        public void TestUpdateTeam() {

            //Given: Ein vorhandenes Team
            Team team = Create.Team();
            TeamService teamService = new TeamService(null, null, null, new TeamNameValidator());
            const string NEW_NAME = "Neuer Name des Teams";
            string NEW_DESCRIPTION = "Neue Beschreibung des Teams";

            //When: Das Team geändert werden soll
            teamService.Update(team, new TeamDto(NEW_NAME, NEW_DESCRIPTION));

            //Then: Müssen alle Änderungen übernommen werden
            team.Name.Should().Be(NEW_NAME);
        }


        /// <summary>
        /// Testet das Ändern eines Teams, auf einen sehr langen Namen
        /// </summary>
        [TestMethod]
        public void TestUpdateTeamToLongName() {

            //Given: Ein vorhandenes Team und ein Name mit maximal zulässiger Länge.
            Team team = Create.Team();
            TeamService teamService = new TeamService(null, null, null, new TeamNameValidator());
            string newName = new string('t', TeamNameValidator.MAX_LENGTH);

            //When: Das Team geändert werden soll
            teamService.Update(team, new TeamDto(newName, ""));

            //Then: Müssen alle Änderungen übernommen werden
            team.Name.Should().Be(newName);
        }


        /// <summary>
        /// Testet das beim Ändern eines Teams, eine Exception geworfen wird, wenn der Name des Teams zu lang ist. 
        /// </summary>
        [TestMethod]
        public void TestUpdateTeamWithTooLongNameShouldThrowArgumentOutOfRangeException() {

            //Given: Ein vorhandenes Team und ein zu langer neuer Name
            const string OLD_NAME = "Bisheriger Name";
            Team team = Create.Team().WithName(OLD_NAME);
            TeamService teamService = new TeamService(null, null, null, new TeamNameValidator());
            string newName = new string('t', TeamNameValidator.MAX_LENGTH + 1);

            //When: Das Team geändert werden soll
            Action action = () => teamService.Update(team, new TeamDto(newName, ""));

            //Then: Muss eine Exception geworfen werden und der Teamname erhalten bleiben.
            action.ShouldThrow<ArgumentOutOfRangeException>();
            team.Name.Should().Be(OLD_NAME);
        }


        /// <summary>
        /// Testet das Hinzufügen eines neuen Mitglieds
        /// </summary>
        [TestMethod]
        public void TestAddMember() {

            //Given: Ein Team ohne Mitglieder
            Team team = Create.Team();
            User user = Create.User();

            TeamService teamService = new TeamService(null, null, null, new TeamNameValidator());

            //When: Das erste Mitglied hinzugefügt wird
            IList<User> members = teamService.AddMembers(team, user);

            //Then: Muss es anschließend als Mitglied eingetragen sein
            members.Should().OnlyContain(mem => mem.Equals(user));
            team.Members.Should().OnlyContain(mem => mem.Equals(user));
        }



        /// <summary>
        /// Testet das Hinzufügen eines weiteren Mitglieds
        /// </summary>
        [TestMethod]
        public void TestAddAnotherMember() {

            //Given: Ein Team mit einem Mitglied
            User existingMember = Create.User();
            User newMember = Create.User();
            Team team = Create.Team().WithMembers(existingMember);
            
            TeamService teamService = new TeamService(null, null, null, new TeamNameValidator());

            //When: Ein weiteres Mitglied hinzugefügt wird
            IList<User> members = teamService.AddMembers(team, newMember);

            //Then: Muss das Team anschließend 2 Mitglieder haben
            members.Should().BeEquivalentTo(existingMember, newMember);
            team.Members.Should().BeEquivalentTo(existingMember, newMember);
        }


        /// <summary>
        /// Testet das Hinzufügen mehrerer Mitglieder
        /// </summary>
        [TestMethod]
        public void TestAddMultipleMembers() {

            //Given: Ein Team mit einem Mitglied
            User existingMember = Create.User();
            User newMember1 = Create.User();
            User newMember2 = Create.User();
            User newMember3 = Create.User();
            Team team = Create.Team().WithMembers(existingMember);

            TeamService teamService = new TeamService(null, null, null, new TeamNameValidator());

            //When: 3 Mitglieder hinzugefügt werden.
            IList<User> members = teamService.AddMembers(team, newMember1, newMember2, newMember3);

            //Then: Muss das Team 4 Mitglieder haben
            members.Should().BeEquivalentTo(existingMember, newMember1, newMember2, newMember3);
            team.Members.Should().BeEquivalentTo(existingMember, newMember1, newMember2, newMember3);
        }

        /// <summary>
        /// Testet das Hinzufügen eines Nutzers, der bereits Mitglied des Teams ist
        /// </summary>
        [TestMethod]
        public void TestAddMemberTwice() {

            //Given: Ein Team in dem ein Nutzer Mitglied ist
            User existingMember = Create.User();
            Team team = Create.Team().WithMembers(existingMember);

            TeamService teamService = new TeamService(null, null, null, new TeamNameValidator());

            //When: Der Nutzer erneut als Mitglied des Teams hinzugefügt werden soll
            IList<User> members = teamService.AddMembers(team, existingMember);

            //Then: Muss das Team unverändert bleiben und nur der eine Nutzer Mitglied sein
            members.Should().BeEquivalentTo(existingMember);
            team.Members.Should().BeEquivalentTo(existingMember);

        }


        /// <summary>
        /// Testet das Entfernen eines Mitglieds aus einem Team
        /// </summary>
        [TestMethod]
        public void TestRemoveMember() {

            //Given: Ein Team mit mehreren Mitgliedern
            User existingMember1 = Create.User();
            User existingMember2 = Create.User();
            User existingMember3 = Create.User();
            Team team = Create.Team().WithMembers(existingMember1, existingMember2, existingMember3);

            Mock<IBoardService> boardServiceMock = new Mock<IBoardService>();
            boardServiceMock.Setup(d => d.FindBoardsWithTeam(It.IsAny<PageRequest>(), team)).Returns(new Page<Board>(new List<Board>(), 0));

            TeamService teamService = new TeamService(null, boardServiceMock.Object, null, new TeamNameValidator());

            //When: Eines der Mitglieder aus dem Team entfernt werden soll
            IList<User> remainingMembers = teamService.RemoveMembers(team, existingMember2);

            //Then: Muss es aus der Liste der Mitglieder verschwinden und die anderen weiterhin Mitglieder bleiben
            remainingMembers.Should().BeEquivalentTo(existingMember1, existingMember3);
            team.Members.Should().BeEquivalentTo(existingMember1, existingMember3);

        }


        /// <summary>
        /// Testet das Entfernen mehrerer Mitglieder aus einem Team
        /// </summary>
        [TestMethod]
        public void TestRemoveMultipleMembers() {

            //Given: Ein Team mit mehreren Mitgliedern
            User existingMember1 = Create.User();
            User existingMember2 = Create.User();
            User existingMember3 = Create.User();
            Team team = Create.Team().WithMembers(existingMember1, existingMember2, existingMember3);

            Mock<IBoardService> boardServiceMock = new Mock<IBoardService>();
            boardServiceMock.Setup(d => d.FindBoardsWithTeam(It.IsAny<PageRequest>(), team)).Returns(new Page<Board>(new List<Board>(), 0));

            TeamService teamService = new TeamService(null, boardServiceMock.Object, null, new TeamNameValidator());

            //When: Mehrere Mitglieder aus dem Team entfernt werden soll
            IList<User> remainingMembers = teamService.RemoveMembers(team, existingMember2, existingMember1);

            //Then: Müssen sie aus der Liste der Mitglieder verschwinden und die anderen weiterhin Mitglieder bleiben
            remainingMembers.Should().BeEquivalentTo(existingMember3);
            team.Members.Should().BeEquivalentTo(existingMember3);
        }


        /// <summary>
        /// Testet das Entfernen des letzten Mitglieds eines Teams
        /// </summary>
        [TestMethod]
        public void TestRemoveLastMember() {

            //Given: Ein Team mit einem Mitglied
            User singleMember = Create.User();
            Team team = Create.Team().WithMembers(singleMember);

            Mock<IBoardService> boardServiceMock = new Mock<IBoardService>();
            boardServiceMock.Setup(d => d.FindBoardsWithTeam(It.IsAny<PageRequest>(), team)).Returns(new Page<Board>(new List<Board>(), 0));

            TeamService teamService = new TeamService(null, boardServiceMock.Object, null, new TeamNameValidator());

            //When: Das letzte Mitglied aus dem Team entfernt werden soll
            IList<User> remainingMembers = teamService.RemoveMembers(team, singleMember);

            //Then: Muss das funktionieren und die Liste der Mitglieder leer sein
            remainingMembers.Should().BeEmpty();
            team.Members.Should().BeEmpty();
        }


        /// <summary>
        /// Testet, dass ein Nutzer, von den Karten der Boards abgemeldet wird, bei denen er nur Mitglied war, weil er Team-Mitglied war 
        /// </summary>
        [TestMethod]
        public void TestRemoveTeamMemberShouldRemoveUserFromCardsOfBoardsHeIsNoLongerMember() {

            //Given: Ein Nutzer der Mitglied eines Teams ist
            User user = Create.User();
            Team team = Create.Team().WithMembers(user);

            //Given: Das Team ist an Boards hinterlegt, wodurch der Nutzer dort Mitglied ist
            Board board = Create.Board().WithTeams(team);

            //Given: Der Nutzer ist an Karten des Boards angemeldet.
            List list = Create.List().OnBoard(board);
            Card card1 = Create.Card().OnList(list).WithAssignedUsers(user);
            Card card2 = Create.Card().OnList(list).WithAssignedUsers(user);

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(s => s.UnassignUsers(card1, user));
            cardServiceMock.Setup(s => s.UnassignUsers(card2, user));

            Mock<IBoardService> boardServiceMock = new Mock<IBoardService>();
            boardServiceMock.Setup(d => d.FindBoardsWithTeam(It.IsAny<PageRequest>(), team)).Returns(new Page<Board>(new List<Board> {board}, 1));

            TeamService teamService = new TeamService(null, boardServiceMock.Object, cardServiceMock.Object, new TeamNameValidator());

            //When: Der Nutzer aus dem Team entfernt wird
            teamService.RemoveMembers(team, user);

            //Then: Muss er von allen Karten an denen er angemeldet war und die auf einem Board waren, bei denen er dann kein Mitglied mehr ist, abgemeldet werden
            cardServiceMock.Verify(s => s.UnassignUsers(card1, user), Times.Once);
            cardServiceMock.Verify(s => s.UnassignUsers(card2, user), Times.Once);

        }
    }
}