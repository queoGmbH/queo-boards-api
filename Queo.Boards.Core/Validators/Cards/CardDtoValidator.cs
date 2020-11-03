using FluentValidation;
using Queo.Boards.Core.Dtos;

namespace Queo.Boards.Core.Validators.Cards {
    /// <summary>
    ///     Validator für <see cref="LabelDto" />
    /// </summary>
    public class CardDtoValidator : AbstractValidator<CardDto> {
        /// <summary>
        /// </summary>
        public CardDtoValidator(CardNameValidator cardNameValidator, DueDateTimeValidator dueDateTimeValidator) {
            RuleFor(x => x.Title).SetValidator(cardNameValidator);
            RuleFor(x => x.Due).SetValidator(dueDateTimeValidator);
        }
    }
}