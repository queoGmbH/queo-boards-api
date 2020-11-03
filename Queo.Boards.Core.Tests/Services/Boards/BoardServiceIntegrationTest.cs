using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Persistence.Impl;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services.Boards {
    [TestClass]
    public class BoardServiceIntegrationTest : ServiceBaseTest {
        public IBoardDao BoardDao { set; private get; }

        public IBoardService BoardService { set; private get; }

        public ICardDao CardDao { get; set; }

        public ILabelDao LabelDao { set; private get; }

        public IListDao ListDao { get; set; }

        public ICommentDao CommentDao {
            get; set;
        }

        public INotificationDao NotificationDao { private get; set; }

        public IChecklistDao ChecklistDao {
            get; set;
        }

        public ITaskDao TaskDao {
            get; set;
        }

        public IListService ListService { get; set; }

        /// <summary>
        ///     Testet das beim erstellen einer Vorlage aus einem Board, sowohl die Labels als auch die Listen (ohne Karten)
        ///     übernommen/kopiert werden.
        /// </summary>
        [TestMethod]
        public void TestCreateTemplateShouldCopyListsWithoutCards() {
            //Given: Ein Board mit Labels sowie Listen und Karten, aus dem eine Vorlage erstellt werden soll
            Board sourceBoard = Create.Board();
            List listWithCards = Create.List().OnBoard(sourceBoard).Title("Liste mit Karten");
            Create.Card().OnList(listWithCards).Build();
            List listWithoutCards = Create.List().OnBoard(sourceBoard).Title("Liste ohne Karten");

            Label label1 = Create.Label().WithName("Label 1").Color("Red").ForBoard(sourceBoard).Build();
            Label label2 = Create.Label().WithName("Label 2").Color("Green").ForBoard(sourceBoard).Build();

            User creator = Create.User();

            //When: Die Vorlage erstellt werden soll
            Board template = BoardService.CreateTemplateFromBoard(sourceBoard, creator);

            //Then: Müssen die Listen als Kopie übernommen werden, jedoch ohne die Karten zu übernehmen/kopieren.
            template.Lists.Should().NotContain(listWithCards);
            template.Lists.Should().NotContain(listWithoutCards);

            template.Lists.Should().HaveCount(2);
            template.Lists.Should().ContainSingle(l => l.Title == listWithCards.Title);
            template.Lists.Should().ContainSingle(l => l.Title == listWithoutCards.Title);
            template.Lists.Should().NotContain(l => l.Cards.Any());

            //Then: Müssen die Labels als Kopie übernommen werden.
            template.Labels.Should().NotContain(label1);
            template.Labels.Should().NotContain(label2);

            template.Labels.Should().HaveCount(2);
            template.Labels.Should().ContainSingle(l => l.Name == label1.Name && l.Color == label1.Color);
            template.Labels.Should().ContainSingle(l => l.Name == label2.Name && l.Color == label2.Color);
        }

        /// <summary>
        ///     Testet das Löschen eines "leeren" Boards
        /// </summary>
        [TestMethod]
        public void TestDeleteBoard() {
            //Given: Ein Board ohne Listen
            Board board = Create.Board();

            //When: Das Board gelöscht wird
            BoardService.Delete(board);
            BoardDao.FlushAndClear();

            //Then: Darf es anschließend nicht mehr vorhanden sein. 
            Action action = () => BoardDao.Get(board.Id);
            action.ShouldThrow<ObjectNotFoundException>();
        }

        /// <summary>
        ///     Testet das Löschen eines Boards mit Listen und Karten
        /// </summary>
        [TestMethod]
        public void TestDeleteBoardCascadeLabels() {
            //Given: Ein Board ohne Listen
            Board board = Create.Board();
            Label label = Create.Label().ForBoard(board);

            //When: Das Board gelöscht wird
            BoardService.Delete(board);
            BoardDao.FlushAndClear();

            //Then: Müssen auch die Labels mit gelöscht werden 
            Action loadLabelAction = () => LabelDao.Get(label.Id);
            loadLabelAction.ShouldThrow<ObjectNotFoundException>();
        }

        /// <summary>
        ///     Testet das Löschen eines Boards mit Listen und Karten
        /// </summary>
        [TestMethod]
        public void TestDeleteBoardCascadeListsAndCards() {
            //Given: Ein Board ohne Listen
            Board board = Create.Board();
            List list = Create.List().OnBoard(board);
            Card card = Create.Card().OnList(list);
            Comment comment = Create.Comment().OnCard(card);
            Checklist checklist = Create.Checklist().OnCard(card);
            Task task = Create.Task().OnChecklist(checklist);

            //When: Das Board gelöscht wird
            BoardService.Delete(board);
            BoardDao.FlushAndClear();

            //Then: Müssen auch die Listen und die anderen Childs davon mit gelöscht werden 
            Action loadListAction = () => ListDao.Get(list.Id);
            loadListAction.ShouldThrow<ObjectNotFoundException>();

            Action loadCardAction = () => CardDao.Get(card.Id);
            loadCardAction.ShouldThrow<ObjectNotFoundException>();

            Action loadCommentAction = () => CommentDao.Get(comment.Id);
            loadCommentAction.ShouldThrow<ObjectNotFoundException>();

            Action loadChecklistAction = () => ChecklistDao.Get(checklist.Id);
            loadChecklistAction.ShouldThrow<ObjectNotFoundException>();

            Action loadTaskAction = () => TaskDao.Get(task.Id);
            loadTaskAction.ShouldThrow<ObjectNotFoundException>();
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestDoubleMoveListOnBoard() {
            // Given: 
            Board board = Create.Board().Build();
            List list1 = Create.List().Position(0).OnBoard(board).Build();
            List list2 = Create.List().Position(1).OnBoard(board).Build();
            List list3 = Create.List().Position(2).OnBoard(board).Build();

            // When: 
            ListService.MoveList(list1, board, board.Lists.IndexOf(list3) + 1);
            //BoardDao.FlushAndClear();
            board = BoardDao.Get(board.Id);

            // Then: 
            board.Lists.First().Should().Be(list2);
            board.Lists[1].Should().Be(list3);
            board.Lists.Last().Should().Be(list1);

            // When:
            board = ListService.MoveList(list2, board, board.Lists.IndexOf(list1) + 1);
            BoardDao.FlushAndClear();
            board = BoardDao.Get(board.Id);

            board.Lists.First().Should().Be(list3);
            board.Lists[1].Should().Be(list1);
            board.Lists.Last().Should().Be(list2);
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

            //When: Die Mitgliedschaft eines der Nutzer am Board beendet werden soll
            BoardService.RemoveMember(board, user1);
            BoardDao.FlushAndClear();
            Board reloadedBoard = BoardDao.Get(board.Id);

            //Then: Darf nur noch der andere Nutzer Mitglied des Boards sein.
            reloadedBoard.Members.Should().BeEquivalentTo(user2);
        }


        /// <summary>
        ///     Testet das beim erstellen einer Vorlage aus einem Board, spowhl die Labels als auch die Listen (ohne Karten)
        ///     übernommen/kopiert werden.
        /// </summary>
        [TestMethod]
        public void TestCopyBoardShouldCopyListsWithAllItsChilds() {
            //Given: Ein Board mit Labels sowie Listen und Karten und Kommentaren und Checklisten
            Board sourceBoard = Create.Board();
            List listWithCards = Create.List().Title("Liste´mit Karten").OnBoard(sourceBoard).Title("Liste mit Karten");
            List archivedList = Create.List().OnBoard(sourceBoard).ArchivedAt(DateTime.UtcNow).Title("Archivierte Liste");
            Card card = Create.Card().WithTitle("Karte").OnList(listWithCards).Build();
            Card archivedCard = Create.Card().WithTitle("Archivierte Karte").OnList(listWithCards).ArchivedAt(DateTime.UtcNow);
            Comment comment = Create.Comment().WithText("Ein Kommentar").OnCard(card);
            Comment deletedComment = Create.Comment().Deleted().WithText("Ein gelöschter Kommentar").OnCard(card);
            Checklist checklist = Create.Checklist().Title("Eine Checkliste").OnCard(card);
            Task task = Create.Task().Title("Kopieren von Boards implementieren").OnChecklist(checklist);
            
            Label label1 = Create.Label().WithName("Label 1").Color("Red").ForBoard(sourceBoard).Build();
            Label label2 = Create.Label().WithName("Label 2").Color("Green").ForBoard(sourceBoard).Build();

            User creator = Create.User();

            //When: Das Board kopiert werden soll
            Board copy = BoardService.Copy(sourceBoard, new BoardDto("Copy", Accessibility.Public, "Schema"), creator);

            //Then: Müssen die Listen als Kopie übernommen werden und das jeweils mit sämtlichen Childs als Kopie.
            copy.Lists.Should().NotContain(listWithCards);
            
            copy.Lists.Should().HaveCount(1);
            copy.Lists.Should().ContainSingle(l => l.Title == listWithCards.Title);

            copy.Lists.Single().Cards.Should().HaveCount(1);
            copy.Lists.Single().Cards.Should().ContainSingle(c => c.Title == card.Title);

            copy.Lists.Single().Cards.Single().Comments.Should().HaveCount(1);
            copy.Lists.Single().Cards.Single().Comments.Should().ContainSingle(c => c.Text == comment.Text && c.Creator.Equals(comment.Creator));

            copy.Lists.Single().Cards.Single().Checklists.Should().HaveCount(1);
            copy.Lists.Single().Cards.Single().Checklists.Should().ContainSingle(c => c.Title == checklist.Title);

            copy.Lists.Single().Cards.Single().Checklists.Single().Tasks.Should().HaveCount(1);
            copy.Lists.Single().Cards.Single().Checklists.Single().Tasks.Should().ContainSingle(t => t.Title == task.Title);

            //Then: Müssen die Labels als Kopie übernommen werden.
            copy.Labels.Should().NotContain(label1);
            copy.Labels.Should().NotContain(label2);

            copy.Labels.Should().HaveCount(2);
            copy.Labels.Should().ContainSingle(l => l.Name == label1.Name && l.Color == label1.Color);
            copy.Labels.Should().ContainSingle(l => l.Name == label2.Name && l.Color == label2.Color);
        }


        /// <summary>
        /// Testet das beim Löschen eines Boards, eventuell erstellte Benachrichtigungen für die Karten des Boards ebenfalls gelöscht werden.
        /// </summary>
        [TestMethod]
        public void TestDeleteArchivedBoardShouldCascadeCardNotifications() {

            //Given: Ein archiviertes Board mit einer Karte, für die Benachrichtigungen erstellt wurden
            Board archivedBoardToDelete = Create.Board().ArchivedAt(DateTime.UtcNow);
            List listOnArchivedBoard = Create.List().OnBoard(archivedBoardToDelete);
            Card cardWithNotifications = Create.Card().OnList(listOnArchivedBoard);
            Notification notification1 = Create.CardNotification().ForCard(cardWithNotifications);
            Notification notification2 = Create.CardNotification().ForCard(cardWithNotifications);

            Action loadBoardAction = () => BoardDao.GetByBusinessId(archivedBoardToDelete.BusinessId);
            Action loadCardNotification1Action = () => NotificationDao.GetByBusinessId(notification1.BusinessId);
            Action loadCardNotification2Action = () => NotificationDao.GetByBusinessId(notification2.BusinessId);


            //When: Das Board endgültig gelöscht werden soll
            BoardService.Delete(archivedBoardToDelete);
            BoardDao.FlushAndClear();

            //Then: Müssen auch die Benachrichtigungen gelöscht werden, damit das Löschen des Boards funktioniert.
            loadBoardAction.ShouldThrow<ObjectNotFoundException>();
            loadCardNotification1Action.ShouldThrow<ObjectNotFoundException>();
            loadCardNotification2Action.ShouldThrow<ObjectNotFoundException>();

        }

        /// <summary>
        /// Testet das beim Löschen eines Boards, eventuell erstellte Kommentare für die Karten des Boards ebenfalls gelöscht werden.
        /// </summary>
        [TestMethod]
        public void TestDeleteArchivedBoardShouldCascadeComments() {

            //Given: Ein archiviertes Board mit einer Karte, zu der Kommentare erstellt wurden, für welche Benachrichtigungen existieren.
            Board archivedBoardToDelete = Create.Board().ArchivedAt(DateTime.UtcNow);
            List listOnArchivedBoard = Create.List().OnBoard(archivedBoardToDelete);
            Card cardWithComments = Create.Card().OnList(listOnArchivedBoard);
            Comment comment = Create.Comment().OnCard(cardWithComments);
            CommentNotification commentNotification1 = Create.CommentNotification().ForComment(comment);
            CommentNotification commentNotification2 = Create.CommentNotification().ForComment(comment);

            Action loadBoardAction = () => BoardDao.GetByBusinessId(archivedBoardToDelete.BusinessId);
            Action loadCommentAction = () => CommentDao.GetByBusinessId(comment.BusinessId);
            Action loadCardCommentNotification1Action = () => NotificationDao.GetByBusinessId(commentNotification1.BusinessId);
            Action loadCardCommentNotification2Action = () => NotificationDao.GetByBusinessId(commentNotification2.BusinessId);


            //When: Das Board endgültig gelöscht werden soll
            BoardService.Delete(archivedBoardToDelete);
            BoardDao.FlushAndClear();

            //Then: Müssen auch die Kommentare und die zugehörigen Benachrichtigungen gelöscht werden, damit das Löschen des Boards funktioniert.
            loadBoardAction.ShouldThrow<ObjectNotFoundException>();
            loadCommentAction.ShouldThrow<ObjectNotFoundException>();
            loadCardCommentNotification1Action.ShouldThrow<ObjectNotFoundException>();
            loadCardCommentNotification2Action.ShouldThrow<ObjectNotFoundException>();

        }
    }
}