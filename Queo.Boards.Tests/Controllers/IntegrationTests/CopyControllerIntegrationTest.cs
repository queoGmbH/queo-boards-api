using System.Linq;
using System.Web.Http.Results;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Commands.Cards;
using Queo.Boards.Commands.Lists;
using Queo.Boards.Controllers;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Models;

namespace Queo.Boards.Tests.Controllers.IntegrationTests {

    [TestClass]
    public class CopyControllerIntegrationTest : ControllerBaseTest {

        public CopyController CopyController { get; set; }


        /// <summary>
        /// Testet das beim Kopieren einer Karte innerhalb eines Boards die Anzahl der Todos korrekt geliefert wird. 
        /// 
        /// Jira: CWETMNDS-495
        /// </summary>
        [TestMethod]
        public void TestCopyCardReturnsCorrectNumberOfTodos() {

            //Given: Eine Karte mit Todos
            Card card = Create.Card();
            Checklist checklist = Create.Checklist().OnCard(card).Build();
            Task openTask1 = Create.Task().OnChecklist(checklist).IsDone(false).Build();
            Task openTask2 = Create.Task().OnChecklist(checklist).IsDone(false).Build();
            Task openTask3 = Create.Task().OnChecklist(checklist).IsDone(false).Build();
            Task doneTask1 = Create.Task().OnChecklist(checklist).IsDone(true).Build();
            Task doneTask2 = Create.Task().OnChecklist(checklist).IsDone(true).Build();

            //When: Die Karte kopiert wird
            OkNegotiatedContentResult<BoardModel> result = (OkNegotiatedContentResult<BoardModel>)CopyController.CopyCard(card.List, Create.User(), new CopyCardCommand() { CopyName = "Kopierte Karte mit Todos", Source = card });

            //Then: Müssen die Count für die Todos stimmen
            result.Content.Cards.Should().HaveCount(2);
            result.Content.Cards.First().TasksDoneCount.Should().Be(2);
            result.Content.Cards.First().TasksOverallCount.Should().Be(5);
            result.Content.Cards.Skip(1).First().TasksDoneCount.Should().Be(2);
            result.Content.Cards.Skip(1).First().TasksOverallCount.Should().Be(5);


        }

        /// <summary>
        /// Testet das Kopieren einer Liste, auf der eine Karte mit Kommentaren enthalten ist.
        /// </summary>
        [TestMethod]
        public void TestCopyListThatHasOneCardWithComments() {

            //Given: Ein Liste mit einer Karte, zu der zwei Kommentare abgegeben wurden.
            List list = Create.List().Build();
            Card card = Create.Card().OnList(list);
            Comment comment1 = Create.Comment().OnCard(card).Build();
            Comment comment2 = Create.Comment().OnCard(card).Build();
            const string COPY_NAME = "Kopierte Liste";

            //When: Die Liste kopiert wird
            OkNegotiatedContentResult<BoardModel> result = (OkNegotiatedContentResult<BoardModel>)CopyController.CopyList(Create.User(), list.Board, new CopyListCommand() { CopyName = "Kopierte Liste", Source = list, InsertAfter = list});
            
            //Then: Müssen die Count für Kommentare an der Liste stimmen.
            result.Content.Lists.Last().Title.Should().Be(COPY_NAME, "Die Liste muss an der richtigen Position eingefügt sein.");
            result.Content.Cards.Last().List.BusinessId.Should().Be(result.Content.Lists.Last().BusinessId, "Es muss die richtige Karte sein die geprüft wird.");
            result.Content.Cards.Last().CommentCount.Should().Be(2);
        }
    }
}