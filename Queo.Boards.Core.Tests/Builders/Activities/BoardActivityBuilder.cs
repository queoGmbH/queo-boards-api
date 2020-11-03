using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Activities;
using Queo.Boards.Core.Persistence;

namespace Queo.Boards.Core.Tests.Builders.Activities {
    public class BoardActivityBuilder : ActivityBuilder {
        private readonly IBoardActivityDao _boardActivityDao;

        private Board _board;

        public BoardActivityBuilder(IActivityBaseDao activityBaseDao, IBoardActivityDao boardActivityDao, UserBuilder userBuilder)
            : base(activityBaseDao, userBuilder) {
            _boardActivityDao = boardActivityDao;
        }

        public BoardActivity Build() {
            ActivityBase activityBase = BuildBase();
            BoardActivity boardActivity = new BoardActivity(activityBase.Creator, activityBase.CreationDate, activityBase.Text, _board);
            if (_boardActivityDao != null) {
                _boardActivityDao.Save(boardActivity);
                _boardActivityDao.Flush();
            }
            return boardActivity;
        }

        public BoardActivityBuilder ForBoard(Board board) {
            _board = board;
            return this;
        }
    }
}