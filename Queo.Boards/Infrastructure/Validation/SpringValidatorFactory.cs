using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Attributes;
using Spring.Context.Support;

namespace Queo.Boards.Infrastructure.Validation {
    /// <summary>
    ///     Validator Factory, welche die Validatoren für die Validierung via FluentValidation (per Attribute) instanziiert.
    /// </summary>
    /// <remarks>
    ///     Die Implementierung der Standard-Factory (<see cref="AttributedValidatorFactory" />) erwartet parameterlose
    ///     Konstruktoren, der Validatoren.
    ///     Da in dieser Anwendung die Validatoren im Spring konfiguriert sind und teilweise keine parameterlosen Konstruktoren
    ///     besitzen, wurde diese Factory implementiert.
    ///     Sie delegiert die Ermittlung der benötigten Validatoren an die <see cref="AttributedValidatorFactory" />, ändert
    ///     aber deren Art eine Instanz der Validatoren zu erzeugen, in dem nicht mehr ein parameterloser Konstruktor
    ///     aufgerufen wird, sondern versucht wird, die Instanz per Spring zu erzeugen.
    /// </remarks>
    public class SpringValidatorFactory : IValidatorFactory {

        /// <summary>
        /// Delegate, über welche die Typen der Validatoren ermittelt werden.
        /// Außerdem wird die "instanceFactory" ausgetauscht, welche die Instanzen der Validatoren erzeugt.
        /// </summary>
        private readonly IValidatorFactory _validatorFactory = new AttributedValidatorFactory(delegate(Type validatorType) {
            IDictionary<string, object> validatorTypes = ContextRegistry.GetContext().GetObjectsOfType(validatorType);
            IValidator validator = validatorTypes.First().Value as IValidator;
            return validator;
        });

        /// <summary>Gets the validator for the specified type.</summary>
        public IValidator<T> GetValidator<T>() {
            return _validatorFactory.GetValidator<T>();
        }

        /// <summary>Gets the validator for the specified type.</summary>
        public IValidator GetValidator(Type type) {
            return _validatorFactory.GetValidator(type);
        }
    }
}