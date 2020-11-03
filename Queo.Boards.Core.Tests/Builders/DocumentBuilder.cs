using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;

namespace Queo.Boards.Core.Tests.Builders {
    public class DocumentBuilder {
        private readonly IDocumentDao _documentDao;
        private readonly CardBuilder _cardBuilder;
        private string _name = "meineTestdatei.txt";
        private Card _card;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public DocumentBuilder(IDocumentDao documentDao, CardBuilder cardBuilder) {
            _documentDao = documentDao;
            _cardBuilder = cardBuilder;
        }

        public DocumentBuilder Card(Card card) {
            _card = card;
            return this;
        }

        public Document Build() {
            if (_card == null) {
                _card = _cardBuilder.Build();
            }
            Document document = new Document(_card,_name);
            if (_documentDao != null) {
                _documentDao.Save(document);
                _documentDao.Flush();
            }
            return document;
        }

        public DocumentBuilder Name(string name) {
            _name = name;
            return this;
        }
    }
}