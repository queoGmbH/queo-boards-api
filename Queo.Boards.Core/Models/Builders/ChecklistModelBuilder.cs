using System.Collections.Generic;
using System.Linq;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Builder für <see cref="ChecklistModel" />
    /// </summary>
    public static class ChecklistModelBuilder {
        /// <summary>
        ///     Erstellt ein neues <see cref="ChecklistModel" />
        /// </summary>
        /// <param name="checklist"></param>
        /// <returns></returns>
        public static ChecklistModel Build(Checklist checklist) {
            IList<TaskModel> tasks = new List<TaskModel>();
            if (checklist.Tasks != null && checklist.Tasks.Any())
            {
                tasks = checklist.Tasks.Select(TaskModelBuilder.Build).ToList(); 
            }
            return new ChecklistModel(checklist.BusinessId, BreadCrumbsModelBuilder.Build(checklist.Card), checklist.Title, tasks);
        }
    }
}