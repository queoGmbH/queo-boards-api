using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Controllers;
using Queo.Boards.Core.Domain;
using Queo.Boards.Infrastructure.Filter;

namespace Queo.Boards.Tests.Security {
    [TestClass]
    public class BoardScopeAuthorizationFilterTests : HttpContextBaseTest {
        /// <summary>
        /// Testet die Berechtigung beim Verschieben einer Liste auf ein Board, bei dem der Nutzer Mitglied ist.
        /// </summary>
        [TestMethod]
        public void TestMoveListToBoardWhereUserIsMember() {
            /* Given: Eine Liste und ein Board bei dem der Nutzer Mitglied ist */

            User user = Create.User();
            Board targetBoard = Create.Board().WithMembers(user);
            HttpActionContext actionContext = CreateActionContext<MoveController>(HttpMethod.Post, new Uri("http://localhost/boards/"+targetBoard.BusinessId+"/movedlists"), user);

            actionContext.ActionArguments.Add("board", targetBoard);
            
            /* When: Die Berechtigung geprüft wird */
            new BoardScopeAuthorizationFilterAttribute().OnActionExecuting(actionContext);

            /* Then: Muss der Nutzer berechtigt sein */
            actionContext.Response.Should().BeNull();
        }

        /// <summary>
        /// Testet die Berechtigung beim Kopieren einer Liste auf ein Board, bei dem der Nutzer Mitglied ist.
        /// </summary>
        [TestMethod]
        public void TestCopyListToBoardWhereUserIsMember() {
            /* Given: Eine Liste und ein Board bei dem der Nutzer Mitglied ist */
            User user = Create.User();
            Board targetBoard = Create.Board().WithMembers(user);
            HttpActionContext actionContext = CreateActionContext<CopyController>(HttpMethod.Post, new Uri("http://localhost/boards/" + targetBoard.BusinessId + "/copiedlists"), user);
            actionContext.ActionArguments.Add("board", targetBoard);

            /* When: Die Berechtigung geprüft wird */
            new BoardScopeAuthorizationFilterAttribute().OnActionExecuting(actionContext);

            /* Then: Muss der Nutzer berechtigt sein */
            actionContext.Response.Should().BeNull();
        }


        /// <summary>
        /// Testet die Berechtigung beim Wiederherstellen einer Karte auf einem Board, bei dem der Nutzer Mitglied ist.
        /// </summary>
        [TestMethod]
        public void TestRestoreCard() {
            /* Given: Eine Liste und ein Board bei dem der Nutzer Mitglied ist */
            User user = Create.User();
            Board board = Create.Board().WithMembers(user);
            List list = Create.List().OnBoard(board);
            Card cardToRestore = Create.Card().OnList(list).ArchivedAt(DateTime.UtcNow);
            HttpActionContext actionContext = CreateActionContext<CardController>(HttpMethod.Put, new Uri("http://localhost/cards/"+cardToRestore.BusinessId+"/isArchived"), user);
            actionContext.ActionArguments.Add("card", cardToRestore);

            /* When: Die Berechtigung geprüft wird */
            new BoardScopeAuthorizationFilterAttribute().OnActionExecuting(actionContext);

            /* Then: Muss der Nutzer berechtigt sein */
            actionContext.Response.Should().BeNull();
        }


        /// <summary>
        /// Testet die Berechtigung beim Wiederherstellen einer Karte auf einem Board, bei dem der Nutzer Mitglied ist.
        /// </summary>
        [TestMethod]
        public void TestRestoreList() {
            /* Given: Eine Liste und ein Board bei dem der Nutzer Mitglied ist */
            User user = Create.User();
            Board board = Create.Board().WithMembers(user);
            List listToRestore = Create.List().OnBoard(board).ArchivedAt(DateTime.UtcNow);
            HttpActionContext actionContext = CreateActionContext<ListController>(HttpMethod.Put, new Uri("http://localhost/lists/" + listToRestore.BusinessId + "/isArchived"), user);
            actionContext.ActionArguments.Add("list", listToRestore);

            /* When: Die Berechtigung geprüft wird */
            new BoardScopeAuthorizationFilterAttribute().OnActionExecuting(actionContext);

            /* Then: Muss der Nutzer berechtigt sein */
            actionContext.Response.Should().BeNull();
        }
    }
}