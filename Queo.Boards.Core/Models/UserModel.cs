using System;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model für einen <see cref="User" />
    /// </summary>
    public class UserModel : EntityModel {
        private readonly string _firstname;
        private readonly string _lastname;
        private readonly string _name;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public UserModel(Guid businessId, string name, string firstname, string lastname)
            : base(businessId) {
            _name = name;
            _firstname = firstname;
            _lastname = lastname;
        }

        /// <summary>
        ///     Liefert den "Vornamen" - http://www.kalzumeus.com/2010/06/17/falsehoods-programmers-believe-about-names/
        /// </summary>
        public string Firstname {
            get { return _firstname; }
        }

        /// <summary>
        ///     Liefert den "Nachnamen" - http://www.kalzumeus.com/2010/06/17/falsehoods-programmers-believe-about-names/
        /// </summary>
        public string Lastname {
            get { return _lastname; }
        }

        /// <summary>
        ///     Liefert den Namen
        /// </summary>
        public string Name {
            get { return _name; }
        }
    }
}