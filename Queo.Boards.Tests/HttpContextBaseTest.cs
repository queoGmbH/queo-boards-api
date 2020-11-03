using System;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Infrastructure.Security;
using Queo.Boards.Tests.Controllers;
using Spring.Context.Support;

namespace Queo.Boards.Tests {
    [TestClass]
    public class HttpContextBaseTest : ControllerBaseTest {
        
        protected HttpActionContext CreateActionContext<TController>(HttpMethod httpMethod, string requestUri, User user)
            where TController : IHttpController {
            return CreateActionContext<TController>(httpMethod, new Uri(requestUri), user);
        }

        protected HttpActionContext CreateActionContext<TController>(HttpMethod httpMethod, Uri requestUri, User user) where TController : IHttpController {
            TController controller = ContextRegistry.GetContext().GetObject<TController>();
            UserStoreAdapter userStoreAdapter = ContextRegistry.GetContext().GetObject<UserStoreAdapter>();
            ApplicationUserManager userManager = new ApplicationUserManager(userStoreAdapter);

            HttpRequestContext httpRequestContext = new HttpRequestContext();
            if (user != null) {
                IPrincipal principal = new ClaimsPrincipal(userManager.CreateIdentityAsync(UserStoreAdapter.CreateQueoBoardsSecurityUser(user), "").Result);
                httpRequestContext.Principal = principal;
            }


            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, requestUri);
            HttpControllerDescriptor httpControllerDescriptor = new HttpControllerDescriptor();
            HttpControllerContext controllerContext = new HttpControllerContext(httpRequestContext, httpRequestMessage, httpControllerDescriptor, controller);
            HttpActionContext httpActionContext = new HttpActionContext(controllerContext, new ReflectedHttpActionDescriptor());

            return httpActionContext;
        }
    }
}