using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;

namespace Queo.Boards.Core.Tests.Builders {
    public class TaskBuilder : Builder<Task> {
        private readonly ChecklistBuilder _checklistBuilder;
        private readonly ITaskDao _taskDao;
        private Checklist _checklist;
        private string _title;
        private bool _isDone;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public TaskBuilder(ITaskDao taskDao, ChecklistBuilder checklistBuilder) {
            _taskDao = taskDao;
            _checklistBuilder = checklistBuilder;
        }

        public override Task Build() {
            if (_checklist == null) {
                _checklist = _checklistBuilder.Build();
            }
            Task task = new Task(_checklist, _title);
            _checklist.Tasks.Add(task);
            task.UpdateDone(_isDone);
            
            if (_taskDao != null) {
                _taskDao.Save(task);
                _taskDao.Flush();
            }
            return task;
        }

        public TaskBuilder OnChecklist(Checklist checklist) {
            _checklist = checklist;
            return this;
        }

        public TaskBuilder Title(string title) {
            _title = title;
            return this;
        }

        public TaskBuilder IsDone(bool isDone = true) {
            _isDone = isDone;
            return this;
        }
    }
}