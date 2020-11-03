using Queo.Boards.Infrastructure.Http;

namespace Queo.Boards.Infrastructure.Controller {
    [Authorize]
    public abstract class AuthorizationRequiredApiController : BaseApiController {

    }
}