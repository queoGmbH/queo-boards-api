using Queo.Boards.Core.Tests.Builders.Activities;
using Queo.Boards.Core.Validators.Cards;
using Queo.Boards.Core.Validators.Lists;
using Spring.Context;

namespace Queo.Boards.Core.Tests.Builders {
    public class Create {
        private readonly IApplicationContext _context;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public Create(IApplicationContext context) {
            _context = context;
        }

        public ActivityBuilder Activity() {
            if (HasContext()) {
                return (ActivityBuilder)_context.GetObject("activityBuilder");
            } else {
                return new ActivityBuilder(null, User());
            }
        }

        public BoardBuilder Board() {
            if (HasContext()) {
                return _context.GetObject<BoardBuilder>();
            } else {
                return new BoardBuilder(null, new UserBuilder(null));
            }
        }

        public BoardActivityBuilder BoardActivity() {
            if (HasContext()) {
                return _context.GetObject<BoardActivityBuilder>();
            } else {
                return new BoardActivityBuilder(null, null, User());
            }
        }

        public CardBuilder Card() {
            if (HasContext()) {
                return _context.GetObject<CardBuilder>();
            } else {
                return new CardBuilder(null, List(), User());
            }
        }

        public CardDtoValidator CardCreateDtoValidator() {
            if (HasContext()) {
                return _context.GetObject<CardDtoValidator>();
            } else {
                return new CardDtoValidator(new CardNameValidator(), new DueDateTimeValidator());
            }
        }

        public CardNotificationBuilder CardNotification() {
            if (HasContext()) {
                return _context.GetObject<CardNotificationBuilder>();
            } else {
                return new CardNotificationBuilder(null, Card(), User());
            }
        }

        public CardTitleValidator CardUpdateDtoValidator() {
            if (HasContext()) {
                return _context.GetObject<CardTitleValidator>();
            } else {
                return new CardTitleValidator();
            }
        }

        public ChecklistBuilder Checklist() {
            if (HasContext()) {
                return _context.GetObject<ChecklistBuilder>();
            } else {
                return new ChecklistBuilder(null, Card());
            }
        }

        public CommentBuilder Comment() {
            if (HasContext()) {
                return _context.GetObject<CommentBuilder>();
            } else {
                return new CommentBuilder(null, Card(), User());
            }
        }

        public CommentNotificationBuilder CommentNotification() {
            if (HasContext()) {
                return _context.GetObject<CommentNotificationBuilder>();
            } else {
                return new CommentNotificationBuilder(null, Comment(), User());
            }
        }

        public DocumentBuilder Document() {
            if (HasContext()) {
                return _context.GetObject<DocumentBuilder>();
            } else {
                return new DocumentBuilder(null, Card());
            }
        }

        public DueValidator DueValidator() {
            if (HasContext()) {
                return _context.GetObject<DueValidator>();
            } else {
                return new DueValidator(new DueDateTimeValidator());
            }
        }

        public LabelBuilder Label() {
            if (HasContext()) {
                return _context.GetObject<LabelBuilder>();
            } else {
                return new LabelBuilder(null, Board());
            }
        }

        public ListBuilder List() {
            if (HasContext()) {
                return _context.GetObject<ListBuilder>();
            } else {
                return new ListBuilder(null, Board());
            }
        }

        public ListCreateAndUpdateValidator ListCreateAndUpdateValidator() {
            if (HasContext()) {
                return _context.GetObject<ListCreateAndUpdateValidator>();
            } else {
                return new ListCreateAndUpdateValidator(new ListNameValidator());
            }
        }

        public TaskBuilder Task() {
            if (HasContext()) {
                return _context.GetObject<TaskBuilder>();
            } else {
                return new TaskBuilder(null, Checklist());
            }
        }

        public TeamBuilder Team() {
            if (HasContext()) {
                return _context.GetObject<TeamBuilder>();
            } else {
                return new TeamBuilder(null, User());
            }
        }

        public UserBuilder User() {
            if (HasContext()) {
                return _context.GetObject<UserBuilder>();
            } else {
                return new UserBuilder(null);
            }
        }

        private bool HasContext() {
            return _context != null;
        }
    }
}