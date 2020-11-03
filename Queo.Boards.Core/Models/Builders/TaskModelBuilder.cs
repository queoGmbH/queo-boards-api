using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Builder für <see cref="TaskModel" />
    /// </summary>
    public static class TaskModelBuilder {
        /// <summary>
        ///     Erstellt ein <see cref="TaskModel" /> aus einem <see cref="Task" />
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static TaskModel Build(Task task) {
            return new TaskModel(task.BusinessId, BreadCrumbsModelBuilder.Build(task.Checklist), task.Title, task.IsDone);
        }
    }
}