using System;
using System.IO;
using System.Threading;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Persistence.Impl {
    /// <summary>
    ///     Dao für Zugriffe aufs Dateisystem
    /// </summary>
    public class FileSystemDao : IFileSystemDao {
        /// <summary>
        ///     Setzt den Dokumentenordner Basispfad
        /// </summary>
        public string DocumentBasePath { set; private get; }

        /// <summary>
        ///     Löscht ein Document
        /// </summary>
        /// <param name="document"></param>
        public void DeleteDocument(Document document) {
            string documentPath = GetDocumentPath(document);
            FileInfo documentFile = new FileInfo(documentPath);
            WaitForFileToBeReady(documentFile);
            if (documentFile.Exists) {
                documentFile.Delete();
            }
        }

        /// <summary>
        ///     Liefert die Dateiinfo zu einem Dokument
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public FileInfo GetFileInfoForDocument(Document document) {
            return new FileInfo(GetDocumentPath(document));
        }

        /// <summary>
        ///     Liefert eien FileStream zu einem Dokument
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public FileStream GetStreamFor(Document document) {
            string cardPath = GetCardPath(document.Card);
            string documentPath = Path.Combine(cardPath, document.BusinessId.ToString());
            return new FileStream(documentPath, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        ///     Speichert ein temporäres Dokument an seinen finalen Ablageort
        /// </summary>
        /// <param name="document"></param>
        /// <param name="tempFileName"></param>
        /// <returns></returns>
        public FileInfo SaveTempDocumentToFinalDestination(Document document, string tempFileName) {
            FileInfo tempFile = new FileInfo(tempFileName);
            string finalDirectory = GetCardPath(document.Card);
            if (!Directory.Exists(finalDirectory)) {
                Directory.CreateDirectory(finalDirectory);
            }
            FileInfo finalFile = tempFile.CopyTo(GetDocumentPath(document));
            return finalFile;
        }

        private string GetCardPath(Card card) {
            return Path.Combine(DocumentBasePath,
                card.List.Board.BusinessId.ToString(),
                card.BusinessId.ToString());
        }

        private string GetDocumentPath(Document document) {
            return Path.Combine(GetCardPath(document.Card), document.BusinessId.ToString());
        }

        private bool IsFileReady(FileInfo sFilename) {
            /* http://stackoverflow.com/questions/1406808/wait-for-file-to-be-freed-by-process */
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try {
                using (FileStream inputStream = File.Open(sFilename.FullName, FileMode.Open, FileAccess.Read, FileShare.None)) {
                    return inputStream.Length > 0;
                }
            } catch (Exception) {
                return false;
            }
        }

        private void WaitForFileToBeReady(FileInfo file) {
            for (int i = 0; i < 50; i++) {
                if (!IsFileReady(file)) {
                    Thread.Sleep(100);
                } else {
                    break;
                }
            }
        }
    }
}