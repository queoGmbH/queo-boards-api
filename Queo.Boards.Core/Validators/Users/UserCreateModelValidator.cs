using FluentValidation;
using Queo.Boards.Core.Models;

namespace Queo.Boards.Core.Validators.Users {
    /// <summary>
    ///     Validator for <see cref="UserCreateModel" />
    /// </summary>
    public class UserCreateModelValidator : AbstractValidator<UserCreateModel> {
        /// <summary>
        /// </summary>
        public UserCreateModelValidator() {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Es muss ein Nutzername angegeben werden.").Length(5, 100)
                .WithMessage("Der Nutzername muss zwischen 5 und 100 Zeichen lang sein.");
            RuleFor(x => x.Company).Length(0, 200).WithMessage("Der Firmenname darf maximal 200 Zeichen lang sein.");
            RuleFor(x => x.Department).Length(0, 200).WithMessage("Der Abteilungsname darf maximal 200 Zeichen lang sein.");
            RuleFor(x => x.Lastname).Length(0, 200).WithMessage("Der Nachname darf maximal 200 Zeichen lang sein.");
            RuleFor(x => x.Phone).Length(0, 200).WithMessage("Die Telefonnummer darf maximal 200 Zeichen lang sein.");
            RuleFor(x => x.Firstname).Length(0, 200).WithMessage("Der Vorname darf maximal 200 Zeichen lang sein.");
            RuleFor(x => x.Mail).EmailAddress().WithMessage("Es muss eine E-Mail Adresse eingegeben werden.").MaximumLength(200)
                .WithMessage("Die E-MailAdresse darf maximal 200 Zeichen lang sein.").Unless(x => string.IsNullOrEmpty(x.Mail));
            RuleFor(x => x.Password).NotEmpty().WithMessage("Es muss ein Passwort angegeben werden.").Length(5, 200)
                .WithMessage("Das Password muss mindestens 5, maximal 200 Zeichen Lang sein.");
        }
    }
}