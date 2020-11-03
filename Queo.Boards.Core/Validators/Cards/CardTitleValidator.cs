using FluentValidation;
using Queo.Boards.Core.Dtos;

namespace Queo.Boards.Core.Validators.Cards {
    /// <summary>
    ///     Validator für <see cref="LabelDto" />
    /// </summary>
    public class CardTitleValidator : AbstractValidator<StringValueDto> {
        /// <summary>
        /// </summary>
        public CardTitleValidator() {
            RuleFor(x => x.Value).NotEmpty().Length(0, 75).WithMessage("Der Titel darf maximal 75 Zeichen lang sein.");
        }
    }
}