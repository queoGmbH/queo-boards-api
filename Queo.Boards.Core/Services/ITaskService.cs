using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Services {
    /// <summary>
    ///     Schnittstelle für Services für <see cref="Task" />
    /// </summary>
    public interface ITaskService {
        /// <summary>
        ///     Erstellt einen neuen Task zu einer <see cref="Checklist" />
        /// </summary>
        /// <param name="checklist"></param>
        /// <param name="taskTitle"></param>
        /// <returns></returns>
        Task Create(Checklist checklist, string taskTitle);

        /// <summary>
        ///     Löscht den Task
        /// </summary>
        /// <param name="task"></param>
        void Delete(Task task);

        /// <summary>
        ///     Aktualisiert ob ein Task abgechlossen ist oder nicht.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="isDone"></param>
        /// <returns></returns>
        Task UpdateDone(Task task, bool isDone);

        /// <summary>
        /// Kopiert einen Task.
        /// </summary>
        /// <param name="targetChecklist">Die Checkliste, an welche der Task kopiert werden soll. Die erstellte Kopie wird direkt der Checkliste angehangen.</param>
        /// <param name="sourceTask">Der zu kopierende Task.</param>
        /// <returns></returns>
        Task Copy(Checklist targetChecklist, Task sourceTask);
    }
}