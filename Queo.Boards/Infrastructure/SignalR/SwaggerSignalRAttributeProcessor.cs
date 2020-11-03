using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NJsonSchema;
using NSwag;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Infrastructure.SignalR {
    /// <summary>
    ///     Prozessor für Swagger, der die Dokumentation für die Benachrichtigung per Signal-R nach Ausführung einer
    ///     Controller-Methode an dieser Methode im Swagger hinterlegt.
    /// </summary>
    public class SwaggerSignalRAttributeProcessor : IDocumentProcessor {
        public async Task ProcessAsync(DocumentProcessorContext context) {
            foreach (Type controller in context.ControllerTypes) {
                foreach (MethodInfo methodInfo in controller.GetMethods()) {
                    SwaggerSignalRAttribute[] attributes;
                    if (TryGetSwaggerSignalRAttribute(methodInfo, out attributes)) {
                        AddSignalrDocumentationToSwagger(context, methodInfo, attributes);
                    }
                }
            }
        }

        private static SwaggerResponse CreateSwaggerSignalrResponse(SwaggerSignalRAttribute attribute) {
            return new SwaggerResponse() {
                Description = string.Format("{0}:::{1} ----------------------------------- {2}", attribute.HubType.Name, attribute.ScopeDescription, attribute.ScopeDescription),
                Headers = new SwaggerHeaders() {
                    {
                        SignalrNotificationAttribute.SIGNAL_R_COMMAND_NAME, new JsonProperty() {
                            Title = SignalrNotificationAttribute.SIGNAL_R_COMMAND_NAME,
                            Description = "Name des Commands"
                        }
                    }, {
                        SignalrNotificationAttribute.SIGNAL_R_IGNORE_CONNECTIONS, new JsonProperty() {
                            Title = SignalrNotificationAttribute.SIGNAL_R_IGNORE_CONNECTIONS,
                            Description = "Liste der zu ignorierenden SignalR-Clients"
                        }
                    }
                }
            };
        }

        private void AddSignalrDocumentationToSwagger(DocumentProcessorContext context, MethodInfo methodInfo, SwaggerSignalRAttribute[] attributes) {
            Require.NotNull(attributes, "attributes");
            Require.NotNull(context, "context");

            SwaggerOperationDescription swaggerOperationDescription = context.Document.Operations.SingleOrDefault(op => op.Operation.OperationId == GetOperationIdFromMethodInfo(methodInfo));
            if (swaggerOperationDescription != null) {
                int signalRIndex = 1;
                foreach (SwaggerSignalRAttribute attribute in attributes) {
                    swaggerOperationDescription.Operation.Responses.Add("SignalR " + signalRIndex, CreateSwaggerSignalrResponse(attribute));
                    signalRIndex++;
                }
            }
        }

        private string GetOperationIdFromMethodInfo(MethodInfo methodInfo) {
            Require.NotNull(methodInfo, "methodInfo");

            return string.Format("{0}_{1}", methodInfo.DeclaringType.Name.Replace("Controller", ""), methodInfo.Name);
        }

        private bool TryGetSwaggerSignalRAttribute(MethodInfo methodInfo, out SwaggerSignalRAttribute[] attributes) {
            attributes = methodInfo.GetCustomAttributes<SwaggerSignalRAttribute>().ToArray();
            return attributes.Any();
        }
    }
}