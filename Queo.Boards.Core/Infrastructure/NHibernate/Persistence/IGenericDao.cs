using System;
using System.Collections.Generic;

namespace Queo.Boards.Core.Infrastructure.NHibernate.Persistence {
    public interface IGenericDao<T, in TKey> {
        /// <summary>
        ///     Leert die Session.
        /// </summary>
        /// <remarks>
        ///     Die Methode sollte nur in Testf�llen verwendet werden.
        /// </remarks>
        void Clear();

        /// <summary>
        ///     L�scht die �bergebene Entit�t
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        ///     �berpr�ft, ob es ein Entity mit dem Prim�rschl�ssel gibt.
        /// </summary>
        /// <param name="primaryKey">Der Prim�rschl�ssel.</param>
        /// <returns>
        ///     <code>true</code>, wenn ein Entity mit dem angegebenen Prim�rschl�ssel existiert, sonst <code>false</code>
        /// </returns>
        bool Exists(TKey primaryKey);

        /// <summary>
        ///     �bernimmt alle offenen �nderungen in die Datenbank.
        /// </summary>
        /// <remarks>
        ///     Im Allgemeinen braucht diese Methode nicht aufgerufen werden, da die Steuerung
        ///     implizit �ber die Session bzw. die Transaktion und �ber den FlushMode erfolgt.
        ///     In bestimmten F�llen ist es aber hilfreich, wie z.B. bei Testf�llen.
        /// </remarks>
        void Flush();

        /// <summary>
        ///     F�hrt ein Flush und ein Clear f�r die Session aus.
        /// </summary>
        void FlushAndClear();

        /// <summary>
        ///     Liefert das Entity mit dem angegebenen Primary Key.
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        T Get(TKey primaryKey);

        /// <summary>
        ///     Liefert eine Liste mit allen Entit�ten.
        /// </summary>
        /// <returns>Liste mit allen Entities.</returns>
        IList<T> GetAll();

        /// <summary>
        ///     Liefert eine Liste mit allen Entit�ten die entsprechend der Beschreibung sortiert sind.
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        IList<T> GetAll(Sort sort);

        /// <summary>
        ///     Liefert eine Liste mit Entities entsprechend der Pageinformationen
        ///     aus der Menge aller Entit�ten.
        /// </summary>
        /// <param name="pageable">Beschreibung wleche Menge der Entit�ten zur�ckgeliefert werden.</param>
        /// <returns>Liste mit Entit�ten.</returns>
        IPage<T> GetAll(IPageable pageable);

        /// <summary>
        ///     Liefert das Entity mit der entsprechenden BusinessId.
        /// </summary>
        /// <param name="businessId">die BusinessId</param>
        /// <returns>Das Entity</returns>
        T GetByBusinessId(Guid businessId);

        /// <summary>
        ///     Liefert ein Entity anhand seines Prim�schl�ssels.
        /// </summary>
        /// <param name="primaryKey">Der Prim�rschl�ssel.</param>
        /// <returns>Das Entity mit dem angegebenen Prim�rschl�sel.</returns>
        T GetByPrimaryKey(TKey primaryKey);

        /// <summary>
        ///     Liefert die Anzahl aller Objekte.
        /// </summary>
        /// <returns>Anzahl der Objekte.</returns>
        long GetCount();

        /// <summary>
        ///     Speichert die �bergebene Entit�t
        /// </summary>
        /// <param name="entity">Das zu speichernde Entity</param>
        /// <returns>Das gespeicherte Entity</returns>
        T Save(T entity);

        /// <summary>
        ///     Speichert alle Entit�ten die in der �bergebene Liste enthalten sind
        /// </summary>
        /// <param name="entities">Liste mit zu speichernden Entities.</param>
        /// <returns>Liste mit gespeicherten Entities</returns>
        IList<T> Save(IList<T> entities);
    }
}