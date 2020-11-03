using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Models.BreadCrumbModels;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model für <see cref="Document" />
    /// </summary>
    public class AttachmentModel {
        private readonly Guid _businessId;
        private readonly CardBreadCrumbModel _card;
        private readonly string _originalFileName;
        private readonly string _documentDownloadToken;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public AttachmentModel(CardBreadCrumbModel card, Guid businessId, string originalFileName, string documentDownloadToken) {
            _card = card;
            _businessId = businessId;
            _originalFileName = originalFileName;
            _documentDownloadToken = documentDownloadToken;
        }

        /// <summary>
        ///     Liefert die Business Id des Dokuments
        /// </summary>
        public Guid BusinessId {
            get { return _businessId; }
        }

        /// <summary>
        ///     Liefert den BreadCrumb der Karte
        /// </summary>
        public CardBreadCrumbModel Card {
            get { return _card; }
        }

        /// <summary>
        /// Ruft das Token für den Download der Datei ab.
        /// 
        /// Für den Download kann der DocumentController verwendet werden.
        /// </summary>
        public string DocumentDownloadToken {
            get { return _documentDownloadToken; }
        }

        /// <summary>
        ///     Liefert den ursprünglichen Dateinamen
        /// </summary>
        public string OriginalFileName {
            get { return _originalFileName; }
        }
    }
}