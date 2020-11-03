using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Models.Builders;

namespace Queo.Boards.Core.Tests.Infrastructure {

    [TestClass]
    public class AttachmentDownloadTokenizerTest {

        /// <summary>
        /// Testet das Erstellen und anschließendes Parsen des Tokens für den Download einer Datei.
        /// </summary>
        [TestMethod]
        public void TestGenerateAndParseToken() {
            //Given: Informationen über den Download eines Anhangs.
            AttachmentDownload attachmentDownload = new AttachmentDownload(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2020,12,24,15,30,20));

            //When: Ein Token für den Download erstellt und anschließend wieder geparsed werden soll
            string token = AttachmentDownloadTokenizer.GetToken(attachmentDownload);
            AttachmentDownload parsedAttachmentDownload = AttachmentDownloadTokenizer.ParseFromToken(token);

            //Then: Müssen alle Informationen noch korrekt vorhanden sein
            parsedAttachmentDownload.Should().Be(attachmentDownload);
        }


    }
}