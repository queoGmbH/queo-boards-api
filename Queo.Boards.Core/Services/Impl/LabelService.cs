using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Persistence;
using Spring.Transaction.Interceptor;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service für <see cref="Label" />
    /// </summary>
    public class LabelService : ILabelService {
        private readonly ILabelDao _labelDao;
        private readonly ICardDao _cardDao;

        /// <summary>
        /// </summary>
        /// <param name="labelDao"></param>
        /// <param name="cardDao"></param>
        public LabelService(ILabelDao labelDao, ICardDao cardDao) {
            _labelDao = labelDao;
            _cardDao = cardDao;
        }

        /// <summary>
        ///     Erstellt ein neues Label zu einem Board
        /// </summary>
        /// <param name="board"></param>
        /// <param name="labelDto"></param>
        /// <returns></returns>
        [Transaction]
        public Label Create(Board board, LabelDto labelDto) {
            Label label = new Label(board, labelDto.Color, labelDto.Name);

            /*Das Label noch dem Board zuweisen.*/
            board.Labels.Add(label);

            _labelDao.Save(label);

            
            return label;
        }

        /// <summary>
        ///     Löscht ein Label und all seine Kartenzuordnungen
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        [Transaction]
        public Label Delete(Label label) {
            IList<Card> cardWithLabel = _cardDao.FindAllCardsWithLabel(label);
            foreach (Card card in cardWithLabel) {
                card.Labels.Remove(label);
                _cardDao.Save(card);
            }
            _labelDao.Delete(label);
            return label;
        }

        /// <summary>
        ///     Aktualisiert ein Label
        /// </summary>
        /// <param name="label"></param>
        /// <param name="labelDto"></param>
        /// <returns></returns>
        [Transaction]
        public Label Update(Label label, LabelDto labelDto) {
            label.Update(labelDto.Name, labelDto.Color);
            return label;
        }
    }
}