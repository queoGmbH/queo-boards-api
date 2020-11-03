using System;
using Newtonsoft.Json;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Dtos {
    /// <summary>
    ///     Dto für eine einzelne Guid
    /// </summary>
    public class GuidValueDto {
        /// <summary>
        ///     Liefert oder setzt die Guid
        /// </summary>
        public Guid Value { get; set; }

        public GuidValueDto() {
        }

        public GuidValueDto(Guid value) {
            Value = value;
        }

        /// <summary>
        /// Implizites Erstellen eines <see cref="GuidValueDto"/> aus einer Guid.
        /// </summary>
        /// <param name="guid"></param>
        public static implicit operator GuidValueDto(Guid guid) {
            return new GuidValueDto(guid);
        }
    }

    /// <summary>
    /// Klasse die ermöglicht, ein Entity per Binding als Body-Parameter zu übergeben.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EntityFromBody<TEntity> where TEntity : Entity {
        public EntityFromBody() {
        }

        public EntityFromBody(TEntity entity) {
            Entity = entity;
        }

        /// <summary>
        /// Ruft die Entity ab oder legt diese fest.
        /// </summary>
        [JsonProperty("value")]
        public TEntity Entity { get; set; }

    }
}