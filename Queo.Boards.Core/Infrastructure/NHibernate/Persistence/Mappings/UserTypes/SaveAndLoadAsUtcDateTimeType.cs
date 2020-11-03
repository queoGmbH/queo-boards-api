using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings.UserTypes {
    /// <summary>
    ///     Custom Type für NHibernate, der alle <see cref="DateTime" /> als UTC-Datum speichert. Dazu werden alle zu
    ///     speichernden Werte geprüft, und wenn nötig in <see cref="DateTime.ToUniversalTime">UTC umgewandelt</see>.
    ///     Beim Abrufen aus der Datenbank sind alle Werte mit <see cref="DateTimeKind.Utc" /> ausgestattet.
    /// </summary>
    public class SaveAndLoadAsUtcDateTimeType : IUserType {
        /// <summary>Are objects of this type mutable?</summary>
        public bool IsMutable {
            get { return false; }
        }

        /// <summary>
        ///     The type returned by <c>NullSafeGet()</c>
        /// </summary>
        public Type ReturnedType {
            get { return typeof(DateTime); }
        }

        /// <summary>
        /// The SQL types for the columns mapped by this type.
        /// </summary>
        public SqlType[] SqlTypes {
            get { return new[] { new SqlType(DbType.DateTime) }; }
        }

        /// <summary>
        /// Reconstruct an object from the cacheable representation. At the very least this
        /// method should perform a deep copy if the type is mutable. (optional operation)
        /// </summary>
        /// <param name="cached">the object to be cached</param>
        /// <param name="owner">the owner of the cached object</param>
        /// <returns>a reconstructed object from the cachable representation</returns>
        public object Assemble(object cached, object owner) {
            return cached;
        }

        /// <summary>
        /// Return a deep copy of the persistent state, stopping at entities and at collections.
        /// </summary>
        /// <param name="value">generally a collection element or entity field</param>
        /// <returns>a copy</returns>
        public object DeepCopy(object value) {
            return value;
        }

        /// <summary>
        /// Transform the object into its cacheable representation. At the very least this
        /// method should perform a deep copy if the type is mutable. That may not be enough
        /// for some implementations, however; for example, associations must be cached as
        /// identifier values. (optional operation)
        /// </summary>
        /// <param name="value">the object to be cached</param>
        /// <returns>a cacheable representation of the object</returns>
        public object Disassemble(object value) {
            return value;
        }

        /// <summary>
        /// Compare two instances of the class mapped by this type for persistent "equality"
        /// ie. equality of persistent state
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(object x, object y) {
            return object.Equals(x, y);
        }

        /// <summary>
        /// Get a hashcode for the instance, consistent with persistence "equality"
        /// </summary>
        public int GetHashCode(object x) {
            if (x != null) {
                return x.GetHashCode();
            } else {
                return 0;
            }
        }

        /// <summary>
        /// Retrieve an instance of the mapped class from a JDBC resultset.
        /// Implementors should handle possibility of null values.
        /// </summary>
        /// <param name="rs">a IDataReader</param>
        /// <param name="names">column names</param>
        /// <param name="owner">the containing entity</param>
        /// <returns></returns>
        /// <exception cref="T:NHibernate.HibernateException">HibernateException</exception>
        public object NullSafeGet(IDataReader rs, string[] names, object owner) {
            object obj = NHibernateUtil.UtcDateTime.NullSafeGet(rs, names[0]);

            if (obj == null) {
                return null;
            }

            return (DateTime)obj;
        }

        /// <summary>
        /// Write an instance of the mapped class to a prepared statement.
        /// Implementors should handle possibility of null values.
        /// A multi-column type should be written to parameters starting from index.
        /// </summary>
        /// <param name="cmd">a IDbCommand</param>
        /// <param name="value">the object to write</param>
        /// <param name="index">command parameter index</param>
        /// <exception cref="T:NHibernate.HibernateException">HibernateException</exception>
        public void NullSafeSet(IDbCommand cmd, object value, int index) {
            if (value == null) {
                ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
            } else {
                DateTime datetime = (DateTime)value;
                ((IDataParameter)cmd.Parameters[index]).Value = datetime.ToUniversalTime();
            }
        }

        /// <summary>
        /// During merge, replace the existing (<paramref name="target" />) value in the entity
        /// we are merging to with a new (<paramref name="original" />) value from the detached
        /// entity we are merging. For immutable objects, or null values, it is safe to simply
        /// return the first parameter. For mutable objects, it is safe to return a copy of the
        /// first parameter. For objects with component values, it might make sense to
        /// recursively replace component values.
        /// </summary>
        /// <param name="original">the value from the detached entity being merged</param>
        /// <param name="target">the value in the managed entity</param>
        /// <param name="owner">the managed entity</param>
        /// <returns>the value to be merged</returns>
        public object Replace(object original, object target, object owner) {
            return original;
        }
    }
}