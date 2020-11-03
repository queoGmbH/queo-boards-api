using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using NJsonSchema.Infrastructure;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Spring.Context.Support;

namespace Queo.Boards {
    /// <summary>
    ///     Konverter zum Deserialisieren von <see cref="Entity" />-Objekten, die im Json eine Id enthalten und anhand dieser,
    ///     durch Laden aus der DB, deserialisiert werden.
    /// </summary>
    public class JsonEntityConverter : JsonConverter {
        /// <summary>
        ///     Überprüft ob der Typ von DomainEntity abgeleitet ist.
        /// </summary>
        /// <param name="typeToCheck"></param>
        /// <returns></returns>
        public static bool IsDomainEntityType(Type typeToCheck) {
            if (typeToCheck.IsSubclassOf(typeof(Entity))) {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Überprüft ob es sich bei einem Typ, um eine generische Liste eines DomainEntity-Types handelt.
        /// </summary>
        /// <param name="typeToCheck"></param>
        /// <returns></returns>
        public static bool IsDomainEntityTypeList(Type typeToCheck) {
            if (!IsListType(typeToCheck)) {
                return false;
            }

            Type[] genericTypes = typeToCheck.GetGenericArguments();
            if (!genericTypes.Any()) {
                return false;
            }

            return IsDomainEntityType(genericTypes[0]);
        }

        /// <summary>
        ///     Überprüft, ob es sich bei einem bestimmten Typ um einen handelt, der eine Mehrfachauswahl zulässt.
        /// </summary>
        /// <param name="typeToCheck">Der zu prüfende Typ. Bei NULL wird false geliefert.</param>
        /// <returns></returns>
        public static bool IsListType(Type typeToCheck) {
            if (typeToCheck == null) {
                return false;
            }

            IList<Type> listTypes = new List<Type> { typeof(IList), typeof(IList<>), typeof(ICollection), typeof(ICollection<>), typeof(List<>), typeof(Collection<>) };

            if (typeToCheck.IsGenericType) {
                return listTypes.Contains(typeToCheck.GetGenericTypeDefinition());
            } else if (typeToCheck.BaseType != null) {
                return IsListType(typeToCheck.BaseType);
            }

            return false;
        }

        /// <summary>
        ///     Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        ///     <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        ///     Der Konverter ist in der Lage <see cref="Entity" /> bzw. Listen von <see cref="Entity" /> zu konvertieren.
        ///     Die kompatiblen Listentypen sind in der Methode <see cref="IsDomainEntityTypeList" /> definiert.
        /// </remarks>
        public override bool CanConvert(Type objectType) {
            if (IsDomainEntityType(objectType)) {
                /*Der Typ erbt von Entity und kann konvertiert werden.*/
                return true;
            }
            if (IsDomainEntityTypeList(objectType)) {
                /*Der Typ ist eine Liste eines Typs, der von Entity erbt und kann konvertiert werden.*/
                return true;
            }

            /*Inkompatibler Typ, der mit diesem Konverter nicht konvertiert werden kann.*/
            return false;
        }

        /// <summary>Reads the JSON representation of the object.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (IsDomainEntityType(objectType)) {
                /*Es handelt sich um ein Entity*/
                return BindEntity(reader, objectType);
            } else if (IsDomainEntityTypeList(objectType)) {
                /*Es handelt sich um eine Liste von Entity*/
                return BindEntityList(reader, objectType);
            } else {
                return null;
            }
        }

        /// <summary>Writes the JSON representation of the object.</summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new InvalidOperationException("Instanzen von Klassen die von Entity erben, können nicht in Json konvertiert werden.");
        }

        private object BindEntity(JsonReader reader, Type objectType) {
            /*Typen des Binders erstellen*/
            Type binderType = typeof(JSonEntityBinder<>).MakeGenericType(objectType);

            /*Typ des Daos erstellen*/
            Type daoType = typeof(IGenericDao<,>).MakeGenericType(objectType, typeof(int));

            /*Dao aus Spring laden.*/
            KeyValuePair<string, object> dao = ContextRegistry.GetContext().GetObjectsOfType(daoType).First();

            /*Instanz des Binder erstellen*/
            IJsonEntityBinder entityBinder = (IJsonEntityBinder)Activator.CreateInstance(binderType, dao.Value);

            /*Entity binden*/
            return entityBinder.Bind(reader);
        }

        private object BindEntityList(JsonReader reader, Type objectType) {
            /*Typ der deserialisiert werden soll ermitteln*/
            Type entityType = objectType.GetGenericTypeArguments()[0];

            /*Typen des Binders erstellen*/
            Type binderType = typeof(JSonEntityBinder<>).MakeGenericType(entityType);

            /*Typ des Daos erstellen*/
            Type daoType = typeof(IGenericDao<,>).MakeGenericType(entityType, typeof(int));

            /*Dao aus Spring laden.*/
            KeyValuePair<string, object> dao = ContextRegistry.GetContext().GetObjectsOfType(daoType).First();

            /*Instanz des Binder erstellen*/
            IJsonEntityBinder entityBinder = (IJsonEntityBinder)Activator.CreateInstance(binderType, dao.Value);

            /*Versuchen die Liste zu binden.*/
            return entityBinder.BindList(reader);
        }
    }
}