using FluentValidation;

namespace Queo.Boards.Core.Validators.Teams {
    /// <summary>
    /// Validator für den Namen eines Teams.
    /// </summary>
    public class TeamNameValidator : AbstractValidator<string> {

        /// <summary>
        /// Ruft die maximale Länge für Teamnamen ab.
        /// </summary>
        public const int MAX_LENGTH = 75;

        public TeamNameValidator() {
            RuleFor(x => x).NotEmpty().Length(0, MAX_LENGTH).WithMessage("Der Titel darf maximal 75 Zeichen lang sein.");
        }
    }
}