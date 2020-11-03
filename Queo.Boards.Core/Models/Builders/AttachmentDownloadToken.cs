using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;

namespace Queo.Boards.Core.Models.Builders {
    public class AttachmentDownloadToken : SecurityToken {

        public AttachmentDownloadToken(DateTime validFrom, DateTime validTo, AttachmentDownload attachmentDownload) {
            _id = Guid.NewGuid().ToString();
            ValidFrom = validFrom;
            ValidTo = validTo;
            _attachmentDownload = attachmentDownload;
        }

        private readonly string _id;
        private readonly AttachmentDownload _attachmentDownload;

        /// <summary>Ruft einen eindeutigen Bezeichner f�r das Sicherheitstoken ab.</summary>
        /// <returns>Der eindeutige Bezeichner des Sicherheitstokens.</returns>
        /// <filterpriority>2</filterpriority>
        public override string Id {
            get { return _id; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys { get; }

        /// <summary>Ruft den fr�hesten Zeitpunkt ab, zu dem dieses Sicherheitstoken g�ltig ist.</summary>
        /// <returns>Ein <see cref="T:System.DateTime" />, der den Zeitpunkt darstellt, zu dem dieses Sicherheitstoken zuerst g�ltig ist.</returns>
        /// <filterpriority>2</filterpriority>
        public override DateTime ValidFrom { get; }

        /// <summary>Ruft den sp�test m�glichen Zeitpunkt ab, zu dem dieses Sicherheitstoken g�ltig ist.</summary>
        /// <returns>Ein <see cref="T:System.DateTime" />, der den Zeitpunkt darstellt, zu dem dieses Sicherheitstoken letztmalig g�ltig ist.</returns>
        /// <filterpriority>2</filterpriority>
        public override DateTime ValidTo { get; }

        public AttachmentDownload AttachmentDownload {
            get { return _attachmentDownload; }
        }
    }
}