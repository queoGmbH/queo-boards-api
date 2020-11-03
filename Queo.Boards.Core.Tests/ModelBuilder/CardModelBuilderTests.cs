using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.ModelBuilder {
    [TestClass]
    public class CardModelBuilderTests : CreateBaseTest{
        
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestBuildCardModelWithoutLabelsShouldHaveEmptyLabelList() {

            // Given: 
            Card card = Create.Card().Build();

            // When: 
            CardModel model = CardModelBuilder.Build(card);

            // Then: 
            Assert.IsNotNull(model.AssignedLabels);
            Assert.AreEqual(0, model.AssignedLabels.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestBuildCardModelWithLabels() {

            // Given: 
            Card card = Create.Card().Build();
            Label label = Create.Label().Build();
            card.AddLabel(label);

            // When: 
            CardModel cardModel = CardModelBuilder.Build(card);

            // Then: 
            Assert.AreEqual(1, cardModel.AssignedLabels.Count);
        }
    }
}