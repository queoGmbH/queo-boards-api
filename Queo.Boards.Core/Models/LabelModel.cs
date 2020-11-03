using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Models.BreadCrumbModels;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model für ein <see cref="Label" />
    /// </summary>
    public class LabelModel : EntityModel{
        private readonly BoardBreadCrumbModel _board;
        private readonly string _color;
        private readonly string _name;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public LabelModel(Guid businessId, string name, string color, BoardBreadCrumbModel board) : base(businessId){
            _name = name;
            _color = color;
            _board = board;
        }

        /// <summary>
        ///     Breadcrumb für das Board, dem das Label zugeordnet ist.
        /// </summary>
        public BoardBreadCrumbModel Board {
            get {
                return _board;
            }
        }

        /// <summary>
        ///     Liefert die Farbe
        /// </summary>
        public string Color {
            get { return _color; }
        }

        /// <summary>
        ///     Liefert den Namen.
        /// </summary>
        public string Name {
            get { return _name; }
        }
    }
}