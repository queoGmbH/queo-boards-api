using FluentValidation;
using Queo.Boards.Core.Dtos;

namespace Queo.Boards.Core.Validators.Labels {
    /// <summary>
    ///     Validator für <see cref="LabelDto" />
    /// </summary>
    public class LabelDtoValidator : AbstractValidator<LabelDto> {
        /// <summary>
        /// </summary>
        public LabelDtoValidator() {
            RuleFor(x => x.Name).NotEmpty().Length(0, 25).WithMessage("Der Name darf maximal 25 Zeichen lang sein.");
            RuleFor(x => x.Color).NotEmpty().WithMessage("Es muss eine Farbe gesetzt sein.");
        }
    }
}