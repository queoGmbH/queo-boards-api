using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Util;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Tests.Builders;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Persistence {
    [TestClass]
    public class CardDaoTests : PersistenceBaseTest {
        public ICardDao CardDao {
            set; private get;
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestAssignLabelToCard() {
            // Given: 
            Card card = Create.Card().Build();
            Label label = Create.Label().Build();

            card.AddLabel(label);
            CardDao.Save(card);
            CardDao.FlushAndClear();

            // When: 
            Card reloaded = CardDao.Get(card.Id);

            // Then: 
            Assert.AreEqual(1, reloaded.Labels.Count);
            Assert.AreEqual(label, EnumerableExtensions.First(card.Labels));
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestDescriptionShouldBeVirtuallyInfinite() {
            // Given: 
            List list = Create.List().Build();
            Card card = Create.Card().OnList(list).LargeDescription().Build();

            // When: 
            Card reloaded = CardDao.Get(card.Id);

            // Then: 
            Assert.AreEqual(reloaded, card);
            Assert.AreEqual(Texts.LargeText, reloaded.Description);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestFindAllCardsByLabel() {
            // Given: 
            Label label = Create.Label().Build();
            Label label2 = Create.Label().Build();
            Card card = Create.Card().WithLabels(label).Build();
            Card cardWithoutLabel = Create.Card().Build();
            Card cardWithAnotherLabel = Create.Card().WithLabels(label2).Build();

            // When: 
            IList<Card> cardsWithLabel = CardDao.FindAllCardsWithLabel(label);

            // Then: 
            cardsWithLabel.Single().Should().Be(card);
        }

        /// <summary>
        ///     Testet, dass Karten auf Boards gefunden werden, die public sind.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserOnPublicBoard() {
            //Given: Eine Karte, auf einem öffentlichen, aktiven Board.
            User user = Create.User();
            Board publicBoard = Create.Board().Public().Build();
            List listOnPublicBoard = Create.List().OnBoard(publicBoard).Build();
            Card cardOnPublicBoard = Create.Card().OnList(listOnPublicBoard);

            //When: Die Karten für den Nutzer abgerufen werden
            IPage<Card> allFoundCards = CardDao.FindCardsForUser(PageRequest.All, user, null);

            //Then: Muss die Karte gefunden werden
            allFoundCards.Should().Contain(cardOnPublicBoard);
        }

        /// <summary>
        ///     Testet, dass Karten auf Boards gefunden werden, die zwar Restricted sind, der Nutzer aber ein Mitglied des Boards
        ///     ist und dadurch Zugang zum Baord hat.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserOnRestrictedButAccessableBoard() {
            //Given: Eine Karte, auf einem nicht öffentlichen, aktiven Board, zu dessen Mitgliedern der Nutzer zählt.
            User user = Create.User();
            Board restrictedBoard = Create.Board().Restricted().WithMembers(user).Build();
            List listOnRestrictedBoard = Create.List().OnBoard(restrictedBoard).Build();
            Card cardOnRestrictedBoard = Create.Card().OnList(listOnRestrictedBoard);

            //When: Die Karten für den Nutzer abgerufen werden
            IPage<Card> allFoundCards = CardDao.FindCardsForUser(PageRequest.All, user, null);

            //Then: Muss die Karte gefunden werden
            allFoundCards.Should().Contain(cardOnRestrictedBoard);
        }

        /// <summary>
        ///     Testet, dass eine Karte gefunden wird, die den Suchbegriff innerhalb der Beschreibung enthält.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserShouldFindCardContainingSearchTermInDescription() {
            //Given: Eine für den Nutzer findbare Karte, die den Suchbegriff am Ende der Beschreibung enthält 
            User user = Create.User();
            const string SEARCH_TERM = "Karte";
            Board board = Create.Board().Public().Build();
            List list = Create.List().OnBoard(board).Build();
            Card cardWithSearchTerm = Create.Card().OnList(list).DescribedWith("Eine " + SEARCH_TERM + " die gefunden werden sollte.");
            Card cardWithoutSearchTerm = Create.Card().OnList(list).DescribedWith("xyz").DescribedWith("xyz");

            //When: Nach Karten die einen bestimmten Suchbegriff enthalten gesucht wird
            IPage<Card> foundCards = CardDao.FindCardsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss die Karte gefunden werden.
            foundCards.Should().Contain(cardWithSearchTerm);
            foundCards.Should().NotContain(cardWithoutSearchTerm);
        }

        /// <summary>
        ///     Testet, dass eine Karte gefunden wird, die den Suchbegriff innerhalb des Titels enthält.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserShouldFindCardContainingSearchTermInTitle() {
            //Given: Eine für den Nutzer findbare Karte, die den Suchbegriff am Ende des Titels enthält 
            User user = Create.User();
            const string SEARCH_TERM = "Karte";
            Board board = Create.Board().Public().Build();
            List list = Create.List().OnBoard(board).Build();
            Card cardWithSearchTerm = Create.Card().OnList(list).WithTitle("Eine " + SEARCH_TERM + " die gefunden werden sollte.");
            Card cardWithoutSearchTerm = Create.Card().OnList(list).WithTitle("xyz").DescribedWith("xyz");

            //When: Nach Karten die einen bestimmten Suchbegriff enthalten gesucht wird
            IPage<Card> foundCards = CardDao.FindCardsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss die Karte gefunden werden.
            foundCards.Should().Contain(cardWithSearchTerm);
            foundCards.Should().NotContain(cardWithoutSearchTerm);
        }

        /// <summary>
        ///     Testet, dass eine Karte gefunden wird, die den Suchbegriff ohne führende und nachfolgende Leerzeichen innerhalb der
        ///     Beschreibung enthält.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserShouldFindCardContainingTrimmedSearchTermInDescription() {
            //Given: Eine für den Nutzer findbare Karte, die den Suchbegriff am Ende der Beschreibung enthält 
            User user = Create.User();
            const string SEARCH_TERM = "Karte";
            Board board = Create.Board().Public().Build();
            List list = Create.List().OnBoard(board).Build();
            Card cardWithSearchTerm = Create.Card().OnList(list).DescribedWith("Eine" + SEARCH_TERM + "die gefunden werden sollte.");
            Card cardWithoutSearchTerm = Create.Card().OnList(list).DescribedWith("xyz").DescribedWith("xyz");

            //When: Nach Karten die einen bestimmten Suchbegriff enthalten gesucht wird
            IPage<Card> foundCards = CardDao.FindCardsForUser(PageRequest.All, user, " " + SEARCH_TERM + " ");

            //Then: Muss die Karte gefunden werden.
            foundCards.Should().Contain(cardWithSearchTerm);
            foundCards.Should().NotContain(cardWithoutSearchTerm);
        }

        /// <summary>
        ///     Testet, dass eine Karte gefunden wird, die den Suchbegriff ohne führende und nachfolgende Leerzeichen innerhalb des
        ///     Titels enthält.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserShouldFindCardContainingTrimmedSearchTermInTitle() {
            //Given: Eine für den Nutzer findbare Karte, die den Suchbegriff am Ende des Titels enthält 
            User user = Create.User();
            const string SEARCH_TERM = "Karte";
            Board board = Create.Board().Public().Build();
            List list = Create.List().OnBoard(board).Build();
            Card cardWithSearchTerm = Create.Card().OnList(list).WithTitle("Eine" + SEARCH_TERM + "die gefunden werden sollte.");
            Card cardWithoutSearchTerm = Create.Card().OnList(list).WithTitle("xyz").DescribedWith("xyz");

            //When: Nach Karten die einen bestimmten Suchbegriff enthalten gesucht wird
            IPage<Card> foundCards = CardDao.FindCardsForUser(PageRequest.All, user, " " + SEARCH_TERM + " ");

            //Then: Muss die Karte gefunden werden.
            foundCards.Should().Contain(cardWithSearchTerm);
            foundCards.Should().NotContain(cardWithoutSearchTerm);
        }

        /// <summary>
        ///     Testet, dass eine Karte gefunden wird, die den Suchbegriff am Anfang der Beschreibung hat.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserShouldFindCardWithSearchTermAtBeginningOfDescription() {
            //Given: Eine für den Nutzer findbare Karte, die den Suchbegriff am Anfang der Beschreibung enthält 
            User user = Create.User();
            const string SEARCH_TERM = "Karte";
            Board board = Create.Board().Public().Build();
            List list = Create.List().OnBoard(board).Build();
            Card cardWithSearchTerm = Create.Card().OnList(list).DescribedWith(SEARCH_TERM + " die gefunden werden soll");
            Card cardWithoutSearchTerm = Create.Card().OnList(list).DescribedWith("xyz").DescribedWith("xyz");

            //When: Nach Karten die einen bestimmten Suchbegriff enthalten gesucht wird
            IPage<Card> foundCards = CardDao.FindCardsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss die Karte gefunden werden.
            foundCards.Should().Contain(cardWithSearchTerm);
            foundCards.Should().NotContain(cardWithoutSearchTerm);
        }

        /// <summary>
        ///     Testet, dass eine Karte gefunden wird, die den Suchbegriff am Anfang des Titels hat.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserShouldFindCardWithSearchTermAtBeginningOfTitle() {
            //Given: Eine für den Nutzer findbare Karte, die den Suchbegriff am Anfang des Titels enthält 
            User user = Create.User();
            const string SEARCH_TERM = "Karte";
            Board board = Create.Board().Public().Build();
            List list = Create.List().OnBoard(board).Build();
            Card cardWithSearchTerm = Create.Card().OnList(list).WithTitle(SEARCH_TERM + " die gefunden werden soll");
            Card cardWithoutSearchTerm = Create.Card().OnList(list).WithTitle("xyz").DescribedWith("xyz");

            //When: Nach Karten die einen bestimmten Suchbegriff enthalten gesucht wird
            IPage<Card> foundCards = CardDao.FindCardsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss die Karte gefunden werden.
            foundCards.Should().Contain(cardWithSearchTerm);
            foundCards.Should().NotContain(cardWithoutSearchTerm);
        }

        /// <summary>
        ///     Testet, dass eine Karte gefunden wird, die den Suchbegriff am Ende der Beschreibung hat.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserShouldFindCardWithSearchTermAtEndingOfDescription() {
            //Given: Eine für den Nutzer findbare Karte, die den Suchbegriff am Ende der Beschreibung enthält 
            User user = Create.User();
            const string SEARCH_TERM = "Karte";
            Board board = Create.Board().Public().Build();
            List list = Create.List().OnBoard(board).Build();
            Card cardWithSearchTerm = Create.Card().OnList(list).DescribedWith("Zu findende " + SEARCH_TERM);
            Card cardWithoutSearchTerm = Create.Card().OnList(list).DescribedWith("xyz").DescribedWith("xyz");

            //When: Nach Karten die einen bestimmten Suchbegriff enthalten gesucht wird
            IPage<Card> foundCards = CardDao.FindCardsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss die Karte gefunden werden.
            foundCards.Should().Contain(cardWithSearchTerm);
            foundCards.Should().NotContain(cardWithoutSearchTerm);
        }

        /// <summary>
        ///     Testet, dass eine Karte gefunden wird, die den Suchbegriff am Ende des Titels hat.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserShouldFindCardWithSearchTermAtEndingOfTitle() {
            //Given: Eine für den Nutzer findbare Karte, die den Suchbegriff am Ende des Titels enthält 
            User user = Create.User();
            const string SEARCH_TERM = "Karte";
            Board board = Create.Board().Public().Build();
            List list = Create.List().OnBoard(board).Build();
            Card cardWithSearchTerm = Create.Card().OnList(list).WithTitle("Zu findende " + SEARCH_TERM);
            Card cardWithoutSearchTerm = Create.Card().OnList(list).WithTitle("xyz").DescribedWith("xyz");

            //When: Nach Karten die einen bestimmten Suchbegriff enthalten gesucht wird
            IPage<Card> foundCards = CardDao.FindCardsForUser(PageRequest.All, user, SEARCH_TERM);

            //Then: Muss die Karte gefunden werden.
            foundCards.Should().Contain(cardWithSearchTerm);
            foundCards.Should().NotContain(cardWithoutSearchTerm);
        }

        /// <summary>
        ///     Testet, dass keine archivierten Karten gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserShouldNotReturnArchivedCards() {
            //Given: Eine archivierte Karte, auf einem öffentlichen Board
            User user = Create.User();
            Board board = Create.Board().Public().Build();
            List list = Create.List().OnBoard(board).Build();
            Card archivedCard = Create.Card().OnList(list).ArchivedAt(DateTime.UtcNow);

            //When: Die Karten für den Nutzer abgerufen werden
            IPage<Card> allFoundCards = CardDao.FindCardsForUser(PageRequest.All, user, null);

            //Then: Darf die Karte nicht gefunden werden
            allFoundCards.Should().NotContain(archivedCard);
        }

        /// <summary>
        ///     Testet, dass keine Karten auf archivierten Boards gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserShouldNotReturnCardsOnArchivedBoard() {
            //Given: Eine Karte, auf einem öffentlichen archivierten Board
            Board archivedBoard = Create.Board().Public().ArchivedAt(DateTime.UtcNow).Build();
            List listOnArchiveBoard = Create.List().OnBoard(archivedBoard).Build();
            Card cardOnArchivedBoard = Create.Card().OnList(listOnArchiveBoard);

            //When: Die Karten für den Nutzer abgerufen werden
            IPage<Card> allFoundCards = CardDao.FindCardsForUser(PageRequest.All, archivedBoard.CreatedBy, null);

            //Then: Darf die Karte nicht gefunden werden
            allFoundCards.Should().NotContain(cardOnArchivedBoard);
        }

        /// <summary>
        ///     Testet, dass keine Karten auf archivierten Listen gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserShouldNotReturnCardsOnArchivedList() {
            //Given: Eine Karte, in einer archivierten Liste auf einem öffentlichen Board
            Board publicBoard = Create.Board().Public().Build();
            List archivedList = Create.List().OnBoard(publicBoard).ArchivedAt(DateTime.UtcNow).Build();
            Card cardOnArchivedList = Create.Card().OnList(archivedList);

            //When: Die Karten für den Nutzer abgerufen werden
            IPage<Card> allFoundCards = CardDao.FindCardsForUser(PageRequest.All, publicBoard.CreatedBy, null);

            //Then: Darf die Karte nicht gefunden werden
            allFoundCards.Should().NotContain(cardOnArchivedList);
        }

        /// <summary>
        ///     Testet, dass keine Karten auf Boards gefunden werden, auf welcher der Nutzer keinen Zugriff hat.
        /// </summary>
        [TestMethod]
        public void TestFindCardForUserShouldNotReturnCardsOnNotAccessableBoard() {
            //Given: Eine Karte, auf einem nicht öffentlichen, aktiven Board, auf welches der abfragende Nutzer keinen Zugriff hat.
            Board restrictedBoard = Create.Board().Restricted().Build();
            List listOnRestrictedBoard = Create.List().OnBoard(restrictedBoard).Build();
            Card cardOnRestrictedBoard = Create.Card().OnList(listOnRestrictedBoard);
            User user = Create.User();

            //When: Die Karten für den Nutzer abgerufen werden
            IPage<Card> allFoundCards = CardDao.FindCardsForUser(PageRequest.All, user, null);

            //Then: Darf die Karte nicht gefunden werden
            allFoundCards.Should().NotContain(cardOnRestrictedBoard);
        }

        /// <summary>
        ///     Testet das keine archivierten Karten geliefert werden
        /// </summary>
        [TestMethod]
        public void TestFindCardsOnBoardForUsersShouldNotReturnArchivedCards() {
            //Given: Eine archivierte Karte der ein Nutzer zugewiesen ist
            User user = Create.User();
            Card archivedCard = Create.Card().ArchivedAt(DateTime.UtcNow).WithAssignedUsers(user);

            //When: Die Karten für Nutzer auf einem Board abgerufen werden sollen
            IList<Card> foundCardsOnBoardForUsers = CardDao.FindCardsOnBoardForUsers(archivedCard.List.Board, user);

            //Then: Darf die Karte nicht gefunden werden
            foundCardsOnBoardForUsers.Should().NotContain(archivedCard);
        }

        /// <summary>
        ///     Testet das keine Karten von archivierten Listen geliefert werden
        /// </summary>
        [TestMethod]
        public void TestFindCardsOnBoardForUsersShouldNotReturnCardsOfArchivedLists() {
            //Given: Eine Karte der ein Nutzer zugewiesen ist, auf einer archivierten Liste
            User user = Create.User();
            List archivedList = Create.List().ArchivedAt(DateTime.UtcNow).Build();
            Card cardOnArchivedList = Create.Card().OnList(archivedList).WithAssignedUsers(user);

            //When: Die Karten für Nutzer auf einem Board abgerufen werden sollen
            IList<Card> foundCardsOnBoardForUsers = CardDao.FindCardsOnBoardForUsers(archivedList.Board, user);

            //Then: Darf die Karte nicht gefunden werden
            foundCardsOnBoardForUsers.Should().NotContain(cardOnArchivedList);
        }

        /// <summary>
        ///     Testet das alle nicht archivierten Karten des Boards geliefert werden, wenn keine Nutzer bei der Suche nach Karten
        ///     auf dem Board übergeben werden.
        /// </summary>
        [TestMethod]
        public void TestFindCardsOnBoardForUsersShoulReturnAllCardsWhenNoUsersSpecified() {
            //Given: Ein Board mit mehreren Karten und noch ein anderes Board mit einer Karte
            Board board = Create.Board().Build();
            List list1 = Create.List().OnBoard(board).Build();
            Card card1OnList1 = Create.Card().OnList(list1);
            Card card2OnList1 = Create.Card().OnList(list1);
            Card card3OnList1 = Create.Card().OnList(list1);
            List list2 = Create.List().OnBoard(board).Build();

            List list3 = Create.List().OnBoard(board).Build();
            Card card1OnList3 = Create.Card().OnList(list3);
            Card card2OnList3 = Create.Card().OnList(list3);

            Board otherBoard = Create.Board().Build();
            List listOnOtherBoard = Create.List().OnBoard(otherBoard).Build();
            Card cardOnOtherBoard = Create.Card().OnList(listOnOtherBoard);

            //When: Nach Karten auf dem Board ohne Einschränkung der Nutzer gesucht werden soll
            IList<Card> foundCardsOnBoardForUsers = CardDao.FindCardsOnBoardForUsers(board);

            //Then: Müssen alle nicht archivierten Karten auf dem Board geliefert werden
            foundCardsOnBoardForUsers.Should().BeEquivalentTo(card1OnList1, card2OnList1, card3OnList1, card1OnList3, card2OnList3);
            foundCardsOnBoardForUsers.Should().NotContain(new[] { cardOnOtherBoard });
        }

        /// <summary>
        ///     Testet, dass beim Abrufen der Karten eines Boards für einen Nutzer, alle nicht archivierten Karten des Boards
        ///     geliefert werden, denen ein Nutzer zugewiesen ist.
        ///     TODO: Dieser "große" Test sollte in mehrere kleine Tests aufgeteilt werden. Ein Test der prüft ob die Karte
        ///     geliefert wird, wenn einer der Nutzer zugewiesen ist. Ein Test, ob die Karte geliefert wird, wenn beide zugewiesen
        ///     sind. Usw.
        /// </summary>
        [TestMethod]
        public void TestFindCardsOnBoardForUsersShoulReturnCardsWhereAtLeastOneOfUsersIsAssigned() {
            //Given: Ein Board mit mehreren Karten und noch ein anderes Board mit einer Karte
            User user1 = Create.User();
            User user2 = Create.User();

            Board board = Create.Board().Build();
            List list1 = Create.List().OnBoard(board).Build();
            Card card1OnList1 = Create.Card().WithTitle("Karate mit user1").OnList(list1).WithAssignedUsers(user1);
            Card card2OnList1 = Create.Card().WithTitle("Karate mit user1 und user2").OnList(list1).WithAssignedUsers(user1, user2);
            Card card3OnList1 = Create.Card().WithTitle("Karate ohne Nutzer").OnList(list1);
            Card card4OnList1 = Create.Card().WithTitle("Karate mit user2").OnList(list1).WithAssignedUsers(user2);

            List list2 = Create.List().OnBoard(board).Build();
            Card card1OnList2 = Create.Card().WithTitle("Karate mit user1 und user2 auf zweiter Liste").OnList(list2).WithAssignedUsers(user1, user2);

            Board otherBoard = Create.Board().Build();
            List listOnOtherBoard = Create.List().OnBoard(otherBoard).Build();
            Card cardOnOtherBoard = Create.Card().WithTitle("Karate auf anderem Board mit user1").OnList(listOnOtherBoard).WithAssignedUsers(user1);

            //When: Nach Karten auf dem Board ohne Einschränkung der Nutzer gesucht werden soll
            IList<Card> foundCardsOnBoardForUsers = CardDao.FindCardsOnBoardForUsers(board, user1, user2);

            //Then: Müssen alle nicht archivierten Karten auf dem Board geliefert werden
            foundCardsOnBoardForUsers.Should().BeEquivalentTo(card1OnList1, card2OnList1, card4OnList1, card1OnList2);
            foundCardsOnBoardForUsers.Should().NotContain(new[] { card3OnList1, cardOnOtherBoard });
        }

        /// <summary>
        ///     Testet, dass beim Abrufen der Karten eines Boards für einen Nutzer, alle nicht archivierten Karten des Boards
        ///     geliefert werden, denen ein Nutzer zugewiesen ist.
        /// </summary>
        [TestMethod]
        public void TestFindCardsOnBoardForUsersShoulReturnCardsWhereAtLeastTheUserAssigned() {
            //Given: Ein Board mit mehreren Karten und noch ein anderes Board mit einer Karte
            User user1 = Create.User();
            User user2 = Create.User();

            Board board = Create.Board().Build();
            List list1 = Create.List().OnBoard(board).Build();
            Card card1OnList1 = Create.Card().OnList(list1).WithAssignedUsers(user1);
            Card card2OnList1 = Create.Card().OnList(list1).WithAssignedUsers(user1, user2);
            Card card3OnList1 = Create.Card().OnList(list1);

            List list2 = Create.List().OnBoard(board).Build();
            Card card1OnList2 = Create.Card().OnList(list2).WithAssignedUsers(user1, user2);

            Board otherBoard = Create.Board().Build();
            List listOnOtherBoard = Create.List().OnBoard(otherBoard).Build();
            Card cardOnOtherBoard = Create.Card().OnList(listOnOtherBoard).WithAssignedUsers(user1);

            //When: Nach Karten auf dem Board ohne Einschränkung der Nutzer gesucht werden soll
            IList<Card> foundCardsOnBoardForUsers = CardDao.FindCardsOnBoardForUsers(board, user1);

            //Then: Müssen alle nicht archivierten Karten auf dem Board geliefert werden
            foundCardsOnBoardForUsers.Should().BeEquivalentTo(card1OnList1, card2OnList1, card1OnList2);
            foundCardsOnBoardForUsers.Should().NotContain(new[] { card3OnList1, cardOnOtherBoard });
        }

        /// <summary>
        ///     Testet das Speichern der zugewiesenen Nutzer
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestPersistAssignedUsers() {
            //Given: Eine bisher nicht persistierte Karte und zwei persistierte Nutzer.
            List list = Create.List().Build();
            EntityCreatedDto entityCreatedDto = new EntityCreatedDto(Create.User(), new DateTime(2017, 08, 08, 08, 08, 08));
            Card card = new Card(list, "Titel", "Description", null, new List<Label>(), entityCreatedDto);
            list.Cards.Add(card);
            User user1 = Create.User();
            User user2 = Create.User();

            //When: Die Nutzer der Karte zugewiesen werden und die Karte gespeichert wird
            card.AssignUser(user1);
            card.AssignUser(user2);
            CardDao.Save(card);
            CardDao.FlushAndClear();

            //Then: Müssen beim Neuladen der Karte die Nutzer weiterhin zugewiesen sein.
            CardDao.Get(card.Id).AssignedUsers.Should().BeEquivalentTo(user1, user2);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestRemoveLabelFromCard() {
            // Given: 
            Card card = Create.Card().Build();
            Label label = Create.Label().Build();

            card.AddLabel(label);
            CardDao.Save(card);
            CardDao.FlushAndClear();

            // When: 
            card.Labels.Remove(label);
            CardDao.Save(card);
            CardDao.FlushAndClear();

            Card reloaded = CardDao.Get(card.Id);

            // Then: 
            Assert.AreEqual(0, reloaded.Labels.Count);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestRemoveLabelFromCardWhileOnOtherCard() {
            // Given: 

            Label label = Create.Label().Build();

            Card card1 = Create.Card().WithLabels(label).Build();
            Card card2 = Create.Card().WithLabels(label).Build();

            // When: 
            card1.Labels.Remove(label);
            CardDao.Save(card1);
            CardDao.FlushAndClear();

            Card reloaded = CardDao.Get(card1.Id);
            Card reloaded2 = CardDao.Get(card2.Id);

            // Then: 
            Assert.AreEqual(0, reloaded.Labels.Count);
            Assert.AreEqual(1, reloaded2.Labels.Count);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestSaveGet() {
            // Given: 
            List list = Create.List().Build();
            DateTime dueExpirationNotificationAt = new DateTime(2017, 2, 3, 4, 0, 0);
            DateTime due = new DateTime(2017, 2, 3, 4, 5, 6);
            Card card = Create.Card()
                .OnList(list)
                .ArchivedAt(DateTime.UtcNow)
                .Due(due)
                .WithDueNotificationDoneAt(dueExpirationNotificationAt)
                .Build();

            // When: 
            Card reloaded = CardDao.Get(card.Id);

            // Then: 
            Assert.AreEqual(reloaded, card);
            Assert.AreEqual(new DateTime(2017, 2, 3, 4, 5, 6).ToUniversalTime(), reloaded.Due);
            Assert.IsTrue(reloaded.IsArchived);
            reloaded.Due.Should().HaveValue();
            reloaded.Due.Value.Kind.Should().Be(DateTimeKind.Utc);

            reloaded.DueExpirationNotificationCreated.Should().BeTrue();
            reloaded.DueExpirationNotificationCreatedAt.Should().HaveValue();
            reloaded.DueExpirationNotificationCreatedAt.Value.Kind.Should().Be(DateTimeKind.Utc);
        }

        /// <summary>
        /// Testet das Suchen nach Karten, deren Fälligkeit abgelaufen ist und für die noch keine Benachrichtigung erstellt wurde.
        /// </summary>
        [TestMethod]
        public void TestFindCardsWithExpiredDueAndWithoutNotifications() {

            //Given: Eine Karte, deren Fälligkeit abgelaufen ist, worüber aber noch keine Benachrichtigung erfolgte
            DateTime dueLimit = new DateTime(2017, 08, 09, 10, 11, 12);
            Card expiredCard = Create.Card().Due(dueLimit.AddSeconds(-1));

            //When: Nach Karten gesucht wird, deren Fälligkeit abgelaufen ist und für die noch keine Benachrichtigung erfolgte
            IList<Card> foundExpiredCards = CardDao.FindCardsWithExpiredDueAndWithoutNotifications(dueLimit);

            //Then: Muss diese Karte gefunden werden.
            foundExpiredCards.Should().Contain(expiredCard);

        }

        /// <summary>
        /// Testet das beim Suchen nach Karten, deren Fälligkeit abgelaufen ist und für die noch keine Benachrichtigung erstellt wurde, Karten nicht gefunden werden, für die schon eine Benachrichtigung erfolgte.
        /// </summary>
        [TestMethod]
        public void TestFindCardsWithExpiredDueAndWithoutNotificationsShouldNotFindCardsWithDueNotificationDone() {

            //Given: Eine Karte, deren Fälligkeit abgelaufen ist, aber bereits eine Benachrichtigung darüber erfolgte
            DateTime dueLimit = new DateTime(2017, 08, 09, 10, 11, 12);
            Card expiredCardWithNotification = Create.Card().Due(dueLimit.AddSeconds(-1)).WithDueNotificationDoneAt();

            //When: Nach Karten gesucht wird, deren Fälligkeit abgelaufen ist und für die noch keine Benachrichtigung erfolgte
            IList<Card> foundExpiredCards = CardDao.FindCardsWithExpiredDueAndWithoutNotifications(dueLimit);

            //Then: Darf  diese Karte NICHT gefunden werden.
            foundExpiredCards.Should().NotContain(expiredCardWithNotification);

        }


        /// <summary>
        /// Testet das beim Suchen nach Karten, deren Fälligkeit abgelaufen ist und für die noch keine Benachrichtigung erstellt wurde, Karten nicht gefunden werden, deren Ablaufdatum nach der Grenze ist.
        /// </summary>
        [TestMethod]
        public void TestFindCardsWithExpiredDueAndWithoutNotificationsShouldNotFindCardsWithDueAfterDueLimit() {

            //Given: Eine Karte, deren Fälligkeit noch nicht abgelaufen ist
            DateTime dueLimit = new DateTime(2017, 08, 09, 10, 11, 12);
            Card notExpiredCard = Create.Card().Due(dueLimit.AddSeconds(+1));

            //When: Nach Karten gesucht wird, deren Fälligkeit abgelaufen ist und für die noch keine Benachrichtigung erfolgte
            IList<Card> foundExpiredCards = CardDao.FindCardsWithExpiredDueAndWithoutNotifications(dueLimit.ToUniversalTime());

            //Then: Darf  diese Karte NICHT gefunden werden.
            foundExpiredCards.Should().NotContain(notExpiredCard);
        }


        /// <summary>
        /// Testet das beim Suchen nach Karten, deren Fälligkeit abgelaufen ist und für die noch keine Benachrichtigung erstellt wurde, Karten nicht gefunden werden, die archiviert sind.
        /// </summary>
        [TestMethod]
        public void TestFindCardsWithExpiredDueAndWithoutNotificationsShouldNotFindArchivedCards() {

            //Given: Eine archivierte Karte, deren Fälligkeit abgelaufen ist
            DateTime dueLimit = new DateTime(2017, 08, 09, 10, 11, 12);
            Card archivedExpiredCard = Create.Card().Due(dueLimit.AddDays(-1)).ArchivedAt(DateTime.UtcNow);

            //When: Nach Karten gesucht wird, deren Fälligkeit abgelaufen ist und für die noch keine Benachrichtigung erfolgte
            IList<Card> foundExpiredCards = CardDao.FindCardsWithExpiredDueAndWithoutNotifications(dueLimit.ToUniversalTime());

            //Then: Darf diese Karte NICHT gefunden werden.
            foundExpiredCards.Should().NotContain(archivedExpiredCard);
        }

        /// <summary>
        /// Testet das beim Suchen nach Karten, deren Fälligkeit abgelaufen ist und für die noch keine Benachrichtigung erstellt wurde, Karten nicht gefunden werden, die sich auf archivierten Boards befinden.
        /// </summary>
        [TestMethod]
        public void TestFindCardsWithExpiredDueAndWithoutNotificationsShouldNotFindCardsOnArchivedBoards() {

            //Given: Eine Karte, deren Fälligkeit abgelaufen ist und die sich auf einem archivierten Board befindet.
            Board archivedBoard = Create.Board().ArchivedAt(DateTime.UtcNow).Build();
            List listOnArchivedBoard = Create.List().OnBoard(archivedBoard).Build();
            DateTime dueLimit = new DateTime(2017, 08, 09, 10, 11, 12);
            Card expiredCardOnArchivedBoard = Create.Card().Due(dueLimit.AddDays(-1)).OnList(listOnArchivedBoard);

            //When: Nach Karten gesucht wird, deren Fälligkeit abgelaufen ist und für die noch keine Benachrichtigung erfolgte
            IList<Card> foundExpiredCards = CardDao.FindCardsWithExpiredDueAndWithoutNotifications(dueLimit.ToUniversalTime());

            //Then: Darf diese Karte NICHT gefunden werden.
            foundExpiredCards.Should().NotContain(expiredCardOnArchivedBoard);
        }

        /// <summary>
        /// Testet das beim Suchen nach Karten, deren Fälligkeit abgelaufen ist und für die noch keine Benachrichtigung erstellt wurde, Karten nicht gefunden werden, die sich auf archivierten Listen befinden.
        /// </summary>
        [TestMethod]
        public void TestFindCardsWithExpiredDueAndWithoutNotificationsShouldNotFindCardsOnArchivedList() {

            //Given: Eine Karte, deren Fälligkeit abgelaufen ist und die sich auf einer archivierten Karte befindet.
            List archivedList = Create.List().ArchivedAt(DateTime.UtcNow).Build();
            DateTime dueLimit = new DateTime(2017, 08, 09, 10, 11, 12);
            Card expiredCardOnArchivedList = Create.Card().Due(dueLimit.AddDays(-1)).OnList(archivedList);

            //When: Nach Karten gesucht wird, deren Fälligkeit abgelaufen ist und für die noch keine Benachrichtigung erfolgte
            IList<Card> foundExpiredCards = CardDao.FindCardsWithExpiredDueAndWithoutNotifications(dueLimit.ToUniversalTime());

            //Then: Darf diese Karte NICHT gefunden werden.
            foundExpiredCards.Should().NotContain(expiredCardOnArchivedList);
        }

        /// <summary>
        /// Testet, dass das Mapping, die Behandlung von UTC-Datums übernimmt
        /// </summary>
        [TestMethod]
        public void TestMappingShouldHandleUtcDates() {

            //Given: Zwei Karten deren  Erstellungszeitpunkt gleich ist, allerdings einmal in UTC und einmal nicht in UTC angegeben ist.
            DateTime creationDateInLocalTime = new DateTime(2017, 01, 02, 03, 04, 05).ToLocalTime();
            List list = Create.List().Build();
            User creator = Create.User();
            Card cardWithCreatedAtInUtc = new Card(list, "Karte in UTC", "", null, new List<Label>(), new EntityCreatedDto(creator, creationDateInLocalTime.ToUniversalTime()));
            Card cardWithCreatedAtInLocal = new Card(list, "Karte in MEZ", "", null, new List<Label>(), new EntityCreatedDto(creator, creationDateInLocalTime));
            list.Cards.Add(cardWithCreatedAtInUtc);
            list.Cards.Add(cardWithCreatedAtInLocal);

            //When: Die Karten gespeichert und anschließend wieder abgerufen werden
            CardDao.Save(new List<Card> { cardWithCreatedAtInLocal, cardWithCreatedAtInUtc });
            CardDao.FlushAndClear();
            Card reloadedCardInUtc = CardDao.Get(cardWithCreatedAtInUtc.Id);
            Card reloadedCardInLocal = CardDao.Get(cardWithCreatedAtInLocal.Id);

            //Then: Müssen die beiden Erstellungszeitpunkte gleich sein
            reloadedCardInLocal.CreatedAt.Should().Be(reloadedCardInUtc.CreatedAt);
        }


        /// <summary>
        /// Testet das auch bei Queries, die Zeitangaben in UTC verwendet werden
        /// </summary>
        [TestMethod]
        public void TestHandleUtcDatesForQueries() {

            //Given: Zwei Karten deren Erstellungszeitpunkt in UTC angegeben und gespeichert ist 
            DateTime dueLimitInLocalTime = new DateTime(2017, 01, 02, 03, 04, 05, DateTimeKind.Local);
            DateTime dueLimitInUtc = dueLimitInLocalTime.ToUniversalTime();
            List list = Create.List().Build();
            User creator = Create.User();
            Card expiredCard = new Card(list, "abgelaufene Karte ", "", dueLimitInUtc.AddSeconds(-1), new List<Label>(), new EntityCreatedDto(creator, DateTime.UtcNow));
            Card notExpiredCard = new Card(list, "nicht abgelaufene Karte", "", dueLimitInUtc.AddSeconds(1), new List<Label>(), new EntityCreatedDto(creator, DateTime.UtcNow));
            list.Cards.Add(expiredCard);
            list.Cards.Add(notExpiredCard);

            CardDao.Save(new List<Card> { expiredCard, notExpiredCard });
            CardDao.FlushAndClear();

            //When: Die Abfrage nach Karten mit abgelaufenem Fälligkeitsdatum einmal mit UTC und einmal mit Localtime erfolgt
            IList<Card> foundExpiredCardsWithLocalTime = CardDao.FindCardsWithExpiredDueAndWithoutNotifications(dueLimitInLocalTime);
            IList<Card> foundExpiredCardsWithUtc = CardDao.FindCardsWithExpiredDueAndWithoutNotifications(dueLimitInUtc);

            //Then: Muss bei beiden das gleiche Datum als Grenze verwendet werden und die gleichen Treffer erzielt werden
            foundExpiredCardsWithUtc.Should().BeEquivalentTo(foundExpiredCardsWithLocalTime);
            foundExpiredCardsWithUtc.Should().Contain(expiredCard);
            foundExpiredCardsWithUtc.Should().NotContain(notExpiredCard);

        }
    }
}