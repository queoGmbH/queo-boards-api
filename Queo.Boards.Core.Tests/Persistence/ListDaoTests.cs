using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Persistence {
    [TestClass]
    public class ListDaoTests : PersistenceBaseTest {

        public IListDao ListDao { set; private get; }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestSaveGet() {

            // Given: 
            List list = Create.List().OnBoard(Create.Board().Build()).ArchivedAt(DateTime.UtcNow).Title("new list").Build();

            // When: 
            List reloaded = ListDao.Get(list.Id);

            // Then: 
            Assert.AreEqual(reloaded, list);
            Assert.IsTrue(reloaded.IsArchived);
        }

        
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestGetListWithCards() {

            // Given: 
            Card card = Create.Card().Build();

            // When: 
            List list = ListDao.Get(card.List.Id);

            // Then: 
            Assert.AreEqual(card, list.Cards[0]);
        }


        /// <summary>
        /// Testet das Löschen einer Liste
        /// </summary>
        [TestMethod]
        public void TestDeleteList() {

            //Given: Eine Liste an einem Board mit mehreren Listen
            Board board = Create.Board().Build();
            List listAtPosition0 = Create.List().OnBoard(board).Build();
            List listToDelete = Create.List().OnBoard(board).Build();
            List listAtPosition2 = Create.List().OnBoard(board).Build();

            //When: Die Liste gelöscht wird
            ListDao.Delete(listToDelete);
            ListDao.FlushAndClear();
            Action action = () => ListDao.GetByBusinessId(listToDelete.BusinessId);

            //Then: Muss sie aus der DB verschwinden und die Positionen am Board korrigiert werden.
            action.ShouldThrow<ObjectNotFoundException>();
            listAtPosition0.GetPositionOnBoard().Should().Be(0);
            listAtPosition2.GetPositionOnBoard().Should().Be(1);
        }

    
    }
}