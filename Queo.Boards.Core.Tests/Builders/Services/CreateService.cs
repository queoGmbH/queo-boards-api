using Moq;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Spring.Context;

namespace Queo.Boards.Core.Tests.Builders.Services {
    /// <summary>
    ///     Creator for service builders or service instances
    /// </summary>
    public class CreateService {
        private readonly IApplicationContext _context;

        /// <summary>
        ///     Initializes a new creator instance
        /// </summary>
        /// <param name="context">
        ///     The context to use to create builder instances or null to let the CreateService create new
        ///     builder instances.
        /// </param>
        public CreateService(IApplicationContext context) {
            _context = context;
        }

        /// <summary>
        ///     Creates a new <see cref="BoardServiceBuilder" />
        /// </summary>
        /// <returns></returns>
        public BoardServiceBuilder BoardService() {
            if (HasContext()) {
                return _context.GetObject<BoardServiceBuilder>();
            } else {
                return new BoardServiceBuilder(
                    new Mock<IBoardDao>().Object,
                    new Mock<ILabelService>().Object,
                    new Mock<IListService>().Object,
                    new Mock<ICardService>().Object,
                    new Mock<IEmailNotificationService>().Object);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="CardServiceBuilder" />
        /// </summary>
        /// <returns></returns>
        public CardServiceBuilder CardService() {
            if (HasContext()) {
                return _context.GetObject<CardServiceBuilder>();
            }

            return new CardServiceBuilder(new Mock<ICardDao>().Object, new Mock<IChecklistService>().Object, new Mock<ICommentService>().Object);
        }

        /// <summary>
        ///     Creates a new <see cref="UserServiceBuilder" />
        /// </summary>
        /// <returns></returns>
        public UserServiceBuilder UserService() {
            if (HasContext()) {
                return _context.GetObject<UserServiceBuilder>();
            } else {
                return new UserServiceBuilder(
                    new Mock<IUserDao>().Object,
                    new Mock<IActiveDirectoryService>().Object,
                    new Mock<ISecurityService>().Object, 
                    new Mock<IEmailNotificationService>().Object);
            }
        }

        private bool HasContext() {
            return _context != null;
        }
    }
}