using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Builder für <see cref="ListModel" />
    /// </summary>
    public static class ListModelBuilder {
        /// <summary>
        ///     Erstellt ein neues <see cref="ListModel" />
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ListModel Build(List list) {
            return new ListModel(list.BusinessId, BreadCrumbsModelBuilder.Build(list.Board), list.Title, list.GetPositionOnBoard(), list.ArchivedAt);
        }
    }
}