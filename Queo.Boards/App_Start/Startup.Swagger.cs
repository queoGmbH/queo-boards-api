using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NHibernate.Impl;
using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;
using NSwag;
using NSwag.AspNet.Owin;
using NSwag.SwaggerGeneration.WebApi.Processors.Security;
using Owin;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;
using Queo.Boards.Infrastructure.SignalR;
using Spring.Context.Support;

namespace Queo.Boards {
    /// <summary>
    ///     Teil der Startup-Klasse, der für die Konfiguration von Swagger verantwortlich ist.
    /// </summary>
    public partial class Startup {
        private static void ConfigureSwagger(IAppBuilder app) {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string version = executingAssembly.GetName().Version.ToString();
            string informationalVersion = executingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            SessionFactoryImpl sessionFactoryImpl = ContextRegistry.GetContext().GetObject<SessionFactoryImpl>();
            string databasename = sessionFactoryImpl.ConnectionProvider.GetConnection().Database;
            string timeStamp;
            try {
                timeStamp = File.GetLastWriteTime(executingAssembly.Location).ToString("f");
            } catch (Exception) {
                timeStamp = "unbekannt";
            }

            SwaggerUiOwinSettings swaggerUiOwinSettings = new SwaggerUiOwinSettings() {
                DefaultEnumHandling = EnumHandling.String,
                DefaultPropertyNameHandling = PropertyNameHandling.CamelCase,
                FlattenInheritanceHierarchy = true,
                Title = "queo boards API",
                Description = string.Format("Datenbank: {0} <br/> Version: {1} <br/> Build-Zeitpunkt: {3} <br/> Git-Version-Hash: {2}", databasename, version, informationalVersion, timeStamp),
                Version = string.Format("{0} ({1})", version, informationalVersion),
                ShowRequestHeaders = true,
                UseJsonEditor = true,
                OAuth2Client = new OAuth2ClientSettings {
                    ClientId = "foo",
                    ClientSecret = "bar",
                    AppName = "my_app",
                    Realm = "my_realm",
                    AdditionalQueryStringParameters = {
                        { "foo", "bar" }
                    }
                },
                DocumentProcessors = {
                    new SecurityDefinitionAppender("token",
                        new SwaggerSecurityScheme() {
                            Type = SwaggerSecuritySchemeType.ApiKey,
                            Name = "Authorization",
                            In = SwaggerSecurityApiKeyLocation.Header,
                            Description = "Bearer token"
                        }),
                    new SwaggerSignalRAttributeProcessor()
                },
                OperationProcessors = {
                    new OperationSecurityScopeProcessor("token")
                }
            };

            /*Alle von Entity abgeleiteten Klassen, die nicht abstrakt sind ermitteln*/
            List<Type> entityTypes = typeof(Entity).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Entity)) && !type.IsAbstract).ToList();
            foreach (Type entityType in entityTypes) {
                /*Alle abgeleiteten Typen von Entity, werden über eine Guid gemapped, die per Dao geladen werden.*/
                swaggerUiOwinSettings.TypeMappers.Add(new PrimitiveTypeMapper(entityType, s => s.Type = JsonObjectType.String));
            }

            app.UseSwaggerUi(typeof(Startup).Assembly, swaggerUiOwinSettings);
        }
    }
}