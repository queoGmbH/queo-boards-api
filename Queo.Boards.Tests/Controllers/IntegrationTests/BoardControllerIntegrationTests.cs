using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Controllers.Boards;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Persistence.Impl;
using Queo.Boards.Core.Tests;

namespace Queo.Boards.Tests.Controllers.IntegrationTests {
    [TestClass]
    public class BoardControllerIntegrationTests : ControllerBaseTest {
        public BoardController BoardController { get; set; }

        public BoardDao BoardDao { set; private get; }

        public BoardMembersController BoardMemberController { get; set; }

        /// <summary>
        ///     Testet das Hinzufügen eines Nutzers zu einem Board und das anschließende Laden der Boards für den Nutzer.
        ///     Jira: CWETMNDS-506
        ///     TODO: Ich weiß nicht, ob das ein guter Testfall ist, da der zwei Sachen nacheinander testet. Sollte mMn in zwei
        ///     Tests aufgeteilt werden.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestAddMemberToBoardAndLoadBoards() {
            //Given: Ein Nutzer
            User userWhoIsAlreadyMember = Create.User().WithUserName("member_of_all_boards");
            User user = Create.User().WithUserName("new_member");

            //Given: Zwei Restricted Boards, wobei der Nutzer Mitglied des einen Boars ist.
            Board boardWithUserAsMember = Create.Board().WithMembers(userWhoIsAlreadyMember, user).WithTitle("Board mit bereits 2 Nutzern").Public().Build();
            Board boardToGetNewMember = Create.Board().WithMembers(userWhoIsAlreadyMember).WithTitle("Board mit Anfangs nur einem Nutzer").Public().Build();

            //When: Der Nutzer zu dem zweiten Board hinzugefügt wird
            OkNegotiatedContentResult<BoardModel> result = (OkNegotiatedContentResult<BoardModel>)BoardMemberController.AddBoardMember(boardToGetNewMember, new EntityFromBody<User>(user), Create.User());

            //Then: Müssen die nun 2 Mitglieder des Boards geliefert werden.
            result.Content.Members.Should().HaveCount(2);
            BoardDao.FlushAndClear();

            //When: Anschließend die Boards für den Nutzer abgerufen werden
            OkNegotiatedContentResult<IList<BoardSummaryModel>> boardSummeriesForUser = (OkNegotiatedContentResult<IList<BoardSummaryModel>>)BoardController.FindBoardsForUser(user);

            //Then: Dürfen beim Abrufen der Boards für den Nutzer nur die zwei Boards geliefert werden und es darf auch kein neues Board angelegt werden.
            boardSummeriesForUser.Content.Should().HaveCount(2);
        }

        /// <summary>
        ///     Testet das Abrufen von Boards für einen Nutzer
        ///     Jira: CWETMNDS-508
        /// </summary>
        [TestMethod]
        public void TestFindBoardsForUser() {
            //Given: Boards verschiedener Arten und ein Nutzer
            User user = Create.User();

            //Given: Öffentliches Board
            Board publicBoard = Create.Board().WithTitle("Öffentliches Board").Public().Build();

            //Given: Restricted Board eines anderen Nutzers
            Board restrictedBoardOfOtherUser = Create.Board().WithTitle("Eingeschränktes Board").Restricted().Build();

            //Given: Restricted Board des Nutzers als Owner
            Board restrictedBoardOfUserAsOwner = Create.Board().WithTitle("Eingeschränktes Board mit Nutzer als Owner").Restricted().WithOwners(user).Build();

            //Given: Restricted Board des Nutzers als Member
            Board restrictedBoardOfUserAsMember = Create.Board().WithTitle("Eingeschränktes Board mit Nutzer als Member").Restricted().WithMembers(user).Build();

            //Given: archiviertes öffentliches Board
            Board archivedPublicBoard = Create.Board().WithTitle("archiviertes öffentliches Board").Public().ArchivedAt(DateTime.UtcNow).Build();

            //Given: archiviertes restricted Board eines anderen Nutzers
            Board archivedRestrictedBoardOfOtherUser = Create.Board().WithTitle("archiviertes, eingeschränktes Board").Restricted().ArchivedAt(DateTime.UtcNow).Build();

            //Given: archiviertes restricted Board des Nutzers als Owner
            Board archivedRestrictedBoardOfUserAsOwner = Create.Board().WithTitle("Archiviertes Eingeschränktes Board mit Nutzer als Owner").Restricted().WithOwners(user).ArchivedAt(DateTime.UtcNow).Build();

            //Given: archiviertes restricted Board des Nutzers als Member
            Board archivedRestrictedBoardOfUserAsMember = Create.Board().WithTitle("Archiviertes Eingeschränktes Board mit Nutzer als Member").Restricted().WithMembers(user).ArchivedAt(DateTime.UtcNow).Build();

            //When: Boards für den Nutzer abgerufen werden
            OkNegotiatedContentResult<IList<BoardSummaryModel>> result = (OkNegotiatedContentResult<IList<BoardSummaryModel>>)BoardController.FindBoardsForUser(user);

            //Then: Dürfen nur die für den Nutzer zugänglichen Boards geliefert werden.
            result.Content.Select(model => model.BusinessId).Should().BeEquivalentTo(new[] { publicBoard.BusinessId, restrictedBoardOfUserAsMember.BusinessId, restrictedBoardOfUserAsOwner.BusinessId });
        }
    }
}