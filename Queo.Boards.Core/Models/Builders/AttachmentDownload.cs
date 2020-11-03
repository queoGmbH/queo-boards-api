using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Bildet einen für einen <see cref="User">Nutzer</see> bis zu einem bestimmten Zeitpunkt bereitgestellten Download
    ///     einer <see cref="Document">Datei</see> ab.
    /// </summary>
    public class AttachmentDownload {
        public AttachmentDownload() {
        }

        public AttachmentDownload(Guid userId, Guid documentId, DateTime expirationDate) {
            UserId = userId;
            DocumentId = documentId;
            ExpirationDate = expirationDate;
        }

        public AttachmentDownload(User user, Document document, DateTime expirationDate) {
            Require.NotNull(user, "user");
            Require.NotNull(document, "document");

            UserId = user.BusinessId;
            DocumentId = document.BusinessId;
            ExpirationDate = expirationDate;
        }

        /// <summary>
        ///     Ruft die Id des Dokuments ab, das zum Download bereitgestellt werden soll.
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        ///     Ruft das Datum ab, bis zu welchem der Download möglich ist.
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        ///     Ruft die Id des Nutzers ab, für welchen der Download ermöglicht werden soll.
        /// </summary>
        public Guid UserId { get; set; }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != GetType()) {
                return false;
            }
            return Equals((AttachmentDownload)obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = UserId.GetHashCode();
                hashCode = hashCode * 397 ^ DocumentId.GetHashCode();
                hashCode = hashCode * 397 ^ ExpirationDate.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(AttachmentDownload other) {
            return UserId.Equals(other.UserId) && DocumentId.Equals(other.DocumentId) && ExpirationDate.Equals(other.ExpirationDate);
        }
    }
}