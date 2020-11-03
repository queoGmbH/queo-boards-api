using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Persistence {
    [TestClass]
    public class DocumentDaoTest : PersistenceBaseTest{

        public IDocumentDao DocumentDao { set; private get; }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestSaveGetDocument() {

            // Given: 
            Card card = Create.Card().Build();
            Document document = Create.Document().Card(card).Build();


            // When: 
            Document reloaded = DocumentDao.Get(document.Id);

            // Then: 
            Assert.AreEqual(document, reloaded);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestFindDocumentsOnCard() {

            // Given: 
            Card card = Create.Card().Build();
            Document doc1 = Create.Document().Card(card).Build();
            Document doc2 = Create.Document().Card(card).Build();

            // When: 
            IList<Document> docsOnCard = DocumentDao.FindAllOnCard(card);

            // Then: 
            CollectionAssert.AreEquivalent(new List<Document>() {doc1, doc2}, docsOnCard.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestDeleteNonExistingDocumentShouldNotThrowException() {

            // Given: 
            Document document = Create.Document().Build();

            // When: 
            DocumentDao.Delete(document);

            // Then:
        }
    }
}