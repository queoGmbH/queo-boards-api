using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards {
    /// <summary>
    ///     Binder der Methoden anbietet um beim Deserialisieren aus einem JSon, Instanzen von <see cref="Entity" /> anhand der
    ///     <see cref="Entity.BusinessId" /> zu "binden".
    ///     Dabei wird anhand der gefundenen Id im JSon versucht über den Dao, das Objekt oder die Objekte zu laden.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class JSonEntityBinder<TEntity> : IJsonEntityBinder
        where TEntity : Entity {
        /// <summary>
        ///     Konstruktor inklusive des Daos, der zum Laden der Objekte verwendet wird.
        /// </summary>
        /// <param name="dao">Der Dao, der zum Laden der Objekte anhand ihrer Id verwendet wird.</param>
        public JSonEntityBinder(IGenericDao<TEntity, int> dao) {
            Dao = dao;
        }

        /// <summary>
        ///     Ruft den Dao für das Laden ab oder legt diesen fest.
        /// </summary>
        private IGenericDao<TEntity, int> Dao { get; set; }

        /// <summary>
        ///     Binden bzw. versucht ein <see cref="TEntity" /> anhand seiner Id zu binden.
        /// </summary>
        /// <param name="reader">Der Reader, in dem das JSon mit der Id für das eine zu ladende Objekt enthalten ist.</param>
        /// <returns></returns>
        public object Bind(JsonReader reader) {
            if (reader == null || reader.Value == null) {
                /*Wenn kein Reader bzw. keine Id übergeben wird, kann nix geladen werden.*/
                return null;
            }

            return GetFromString(reader.Value.ToString());
        }

        /// <summary>
        ///     Bindet bzw. versucht eine Liste von <see cref="TEntity" /> anhand von Ids zu binden.
        /// </summary>
        /// <param name="reader">
        ///     Der Reader, in dem das JSon mit den Ids für das eine zu ladende Objekt enthalten ist.
        ///     <remarks>
        ///         Es wird ein <see cref="JsonReader.Read" /> ausgeführt, bis keine Ids mehr gelesen werden können.
        ///     </remarks>
        /// </param>
        /// <returns></returns>
        public object BindList(JsonReader reader) {
            if (reader == null) {
                /*Wenn kein Reader übergeben wird, kann nix geladen oder gebunden werden.*/
                return null;
            }

            IList<string> ids = new List<string>();
            while (reader.Read() && reader.Value != null) {
                /*Solange Ids gelesen werden können, werden diese gesammelt, um sie später zu verwenden.*/
                ids.Add(reader.Value.ToString());
            }

            /*Anhand der Id die Liste der Objekte laden.*/
            /*TODO 1: Was ist, wenn es kein Objekt zu einer der Ids gibt?*/
            /*TODO 2: Optimierung, dass alle Objekte mit einmal geladen werden, anstatt alle einzeln zu laden. !Achtung bei großen Mengen an Objekten => Einschränkung bei SQLServer auf max. 2000 Parameter!*/
            IList<TEntity> bindList = ids.Select(GetFromString).ToList();
            return bindList;
        }

        private TEntity GetFromString(string value) {
            if (value == null) {
                return null;
            }

            Guid businessId;
            if (Guid.TryParse(value, out businessId)) {
                /*Wenn der Wert eine Guid ist, dann versuchen, anhand dieser das Objekt zu laden.*/
                return Dao.GetByBusinessId(businessId);
            } else {
                /*Wenn es keine Guid ist, kann nix geladen werden und dann wird null geliefert.*/
                return null;
            }
        }
    }
}