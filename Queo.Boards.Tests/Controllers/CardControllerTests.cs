using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http.Results;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Controllers;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Tests;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Tests.Controllers {
    [TestClass]
    public class CardControllerTests : CreateBaseTest {
        /// <summary>
        ///     Testet das Zuweisen eines Nutzers der nicht dem Board auf welcher die Karte ist zugewiesen ist.
        /// </summary>
        [TestMethod]
        public void TestAssignUserNotAssignedToBoard() {
            //Given: Eine Karte und ein Nutzer, der nicht dem Board der Karte zugewiesen ist.
            User user = Create.User();
            Board board = Create.Board().Build();
            List list = Create.List().OnBoard(board).Build();
            Card card = Create.Card().OnList(list).Build();

            Mock<IUserService> userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetById(user.BusinessId)).Returns(user);

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(x => x.AssignUser(card, user)).Returns(new List<User> { user });
            CardController controller = new CardController(cardServiceMock.Object, null, null, null, userServiceMock.Object, null, null);

            //When: Der Nutzer der Karte zugewiesen werden soll
            ResponseMessageResult result = (ResponseMessageResult)controller.AssignUser(card, new GuidValueDto() { Value = user.BusinessId });

            //Then: Darf das nicht funktionieren und es muss ein HttpStatusCode = 409 (Conflict) geliefert werden
            result.Response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        /// <summary>
        ///     Testet das Zuweisen eines Nutzers zu einer Karte, wenn der Nutzer Board-Mitglied ist.
        /// </summary>
        [TestMethod]
        public void TestAssignUserWhoIsBoardMember() {
            //Given: Eine Karte und ein Nutzer, der Mitglied des Boards ist und dieser Karte bisher nicht zugewiesen ist,
            User user = Create.User();
            Board board = Create.Board().WithMembers(user).Build();
            List list = Create.List().OnBoard(board).Build();
            Card card = Create.Card().OnList(list);

            Mock<IUserService> userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetById(user.BusinessId)).Returns(user);

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(x => x.AssignUser(card, user)).Returns(new List<User> { user });
            CardController controller = new CardController(cardServiceMock.Object, null, null, null, userServiceMock.Object, null, null);

            //When: Der Nutzer der Karte zugewiesen werden soll
            //Then: Muss das korrekt funktionieren.
            OkNegotiatedContentResult<CardModel> result = (OkNegotiatedContentResult<CardModel>)controller.AssignUser(card, new GuidValueDto() { Value = user.BusinessId });

        }

        /// <summary>
        ///     Testet das Zuweisen eines Nutzers zu einer Karte.
        /// </summary>
        [TestMethod]
        public void TestAssignUserWhoIsBoardOwnerMember() {
            //Given: Eine Karte und ein Nutzer, der Besitzer des Boards ist und dieser Karte bisher nicht zugewiesen ist,
            User user = Create.User();
            Board board = Create.Board().WithOwners(user).Build();
            List list = Create.List().OnBoard(board).Build();
            Card card = Create.Card().OnList(list);

            Mock<IUserService> userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetById(user.BusinessId)).Returns(user);

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(x => x.AssignUser(card, user)).Returns(new List<User> { user });
            CardController controller = new CardController(cardServiceMock.Object, null, null, null, userServiceMock.Object, null, null);

            //When: Der Nutzer der Karte zugewiesen werden soll
            //Then: Muss das korrekt funktionieren.
            OkNegotiatedContentResult<CardModel> result = (OkNegotiatedContentResult<CardModel>)controller.AssignUser(card, new GuidValueDto() { Value = user.BusinessId });
        }

        /// <summary>
        ///     Testet das Zuweisen eines Nutzers zu einer Karte, wenn der Nutzer Mitglied eines Teams ist, welches dem Board
        ///     zugewiesen ist.
        /// </summary>
        [TestMethod]
        public void TestAssignUserWhoIsMemberOfBoardTeam() {
            //Given: Eine Karte und ein Nutzer, der Mitglied eines Teams ist, welches dem Board zugewiesen ist und dieser Karte bisher nicht zugewiesen ist,
            User user = Create.User();
            Team team = Create.Team().WithMembers(user);

            Board board = Create.Board().WithTeams(team).Build();
            List list = Create.List().OnBoard(board).Build();
            Card card = Create.Card().OnList(list);

            Mock<IUserService> userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetById(user.BusinessId)).Returns(user);

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(x => x.AssignUser(card, user)).Returns(new List<User> { user });
            CardController controller = new CardController(cardServiceMock.Object, null, null, null, userServiceMock.Object, null, null);

            //When: Der Nutzer der Karte zugewiesen werden soll
            //Then: Muss das korrekt funktionieren.
            OkNegotiatedContentResult<CardModel> result = (OkNegotiatedContentResult<CardModel>)controller.AssignUser(card, new GuidValueDto() { Value = user.BusinessId });
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateChecklist() {
            // Given: 
            Card card = Create.Card().Build();
            ChecklistCreateDto titleDto = new ChecklistCreateDto() { Title = "Checklist WithTitle" };
            Checklist checklist = Create.Checklist().Title(titleDto.Title).OnCard(card).Build();

            Mock<IChecklistService> checklistServiceMock = new Mock<IChecklistService>();
            checklistServiceMock.Setup(x => x.Create(card, titleDto.Title, null)).Returns(checklist);

            CardController controller = new CardController(null, checklistServiceMock.Object, null, null, null, null, null);

            // When: 
            OkNegotiatedContentResult<ChecklistModel> result = (OkNegotiatedContentResult<ChecklistModel>)controller.CreateChecklist(card, titleDto);

            // Then: 
            Assert.AreEqual(titleDto.Title, result.Content.Title);
            checklistServiceMock.Verify(x => x.Create(card, titleDto.Title, null), Times.Once);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestGetCard() {
            // Given: 
            Card card = Create.Card().Build();

            // When: 
            CardModel model =
            ((OkNegotiatedContentResult<CardModel>)
                new CardController(null, null, null, null, null, Create.CardUpdateDtoValidator(), Create.DueValidator()).Get(card)).Content;

            // Then: 
            Assert.AreEqual(model.BusinessId, card.BusinessId);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestSetDueInPastShouldReturnError() {
            // Given: 
            DateTime due = new DateTime(2016, 1, 1);
            DateTimeUpdateDto dto = new DateTimeUpdateDto() { Value = due };
            CardController controller = new CardController(null, null, null, null, null, Create.CardUpdateDtoValidator(), Create.DueValidator());
            Card card = Create.Card().Build();

            // When: 
            ResponseMessageResult result = (ResponseMessageResult)controller.SetDue(card, dto);

            // Then: 
            Assert.AreEqual((HttpStatusCode)422, result.Response.StatusCode);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestSetNullDueShouldUpdateCard() {
            // Given: 
            DateTimeUpdateDto dto = new DateTimeUpdateDto() { Value = null };
            Card card = Create.Card().Build();
            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(x => x.UpdateDue(card, null)).Returns(card);
            CardController controller = new CardController(cardServiceMock.Object, null, null, null, null, Create.CardUpdateDtoValidator(), Create.DueValidator());

            // When: 
            OkNegotiatedContentResult<CardModel> actionResult = (OkNegotiatedContentResult<CardModel>)controller.SetDue(card, dto);

            // Then: 
            cardServiceMock.Verify(x => x.UpdateDue(card, null), Times.Once);
        }

        /// <summary>
        ///     Testet die Anfrage zum entfernen eines zugeordneten Nutzers von einer Karte.
        /// </summary>
        [TestMethod]
        public void TestUnassignUserFromCard() {
            //Given: Ein Karte mit zwei zugewiesenen Nutzern
            User user1 = Create.User();
            User user2 = Create.User();
            Card card = Create.Card().WithAssignedUsers(user1, user2);

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(x => x.UnassignUsers(card, user1)).Returns(new List<User> { user2 });

            CardController controller = new CardController(cardServiceMock.Object, null, null, null, null, null, null);

            //When: Einer der Nutzer von der Karte entfernt werden soll
            //Then: Muss eine 200 kommen, mit der Liste der verbleibenden zugeordneten Nutzer an der Karte
            OkNegotiatedContentResult<CardModel> result = (OkNegotiatedContentResult<CardModel>)controller.UnassignUser(card, user1);                        
        }

        /// <summary>
        ///     Testet, dass wenn ein Nutzer, der einer Karate nicht zugewiesen ist, von dieser entfernt werden soll eine 404
        ///     geliefert wird
        /// </summary>
        [TestMethod]
        public void TestUnassignUserNotAssignedToCardShouldReturnNotFound() {
            //Given: Eine Karte mit zugewiesenen Nutzern und ein Nutzer, welcher der Karte nicht zugewiesen ist
            User user1 = Create.User();
            User user2 = Create.User();
            User notAssignedUser = Create.User();
            Card card = Create.Card().WithAssignedUsers(user1, user2);

            CardController controller = new CardController(null, null, null, null, null, null, null);

            //When: Der nicht zugewiesene Nutzer von der KArte entfernt werden soll
            NotFoundResult result = controller.UnassignUser(card, notAssignedUser) as NotFoundResult;

            //Then: Muss eine 404 NotFound geliefert werden
            result.Should().NotBeNull();
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateCardDescription() {
            // Given: 
            User user = Create.User().Build();
            Card card = Create.Card().Build();
            StringValueDto createDto = new StringValueDto() { Value = "neuer Titel" };

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(x => x.UpdateDescription(card, createDto.Value)).Returns(card);

            CardController controller = new CardController(cardServiceMock.Object, null, null, null, null, Create.CardUpdateDtoValidator(), Create.DueValidator());

            // When: 
            CardModel cardModel = ((OkNegotiatedContentResult<CardModel>)controller.UpdateCardDescription(user, card, createDto)).Content;

            // Then: 
            cardServiceMock.Verify(x => x.UpdateDescription(card, createDto.Value), Times.Once);
            Assert.AreEqual(card.BusinessId, cardModel.BusinessId);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateCardDescriptionWithoutWriteRights() {
            // Given: 
            User user = Create.User().CanWrite(false).Build();
            Card card = Create.Card().Build();

            // When: 
            UnauthorizedResult result =
                    (UnauthorizedResult)
                    new CardController(null, null, null, null, null, Create.CardUpdateDtoValidator(), Create.DueValidator()).UpdateCardDescription(user,
                        card,
                        new StringValueDto());

            // Then: 
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateCardTitle() {
            // Given: 
            User user = Create.User().Build();
            Card card = Create.Card().Build();
            StringValueDto createDto = new StringValueDto() { Value = "neuer Titel" };

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(x => x.UpdateTitle(card, createDto.Value)).Returns(card);

            CardController controller = new CardController(cardServiceMock.Object, null, null, null, null, Create.CardUpdateDtoValidator(), Create.DueValidator());

            // When: 
            CardModel cardModel = ((OkNegotiatedContentResult<CardModel>)controller.UpdateCardTitle(user, card, createDto)).Content;

            // Then: 
            cardServiceMock.Verify(x => x.UpdateTitle(card, createDto.Value), Times.Once);
            Assert.AreEqual(card.BusinessId, cardModel.BusinessId);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateCardTitleWithoutWriteRights() {
            // Given: 
            User user = Create.User().CanWrite(false).Build();
            Card card = Create.Card().Build();

            // When: 
            UnauthorizedResult result =
                    (UnauthorizedResult)
                    new CardController(null, null, null, null, null, Create.CardUpdateDtoValidator(), Create.DueValidator()).UpdateCardTitle(user,
                        card,
                        new StringValueDto());

            // Then: 
            Assert.IsNotNull(result);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateCardTitleWithTitleExeeding75Chars() {
            // Given: 
            User user = Create.User().Build();
            Card card = Create.Card().Build();
            StringValueDto createDto = new StringValueDto()
                    { Value = "9p48z c9ah 498h f8af9hfg9h 9ü85hyü9f 9ü8hr9ühf ahrü9ha9 rh9haer9h f98har9gh9" };

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();

            CardController controller = new CardController(cardServiceMock.Object, null, null, null, null, Create.CardUpdateDtoValidator(), Create.DueValidator());

            // When: 
            ResponseMessageResult result = (ResponseMessageResult)controller.UpdateCardTitle(user, card, createDto);

            // Then: 
            Assert.AreEqual((HttpStatusCode)422, result.Response.StatusCode);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateIsArchived() {
            // Given: 

            BoolValueDto boolValueDto = new BoolValueDto(true);
            Card card = Create.Card().ArchivedAt(DateTime.UtcNow).Build();

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(x => x.UpdateArchived(card, boolValueDto.Value)).Returns(card);

            CardController controller = new CardController(cardServiceMock.Object, null, null, null, null, null, null);

            // When: 
            OkNegotiatedContentResult<CardModel> result = (OkNegotiatedContentResult<CardModel>)controller.UpdateIsArchived(card, boolValueDto);

            // Then: 
            cardServiceMock.Verify(x => x.UpdateArchived(card, boolValueDto.Value), Times.Once);
            Assert.AreEqual(result.Content.BusinessId, card.BusinessId);
        }
    }
}