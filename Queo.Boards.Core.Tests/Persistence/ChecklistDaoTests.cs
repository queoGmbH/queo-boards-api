using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Persistence {
    [TestClass]
    public class ChecklistDaoTests : PersistenceBaseTest {

        public IChecklistDao ChecklistDao { set; private get; }
        public ITaskDao TaskDao { set; private get; }


        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestSaveGetChecklist() {

            // Given: 
            Card card = Create.Card().Build();
            Checklist checklist = Create.Checklist().OnCard(card).Build();

            // When: 
            ChecklistDao.Save(checklist);
            Checklist reloaded = ChecklistDao.Get(checklist.Id);

            // Then: 
            Assert.AreEqual(checklist, reloaded);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestGetAllChecklistsOnBoard() {

            /* Given: Board 1 */
            Board board1 = Create.Board().Build();
            List board1List1 = Create.List().OnBoard(board1).Build();
            List board1List2 = Create.List().OnBoard(board1).Build();
            List board1List3 = Create.List().ArchivedAt(DateTime.UtcNow).OnBoard(board1).Build();

            Card board1Card1 = Create.Card().OnList(board1List1).Build();
            Card board1Card2 = Create.Card().OnList(board1List2).ArchivedAt(DateTime.UtcNow).Build();
            Card board1Card3 = Create.Card().OnList(board1List3).Build();

            Checklist board1ChkList1 = Create.Checklist().OnCard(board1Card1).Build();
            Checklist board1ChkList2 = Create.Checklist().OnCard(board1Card2).Build();
            Checklist board1ChkList3 = Create.Checklist().OnCard(board1Card3).Build();

            /* Given: Board 2 */
            Board board2 = Create.Board().Build();
            List board2List1 = Create.List().OnBoard(board2).Build();

            Card board2Card1 = Create.Card().OnList(board2List1).Build();

            Checklist board2ChkList1 = Create.Checklist().OnCard(board2Card1).Build();

            // When: 
            IList<Checklist> checklists = ChecklistDao.FindAllOnBoard(board1);

            // Then: 
            Assert.AreEqual(1, checklists.Count);
            CollectionAssert.AreEquivalent(new List<Checklist>() {board1ChkList1}, checklists.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestGetAllChecklistsOnCard() {

            // Given: 
            Board board1 = Create.Board().Build();
            List list1 = Create.List().OnBoard(board1).Build();
            List list2 = Create.List().OnBoard(board1).Build();

            Card card1 = Create.Card().OnList(list1).Build();
            Card card2 = Create.Card().OnList(list2).Build();

            Checklist chkList1 = Create.Checklist().OnCard(card1).Build();
            Checklist chkList2 = Create.Checklist().OnCard(card2).Build();

            Board board2 = Create.Board().Build();
            List list3 = Create.List().OnBoard(board2).Build();

            Card card3 = Create.Card().OnList(list3).Build();

            Checklist chkList3 = Create.Checklist().OnCard(card3).Build();

            // When: 
            IList<Checklist> checklistsOnCard2 = ChecklistDao.FindAllOnCard(card2);

            // Then: 
            CollectionAssert.AreEquivalent(new List<Checklist>() {chkList2}, checklistsOnCard2.ToList() );
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestDeleteTaskFromChecklist()
        {

            // Given: 
            Task task = Create.Task().Build();
            Task task2 = Create.Task().OnChecklist(task.Checklist).Build();

            // When: 
            TaskDao.Delete(task);
            TaskDao.FlushAndClear();
            Checklist checklist = ChecklistDao.Get(task2.Checklist.Id);


            // Then: 
            Assert.AreEqual(task2, checklist.Tasks.Single());
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestDeleteChecklistOnCardShouldDeleteItemsOnList() {

            // Given: 
            Card card = Create.Card().Build();
            Checklist checklist = Create.Checklist().OnCard(card).Build();
            Task task = Create.Task().OnChecklist(checklist).Build();

            // When: 
            ChecklistDao.Delete(checklist);
            ChecklistDao.FlushAndClear();

            // Then: 
            Action getDeletedTask = ()=>TaskDao.Get(task.Id);
            getDeletedTask.ShouldThrow<ObjectNotFoundException>();

            Action getDeletedChecklist = ()=>ChecklistDao.Get(checklist.Id);
            getDeletedChecklist.ShouldThrow<ObjectNotFoundException>();
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestChecklistContainsTasks() {

            // Given: 
            Task task = Create.Task().Build();

            // When: 
            Checklist checklist = ChecklistDao.Get(task.Checklist.Id);

            // Then: 
            Assert.AreEqual(task, checklist.Tasks.Single());
        }
    }
}