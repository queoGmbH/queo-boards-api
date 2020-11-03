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
using Task = System.Threading.Tasks.Task;

namespace Queo.Boards.Tests.Controllers {
    [TestClass]
    public class ListControllerTests : CreateBaseTest {
        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateCard() {
            // Given: 

            List list = Create.List().Build();
            CardDto dto = new CardDto() { Title = "Titel", Description = "description" };
            Card resultingCard = Create.Card().OnList(list).WithTitle("Titel").DescribedWith("description").Build();
            User currentUser = Create.User();

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(x => x.Create(list, dto, new List<User>(), currentUser)).Returns(resultingCard);

            ListController controller = new ListController(null, cardServiceMock.Object, null, null, Create.CardCreateDtoValidator());

            // When: 
            OkNegotiatedContentResult<CardModel> result = (OkNegotiatedContentResult<CardModel>)controller.CreateCard(list, currentUser, new CardCreateCommand(new List<User>(), new List<Label>(), dto.Title, dto.Description, dto.Due));

            // Then: 
            cardServiceMock.Verify(x => x.Create(list, dto, new List<User>(), currentUser), Times.Once);
            Assert.AreEqual("Titel", result.Content.Title);
            Assert.AreEqual("description", result.Content.Description);
        }

        /// <summary>
        ///     Testet das Erstellen einer neuen Karte inklusive initialer Zuweisung von Nutzern.
        /// </summary>
        [TestMethod]
        public void TestCreateCardAndInitiallyAssignUsers() {
            //Given: Eine Liste und Daten für eine neue Karte sowie zwei Nutzer, die Mitglieder des Board der Liste sind
            User user1 = Create.User();
            User user2 = Create.User();
            Board board = Create.Board().WithMembers(user1, user2).Build();
            List list = Create.List().OnBoard(board).Build();
            User currentUser = Create.User();

            CardDto dto = new CardDto() { Title = "Karte", Description = "Das ist eine Karte mit initialen Nutzern" };

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(x => x.Create(list, It.IsAny<CardDto>(), It.Is<IList<User>>(userList => userList.Contains(user1) && userList.Contains(user2)), currentUser)).Returns(() => {
                Card card = new Card(list, dto.Title, dto.Description, dto.Due, new List<Label>(), new EntityCreatedDto(currentUser, DateTime.Now));
                card.AssignUser(user1);
                card.AssignUser(user2);
                card.List.Cards.Add(card);
                return card;
            });

            Mock<IUserService> userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetById(user1.BusinessId)).Returns(user1);
            userServiceMock.Setup(s => s.GetById(user2.BusinessId)).Returns(user2);

            ListController controller = new ListController(null, cardServiceMock.Object, userServiceMock.Object, null, Create.CardCreateDtoValidator());

            //When: Eine neue Karte auf der Liste inklusive initialer Zuordnung der Nutzer zur Karte erstellt werden soll
            OkNegotiatedContentResult<CardModel> result = (OkNegotiatedContentResult<CardModel>)controller.CreateCard(list, currentUser, new CardCreateCommand(new List<User> { user1, user2 }, new List<Label>(), dto.Title, dto.Description, dto.Due));

            //Then: Muss das korrekt funktionieren.
        }

        /// <summary>
        ///     Testet das Erstellen einer neuen Karte inklusive initialer Zuweisung eines Nutzers der kein BoardMember ist.
        /// </summary>
        [TestMethod]
        public void TestCreateCardAndInitiallyAssignUserWhoIsNoBoardMemberShouldThrowException() {
            //Given: Eine Liste und Daten für eine neue Karte sowie zwei Nutzer, die Mitglieder des Board der Liste sind
            User user = Create.User();
            Board board = Create.Board().Build();
            List list = Create.List().OnBoard(board).Build();

            CardDto dto = new CardDto() { Title = "Karte", Description = "Das ist eine Karte mit initialen Nutzern" };

            Mock<IUserService> userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetById(user.BusinessId)).Returns(user);

            ListController controller = new ListController(null, null, userServiceMock.Object, null, Create.CardCreateDtoValidator());

            //When: Eine neue Karte auf der Liste inklusive initialer Zuordnung der Nutzer zur Karte erstellt werden soll
            ResponseMessageResult result = (ResponseMessageResult)controller.CreateCard(list, user, new CardCreateCommand(new List<User> { user }, new List<Label>(), dto.Title, dto.Description, dto.Due));

            //Then: Muss das korrekt funktionieren.
            //Then: Darf das nicht funktionieren und es muss ein HttpStatusCode = 409 (Conflict) geliefert werden
            result.Response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        /// <summary>
        ///     Testet, dass die Create-Methode auch mit NULL als Wert für initial der Karte hinzuzufügende Nutzer umgehen kann.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateCardCanHandleInitialUserNull() {
            // Given: 
            User currentUser = Create.User();
            List list = Create.List().Build();
            CardDto dto = new CardDto() { Title = "Titel", Description = "description" };
            Card resultingCard = Create.Card().OnList(list).WithTitle("title").DescribedWith("description").Build();

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(x => x.Create(list, dto, new List<User>(), currentUser)).Returns(resultingCard);
            ListController controller = new ListController(null, cardServiceMock.Object, null, null, Create.CardCreateDtoValidator());

            // When: Der Aufruf gemacht wird
            // Then: Muss Ok geliefert werden
            OkNegotiatedContentResult<CardModel> result = (OkNegotiatedContentResult<CardModel>)controller.CreateCard(list, currentUser, new CardCreateCommand(new List<User>(), new List<Label>(), dto.Title, dto.Description, dto.Due));
            cardServiceMock.Verify(x => x.Create(list, dto, new List<User>(), currentUser), Times.Once);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateCardWithPastDueShouldGiveError() {
            // Given: 
            User currentUser = Create.User();
            List list = Create.List().Build();
            CardDto dto = new CardDto()
                    { Title = "test", Description = "description", Due = new DateTime(2016, 1, 1) };

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();

            ListController controller = new ListController(null, cardServiceMock.Object, null, null, Create.CardCreateDtoValidator());

            // When: 
            ResponseMessageResult result = (ResponseMessageResult)controller.CreateCard(list, currentUser, new CardCreateCommand(new List<User>(), new List<Label>(), dto.Title, dto.Description, dto.Due));

            // Then: 
            Assert.AreEqual((HttpStatusCode)422, result.Response.StatusCode);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateCardWithTitleExeeding75Chars() {
            // Given: 
            User currentUser = Create.User();
            List list = Create.List().Build();
            CardDto dto = new CardDto()
                    { Title = "9p48z c9ah 498h f8af9hfg9h 9ü85hyü9f 9ü8hr9ühf ahrü9ha9 rh9haer9h f98har9gh9", Description = "description" };

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();

            ListController controller = new ListController(null, cardServiceMock.Object, null, null, Create.CardCreateDtoValidator());

            // When: 
            ResponseMessageResult result = (ResponseMessageResult)controller.CreateCard(list, currentUser, new CardCreateCommand(new List<User>(), new List<Label>(), dto.Title, dto.Description, dto.Due));

            // Then: 
            Assert.AreEqual((HttpStatusCode)422, result.Response.StatusCode);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public async Task TestCreateListShouldCallService() {
            // Given: 
            User user = Create.User().CanWrite(true).Build();
            Board board = Create.Board().Build();
            string listName = "newList";

            Mock<IListService> listServiceMock = new Mock<IListService>();
            listServiceMock.Setup(x => x.Create(board, listName)).Returns(new List(board, listName));

            ListController controller = new ListController(listServiceMock.Object, null, null, null, null);

            // When: 
            OkNegotiatedContentResult<ListModel> listResult =
                    (OkNegotiatedContentResult<ListModel>)controller.CreateList(user, board, new StringValueDto() { Value = listName });

            // Then: 
            listServiceMock.Verify(x => x.Create(board, listName), Times.Once);
            Assert.AreEqual(listName, listResult.Content.Title);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public async Task TestCreateListWithoutWriteRightsShouldReturn401() {
            // Given: 
            User nonWriteUser = Create.User().CanWrite(false).Build();

            ListController listController = new ListController(null, null, null, null, null);

            // When: 
            UnauthorizedResult uar = (UnauthorizedResult)listController.CreateList(nonWriteUser, null, null);

            // Then: 
            Assert.IsNotNull(uar);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestEnsureTitleLenghtOnUpdate() {
            // Given: 
            User user = Create.User().CanWrite(true).Build();
            List list = Create.List().Build();
            string listName = "p9q8zncp837zvo85gb8o7tgstbz45s8bztgo57tb 7z5tz 547ot54at o5gtgsf87gh8k7rl6hglrthl8gtliughtliuvilurthglitrbgilutgliiluvitluvtilruviltrvi tritiugsli gliuegil lig liet rgilubgiviu ili ivbi";

            Mock<IListService> listServiceMock = new Mock<IListService>();
            ListController controller = new ListController(listServiceMock.Object, null, null, Create.ListCreateAndUpdateValidator(), null);

            // When: 
            ResponseMessageResult badRequest = (ResponseMessageResult)controller.UpdateListTitle(user, list, new StringValueDto() { Value = listName });

            // Then: 
            Assert.AreEqual((HttpStatusCode)422, badRequest.Response.StatusCode);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateIsArchivedCallsService() {
            // Given: 
            List list = Create.List().ArchivedAt(DateTime.UtcNow).Build();
            BoolValueDto boolValueDto = new BoolValueDto(true);

            Mock<IListService> listServiceMock = new Mock<IListService>();
            listServiceMock.Setup(x => x.UpdateArchived(list, boolValueDto.Value)).Returns(list);

            ListController listController = new ListController(listServiceMock.Object, null, null, null, null);

            // When: 
            OkNegotiatedContentResult<ListWithCardsModel> result = (OkNegotiatedContentResult<ListWithCardsModel>)listController.UpdateIsArchived(list, boolValueDto);

            // Then: 
            listServiceMock.Verify(x => x.UpdateArchived(list, boolValueDto.Value), Times.Once);
            Assert.AreEqual(result.Content.BusinessId, list.BusinessId);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateListTitle() {
            // Given: 
            User user = Create.User().Build();
            List list = Create.List().Build();
            string newName = "new list name";
            list.Update(newName);

            Mock<IListService> listServiceMock = new Mock<IListService>();
            listServiceMock.Setup(x => x.Update(list, newName)).Returns(list);

            // When: 
            ListController controller = new ListController(listServiceMock.Object, null, null, Create.ListCreateAndUpdateValidator(), null);
            OkNegotiatedContentResult<ListModel> result = (OkNegotiatedContentResult<ListModel>)controller.UpdateListTitle(user, list, new StringValueDto() { Value = newName });

            // Then: 
            listServiceMock.Verify(x => x.Update(list, newName), Times.Once);
            Assert.AreEqual(newName, result.Content.Title);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateListWithoutWriteRightsShouldReturn401() {
            // Given: 
            User nonWriteUser = Create.User().CanWrite(false).Build();
            ListController listController = new ListController(null, null, null, Create.ListCreateAndUpdateValidator(), null);
            List list = Create.List().Build();

            // When: 
            UnauthorizedResult uar = (UnauthorizedResult)listController.UpdateListTitle(nonWriteUser, list, new StringValueDto() { Value = "test" });

            // Then: 
            Assert.IsNotNull(uar);
        }
    }
}