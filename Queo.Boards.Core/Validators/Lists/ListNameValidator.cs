using FluentValidation;

namespace Queo.Boards.Core.Validators.Lists {
    /// <summary>
    /// Validator für den Namen einer Liste.
    /// </summary>
    public class ListNameValidator : AbstractValidator<string> {
        public ListNameValidator() {
            RuleFor(x => x).NotEmpty().Length(0, 120).WithMessage("Der Titel darf maximal 120 Zeichen lang sein.");
        }
    }
}