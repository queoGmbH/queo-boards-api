using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Com.QueoFlow.Thumbnailer.Shared;
using Com.QueoFlow.Thumbnailer.Shared.Options;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.Thumbnailing;
using Queo.Boards.Core.Persistence;
using Spring.Transaction.Interceptor;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service für <see cref="Document" />
    /// </summary>
    public class DocumentService : IDocumentService {
        private static readonly ConcurrentDictionary<Document, object> CreateThumbnailLocks = new ConcurrentDictionary<Document, object>();
        private readonly IDocumentDao _documentDao;
        private readonly IFileSystemDao _fileSystemDao;
        private readonly ImageThumbnailer _imageThumbnailer;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public DocumentService(IFileSystemDao fileSystemDao, IDocumentDao documentDao, ImageThumbnailer imageThumbnailer) {
            _fileSystemDao = fileSystemDao;
            _documentDao = documentDao;
            _imageThumbnailer = imageThumbnailer;
        }

        /// <summary>
        ///     Gibt die in der Konfiguration erlaubten Dateiendungen zurück (wir mit Spring gesetzt)
        /// </summary>
        public string AllowedUploadFileExtensions { private get; set; }

        /// <summary>
        ///     Prüft ob die Dateiendung für den Upload erlaubt ist
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool CanUploadFileWithExtension(string fileName) {
            Require.NotNull(fileName, nameof(fileName));

            if (AllowedUploadFileExtensions == "*") {
                return true;
            }

            foreach (string fileExtension in AllowedUploadFileExtensions.Split(',')) {
                if (fileName.ToUpper().Contains($".{fileExtension.ToUpper()}")) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Erstellt ein Anhang zu einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <param name="tempFileName"></param>
        /// <param name="originalFileName"></param>
        /// <returns></returns>
        [Transaction]
        public Document CreateDocumentAtCard(Card card, string tempFileName, string originalFileName) {
            Require.NotNull(card, nameof(card));
            Require.NotNullOrWhiteSpace(tempFileName, nameof(tempFileName));
            Require.NotNullOrWhiteSpace(originalFileName, nameof(originalFileName));

            Document document = new Document(card, originalFileName);
            FileInfo finalFile = _fileSystemDao.SaveTempDocumentToFinalDestination(document, tempFileName);
            _documentDao.Save(document);
            return document;
        }

        /// <summary>
        ///     Löscht ein Dokument einer Karte
        /// </summary>
        /// <param name="documentToDelete"></param>
        /// <returns></returns>
        [Transaction]
        public Document DeleteDocument(Document documentToDelete) {
            documentToDelete.Card.Documents.Remove(documentToDelete);
            _fileSystemDao.DeleteDocument(documentToDelete);
            _documentDao.Delete(documentToDelete);
            return documentToDelete;
        }

        /// <summary>
        ///     Liefert alle Dokumente einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public IList<Document> FindAllOnCard(Card card) {
            return _documentDao.FindAllOnCard(card);
        }

        public Document GetByBusinessId(Guid businessId) {
            return _documentDao.GetByBusinessId(businessId);
        }

        /// <summary>
        ///     Liefert einen FileStream zu einem Dokument
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public FileStream GetDocumentStream(Document document) {
            return _fileSystemDao.GetStreamFor(document);
        }

        /// <summary>
        ///     Liefert ein THumbnail zu einem Dokument
        /// </summary>
        /// <param name="document"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public async Task<FileStream> GetThumbnailForDocumentAsync(Document document, int width, int height) {
            FileInfo fileInfoForDocument = _fileSystemDao.GetFileInfoForDocument(document);
            FileInfo thumbnail;

            /*Der Lock erfolgt pro Dokument. Es kann pro Dokument jeweils nur 1 Vorschaubild erzeugt werden, jedoch können für mehrere Dokumente parallel Vorschaubilder erzeugt werden.*/
            lock (GetLockObjectFromForDocument(document)) {
                using (FileStream stream = new FileStream(fileInfoForDocument.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    StreamWrapper streamWrapper = new StreamWrapper(stream, fileInfoForDocument.Name);
                    thumbnail = _imageThumbnailer.GetThumbnailAsync(
                        streamWrapper,
                        new ConvertOptions {Height = height, Width = width, ImageFormat = ThumbnailImageFormat.Png}).Result;
                }
            }

            return thumbnail.OpenRead();
        }

        /// <summary>
        ///     Liefert für ein Dokument das verwendete Lockobjekt oder erzeugt eines, wenn für das Dokument noch keines existiert.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private static object GetLockObjectFromForDocument(Document document) {
            return CreateThumbnailLocks.GetOrAdd(document, new object());
        }
    }
}