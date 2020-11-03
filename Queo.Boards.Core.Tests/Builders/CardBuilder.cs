using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Persistence;

namespace Queo.Boards.Core.Tests.Builders {
    public class CardBuilder : Builder<Card> {
        private readonly IList<User> _assignedUsers = new List<User>();
        private readonly ICardDao _cardDao;
        private readonly List<Label> _labels = new List<Label>();
        private readonly ListBuilder _listBuilder;
        private readonly UserBuilder _userBuilder;
        private DateTime _createdAt = DateTime.Now;
        private User _createdBy;
        private string _description = "Eine kleine Beschreibung";
        private DateTime? _due;
        private DateTime? _archivedAt;
        private List _list;
        private int? _positionInList;
        private string _title = "Sandmann schauen";
        private DateTime? _dueExpirationNotificationAt = null;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public CardBuilder(ICardDao cardDao, ListBuilder listBuilder, UserBuilder userBuilder) {
            _cardDao = cardDao;
            _listBuilder = listBuilder;
            _userBuilder = userBuilder;
        }

        public override Card Build() {
            if (_list == null) {
                _list = _listBuilder.Build();
            }

            if (_due.HasValue && _due.Value.Kind != DateTimeKind.Utc) {
                _due = _due.Value.ToUniversalTime();
            }

            if (_createdBy == null) {
                _createdBy = _userBuilder.Build();
            }

            if (_createdAt.Kind != DateTimeKind.Utc) {
                _createdAt = _createdAt.ToUniversalTime();
            }


            Card card = new Card(_list, _title, _description, _due, _labels, new EntityCreatedDto(_createdBy, _createdAt));
            if (_archivedAt.HasValue) {
                card.Archive(_archivedAt.Value.ToUniversalTime());
            }
            
            if (_dueExpirationNotificationAt.HasValue) {
                if (_dueExpirationNotificationAt.Value.Kind != DateTimeKind.Utc) {
                    _dueExpirationNotificationAt = _dueExpirationNotificationAt.Value.ToUniversalTime();
                }
                card.UpdateDueExpirationNotificationCreated(_dueExpirationNotificationAt.Value);
            }
            

            if (_positionInList.HasValue) {
                _list.Cards.Insert(_positionInList.Value, card);
            } else {
                _list.Cards.Add(card);
            }

            foreach (User assignedUser in _assignedUsers) {
                card.AssignUser(assignedUser);
            }

            if (_cardDao != null) {
                _cardDao.Save(card);
                _cardDao.Flush();
            }
            try {
                return card;
            } finally {
                ResetDefaults();
            }
        }

        public CardBuilder CreatedBy(User createdBy) {
            _createdBy = createdBy;

            return this;
        }

        public CardBuilder DescribedWith(string description) {
            _description = description;
            return this;
        }

        public CardBuilder Due(DateTime? due) {
            _due = due;
            return this;
        }

        public CardBuilder ArchivedAt(DateTime archivedAt) {
            _archivedAt = archivedAt;
            return this;
        }

        public CardBuilder LargeDescription() {
            _description = Texts.LargeText;
            return this;
        }

        public CardBuilder OnList(List list) {
            _list = list;
            return this;
        }

        /// <summary>
        ///     Setzt die Position der Karte innerhalb der Liste
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public CardBuilder Position(int position) {
            _positionInList = position;
            return this;
        }

        public CardBuilder WithAssignedUsers(params User[] users) {
            foreach (User user in users) {
                if (!_assignedUsers.Contains(user)) {
                    _assignedUsers.Add(user);
                }
            }

            return this;
        }

        public CardBuilder WithLabels(params Label[] labels) {
            foreach (Label label in labels) {
                if (!_labels.Contains(label)) {
                    _labels.Add(label);
                }
            }

            return this;
        }

        public CardBuilder WithTitle(string title) {
            _title = title;
            return this;
        }

        private void ResetDefaults() {
            _archivedAt = null;
            _description = "Eine kleine Beschreibung";
            _due = null;
            _list = null;
            _title = "Sandmann schauen";

            _labels.Clear();
            _assignedUsers.Clear();
        }

        /// <summary>
        /// Markiert die Karte, als "Benachrichtigung über abgelaufene Fälligkeit" gesetzt.
        /// Wird kein Datum übergeben, wird die aktuelle Zeit verwendet.
        /// </summary>
        /// <param name="dueExpirationNotificationAt"></param>
        /// <returns></returns>
        public CardBuilder WithDueNotificationDoneAt(DateTime? dueExpirationNotificationAt = null) {
            if (dueExpirationNotificationAt.HasValue) {
                _dueExpirationNotificationAt = dueExpirationNotificationAt;
            } else {
                _dueExpirationNotificationAt = DateTime.Now;
            }

            return this;
        }
    }
}