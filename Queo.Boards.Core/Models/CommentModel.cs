using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Models.BreadCrumbModels;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model für <see cref="Comment" />
    /// </summary>
    public class CommentModel : EntityModel {
        private readonly DateTime _createdAt;
        private readonly bool _isDeleted;
        private readonly UserModel _createdBy;
        private readonly string _text;
        private readonly CardBreadCrumbModel _card;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public CommentModel(Guid businessId, UserModel createdBy, string text, DateTime createdAt, bool isDeleted, CardBreadCrumbModel card)
            : base(businessId) {
            _createdBy = createdBy;
            _text = text;
            _createdAt = createdAt;
            _isDeleted = isDeleted;
            _card = card;
        }

        /// <summary>
        ///     Liefert das Erstellungsdatum
        /// </summary>
        public DateTime CreatedAt {
            get { return _createdAt; }
        }

        /// <summary>
        ///     Liefert den Ersteller
        /// </summary>
        public UserModel CreatedBy {
            get { return _createdBy; }
        }

        /// <summary>
        ///     Liefert den Text
        /// </summary>
        public string Text {
            get { return _text; }
        }

        /// <summary>
        /// Liefert ob ein Kommentar als gelöscht markiert ist.
        /// </summary>
        public bool IsDeleted {
            get { return _isDeleted; }
        }

        /// <summary>
        /// Liefert den Breadcrumb der Karte, welcher der Kommentar zugeordnet ist wurde.
        /// </summary>
        public CardBreadCrumbModel Card {
            get { return _card; }
        }
    }
}