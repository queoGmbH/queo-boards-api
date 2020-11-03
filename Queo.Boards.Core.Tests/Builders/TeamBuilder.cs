using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Persistence;

namespace Queo.Boards.Core.Tests.Builders {
    public class TeamBuilder : Builder<Team> {
        readonly IList<User> _members = new List<User>();

        private readonly ITeamDao _teamDao;
        private readonly UserBuilder _userBuilder;
        private DateTime _createdAt = DateTime.UtcNow;
        private User _createdBy;
        private string _description = "";
        private string _teamName;

        public TeamBuilder(ITeamDao teamDao, UserBuilder userBuilder) {
            _teamDao = teamDao;
            _userBuilder = userBuilder;

            _teamName = "Team " + GetRandomString(4);
        }

        public override Team Build() {
            if (_createdAt.Kind != DateTimeKind.Utc) {
                _createdAt = _createdAt.ToUniversalTime();
            }
            if (_createdBy == null) {
                _createdBy = _userBuilder.Build();
            }

            TeamDto teamDto = new TeamDto(_teamName, _description);
            Team team = new Team(teamDto, _members, new EntityCreatedDto(_createdBy, _createdAt));

            if (_teamDao != null) {
                _teamDao.Save(team);
                _teamDao.Flush();
            }

            return team;
        }

        /// <summary>
        ///     Legt fest, wann das Team erstellt wurde.
        /// </summary>
        /// <param name="createdAt"></param>
        /// <returns></returns>
        public TeamBuilder CreatedAt(DateTime createdAt) {
            _createdAt = createdAt;

            return this;
        }

        /// <summary>
        ///     Legt fest, von wem das Team erstellt wurde.
        /// </summary>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public TeamBuilder CreatedBy(User createdBy) {
            _createdBy = createdBy;

            return this;
        }

        /// <summary>
        ///     Legt die Beschreibung des Teams fest.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public TeamBuilder DescribedWith(string description) {
            _description = description;
            return this;
        }

        /// <summary>
        ///     Fügt weitere Mitglieder zum Team hinzu, insofern sie nicht schon Mitglieder des Teams sind.
        /// </summary>
        /// <param name="boardMembers"></param>
        /// <returns></returns>
        public TeamBuilder WithMembers(params User[] boardMembers) {
            foreach (User boardMember in boardMembers) {
                if (!_members.Contains(boardMember)) {
                    _members.Add(boardMember);
                }
            }

            return this;
        }

        /// <summary>
        ///     Legt den Namen des Teams fest.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TeamBuilder WithName(string name) {
            _teamName = name;
            return this;
        }
    }

    
}