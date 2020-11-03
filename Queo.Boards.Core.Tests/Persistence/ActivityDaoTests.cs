using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Activities;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Persistence {
    [TestClass]
    public class ActivityDaoTests : PersistenceBaseTest{

        public IActivityBaseDao ActivityDao { set; private get; }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory("integration")]
        [Ignore]
        // TODO: Test schlägt fehl, weil der SchemaCreator die Board_Id Spalte der tblActivityBase auf NOT NULL setzt
        public void TestSaveAndGetActivity() {

            // Given: 
            User creator = Create.User().Build();
            ActivityBase activity = Create.Activity().Creator(creator).BuildBase();
            
            // When: 
            ActivityBase reloaded = ActivityDao.Get(activity.Id);

            // Then: 
            Assert.AreEqual(activity.Creator, reloaded.Creator);
        }
    }
}