using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence.Impl {
    /// <summary>
    ///     Dao für <see cref="Document" />
    /// </summary>
    public class DocumentDao : GenericDao<Document, int>, IDocumentDao {
        /// <summary>
        ///     Liefert alle Dokumente einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public IList<Document> FindAllOnCard(Card card) {
            return Session.QueryOver<Document>().Where(x => x.Card == card).List();
        }
    }
}