using FluentValidation;
using Queo.Boards.Core.Dtos;

namespace Queo.Boards.Core.Validators.Lists {
    /// <summary>
    ///     Validator für <see cref="LabelDto" />
    /// </summary>
    public class ListCreateAndUpdateValidator : AbstractValidator<StringValueDto> {
        /// <summary>
        /// </summary>
        public ListCreateAndUpdateValidator(ListNameValidator listNameValidator) {
            RuleFor(x => x.Value).SetValidator(listNameValidator);
        }
    }
}