using FluentValidation;

namespace Queo.Boards.Core.Validators.Cards {
    /// <summary>
    /// Validator für den Namen einer Karte.
    /// </summary>
    public class CardNameValidator : AbstractValidator<string> {
        public CardNameValidator() {
            RuleFor(x => x).NotEmpty().Length(0, 75).WithMessage("Der Titel darf maximal 75 Zeichen lang sein.");
        }
    }
}