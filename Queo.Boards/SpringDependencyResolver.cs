using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Spring.Context.Support;

namespace Queo.Boards {
    public class SpringDependencyResolver : DefaultDependencyResolver {
        public override object GetService(Type serviceType) {
            string typeName = GetSpringNetName(serviceType);
            object foundInSpring = null;
            try {
                foundInSpring = ContextRegistry.GetContext().GetObject(typeName);
            } catch {
            }

            return foundInSpring ?? base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType) {
            return new List<object>() { GetService(serviceType) };
        }

        private string GetSpringNetName(Type serviceType) {
            string typeName = serviceType.Name;
            if (serviceType.IsInterface) {
                /* Remove "I" if its an interface */
                typeName = typeName.Substring(1, typeName.Length - 1);
            }
            return typeName[0].ToString().ToLower() + typeName.Substring(1, typeName.Length - 1);
        }
    }
}