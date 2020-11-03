using System;
using System.Collections.Generic;
using System.Linq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Persistence;

namespace Queo.Boards.Core.Tests.Builders {
    public class BoardBuilder : Builder<Board> {
        private readonly IBoardDao _boardDao;
        private readonly IList<User> _boardMembers = new List<User>();
        private readonly IList<Team> _boardTeams = new List<Team>();
        private readonly IList<User> _owners = new List<User>();
        private readonly UserBuilder _userBuilder;
        private DateTime? _archivedAt;
        private string _colorScheme = "blau";
        private DateTime? _creationDate;
        private User _creator;
        private bool _isTemplate;
        private string _title = "Test Board";
        private Accessibility _visibility = Accessibility.Restricted;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public BoardBuilder(IBoardDao boardDao, UserBuilder userBuilder) {
            _boardDao = boardDao;
            _userBuilder = userBuilder;
        }

        public BoardBuilder ArchivedAt(DateTime archivedAt) {
            _archivedAt = archivedAt;
            return this;
        }

        /// <summary>
        ///     Markiert das Board als Vorlage für andere Boards.
        /// </summary>
        /// <returns></returns>
        public BoardBuilder AsTemplate() {
            _isTemplate = true;
            return this;
        }

        public override Board Build() {
            if (!_creationDate.HasValue) {
                _creationDate = DateTime.UtcNow;
            }
            if (_creator == null) {
                _creator = _userBuilder.Build();
            }
            BoardDto boardDto = new BoardDto() { Accessibility = _visibility, ColorScheme = _colorScheme, Title = _title };

            if (!_owners.Any()) {
                /*Wenn keine abweichenden Eigentümer angegeben, dann den Ersteller als Besitzer festlegen.*/
                _owners.Add(_creator);
            }

            Board board = new Board(boardDto, new EntityCreatedDto(_creator, _creationDate.Value), _owners);
            if (_archivedAt.HasValue) {
                board.Archive(_archivedAt.Value.ToUniversalTime());
            }

            foreach (User boardMember in _boardMembers) {
                board.AddMember(boardMember);
            }

            foreach (Team boardTeam in _boardTeams) {
                board.AddTeam(boardTeam);
            }

            if (_isTemplate) {
                /*Das Board ist ein Vorlage*/
                board.MakeTemplate();
            }

            if (_boardDao != null) {
                _boardDao.Save(board);
                _boardDao.Flush();
            }
            return board;
        }

        public BoardBuilder ColorScheme(string scheme) {
            _colorScheme = scheme;
            return this;
        }

        public BoardBuilder CreatedAt(DateTime creationDate) {
            _creationDate = creationDate;
            return this;
        }

        public BoardBuilder Creator(User creator) {
            _creator = creator;

            if (!_boardMembers.Contains(creator)) {
                _boardMembers.Add(creator);
            }

            return this;
        }

        public BoardBuilder Public() {
            _visibility = Accessibility.Public;
            return this;
        }

        public BoardBuilder Restricted() {
            _visibility = Accessibility.Restricted;
            return this;
        }

        public BoardBuilder WithMembers(params User[] boardMembers) {
            foreach (User boardMember in boardMembers) {
                if (!_boardMembers.Contains(boardMember)) {
                    _boardMembers.Add(boardMember);
                }
            }

            return this;
        }

        /// <summary>
        ///     Legt die Besitzer des Boards fest.
        ///     Wird ein Besitzer festgelegt, ist der Ersteller des Boards KEIN Owner des Boards mehr.
        /// </summary>
        /// <param name="owners"></param>
        /// <returns></returns>
        public BoardBuilder WithOwners(params User[] owners) {
            foreach (User owner in owners) {
                if (!_owners.Contains(owner)) {
                    _owners.Add(owner);
                }
            }

            return this;
        }

        /// <summary>
        /// Legt die dem Board zugeordneten Teams fest.
        /// </summary>
        /// <param name="teams"></param>
        /// <returns></returns>
        public BoardBuilder WithTeams(params Team[] teams) {
            foreach (Team team in teams) {
                if (!_boardTeams.Contains(team)) {
                    _boardTeams.Add(team);
                }
            }

            return this;
        }

        public BoardBuilder WithTitle(string title) {
            _title = title;
            return this;
        }
    }
}