using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using NSwag.Annotations;
using Queo.Boards.Infrastructure.Controller;

namespace Queo.Boards.Controllers {
    [RoutePrefix("api/systemconfiguration")]
    public class SystemConfigurationController : AuthorizationRequiredApiController {

        /// <summary>
        /// Configuration value of max users, set by spring
        /// </summary>
        public int MaxUser { get; set; }

        /// <summary>
        /// Returns the current system configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Dictionary<string, object>), Description = "A dictionary with a system configuration.")]
        public IHttpActionResult Get() {
            Dictionary<string, object> systemConfiguration = new Dictionary<string, object> {{"maxUser", MaxUser}};

            return Ok(systemConfiguration);
        }
    }
}