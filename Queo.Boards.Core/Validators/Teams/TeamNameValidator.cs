using FluentValidation;

namespace Queo.Boards.Core.Validators.Teams {
    /// <summary>
    /// Validator f�r den Namen eines Teams.
    /// </summary>
    public class TeamNameValidator : AbstractValidator<string> {

        /// <summary>
        /// Ruft die maximale L�nge f�r Teamnamen ab.
        /// </summary>
        public const int MAX_LENGTH = 75;

        public TeamNameValidator() {
            RuleFor(x => x).NotEmpty().Length(0, MAX_LENGTH).WithMessage("Der Titel darf maximal 75 Zeichen lang sein.");
        }
    }
}