using FluentValidation;

namespace Queo.Boards.Core.Validators.Boards {
    /// <summary>
    ///     Validator für einen Board Titel
    /// </summary>
    public class BoardTitleValidator : AbstractValidator<string> {
        /// <summary>
        /// </summary>
        public BoardTitleValidator() {
            RuleFor(x => x).NotEmpty().Length(0, 60).WithMessage("Der Titel darf maximal 60 Zeichen lang sein.");
        }
    }
}