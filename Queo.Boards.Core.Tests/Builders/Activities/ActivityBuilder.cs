using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Activities;
using Queo.Boards.Core.Persistence;

namespace Queo.Boards.Core.Tests.Builders.Activities {
    public class ActivityBuilder {
        private readonly IActivityBaseDao _activityBaseDao;
        private readonly UserBuilder _userBuilder;
        private ActivityType _activityType;
        private DateTime? _createdAt;
        private User _creator;
        private string _text;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public ActivityBuilder(IActivityBaseDao activityBaseDao, UserBuilder userBuilder) {
            _activityBaseDao = activityBaseDao;
            _userBuilder = userBuilder;
        }

        public ActivityBase BuildBase() {
            if (_creator == null) {
                _userBuilder.Build();
            }
            if (!_createdAt.HasValue) {
                _createdAt = new DateTime(2017,5,12);
            }
            if (string.IsNullOrWhiteSpace(_text)) {
                _text = "Dummytext";
            }
            ActivityBase activityBase = new ActivityBase(_creator, _createdAt.Value, _text, _activityType);
            if (_activityBaseDao != null) {
                _activityBaseDao.Save(activityBase);
                _activityBaseDao.Flush();
            }
            return activityBase;
        }

        public ActivityBuilder CreatedAt(DateTime createdAt) {
            _createdAt = createdAt;
            return this;
        }

        public ActivityBuilder Creator(User creator) {
            _creator = creator;
            return this;
        }

        public ActivityBuilder ForActivityType(ActivityType activityType) {
            _activityType = activityType;
            return this;
        }

        public ActivityBuilder WithText(string text) {
            _text = text;
            return this;
        }
    }
}