using FluentValidation;
using Queo.Boards.Core.Dtos;

namespace Queo.Boards.Core.Validators.Boards {
    /// <summary>
    ///     Validator für Create und Update eines Boards
    /// </summary>
    public class BoardCreateAndUpdateValidator : AbstractValidator<BoardDto> {
        /// <summary>
        /// </summary>
        /// <param name="boardTitleValidator"></param>
        public BoardCreateAndUpdateValidator(BoardTitleValidator boardTitleValidator) {
            RuleFor(x => x.Title).SetValidator(boardTitleValidator);
        }
    }
}