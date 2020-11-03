using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;

namespace Queo.Boards.Core.Tests.Builders {
    public class ChecklistBuilder : Builder<Checklist> {
        private readonly CardBuilder _cardBuilder;
        private readonly IChecklistDao _checklistDao;

        private Card _card;

        private string _title = "Checklist WithTitle";

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public ChecklistBuilder(IChecklistDao checklistDao, CardBuilder cardBuilder) {
            _checklistDao = checklistDao;
            _cardBuilder = cardBuilder;
        }

        /// <summary>
        ///     Erzeugt eine neue Checkliste und speichert diese optional in der DB
        /// </summary>
        /// <param name="persist"></param>
        /// <returns></returns>
        public override Checklist Build() {
            if (_card == null) {
                _card = _cardBuilder.Build();
            }
            Checklist checklist = new Checklist(_card, _title);
            _card.Checklists.Add(checklist);
            if (_checklistDao != null) {
                _checklistDao.Save(checklist);
                _checklistDao.Flush();
            }
            return checklist;
        }

        public ChecklistBuilder OnCard(Card card) {
            _card = card;
            return this;
        }

        public ChecklistBuilder Title(string title) {
            _title = title;
            return this;
        }
    }
}