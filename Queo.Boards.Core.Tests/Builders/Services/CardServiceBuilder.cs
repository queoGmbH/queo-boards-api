using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Services.Impl;

namespace Queo.Boards.Core.Tests.Builders.Services {
    /// <summary>
    ///     Bilder for <see cref="CardService" />
    /// </summary>
    public class CardServiceBuilder : Builder<CardService> {
        private ICardDao _cardDao;
        private IChecklistService _checklistService;
        private ICommentService _commentService;

        public CardServiceBuilder(ICardDao cardDao, IChecklistService checklistService, ICommentService commentService) {
            _cardDao = cardDao;
            _checklistService = checklistService;
            _commentService = commentService;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="CardService" />
        /// </summary>
        /// <returns></returns>
        public override CardService Build() {
            return new CardService(_cardDao, _checklistService, _commentService);
        }

        /// <summary>
        ///     Sets a <see cref="ICardDao" />
        /// </summary>
        /// <param name="cardDao"></param>
        /// <returns></returns>
        public CardServiceBuilder With(ICardDao cardDao) {
            _cardDao = cardDao;
            return this;
        }

        /// <summary>
        ///     Sets a <see cref="IChecklistService" />
        /// </summary>
        /// <param name="checklistService"></param>
        /// <returns></returns>
        public CardServiceBuilder With(IChecklistService checklistService) {
            _checklistService = checklistService;
            return this;
        }

        /// <summary>
        ///     Sets a <see cref="ICommentService" />
        /// </summary>
        /// <param name="commentService"></param>
        /// <returns></returns>
        public CardServiceBuilder With(ICommentService commentService) {
            _commentService = commentService;
            return this;
        }
    }
}