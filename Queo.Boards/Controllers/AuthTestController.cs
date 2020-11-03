using System.Web.Http;

namespace Queo.Boards.Controllers {
    public class AuthTestController : ApiController {
        [HttpPost]
        [Route("Token")]
        public IHttpActionResult Token(LoginModel loginModel) {
            return NotFound();
        }
    }

    public class LoginModel {
        
        public string Username { get; set; }

        public string Password {
            get; set;
        }
    }
}