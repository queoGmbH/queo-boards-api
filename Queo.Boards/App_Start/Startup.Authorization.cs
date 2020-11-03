using System.Web.Http;
using Owin;
using Queo.Boards.Infrastructure.Filter;

namespace Queo.Boards {
    /// <summary>
    ///     Teil der Startup-Klasse, der für die Konfiguration der Autorisierung verantwortlich ist.
    /// 
    ///     Dazu können zum Beispiel Anwendungsweite Filter hinzugefügt werden, die sich um die Zugriffsprüfung für bestimmte Anfragen kümmern.
    /// </summary>
    public partial class Startup {
        private void ConfigureAuthorization(IAppBuilder app) {

            /*Filter, für den korrekten Board-Typen. Muss vor BoardScopeAuthorizationFilterAttribute hinzugefügt werden*/
            GlobalConfiguration.Configuration.Filters.Add(new BoardTypeFilterAttribute());

            /*Filter, der sicherstellt, dass der Nutzer berechtigt ist, dass Board zu sehen bzw. zu bearbeiten.*/
            GlobalConfiguration.Configuration.Filters.Add(new BoardScopeAuthorizationFilterAttribute());

            /*Aufruf der FluentValidation für die Parameter, wenn definiert.*/
            GlobalConfiguration.Configuration.Filters.Add(new FluentValidationFilterAttribute());

            /*Überprüft, den ModelState und manipuliert den HttpStatusCode, wenn der ModelState nicht valide ist.*/
            GlobalConfiguration.Configuration.Filters.Add(new ValidateModelStateFilter());
            
        }
    }
}