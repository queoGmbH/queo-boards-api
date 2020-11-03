using System;
using FluentValidation;

namespace Queo.Boards.Core.Validators.Cards {
    /// <summary>
    ///     Validator für ein Fälligkeitsdatum
    /// </summary>
    public class DueDateTimeValidator : AbstractValidator<DateTime?> {
        /// <summary>
        /// </summary>
        public DueDateTimeValidator() {
            RuleFor(x => x).GreaterThan(DateTime.Now.ToUniversalTime()).WithMessage("Der Wert muss in der Zukunft liegen");
        }
    }
}