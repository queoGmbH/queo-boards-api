using System;
using Com.QueoFlow.Commons;
using NSwag.Annotations;

namespace Queo.Boards.Infrastructure.SignalR {
    /// <summary>
    ///     Attribute zur Dokumentation von Signal R Benachrichtigungen per Swagger.
    /// </summary>
    public class SwaggerSignalRAttribute : SwaggerResponseAttribute {
        public SwaggerSignalRAttribute(Type hubType, SignalrNotificationScope scope)
            : base(hubType.ToString(), hubType) {
            HubType = hubType;
            ScopeName = scope.ToString();
            ScopeDescription = scope.GetDescription();
            Description = scope.ToString();
        }

        public SwaggerSignalRAttribute(Type hubType, string scopeName, string scopeDesciption)
            : base(hubType.ToString(), hubType) {
            HubType = hubType;
            ScopeName = scopeName;
            ScopeDescription = scopeDesciption;
            Description = scopeDesciption;
        }

        /// <summary>
        ///     Ruft den Typen des Hubs ab, über welchen die Signal-R-Benachrichtigung erfolgt.
        /// </summary>
        public Type HubType { get; }

        /// <summary>
        ///     Ruft die Beschreibung ab, über welchen Kanal die Benachrichtigung erfolgt.
        /// </summary>
        public string ScopeDescription { get; }

        /// <summary>
        ///     Ruft die Bezeichnung des Scopes ab.
        /// </summary>
        public string ScopeName { get; set; }
    }
}