using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Services.Impl;

namespace Queo.Boards.Core.Tests.Builders.Services {
    public class BoardServiceBuilder : Builder<BoardService> {
        private IBoardDao _boardDao;
        private IEmailNotificationService _emailNotificationService;
        private ILabelService _labelService;
        private IListService _listService;
        private ICardService _cardService;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public BoardServiceBuilder(IBoardDao boardDao, ILabelService labelService, IListService listService, ICardService cardService, IEmailNotificationService emailNotificationService) {
            _boardDao = boardDao;
            _emailNotificationService = emailNotificationService;
            _labelService = labelService;
            _listService = listService;
            _cardService = cardService;
        }

        public BoardServiceBuilder BoardDao(IBoardDao boardDao) {
            _boardDao = boardDao;
            return this;
        }

        public override BoardService Build() {
            return new BoardService(_boardDao, _labelService, _listService, _cardService, _emailNotificationService);
        }

        public BoardServiceBuilder EmailNotificationService(IEmailNotificationService emailNotificationService) {
            _emailNotificationService = emailNotificationService;
            return this;
        }

        public BoardServiceBuilder LabelService(ILabelService labelService) {
            _labelService = labelService;
            return this;
        }

        public BoardServiceBuilder ListService(IListService listService) {
            _listService = listService;
            return this;
        }

        public BoardServiceBuilder CardService(ICardService cardService) {
            _cardService = cardService;
            return this;
        }
    }
}