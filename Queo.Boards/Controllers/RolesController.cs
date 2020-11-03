using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using NSwag.Annotations;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Services;
using Queo.Boards.Infrastructure.Controller;

namespace Queo.Boards.Controllers {
    /// <summary>
    ///     Controller for <see cref="UserRole" /> entries
    /// </summary>
    [RoutePrefix("api/roles")]
    public class RolesController : AuthorizationRequiredApiController {
        private readonly IRoleService _roleService;

        /// <summary>
        ///     ctor.
        /// </summary>
        /// <param name="roleService"></param>
        public RolesController(IRoleService roleService) {
            _roleService = roleService;
        }

        /// <summary>
        ///     Returns a list of all <see cref="UserRole" /> entries.
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<string>))]
        [Queo.Boards.Infrastructure.Http.Authorize(Roles = UserRole.SYSTEM_ADMINISTRATOR)]
        public IHttpActionResult GetAllRoles() {
            return Ok(_roleService.GetAll());
        }
    }
}