using System;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     DTO mit einer Zusammenfassung eines Boards
    /// </summary>
    public class BoardSummaryModel : EntityModel {
        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public BoardSummaryModel(Guid businessId, string title, Accessibility accessibility, string colorScheme, bool isPrivate, DateTime creationDateTime, bool isTemplate, bool isArchived, DateTime? archivedAt)
            : base(businessId) {
            Title = title;
            Accessibility = accessibility;
            ColorScheme = colorScheme;
            IsPrivate = isPrivate;
            CreatedAt = creationDateTime;
            IsArchived = isArchived;
            ArchivedAt = archivedAt;
            IsTemplate = isTemplate;
        }

        /// <summary>
        ///     Liefert die Sichtbarkeit
        /// </summary>
        public Accessibility Accessibility { get; private set; }

        /// <summary>
        ///     Ruft ab, wann das Board archiviert wurde oder null, wenn das Board nicht archiviert ist.
        /// </summary>
        public DateTime? ArchivedAt { get; private set; }

        /// <summary>
        ///     Liefert das Farbschema
        /// </summary>
        public string ColorScheme { get; private set; }

        /// <summary>
        ///     Ruft ab, ob das Board archiviert ist.
        /// </summary>
        public bool IsArchived { get; private set; }

        /// <summary>
        ///     Ruft ab, ob es sich um eine Board-Vorlage handelt.
        /// </summary>
        public bool IsTemplate {
            get; private set;
        }

        /// <summary>
        ///     Liefert ob es sich um ein privates Board handelt. Hierbei ist der Eigentümer das einzige Board Mitglied.
        /// </summary>
        public bool IsPrivate { get; private set; }

        /// <summary>
        /// Ruft den Zeitpunkt der Erstellung des Boards ab.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        ///     Liefert den Titel
        /// </summary>
        public string Title { get; private set; }
    }
}