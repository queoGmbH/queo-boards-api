using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Queo.Boards.Core.Tests.Asserts;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Services.Impl;
using Queo.Boards.Core.Tests.Infrastructure;
using Spring.Transaction.Interceptor;

namespace Queo.Boards.Core.Tests.Services.Boards {
    [TestClass]
    public partial class BoardServiceTests : CreateBaseTest {
        /// <summary>
        ///     Testet das Hinzufügen eines neuen Eigentümers zum Board.
        /// </summary>
        [TestMethod]
        public void TestAddOwnerToBoard() {
            //Given: Ein Board und ein Nutzer
            Board board = Create.Board().Build();
            User newOwner = Create.User();
            IList<User> ownersBefore = board.Owners.ToList();

            


            //When: Der Nutzer als Eigentümer des Boards festgelegt werden soll
            IList<User> owners = CreateService.BoardService().Build().AddOwner(board, newOwner);

            //Then: Muss er anschließend als Eigentümer des Boards definiert sein
            owners.Should().BeEquivalentTo(ownersBefore.Concat(new[] { newOwner }));
            board.Owners.Should().BeEquivalentTo(ownersBefore.Concat(new[] { newOwner }));
            board.Members.Should().NotContain(newOwner);
            board.GetOwnersAndMembers().Should().Contain(newOwner);
        }

        

        /// <summary>
        ///     Testet das, wenn ein Nutzer der einem Board bereits zugeordnet ist, erneut zugeordnet werden soll, kein neues
        ///     BoardMitglied erstellt wird, sondern der bestehende geliefert wird
        /// </summary>
        [TestMethod]
        public void TestAddUserToBoaradAgainShouldNotCreateNewBoardMemberButReturnExistingBoardMember() {
            //Given: Ein Board dem ein Nutzer zugeordnet ist
            User user = Create.User();
            User addingUser = Create.User();
            Board board = Create.Board().Build();

            Mock<IEmailNotificationService> emailNotificationServiceMock = new Mock<IEmailNotificationService>();
            emailNotificationServiceMock.Setup(s => s.NotifyUserAddedToBoard(user, board, addingUser));

            IBoardService service = CreateService.BoardService().EmailNotificationService(emailNotificationServiceMock.Object).Build();

            //When: Der Nutzer dem Board erneut zugeordnet werden soll
            service.AddMember(board, user, addingUser);

            //Then: Darf kein neues BoardMitglied erstellt werden sondern muss das bestehende Board-Mitglied geliefert werden
            board.Members.Should().BeEquivalentTo(user);
            emailNotificationServiceMock.Verify(s => s.NotifyUserAddedToBoard(user, board, addingUser), Times.Once);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestAddUserToBoardShouldCreateNewBoardMemberInBoard() {
            // Given: 
            User user = Create.User().Build();
            User addingUser = Create.User();
            Board board = Create.Board().Build();

            Mock<IEmailNotificationService> emailNotificationServiceMock = new Mock<IEmailNotificationService>();
            emailNotificationServiceMock.Setup(s => s.NotifyUserAddedToBoard(user, board, addingUser));

            IBoardService service = CreateService.BoardService().EmailNotificationService(emailNotificationServiceMock.Object).Build();

            // When: 
            service.AddMember(board, user, addingUser);

            // Then: 
            Assert.AreEqual(board.Members.First(), user);
            emailNotificationServiceMock.Verify(s => s.NotifyUserAddedToBoard(user, board, addingUser), Times.Once);
        }

        /// <summary>
        ///     Testet, dass keine archivierten Boards kopiert werden können.
        /// </summary>
        [TestMethod]
        public void TestCopyArchivedBoardShouldThrowArgumentOutOfRangeException() {
            //Given: Ein archiviertes Board
            Board archivedBoard = Create.Board().ArchivedAt(DateTime.UtcNow);

            //When: Das archivierte Board kopiert werden soll
            Action action = () => new BoardService(null, null, null, null, null).Copy(archivedBoard, new BoardDto("Kopie von archiviertem Board", Accessibility.Public, "Schema F"), Create.User());

            //Then: Darf das nicht funktionieren und es muss eine Exception fliegen.
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        /// <summary>
        ///     Testet das Kopieren eines Boards
        /// </summary>
        [TestMethod]
        public void TestCopyBoard() {
            //Given: Ein Board, welches kopiert werden soll
            Board sourceBoard = Create.Board();
            Mock<IBoardDao> boardDaoMock = new Mock<IBoardDao>();
            boardDaoMock.Setup(d => d.Save(It.IsAny<Board>()));
            BoardService boardService = new BoardService(boardDaoMock.Object, null, null, null, null);
            BoardDto copyBoardDto = new BoardDto("Das ist eine Kopie", Accessibility.Restricted, "Schema Kopie");
            User createdBy = Create.User();

            //When: Das Board kopiert wird
            Board board = boardService.Copy(sourceBoard, copyBoardDto, createdBy);

            //Then: Muss eine Kopie erstellt werden, welche die gleichen Eigenschaften, wie die Quelle hat.
            board.GetDto().Should().Be(copyBoardDto);
            board.CreatedBy.Should().Be(createdBy);
        }

        /// <summary>
        ///     Testet, dass beim Kopieren eines Boards, die Labels auch kopiert werden
        /// </summary>
        [TestMethod]
        public void TestCopyBoardShouldCopyLabels() {
            //Given: Ein Board mit Labels
            Board boardWithLabels = Create.Board();
            Label label1 = Create.Label().ForBoard(boardWithLabels);
            Label label2 = Create.Label().ForBoard(boardWithLabels);

            Mock<IBoardDao> boardDaoMock = new Mock<IBoardDao>();
            boardDaoMock.Setup(d => d.Save(It.IsAny<Board>()));
            Mock<ILabelDao> labelDaoMock = new Mock<ILabelDao>();
            labelDaoMock.Setup(d => d.Save(It.IsAny<Label>()));
            BoardService boardService = new BoardService(boardDaoMock.Object, new LabelService(labelDaoMock.Object, null), null, null, null);

            //When: Das Board kopiert wird
            Board copy = boardService.Copy(boardWithLabels, new BoardDto("Kopie mit Labels", Accessibility.Public, "Label Schema"), Create.User());

            //Then: Müssen auch die Labels mit übernommen werden. 
            copy.Labels.Should().NotContain(label1, "Das Original-Label darf nicht übernommen werden.");
            copy.Labels.Should().NotContain(label2, "Das Original-Label darf nicht übernommen werden.");
            copy.Labels.Should().Contain(l => l.Name == label1.Name && l.Color == label1.Color);
            copy.Labels.Should().Contain(l => l.Name == label2.Name && l.Color == label2.Color);
        }

        /// <summary>
        ///     Testet, dass beim Kopieren eines Boards auch eine Kopie der Listen übernommen wird
        /// </summary>
        [TestMethod]
        public void TestCopyBoardShouldCopyListsWithChilds() {
            //Given: Ein Board mit Listen
            Board boardWithLists = Create.Board();
            List list1 = Create.List().OnBoard(boardWithLists);
            List list2 = Create.List().OnBoard(boardWithLists);

            Mock<IBoardDao> boardDaoMock = new Mock<IBoardDao>();
            boardDaoMock.Setup(d => d.Save(It.IsAny<Board>()));
            Mock<IListDao> listDaoMock = new Mock<IListDao>();
            listDaoMock.Setup(d => d.Save(It.IsAny<List>()));
            BoardService boardService = new BoardService(boardDaoMock.Object, null, new ListService(listDaoMock.Object, null, null), null, null);

            //When: Das Board kopiert wird
            Board copy = boardService.Copy(boardWithLists, new BoardDto("Kopie mit Listen", Accessibility.Restricted, "Listen Schema"), Create.User());

            //Then: Muss auch eine Kopie der Listen übernommen werden.
            copy.Lists.Should().NotContain(list1, "Die Original-Liste darf nicht übernommen werden, sondern es muss ein Kopie erstellt werden.");
            copy.Lists.Should().NotContain(list2, "Die Original-Liste darf nicht übernommen werden, sondern es muss ein Kopie erstellt werden.");
            copy.Lists.Should().Contain(l => l.Title == list1.Title);
            copy.Lists.Should().Contain(l => l.Title == list2.Title);
        }

        /// <summary>
        ///     Testet, dass beim Kopieren eines Boards keine archivierten Listen kopiert werden.
        /// </summary>
        [TestMethod]
        public void TestCopyBoardShouldNotCopyArchivedLists() {
            //Given: Ein Board mit einer archivierten Liste
            Board boardWithArchivedList = Create.Board();
            Create.List().OnBoard(boardWithArchivedList).ArchivedAt(DateTime.UtcNow).Build();

            Mock<IBoardDao> boardDaoMock = new Mock<IBoardDao>();
            boardDaoMock.Setup(d => d.Save(It.IsAny<Board>()));
            BoardService boardService = new BoardService(boardDaoMock.Object, null, null, null, null);

            //When: Das Board kopiert wird
            Board copy = boardService.Copy(boardWithArchivedList, new BoardDto("Kopie, aber bitte ohne archivierte Listen", Accessibility.Public, "Schema"), Create.User());

            //Then: Darf die Liste nicht mit kopiert werden
            copy.Lists.Should().BeEmpty();
        }

        /// <summary>
        ///     Testet das Kopieren einer Board-Vorlage
        /// </summary>
        [TestMethod]
        public void TestCopyBoardShouldWorkForTemplates() {
            //Given: Eine Board-Vorlage, welche kopiert werden soll
            Board template = Create.Board().AsTemplate();
            Mock<IBoardDao> boardDaoMock = new Mock<IBoardDao>();
            boardDaoMock.Setup(d => d.Save(It.IsAny<Board>()));
            BoardService boardService = new BoardService(boardDaoMock.Object, null, null, null, null);
            BoardDto copyBoardDto = new BoardDto("Das ist eine Kopie, die aus einer Voralge erstellt wird", Accessibility.Public, "Schema Kopie");

            //When: Die Vorlage kopiert wird
            Board board = boardService.Copy(template, copyBoardDto, Create.User());

            //Then: Muss eine Kopie erstellt werden, die keine Vorlage ist.
            board.GetDto().Should().Be(copyBoardDto);
            board.IsTemplate.Should().BeFalse();
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateNewBoard() {
            // Given: 
            BoardDto dto = new BoardDto() { Accessibility = Accessibility.Public, Title = "mein board", ColorScheme = "grün" };

            User user = Create.User().Build();

            Mock<IBoardDao> boardDaoMock = new Mock<IBoardDao>();
            boardDaoMock.Setup(x => x.Save(It.IsAny<Board>()));

            IBoardService boardService = CreateService.BoardService().BoardDao(boardDaoMock.Object).Build();

            // When: 
            Board createdBoard = boardService.Create(dto, user);

            // Then: 
            boardDaoMock.Verify(x => x.Save(createdBoard), Times.Once);

            Assert.AreEqual(dto.ColorScheme, createdBoard.ColorScheme);
            Assert.AreEqual(dto.Title, createdBoard.Title);
            Assert.AreEqual(user, createdBoard.Owners.First());
            Assert.AreEqual(user, createdBoard.CreatedBy);

            createdBoard.Owners.Should().BeEquivalentTo(user);
        }

        /// <summary>
        ///     Testet das Erstellen einer Vorlage
        /// </summary>
        [TestMethod]
        public void TestCreateTemplate() {
            //Given: Ein Board, aus dem eine Vorlage erstellt werden soll
            Board sourceBoard = Create.Board().Public();

            Mock<IBoardDao> boardDaoMock = new Mock<IBoardDao>();
            boardDaoMock.Setup(d => d.Save(It.Is<Board>(savedBoard => savedBoard.IsTemplate))).Returns<Board>(b => b);
            BoardService boardService = CreateService.BoardService().BoardDao(boardDaoMock.Object).Build();
            User creator = Create.User();

            //When: Die Vorlage erstellt werden soll
            Board template = boardService.CreateTemplateFromBoard(sourceBoard, creator);

            //Then: Muss ein Board mit dem Flag IsTemplate==true geliefert werden
            template.IsTemplate.Should().BeTrue();

            //Then: Muss die erstellt Vorlage gespeichert werden
            boardDaoMock.Verify(d => d.Save(It.Is<Board>(savedBoard => savedBoard.IsTemplate)), Times.Once);

            template.Should().NotBe(sourceBoard);
            template.Title.Should().Be(sourceBoard.Title);
            template.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, 1000);
            template.CreatedBy.Should().Be(creator);
            template.Accessibility.Should().Be(sourceBoard.Accessibility);
        }

        /// <summary>
        ///     Testet, dass man keine Vorlage aus einem archivierten Board erstellen kann
        /// </summary>
        [TestMethod]
        public void TestCreateTemplateFromBoardShouldThrowExceptionForArchivedBoardAsSource() {
            //Given: Ein archiviertes Board
            Board archivedBoard = Create.Board().ArchivedAt(DateTime.UtcNow);

            //When: Eine Vorlage aus dem archivierten Board erstellt werden soll
            Action action = () => CreateService.BoardService().Build().CreateTemplateFromBoard(archivedBoard, Create.User());

            //Then: Muss eine Exception fliegen
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        /// <summary>
        ///     Testet, dass man keine Vorlage aus einem Board erstellen kann, das schon eine Vorlage ist.
        /// </summary>
        [TestMethod]
        public void TestCreateTemplateFromBoardShouldThrowExceptionForTemplateAsSource() {
            //Given: Ein Board, welches eine Vorlage ist
            Board templateAsSource = Create.Board().AsTemplate();

            //When: Eine Vorlage aus der schon vorhandenen Vorlage erstellt werden soll
            Action action = () => CreateService.BoardService().Build().CreateTemplateFromBoard(templateAsSource, Create.User());

            //Then: Muss eine Exception fliegen
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        /// <summary>
        ///     Testet das beim erstellen einer Vorlage aus einem Board, die Labels übernommen/kopiert werden.
        /// </summary>
        [TestMethod]
        public void TestCreateTemplateShouldCopyLabels() {
            //Given: Ein Board mit Labels, aus dem eine Vorlage erstellt werden soll
            Board sourceBoard = Create.Board();
            Label label1 = Create.Label().WithName("Label 1").Color("Red").ForBoard(sourceBoard).Build();
            Label label2 = Create.Label().WithName("Label 2").Color("Green").ForBoard(sourceBoard).Build();
            User creator = Create.User();

            Mock<IBoardDao> boardDaoMock = new Mock<IBoardDao>();
            boardDaoMock.Setup(d => d.Save(It.Is<Board>(savedBoard => savedBoard.IsTemplate))).Returns<Board>(b => b);

            Mock<ILabelService> labelServiceMock = new Mock<ILabelService>();
            // TODO: Dieser Mock ist irgendwo unschön. Evtl. ist die ganze Architektur mit den Services hier ungünstig.
            labelServiceMock.Setup(s => s.Create(It.Is<Board>(b => b.IsTemplate), label2.GetDto())).Returns<Board, LabelDto>((b, labelDto) => {
                Label tempLabel = new Label(b, labelDto.Color, labelDto.Name);
                b.Labels.Add(tempLabel);
                return tempLabel;
            });
            labelServiceMock.Setup(s => s.Create(It.Is<Board>(b => b.IsTemplate), label1.GetDto())).Returns<Board, LabelDto>((b, labelDto) => {
                Label tempLabel = new Label(b, labelDto.Color, labelDto.Name);
                b.Labels.Add(tempLabel);
                return tempLabel;
            });

            BoardService boardService = CreateService.BoardService().BoardDao(boardDaoMock.Object).LabelService(labelServiceMock.Object);

            //When: Die Vorlage erstellt werden soll
            Board template = boardService.CreateTemplateFromBoard(sourceBoard, creator);

            //Then: Müssen die Labels als Kopie übernommen werden.
            template.Labels.Should().NotContain(label1);
            template.Labels.Should().NotContain(label2);

            template.Labels.Should().HaveCount(2);
            template.Labels.Should().ContainSingle(l => l.Name == label1.Name && l.Color == label1.Color);
            template.Labels.Should().ContainSingle(l => l.Name == label2.Name && l.Color == label2.Color);

            labelServiceMock.Verify(s => s.Create(template, label1.GetDto()), Times.Once());
            labelServiceMock.Verify(s => s.Create(template, label2.GetDto()), Times.Once());
        }

        /// <summary>
        ///     Testet das beim erstellen einer Vorlage aus einem Board, die Listen (ohne Karten) übernommen/kopiert werden.
        /// </summary>
        [TestMethod]
        public void TestCreateTemplateShouldCopyListsWithoutCards() {
            //Given: Ein Board mit Listen und Karten, aus dem eine Vorlage erstellt werden soll
            Board sourceBoard = Create.Board();
            List listWithCards = Create.List().OnBoard(sourceBoard).Title("Liste mit Karten");
            Create.Card().OnList(listWithCards).Build();
            List listWithoutCards = Create.List().OnBoard(sourceBoard).Title("Liste ohne Karten");
            User creator = Create.User();

            Mock<IBoardDao> boardDaoMock = new Mock<IBoardDao>();
            boardDaoMock.Setup(d => d.Save(It.Is<Board>(savedBoard => savedBoard.IsTemplate))).Returns<Board>(b => b);

            Mock<IListService> listServiceMock = new Mock<IListService>();
            // TODO: Siehe Testfall mit Labels
            listServiceMock.Setup(s => s.Create(It.Is<Board>(b => b.IsTemplate), listWithCards.Title)).Returns<Board, string>((b, title) => {
                List tempList = new List(b, title);
                b.Lists.Add(tempList);
                return tempList;
            });
            listServiceMock.Setup(s => s.Create(It.Is<Board>(b => b.IsTemplate), listWithoutCards.Title)).Returns<Board, string>((b, title) => {
                List tempList = new List(b, title);
                b.Lists.Add(tempList);
                return tempList;
            });

            BoardService boardService = CreateService.BoardService().BoardDao(boardDaoMock.Object).ListService(listServiceMock.Object);

            //When: Die Vorlage erstellt werden soll
            Board template = boardService.CreateTemplateFromBoard(sourceBoard, creator);

            //Then: Müssen die Listen als Kopie übernommen werden, jedoch ohne die Karten zu übernehmen/kopieren.
            template.Lists.Should().NotContain(listWithCards);
            template.Lists.Should().NotContain(listWithoutCards);

            template.Lists.Should().HaveCount(2);
            template.Lists.Should().ContainSingle(l => l.Title == listWithCards.Title);
            template.Lists.Should().ContainSingle(l => l.Title == listWithoutCards.Title);
            template.Lists.Should().NotContain(l => l.Cards.Any());

            listServiceMock.Verify(s => s.Create(template, listWithCards.Title), Times.Once());
            listServiceMock.Verify(s => s.Create(template, listWithoutCards.Title), Times.Once());
        }

        /// <summary>
        ///     Testet das Entfernen der Mitgliedschaft eines Nutzers an einem Board
        /// </summary>
        [TestMethod]
        public void TestRemoveMemberFromBoard() {
            //Given: Zwei Nutzer die Mitglied eines Boards sind.
            User user1 = Create.User();
            User user2 = Create.User();

            Board board = Create.Board().WithMembers(user1, user2).Build();
            List list1OnBoard = Create.List().OnBoard(board).Build();
            Card card1OnList1 = Create.Card().OnList(list1OnBoard).WithAssignedUsers(user1, user2);
            Card card2OnList1 = Create.Card().OnList(list1OnBoard).WithAssignedUsers(user1);
            Card card3OnList1 = Create.Card().OnList(list1OnBoard).WithAssignedUsers(user2);

            List list2OnBoard = Create.List().OnBoard(board).Build();
            Card card1OnList2 = Create.Card().OnList(list1OnBoard).WithAssignedUsers();
            Card card2OnList2 = Create.Card().OnList(list1OnBoard).WithAssignedUsers(user1);

            BoardService boardService = CreateService.BoardService().Build();

            //When: Die Mitgliedschaft eines der Nutzer am Board beendet werden soll
            boardService.RemoveMember(board, user1);

            //Then: Darf nur noch der andere Nutzer Mitglied des Boards sein und der entfernte Nutzer darf an keiner Karte mehr Mitglied sein.
            board.Members.Should().BeEquivalentTo(user2);

            card1OnList1.AssignedUsers.Should().BeEquivalentTo(user2);
            card2OnList1.AssignedUsers.Should().BeEmpty();
            card3OnList1.AssignedUsers.Should().BeEquivalentTo(user2);

            card1OnList2.AssignedUsers.Should().BeEmpty();
            card2OnList2.AssignedUsers.Should().BeEmpty();
        }

        /// <summary>
        ///     Testet das Löschen des letzten Besitzers von einem Board
        /// </summary>
        [TestMethod]
        public void TestRemoveOwnerShouldRemoveInvalidOperationExceptionWhenRemovingTheLastRemainingOwner() {
            //Given: Ein Board mit genau einem Besitzer
            User onlyOwnerOfBoard = Create.User();
            Board board = Create.Board().WithOwners(onlyOwnerOfBoard).Build();

            //When: Der einzige Eigentümer des Boards entfernt werden soll
            Action action = () => CreateService.BoardService().Build().RemoveOwner(board, onlyOwnerOfBoard);

            //Then: Darf das nicht funktionieren und es muss eine Exception geworfen werden
            action.ShouldThrow<InvalidOperationException>();
        }

        /// <summary>
        ///     Testet das Löschen eines Besitzers von einem Board
        /// </summary>
        [TestMethod]
        public void TestRemoveOwnerShouldRemoveUserAsOwnerFromBoard() {
            //Given: Ein Board mit zwei Nutzern
            User ownerToRemove = Create.User();
            User ownerToRemain = Create.User();
            Board board = Create.Board().WithOwners(ownerToRemove, ownerToRemain).Build();

            //When: Einer der beiden Nutzer als Eigentümer entfernt werden soll
            IList<User> remainingOwners = CreateService.BoardService().Build().RemoveOwner(board, ownerToRemove);

            //Then: Muss eine Liste mit dem verbleibenden Nutzer geliefert werden und nur noch der andere Nutzer als Besitzer des Boards eingetragen sein
            remainingOwners.Should().Contain(ownerToRemain);
            remainingOwners.Should().NotContain(ownerToRemove);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateExistingBoard() {
            // Given: 
            Board board = Create.Board().WithTitle("board1").Restricted().ColorScheme("blau").Build();

            IBoardService boardService = CreateService.BoardService().Build();

            BoardDto dto = new BoardDto() { Accessibility = Accessibility.Public, ColorScheme = "gelb", Title = "board2" };

            // When: 
            Board updatedBoard = boardService.Update(board, dto);

            // Then: 
            Assert.AreEqual(Accessibility.Public, updatedBoard.Accessibility);
            Assert.AreEqual("gelb", updatedBoard.ColorScheme);
            Assert.AreEqual("board2", updatedBoard.Title);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateIsArchivatedGetsCalledOnBoard() {
            // Given: 
            Board board = Create.Board().ArchivedAt(DateTime.UtcNow).Build();
            Mock<Board> boardMock = new Mock<Board>();
            boardMock.Setup(x => x.Archive(It.IsAny<DateTime>()));

            // When: 
            IBoardService boardService = CreateService.BoardService().Build();
            Board updatedBoard = boardService.UpdateIsArchived(boardMock.Object, true);

            // Then: 
            boardMock.Verify(x => x.Archive(It.IsAny<DateTime>()), Times.Once);
            Assert.AreEqual(boardMock.Object, updatedBoard);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateIsArchivatedHasTransactionAttribute() {
            Expression<Action> action = () => CreateService.BoardService().Build().UpdateIsArchived(default(Board), default(bool));
            action.ShouldHaveAttribute<TransactionAttribute>();
        }
    }
}