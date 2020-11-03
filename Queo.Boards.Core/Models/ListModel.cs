using System;
using Queo.Boards.Core.Models.BreadCrumbModels;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model einer Liste
    /// </summary>
    public class ListModel : EntityModel {
        private readonly BoardBreadCrumbModel _board;
        private readonly int _positionOnBoard;
        private readonly DateTime? _archivedAt;
        private readonly string _title;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public ListModel(Guid businessId, BoardBreadCrumbModel board, string title, int position, DateTime? archivedAt)
            : base(businessId) {
            _board = board;
            _title = title;
            _positionOnBoard = position;
            _archivedAt = archivedAt;
        }

        /// <summary>
        ///     Breadcrumb für das Board, dem die Liste zugeordnet ist.
        /// </summary>
        public BoardBreadCrumbModel Board {
            get { return _board; }
        }

        /// <summary>
        ///     Liefert die Position der Liste auf dem Board
        /// </summary>
        public int PositionOnBoard {
            get { return _positionOnBoard; }
        }

        /// <summary>
        ///     Titel der Liste
        /// </summary>
        public string Title {
            get { return _title; }
        }

        /// <summary>
        /// Ruft das Archivierungsdatum der Liste ab oder null, wenn die Liste nicht archiviert ist.
        /// </summary>
        public DateTime? ArchivedAt {
            get { return _archivedAt; }
        }
    }
}