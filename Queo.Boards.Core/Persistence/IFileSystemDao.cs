using System.IO;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Persistence {
    /// <summary>
    ///     Schnittstelle für Daos für Zugriffe aufs Dateisystem
    /// </summary>
    public interface IFileSystemDao {
        /// <summary>
        ///     Liefert die Dateiinfo zu einem Dokument
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        FileInfo GetFileInfoForDocument(Document document);

        /// <summary>
        ///     Liefert eien FileStream zu einem Dokument
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        FileStream GetStreamFor(Document document);

        /// <summary>
        ///     Speichert ein temporäres Dokument an seinen finalen Ablageort
        /// </summary>
        /// <param name="document"></param>
        /// <param name="tempFileName"></param>
        /// <returns></returns>
        FileInfo SaveTempDocumentToFinalDestination(Document document, string tempFileName);

        /// <summary>
        ///     Löscht ein Document
        /// </summary>
        /// <param name="document"></param>
        void DeleteDocument(Document document);
    }
}