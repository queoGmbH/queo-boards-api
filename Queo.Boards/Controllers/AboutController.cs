using System.Reflection;
using System.Web.Http;

namespace Queo.Boards.Controllers {

    [RoutePrefix("api/about")]
    public class AboutController : ApiController {

        [HttpGet]
        [Route("version")]
        public IHttpActionResult Version() {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return Ok(version);
        }

        [HttpGet]
        [Route("git")]
        public IHttpActionResult GitHash() {
            string informationalVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            return Ok(informationalVersion);
        }
    }
}