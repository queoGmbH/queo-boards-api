using System.Collections.Generic;
using System.Linq;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Model Builder für <see cref="CardWithChecklistsMetadataModel" />
    /// </summary>
    public static class CardWithChecklistsMetadataModelBuilder {
        /// <summary>
        ///     Erstellt neue <see cref="CardWithChecklistsMetadataModel" />
        /// </summary>
        /// <param name="checklists"></param>
        /// <returns></returns>
        public static IList<CardWithChecklistsMetadataModel> Build(IList<Checklist> checklists) {
            IList<CardWithChecklistsMetadataModel> models = new List<CardWithChecklistsMetadataModel>();
            foreach (IGrouping<Card, Checklist> grouping in checklists.GroupBy(x=>x.Card)) {
                IList<ChecklistMetadataModel> checklistMetadataModels = grouping.Select(checklist => new ChecklistMetadataModel(checklist.BusinessId, checklist.Title, BreadCrumbsModelBuilder.Build(checklist.Card))).ToList();
                CardWithChecklistsMetadataModel model = new CardWithChecklistsMetadataModel(grouping.Key.Title, BreadCrumbsModelBuilder.Build(grouping.Key.List), checklistMetadataModels);
                models.Add(model);}
            return models;
        }
    }
}