using System.Linq;
using System.Web.Http.Results;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Controllers;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Models;

namespace Queo.Boards.Tests.Controllers.IntegrationTests {

    [TestClass]
    public class SearchControllerIntegrationTest : ControllerBaseTest {

        public SearchController SearchController { get; set; }

        /// <summary>
        /// Testet das Suchen anhand eines Suchbegriffs mit Umlaut 
        /// CWETMNDS-687
        /// </summary>
        [TestMethod]
        public void TestSearchWithUmlaut() {
            //Given: Ein Board, eine Karte und ein Kommentar, die alle einen Suchbegriff mit Umlaut enthalten
            Board board = Create.Board().WithTitle("Prüfung");
            Card card = Create.Card().WithTitle("Prüfung");
            Comment comment = Create.Comment().WithText("Das ist eine Prüfung der Suchfunktionalität");
            User adminUser = Create.User().WithRoles(UserRole.ADMINISTRATOR);

            //When: Die Suche durchgeführt wird
            OkNegotiatedContentResult<SearchResultModel> result = (OkNegotiatedContentResult<SearchResultModel>)SearchController.Search(adminUser, "Pr%C3%BCf");

            

            //Then: Muss in allen 3 Kategorien je ein Treffer geliefert werden.
            new[] { result }
                .All(r => r.Content.Boards.Count == 1 && r.Content.Cards.Count == 1 && r.Content.Comments.Count == 1).Should().BeTrue();

            new[] { result }
                .Should().OnlyContain(r => r.Content.Boards.Count == 1 && r.Content.Cards.Count == 1 && r.Content.Comments.Count == 1);
        }


    }
}