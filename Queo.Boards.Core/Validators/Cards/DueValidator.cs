using FluentValidation;
using Queo.Boards.Core.Dtos;

namespace Queo.Boards.Core.Validators.Cards {
    /// <summary>
    ///     Validator für <see cref="DateTimeUpdateDto" /> im Kontext eines Fälligkeitsdatums.
    /// </summary>
    public class DueValidator : AbstractValidator<DateTimeUpdateDto> {
        /// <summary>
        /// </summary>
        public DueValidator(DueDateTimeValidator dueDateTimeValidator) {
            RuleFor(x => x.Value).SetValidator(dueDateTimeValidator);
        }
    }
}