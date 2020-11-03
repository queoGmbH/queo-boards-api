using Moq;
using Queo.Boards.Controllers.Boards;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Validators.Lists;

namespace Queo.Boards.Tests.Builder.Controller {
    public class BoardControllerBuilder {
        private IBoardService _boardService;
        private IChecklistService _checklistService;
        private IListService _listService;
        private ILabelService _labelService;
        private IUserService _userService;
        private ListCreateAndUpdateValidator _listCreateAndUpdateValidator;

        public BoardController Build() {
            if (_boardService == null) {
                _boardService = new Mock<IBoardService>().Object;
            }
            if (_checklistService == null) {
                _checklistService = new Mock<IChecklistService>().Object;
            }
            if (_listService == null) {
                _listService = new Mock<IListService>().Object;
            }
            if (_labelService == null) {
                _labelService = new Mock<ILabelService>().Object;
            }
            if (_userService == null) {
                _userService = new Mock<IUserService>().Object;
            }
            if (_listCreateAndUpdateValidator == null) {
                //_listCreateAndUpdateValidator = Create.
            }
            //return new BoardController(_boardService,_checklistService,_listService,_labelService,_userService,);
            /* TODO */
            return null;
        }
    }
}