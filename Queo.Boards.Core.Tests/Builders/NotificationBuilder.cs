using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Persistence.Impl;

namespace Queo.Boards.Core.Tests.Builders {
    public abstract class NotificationBuilder<TEntity> : Builder<TEntity>
        where TEntity : Notification {
        private readonly INotificationDao _notificationDao;

        protected NotificationBuilder(INotificationDao notificationDao) {
            _notificationDao = notificationDao;
        }

        protected INotificationDao NotificationDao {
            get { return _notificationDao; }
        }
    }

    public abstract class NotificationBuilder<TInheritant, TEntity> : NotificationBuilder<TEntity>
        where TInheritant : NotificationBuilder<TEntity> where TEntity : Notification {
        protected DateTime CreationDateTime = DateTime.UtcNow;
        protected bool EmailSend;
        protected DateTime? EmailSendAt;
        protected bool IsMarkedAsRead;
        protected User NotificationFor;

        protected NotificationBuilder(INotificationDao notificationDao)
            : base(notificationDao) {
        }

        protected abstract TInheritant ActualBuilder { get; }

        public TInheritant CreatedAt(DateTime createdAt) {
            CreationDateTime = createdAt;

            return ActualBuilder;
        }

        public TInheritant For(User user) {
            NotificationFor = user;
            return ActualBuilder;
        }

        public TInheritant MarkedAsRead(bool isRead = true) {
            IsMarkedAsRead = isRead;

            return ActualBuilder;
        }

        public TInheritant WithEmailSendAt(DateTime emailSendAt) {
            EmailSendAt = emailSendAt;
            EmailSend = true;

            return ActualBuilder;
        }
    }
}