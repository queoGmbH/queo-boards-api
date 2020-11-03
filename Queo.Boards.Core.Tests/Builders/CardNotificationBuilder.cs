using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Persistence.Impl;

namespace Queo.Boards.Core.Tests.Builders {

    /// <summary>
    /// Builder zum Erstellen von Benachrichtigungen für Karten.
    /// </summary>
    public class CardNotificationBuilder : NotificationBuilder<CardNotificationBuilder, CardNotification> {
        private readonly CardBuilder _cardBuilder;
        private readonly UserBuilder _userBuilder;
        private Card _card;
        private CardNotificationReason _notificationReason;

        public CardNotificationBuilder(INotificationDao notificationDao, CardBuilder cardBuilder, UserBuilder userBuilder)
            : base(notificationDao) {
            Require.NotNull(userBuilder, "userBuilder");
            Require.NotNull(cardBuilder, "cardBuilder");

            _cardBuilder = cardBuilder;
            _userBuilder = userBuilder;
        }

        protected override CardNotificationBuilder ActualBuilder {
            get { return this; }
        }

        public override CardNotification Build() {

            if (_card == null) {
                _card = _cardBuilder.Build();
            }
            if (NotificationFor == null) {
                NotificationFor = _userBuilder.Build();
            }

            CardNotification cardNotification = new CardNotification(_card, _notificationReason, NotificationFor, CreationDateTime, EmailSend, EmailSendAt, IsMarkedAsRead);
            _card.CardNotifications.Add(cardNotification);
            if (NotificationDao != null) {
                NotificationDao.Save(cardNotification);
                NotificationDao.Flush();
            }
            return cardNotification;
        }

        /// <summary>
        /// Legt die Karte fest, für welche die Benachrichtigung erstellt wurde.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public CardNotificationBuilder ForCard(Card card) {
            _card = card;

            return this;
        }

        /// <summary>
        /// Legt den Grund für die Benachrichtigung fest.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public CardNotificationBuilder WithReason(CardNotificationReason reason) {
            _notificationReason = reason;
            return this;
        }
    }
}