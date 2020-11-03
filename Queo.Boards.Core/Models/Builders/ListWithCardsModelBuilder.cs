using System.Linq;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Builder für <see cref="ListWithCardsModel" />
    /// </summary>
    public static class ListWithCardsModelBuilder {
        /// <summary>
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ListWithCardsModel Build(List list) {
            return new ListWithCardsModel(list.BusinessId, BreadCrumbsModelBuilder.Build(list.Board), list.Cards.Where(x=>x != null).Select(CardModelBuilder.Build).ToList(), list.Title, list.GetPositionOnBoard(), list.ArchivedAt);
        }
    }
}