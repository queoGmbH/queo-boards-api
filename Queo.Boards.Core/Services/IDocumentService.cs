using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Services {
    /// <summary>
    ///     Schnittstelle für Services run um Dokumente
    /// </summary>
    public interface IDocumentService {
        /// <summary>
        ///     Erstellt ein Ahnang zu einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <param name="tempFileName"></param>
        /// <param name="originalFileName"></param>
        /// <returns></returns>
        Document CreateDocumentAtCard(Card card, string tempFileName, string originalFileName);

        /// <summary>
        ///     Liefert alle Dokumente einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        IList<Document> FindAllOnCard(Card card);

        /// <summary>
        ///     Liefert einen FileStream zu einem Dokument
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        FileStream GetDocumentStream(Document document);

        /// <summary>
        ///     Liefert ein THumbnail zu einem Dokument
        /// </summary>
        /// <param name="document"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        Task<FileStream> GetThumbnailForDocumentAsync(Document document, int width, int height);

        /// <summary>
        /// Löscht ein Dokument einer Karte
        /// </summary>
        /// <param name="documentToDelete"></param>
        /// <returns></returns>
        Document DeleteDocument(Document documentToDelete);

        Document GetByBusinessId(Guid businessId);

        /// <summary>
        /// Prüft ob die Dateiendung für den Upload erlaubt ist
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool CanUploadFileWithExtension(string fileName);
    }
}