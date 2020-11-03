using System.Web.Http;
using Queo.Boards;
using Microsoft.Owin;
using Owin;
using Queo.Boards.Infrastructure.Filter;

[assembly: OwinStartup(typeof(Startup))]

namespace Queo.Boards {


    /// <summary>
    /// Allgemeiner Teil der Startup-Klasse, der für den Aufruf der spezifischen Konfigurationen verantwortlich ist.
    /// Für die einzelnen Bestandteile sind jeweils Partial-Klassen anzulegen. Die Dateien werden im Ordner AppStart angelegt und sollten folgendes
    /// Namensschema haben: Startup.[PartialName].cs
    /// In der Partial-Klasse ist eine statische Methode zu implementieren, die als Parameter IAppBuilder hat und von dieser Klasse hier aufgerufen wird.
    /// </summary>
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            // Weitere Informationen zm Konfigurieren Ihrer Anwendung finden Sie unter "http://go.microsoft.com/fwlink/?LinkID=316888".

            SpringDependencyResolver springDependencyResolver = new SpringDependencyResolver();

            GlobalConfiguration.Configuration.Filters.Add(new ErrorLoggingFilterAttribute());

            ConfigureAuthentication(app);
            ConfigureAuthorization(app);

            ConfigureSignalR(app, springDependencyResolver);

            ConfigureSwagger(app);

        }
    }
}