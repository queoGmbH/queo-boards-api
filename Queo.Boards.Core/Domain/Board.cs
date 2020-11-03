using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Domain {
    /// <summary>
    ///     Ein Board.
    /// </summary>
    public class Board : BoardTemplate {
        private readonly DateTime _createdAt;
        private readonly User _createdBy;
        private readonly IList<Label> _labels = new List<Label>();
        private readonly IList<List> _lists = new List<List>();
        private readonly ICollection<User> _members = new List<User>();
        private readonly ICollection<User> _owners = new List<User>();
        private readonly ICollection<Team> _teams = new List<Team>();
        private Accessibility _accessibility;
        private DateTime? _archivedAt;
        private bool _isArchived;
        private bool _isTemplate;

        /// <summary>
        ///     Ctor für NHibernate
        /// </summary>
        public Board() {
        }

        /// <summary>
        ///     Erzeugt ein neues Board ohne Eigentümer oder Members.
        /// </summary>
        /// <param name="boardDto">Allgemeine Informationen zum Board</param>
        /// <param name="createDto">Informationen über die Erstellung des Boards</param>
        /// <param name="isTemplate">Ist das Erstellte Board eine Vorlage?</param>
        public Board(BoardDto boardDto, EntityCreatedDto createDto, bool isTemplate)
            : this(boardDto, createDto, new List<User>()) {
            _isTemplate = isTemplate;
        }

        /// <summary>
        /// </summary>
        /// <param name="boardDto">Allgemeine Informationen zum Board</param>
        /// <param name="createDto">Informationen über die Erstellung des Boards</param>
        /// <param name="owners"></param>
        public Board(BoardDto boardDto, EntityCreatedDto createDto, IList<User> owners)
            : base(boardDto.ColorScheme, boardDto.Title) {
            Require.NotNull(boardDto);
            Require.NotNull(createDto);
            Require.NotNull(owners, "owners");

            _accessibility = boardDto.Accessibility;
            _createdBy = createDto.CreatedBy;
            _createdAt = createDto.CreatedAt;
            _owners = owners;
        }

        /// <summary>
        ///     LIefert die SIchtbarkeit des Boards
        /// </summary>
        public virtual Accessibility Accessibility {
            get { return _accessibility; }
        }

        /// <summary>
        ///     Ruft ab, wann das Board archiviert wurde.
        ///     Wurde das Board bisher nicht archiviert, wird NULL geliefert.
        /// </summary>
        public virtual DateTime? ArchivedAt {
            get { return _archivedAt; }
        }

        /// <summary>
        ///     Liefert das Datum, an dem das Board erstellt wurde
        /// </summary>
        public virtual DateTime CreatedAt {
            get { return _createdAt; }
        }

        /// <summary>
        ///     Liefert den Nutzer, der das Board erstellt hat. Dieser muss nicht zwangsläufig Teilnehmer am Board sein.
        /// </summary>
        public virtual User CreatedBy {
            get { return _createdBy; }
        }

        /// <summary>
        ///     Liefert ob das Board archiviert ist.
        /// </summary>
        public virtual bool IsArchived {
            get { return _isArchived; }
        }

        /// <summary>
        ///     Liefert ob es sich um ein privates Board handelt.
        ///     Privat = Eigentümer ist einziges Mitglied.
        /// </summary>
        public virtual bool IsPrivate {
            get {
                return
                        /*Das Board darf nicht public sein*/
                        Accessibility != Accessibility.Public &&
                        /*Und nur ein Mitglied/Besitzer haben*/
                        GetOwnersAndMembers().Count == 1;
            }
        }

        /// <summary>
        ///     Ruft ab, ob es sich um eine Vorlage handelt oder nicht.
        /// </summary>
        public virtual bool IsTemplate {
            get { return _isTemplate; }
        }

        /// <summary>
        ///     Liefert die Labels
        /// </summary>
        public virtual IList<Label> Labels {
            get { return _labels; }
        }

        /// <summary>
        ///     Liefert die Listen des Boards
        /// </summary>
        [JsonIgnore]
        public virtual IList<List> Lists {
            get { return _lists; }
        }

        /// <summary>
        ///     Liefert die Mitglieder des Boards
        /// </summary>
        public virtual ICollection<User> Members {
            get {
                return new ReadOnlyCollection<User>(
                    _members.ToList());
            }
        }

        /// <summary>
        ///     Liefert die Liste der Eigentümer des Boards.
        ///     Es gibt immer mindestens 1 Besitzer des Boards.
        /// </summary>
        public virtual ICollection<User> Owners {
            get { return new ReadOnlyCollection<User>(_owners.ToList()); }
        }

        /// <summary>
        ///     Ruft die dem Board zugewiesenen Teams ab.
        /// </summary>
        public virtual ICollection<Team> Teams {
            get { return new ReadOnlyCollection<Team>(_teams.ToList()); }
        }

        /// <summary>
        ///     Fügt dem Board einen Member hinzu
        /// </summary>
        /// <param name="user"></param>
        public virtual void AddMember(User user) {
            if (!_members.Contains(user)) {
                _members.Add(user);
            }
        }

        /// <summary>
        ///     Fügt dem Board einen weiteren Besitzer hinzu.
        /// </summary>
        /// <param name="user"></param>
        public virtual void AddOwner(User user) {
            if (!_owners.Contains(user)) {
                _owners.Add(user);
            }
        }

        /// <summary>
        ///     Fügt dem Board ein weiteres Team hinzu, insofern dieses nicht bereits dem Board zugeordnet ist.
        /// </summary>
        /// <param name="team"></param>
        public virtual void AddTeam(Team team) {
            Require.NotNull(team, "team");

            if (!_teams.Contains(team)) {
                _teams.Add(team);
            }
        }

        /// <summary>
        ///     Archiviert das Board.
        /// </summary>
        /// <param name="archivedAt"></param>
        public virtual void Archive(DateTime archivedAt) {
            _isArchived = true;
            _archivedAt = archivedAt;
        }

        /// <summary>
        ///     Ruft alle Nutzer des Boards ab.
        ///     Dazu zählen alle <see cref="Owners">Eigentümer</see> und <see cref="Members">Mitglieder</see> sowie
        ///     <see cref="Team.Members">Mitglieder</see> zugeordneter <see cref="Teams" />.
        ///     Nutzer die sowohl <see cref="Owners">Besitzer</see>, <see cref="Members">Mitglied</see> oder Mitglied eines
        ///     <see cref="Teams">zugewiesenen Teams</see> sind, sind in der Liste trotzdem nur einmal enthalten.
        /// </summary>
        /// <returns></returns>
        public virtual IList<User> GetBoardUsers() {
            return new List<User>(_owners.Concat(_members).Concat(_teams.SelectMany(t => t.Members)).Distinct());
        }

        /// <summary>
        ///     Erzeugt ein <see cref="BoardDto" /> für dieses Board.
        /// </summary>
        /// <returns></returns>
        public virtual BoardDto GetDto() {
            return new BoardDto(Title, _accessibility, ColorScheme);
        }

        /// <summary>
        ///     Ruft alle Nutzer des Boards ab, die entweder Member oder Owner des Boards sind.
        /// </summary>
        /// <returns></returns>
        public virtual IList<User> GetOwnersAndMembers() {
            return
                    new ReadOnlyCollection<User>(Members.Union(Owners).Distinct().ToList());
        }

        /// <summary>
        ///     Markiert das Board als Template.
        /// </summary>
        public virtual void MakeTemplate() {
            _isTemplate = true;
        }

        /// <summary>
        ///     Entfernt die Mitgliedschaft eines Nutzers am Boards, sofern der Nutzer überhaupt Mitglied ist.
        /// </summary>
        /// <param name="member">Das zu entfernende Mitglied.</param>
        public virtual void RemoveMember(User member) {
            if (_members.Contains(member)) {
                _members.Remove(member);
            }
        }

        /// <summary>
        ///     Entfernt einen Besitzer des Boards.
        /// </summary>
        /// <param name="user"></param>
        public virtual void RemoveOwner(User user) {
            if (_owners.Contains(user)) {
                _owners.Remove(user);
            }
        }

        /// <summary>
        ///     Entfernt ein dem Board zugeordnetes Team.
        ///     Ist das Team dem Board nicht zugewiesen, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="team"></param>
        public virtual void RemoveTeam(Team team) {
            Require.NotNull(team, "team");

            if (_teams.Contains(team)) {
                _teams.Remove(team);
            }
        }

        /// <summary>
        ///     Stellt das Board wieder her und hebt damit die Archivierung auf.
        /// </summary>
        public virtual void Restore() {
            _isArchived = false;
            _archivedAt = null;
        }

        /// <summary>
        ///     Aktualisiert die Stammdaten des Boards
        /// </summary>
        /// <param name="dto"></param>
        public virtual void Update(BoardDto dto) {
            Require.NotNull(dto);
            Update(dto.Title, dto.ColorScheme);
            _accessibility = dto.Accessibility;
        }
    }
}