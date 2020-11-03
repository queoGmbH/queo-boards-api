using System.Collections.Generic;
using System.Linq;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Builder für <see cref="CardModel" />
    /// </summary>
    public static class CardModelBuilder {
        /// <summary>
        ///     Erstellt ein neues <see cref="CardModel" />
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static CardModel Build(Card card) {
            IList<LabelModel> labelModels = new List<LabelModel>();
            if (card.Labels != null && card.Labels.Any()) {
                labelModels = card.Labels.Select(LabelModelBuilder.Build).ToList();
            }
            int commentCount = 0;
            if (card.Comments != null) {
                commentCount = card.Comments.Count;
            }
            int toDoOverallCount = 0;
            int toDoDoneCount = 0;
            if (card.Checklists != null) {
                IList<Task> allTasks = card.Checklists.Where(x => x.Tasks != null).SelectMany(x => x.Tasks).ToList();
                toDoOverallCount = allTasks.Count;
                toDoDoneCount = allTasks.Count(x => x.IsDone);
            }

            int attachmentsCount = 0;
            if (card.Documents != null && card.Documents.Any(x => x != null)) {
                attachmentsCount = card.Documents.Count(x => x != null);
            }

            return new CardModel(card.BusinessId, card.Title, card.Description, BreadCrumbsModelBuilder.Build(card.List), card.GetPositionInList(), card.CreatedAt, card.Due, labelModels, commentCount, toDoOverallCount, toDoDoneCount, UserModelBuilder.BuildUsers(card.AssignedUsers), attachmentsCount, card.ArchivedAt);
        }
    }
}