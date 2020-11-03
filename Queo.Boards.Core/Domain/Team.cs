using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain {
    /// <summary>
    ///     Bildet ein Team ab. Ein Team entspricht einer benannten Liste von Nutzern.
    ///     Jeder Nutzer darf nur einmal im Team enthalten sein, kann aber mehreren Teams zugeordnet sein.
    ///     Ein Team kann zum Beispiel einem Board zugeordnet werden, wodurch alle Mitglieder des Teams auch gleichzeitig
    ///     Mitglieder des Boards sind.
    /// </summary>
    public class Team : Entity {
        private readonly DateTime _createdAt;
        private readonly User _createdBy;
        private readonly ICollection<User> _members = new HashSet<User>();
        private string _description;
        private string _name;

        /// <summary>
        ///     Konstruktor für NHibernate und Testfälle.
        /// </summary>
        public Team() {
        }

        /// <summary>
        ///     Erstellt ein Team ohne zugeordnete Mitglieder.
        /// </summary>
        /// <param name="teamDto">Allgemeine Informationen zum Team</param>
        /// <param name="createDto">Informationen über die Erstellung des Teams</param>
        public Team(TeamDto teamDto, EntityCreatedDto createDto) {
            Require.NotNull(createDto, "createDto");
            Require.NotNull(teamDto, "teamDto");

            UpdateTeamDto(teamDto);
            _createdAt = createDto.CreatedAt;
            _createdBy = createDto.CreatedBy;
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="Entity" />-Klasse.
        /// </summary>
        /// <param name="teamDto">Allgemeine Informationen zum Team</param>
        /// <param name="members">Nutzer die dem Team zugeordnet sind.</param>
        /// <param name="createDto">Informationen über die Erstellung des Teams</param>
        public Team(TeamDto teamDto, IList<User> members, EntityCreatedDto createDto) : this(teamDto, createDto) {
            Require.NotNull(members, "members");

            _members = members;
        }

        /// <summary>
        ///     Ruft ab, wann das Team erstellt wurde.
        /// </summary>
        public virtual DateTime CreatedAt {
            get { return _createdAt; }
        }

        /// <summary>
        ///     Ruft ab, von wem das Team erstellt wurde.
        ///     Ist evtl. null, wenn der Nutzer der das Team erstellt hat, gelöscht wurde.
        /// </summary>
        public virtual User CreatedBy {
            get { return _createdBy; }
        }

        /// <summary>
        ///     Ruft die Beschreibung des Teams ab.
        /// </summary>
        public virtual string Description {
            get { return _description; }
        }

        /// <summary>
        ///     Ruft eine schreibgeschützte Kopie der Liste mit Teammitgliedern ab.
        /// </summary>
        public virtual IList<User> Members {
            get { return new ReadOnlyCollection<User>(_members.ToList()); }
        }

        /// <summary>
        ///     Ruft den Namen des Teams ab.
        /// </summary>
        public virtual string Name {
            get { return _name; }
        }

        /// <summary>
        ///     Ändert das Team.
        /// </summary>
        /// <param name="teamDto"></param>
        public virtual void Update(TeamDto teamDto) {
            UpdateTeamDto(teamDto);
        }

        /// <summary>
        ///     Ändert die Eigenschaften die im TeamDto enthalten sind.
        /// </summary>
        /// <param name="teamDto"></param>
        private void UpdateTeamDto(TeamDto teamDto) {
            _name = teamDto.Name;
            _description = teamDto.Description;
        }


        /// <summary>
        /// Fügt dem Team neue Nutzer als Mitglieder hinzu.
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        public virtual IList<User> AddMembers(params User[] members) {
            Require.NotNull(members, "members");

            foreach (User newMember in members.Except(_members)) {
                /*Alle bisher nicht zugewiesenen Nutzer als neue Mitglieder hinzufügen*/
                _members.Add(newMember);
            }

            return Members;
        }

        /// <summary>
        /// Entfernt Mitglieder eines Teams.
        /// </summary>
        /// <param name="members">Die aus dem Team zu entfernenden Mitglieder.</param>
        /// <returns></returns>
        public virtual IList<User> RemoveMembers(params User[] members) {
            Require.NotNull(members, "members");

            var membersToRemove = members.Intersect(_members);
            foreach (User removeMember in membersToRemove) {
                /*Alle Nutzer die auch wirklich Mitglieder des Teams sind löschen.*/
                _members.Remove(removeMember);
            }

            return Members;
        }
    }
}