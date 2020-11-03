using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Tests.Builders;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Persistence {
    [TestClass]
    public class CommentDaoTest : PersistenceBaseTest {

        public ICommentDao CommentDao { set; private get; }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestSaveGet() {

            // Given: 

            string text = "";

            Card card = Create.Card().Build();
            User creator = Create.User().Build();
            DateTime creationDate = new DateTime(2017,5,13);
            Comment comment = Create.Comment().OnCard(card).Creator(creator).CreationDate(creationDate).LargeText().Build();

            // When: 
            Comment reloaded = CommentDao.Get(comment.Id);

            // Then: 
            Assert.AreEqual(comment, reloaded);
            Assert.AreEqual(Texts.LargeText, reloaded.Text);
            Assert.AreEqual(creator, reloaded.Creator);
            Assert.AreEqual(creationDate, reloaded.CreationDate);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestFindAllCommentsOnCard() {

            // Given: 
            Card card = Create.Card().Build();
            Card card2 = Create.Card().Build();
            Comment comment1 = Create.Comment().OnCard(card).Build();
            Comment comment2 = Create.Comment().OnCard(card).Build();
            Comment comment3 = Create.Comment().OnCard(card2).Build();

            // When: 
            IList<Comment> comments = CommentDao.FindAllOnCard(card);

            // Then: 
            CollectionAssert.AreEquivalent(new List<Comment>() {comment1, comment2}, comments.ToList());
        }


        /// <summary>
        /// Testet, dass keine gelöschten Kommentare gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindCommentsForUserShouldNotFindDeletedComments() {

            //Given: Ein gelöschter Kommentar zu einer Karte auf einem öffentlichen Board.
            User user = Create.User();
            Board board = Create.Board().Public().Build();
            List list = Create.List().OnBoard(board).Build();
            Card card = Create.Card().OnList(list).Build();
            Comment deletedComment = Create.Comment().OnCard(card).Deleted(true).Build();

            //When: Ein Nutzer nach Kommentaren sucht
            IPage<Comment> foundComments = CommentDao.FindCommentsForUser(PageRequest.All, user, null);


            //Then: darf der gelöschte nicht gefunden werden
            foundComments.Should().NotContain(deletedComment);

        }

        /// <summary>
        /// Testet, dass keine Kommentare auf archivierten Karten gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindCommentsForUserShouldNotFindCommentsOnArchivedCards() {

            //Given: Ein Kommentar auf einer archivierten Karte auf einem öffentlichen Board.
            User user = Create.User();
            Board board = Create.Board().Public().Build();
            List list = Create.List().OnBoard(board).Build();
            Card archivedCard = Create.Card().OnList(list).ArchivedAt(DateTime.UtcNow).Build();
            Comment commentOnArchivedCard = Create.Comment().OnCard(archivedCard).Build();

            //When: Ein Nutzer nach Kommentaren sucht
            IPage<Comment> foundComments = CommentDao.FindCommentsForUser(PageRequest.All, user, null);


            //Then: darf der Kommentar nicht gefunden werden
            foundComments.Should().NotContain(commentOnArchivedCard);

        }

        /// <summary>
        /// Testet, dass keine Kommentare auf archivierten Listen gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindCommentsForUserShouldNotFindCommentsOnArchivedLists() {

            //Given: Ein Kommentar auf einer archivierten Karte auf einem öffentlichen Board.
            User user = Create.User();
            Board board = Create.Board().Public().Build();
            List list = Create.List().OnBoard(board).ArchivedAt(DateTime.UtcNow).Build();
            Card cardOnArchivedList = Create.Card().OnList(list).Build();
            Comment commentOnArchivedList = Create.Comment().OnCard(cardOnArchivedList).Build();

            //When: Ein Nutzer nach Kommentaren sucht
            IPage<Comment> foundComments = CommentDao.FindCommentsForUser(PageRequest.All, user, null);

            //Then: darf der Kommentar nicht gefunden werden
            foundComments.Should().NotContain(commentOnArchivedList);

        }


        /// <summary>
        /// Testet, dass keine Kommentare auf archivierten Boards gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindCommentsForUserShouldNotFindCommentsOnArchivedBoards() {

            //Given: Ein Kommentar auf einer archivierten Karte auf einem öffentlichen Board.
            User user = Create.User();
            Board archived = Create.Board().Public().ArchivedAt(DateTime.UtcNow).Build();
            List list = Create.List().OnBoard(archived).Build();
            Card cardOnArchivedBoard = Create.Card().OnList(list).Build();
            Comment commentOnArchivedBoard = Create.Comment().OnCard(cardOnArchivedBoard).Build();

            //When: Ein Nutzer nach Kommentaren sucht
            IPage<Comment> foundComments = CommentDao.FindCommentsForUser(PageRequest.All, user, null);

            //Then: darf der Kommentar nicht gefunden werden
            foundComments.Should().NotContain(commentOnArchivedBoard);
        }


        /// <summary>
        /// Testet das keine Kommentare gefunden werden, die zu Karten auf Boards abgegeben worden, auf welche der Nutzer keinen Zugriff hat.
        /// </summary>
        [TestMethod]
        public void TestFindCommentsForUserShouldNotFindCommentsOnUnaccessableBoards() {

            //Given: Ein Kommentar zu einer Karte, die auf einem Board ist, auf welches der Nutzer keinen Zugriff hat.
            User user = Create.User();
            Board unaccessableBoard = Create.Board().Restricted().Build();
            List list = Create.List().OnBoard(unaccessableBoard).Build();
            Card cardOnUnaccessableBoard = Create.Card().OnList(list).Build();
            Comment commentOnUnaccessableBoard = Create.Comment().OnCard(cardOnUnaccessableBoard).Build();

            //When: Der Nutzer nach Kommentaren sucht
            IPage<Comment> foundComments = CommentDao.FindCommentsForUser(PageRequest.All, user, null);

            //Then: Darf der Kommentar nicht gefunden werden
            foundComments.Should().NotContain(commentOnUnaccessableBoard);
        }


        /// <summary>
        /// Testet das Kommentare gefunden werden, die zu Karten auf beschränkten Boards abgegeben worden, auf welche der Nutzer Zugriff hat, da er Member ist.
        /// </summary>
        [TestMethod]
        public void TestFindCommentsForUserShouldFindCommentsForMembersOnUnaccessableBoards() {

            //Given: Ein Kommentar zu einer Karte, die auf einem Board ist, auf welches der Nutzer Zugriff hat, da er Member ist.
            User user = Create.User();
            Board memberBoard = Create.Board().Restricted().WithMembers(user).Build();
            List list = Create.List().OnBoard(memberBoard).Build();
            Card cardOnMemberBoard = Create.Card().OnList(list).Build();
            Comment commentOnMemberBoard = Create.Comment().OnCard(cardOnMemberBoard).Build();

            //When: Der Nutzer nach Kommentaren sucht
            IPage<Comment> foundComments = CommentDao.FindCommentsForUser(PageRequest.All, user, null);

            //Then: Muss der Kommentar gefunden werden
            foundComments.Should().Contain(commentOnMemberBoard);
        }

        /// <summary>
        /// Testet das Kommentare gefunden werden, die zu Karten auf öffentlichen Boards abgegeben worden.
        /// </summary>
        [TestMethod]
        public void TestFindCommentsForUserShouldFindCommentsOnPublicBoards() {

            //Given: Ein Kommentar zu einer Karte, die auf einem öffentlichen Board ist.
            User user = Create.User();
            Board publicBoard = Create.Board().Public().Build();
            List list = Create.List().OnBoard(publicBoard).Build();
            Card cardOnPublicBoard = Create.Card().OnList(list).Build();
            Comment commentPublicBoard = Create.Comment().OnCard(cardOnPublicBoard).Build();

            //When: Der Nutzer nach Kommentaren sucht
            IPage<Comment> foundComments = CommentDao.FindCommentsForUser(PageRequest.All, user, null);

            //Then: Muss der Kommentar gefunden werden
            foundComments.Should().Contain(commentPublicBoard);
        }


        /// <summary>
        /// Testet das Kommentare gefunden werden, die zu Karten auf beschränkten Boards abgegeben worden, auf welche der Nutzer Zugriff hat, da er Owner ist.
        /// </summary>
        [TestMethod]
        public void TestFindCommentsForUserShouldFindCommentsForOwnerOnUnaccessableBoards() {

            //Given: Ein Kommentar zu einer Karte, die auf einem Board ist, auf welches der Nutzer Zugriff hat, da er Owner ist.
            User user = Create.User();
            Board ownerBoard = Create.Board().Restricted().WithOwners(user).Build();
            List list = Create.List().OnBoard(ownerBoard).Build();
            Card cardOnOwnedBoard = Create.Card().OnList(list).Build();
            Comment commentOnOwnedBoard = Create.Comment().OnCard(cardOnOwnedBoard).Build();

            //When: Der Nutzer nach Kommentaren sucht
            IPage<Comment> foundComments = CommentDao.FindCommentsForUser(PageRequest.All, user, null);

            //Then: Muss der Kommentar gefunden werden
            foundComments.Should().Contain(commentOnOwnedBoard);
        }

        /// <summary>
        /// Testet, das ein Kommentar gefunden wird, der die Suchzeichenfolge am Anfang des Kommentartextes enthält
        /// </summary>
        [TestMethod]
        public void TestFindCommentsForUserShouldFindCommentWithSearchTermAtBeginningOfText() {

            //Given: Ein Kommentar zu einer Karte auf einem öffentlichen Board, der die Suchzeichenfolge am Anfang des Kommentartextes enthält.
            const string SEARCH_TERM = "Kommentar";
            User user = Create.User();
            Board publicBoard = Create.Board().Public().Build();
            List list = Create.List().OnBoard(publicBoard).Build();
            Card cardOnPublicBoard = Create.Card().OnList(list).Build();
            Comment commentWithSearchTerm = Create.Comment().OnCard(cardOnPublicBoard).WithText(SEARCH_TERM + " der sagt, dass die Karte gefunden werden soll").Build();
            Comment commentWithoutSearchTerm = Create.Comment().OnCard(cardOnPublicBoard).WithText("Ich will nicht, dass das gefunden wird").Build();

            //When: Nach Kommentaren mit der Suchzeichenfolge gesucht wird
            IPage<Comment> foundComments = CommentDao.FindCommentsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss der Kommentar gefunden werden.
            foundComments.Should().Contain(commentWithSearchTerm);
            foundComments.Should().NotContain(commentWithoutSearchTerm);
        }

        /// <summary>
        /// Testet, das ein Kommentar gefunden wird, der die Suchzeichenfolge am Ende des Kommentartextes enthält
        /// </summary>
        [TestMethod]
        public void TestFindCommentsForUserShouldFindCommentWithSearchTermAtEndOfText() {

            //Given: Ein Kommentar zu einer Karte auf einem öffentlichen Board, der die Suchzeichenfolge am Ende des Kommentartextes enthält.
            const string SEARCH_TERM = "Kommentar";
            User user = Create.User();
            Board publicBoard = Create.Board().Public().Build();
            List list = Create.List().OnBoard(publicBoard).Build();
            Card cardOnPublicBoard = Create.Card().OnList(list).Build();
            Comment commentWithSearchTerm = Create.Comment().OnCard(cardOnPublicBoard).WithText("Das ist ein " + SEARCH_TERM).Build();
            Comment commentWithoutSearchTerm = Create.Comment().OnCard(cardOnPublicBoard).WithText("Ich will nicht, dass das gefunden wird").Build();

            //When: Nach Kommentaren mit der Suchzeichenfolge gesucht wird
            IPage<Comment> foundComments = CommentDao.FindCommentsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss der Kommentar gefunden werden.
            foundComments.Should().Contain(commentWithSearchTerm);
            foundComments.Should().NotContain(commentWithoutSearchTerm);
        }

        /// <summary>
        /// Testet, das ein Kommentar gefunden wird, der die Suchzeichenfolge irgendwo innerhalb des Kommentartextes enthält
        /// </summary>
        [TestMethod]
        public void TestFindCommentsForUserShouldFindCommentContainingSearchTerm() {

            //Given: Ein Kommentar zu einer Karte auf einem öffentlichen Board, der die Suchzeichenfolge irgendwo innerhalb des Kommentartextes enthält.
            const string SEARCH_TERM = "Kommentar";
            User user = Create.User();
            Board publicBoard = Create.Board().Public().Build();
            List list = Create.List().OnBoard(publicBoard).Build();
            Card cardOnPublicBoard = Create.Card().OnList(list).Build();
            Comment commentWithSearchTerm = Create.Comment().OnCard(cardOnPublicBoard).WithText("Das ist ein " + SEARCH_TERM + " der bei der Suche gefunden werden soll").Build();
            Comment commentWithoutSearchTerm = Create.Comment().OnCard(cardOnPublicBoard).WithText("Ich will nicht, dass das gefunden wird").Build();

            //When: Nach Kommentaren mit der Suchzeichenfolge gesucht wird
            IPage<Comment> foundComments = CommentDao.FindCommentsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss der Kommentar gefunden werden.
            foundComments.Should().Contain(commentWithSearchTerm);
            foundComments.Should().NotContain(commentWithoutSearchTerm);
        }

        /// <summary>
        /// Testet, das ein Kommentar gefunden wird, der die Suchzeichenfolge ohne führende und nachgestellte Leerzeichen irgendwo innerhalb des Kommentartextes enthält
        /// </summary>
        [TestMethod]
        public void TestFindCommentsForUserShouldFindCommentContainingTrimmedSearchTerm() {

            //Given: Ein Kommentar zu einer Karte auf einem öffentlichen Board, der die Suchzeichenfolge ohne führende und nachgestellte Leerzeichen irgendwo innerhalb des Kommentartextes enthält.
            const string SEARCH_TERM = "Kommentar";
            User user = Create.User();
            Board publicBoard = Create.Board().Public().Build();
            List list = Create.List().OnBoard(publicBoard).Build();
            Card cardOnPublicBoard = Create.Card().OnList(list).Build();
            Comment commentWithSearchTerm = Create.Comment().OnCard(cardOnPublicBoard).WithText("Das ist ein" + SEARCH_TERM + "der bei der Suche gefunden werden soll").Build();
            Comment commentWithoutSearchTerm = Create.Comment().OnCard(cardOnPublicBoard).WithText("Ich will nicht, dass das gefunden wird").Build();

            //When: Nach Kommentaren mit der Suchzeichenfolge gesucht wird
            IPage<Comment> foundComments = CommentDao.FindCommentsForUser(PageRequest.All, user, " " + SEARCH_TERM + " ");

            //Then: Muss der Kommentar gefunden werden.
            foundComments.Should().Contain(commentWithSearchTerm);
            foundComments.Should().NotContain(commentWithoutSearchTerm);
        }
    }
}