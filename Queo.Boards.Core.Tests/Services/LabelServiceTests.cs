using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Services.Impl;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services {
    [TestClass]
    public class LabelServiceTests : CreateBaseTest{

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateLabel() {

            // Given: 
            Mock<ILabelDao> labelDaoMock = new Mock<ILabelDao>();
            labelDaoMock.Setup(x => x.Save(It.IsAny<Label>()));

            ILabelService labelService = new LabelService(labelDaoMock.Object, null);

            Board board = Create.Board().Build();
            User user = Create.User();
            // When: 
            Label created = labelService.Create(board, new LabelDto() { Name = "label 1", Color = "color 2" });

            // Then: 
            labelDaoMock.Verify(x=>x.Save(created), Times.Once);
            Assert.AreEqual("label 1", created.Name);
            Assert.AreEqual("color 2", created.Color);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestDeleteLabel() {

            // Given: 
            Label label = Create.Label().Build();
            Card card = Create.Card().WithLabels(label).Build();

            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            cardDaoMock.Setup(x => x.FindAllCardsWithLabel(label)).Returns(new List<Card>() { card });

            Mock<ILabelDao> labelDaoMock = new Mock<ILabelDao>();
            labelDaoMock.Setup(x => x.Delete(label));

            ILabelService labelService = new LabelService(labelDaoMock.Object, cardDaoMock.Object);

            // When: 
            Label deletedLabel = labelService.Delete(label);

            // Then: 
            cardDaoMock.Verify(x=>x.FindAllCardsWithLabel(label), Times.Once);
            labelDaoMock.Verify(x=>x.Delete(label), Times.Once);
            card.Labels.Count.Should().Be(0);
            deletedLabel.Should().Be(label);
        }
    }
}