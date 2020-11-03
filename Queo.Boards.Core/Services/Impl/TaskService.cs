using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Persistence;
using Spring.Transaction.Interceptor;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service für <see cref="Task" />
    /// </summary>
    public class TaskService : ITaskService {
        private readonly ITaskDao _taskDao;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public TaskService(ITaskDao taskDao) {
            _taskDao = taskDao;
        }

        /// <summary>
        ///     Erstellt einen neuen Task zu einer <see cref="Checklist" />
        /// </summary>
        /// <param name="checklist"></param>
        /// <param name="taskTitle"></param>
        /// <returns></returns>
        [Transaction]
        public Task Create(Checklist checklist, string taskTitle) {
            Task task = new Task(checklist, taskTitle);
            _taskDao.Save(task);

            checklist.Tasks.Add(task);
            return task;
        }

        /// <summary>
        ///     Löscht den Task
        /// </summary>
        /// <param name="task"></param>
        [Transaction]
        public void Delete(Task task) {
            _taskDao.Delete(task);
        }

        /// <summary>
        ///     Aktualisiert ob ein Task abgeschlossen ist oder nicht.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="isDone"></param>
        /// <returns></returns>
        [Transaction]
        public Task UpdateDone(Task task, bool isDone) {
            task.UpdateDone(isDone);
            return task;
        }

        [Transaction]
        public Task Copy(Checklist targetChecklist, Task sourceTask) {
            Require.NotNull(targetChecklist, "targetChecklist");
            Require.NotNull(sourceTask, "sourceTask");


            Task copy = Create(targetChecklist, sourceTask.Title);
            copy.UpdateDone(sourceTask.IsDone);

            return copy;
        }
    }
}