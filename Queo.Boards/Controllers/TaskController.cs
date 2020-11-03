using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using NSwag.Annotations;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Hubs;
using Queo.Boards.Infrastructure.Controller;
using Queo.Boards.Infrastructure.SignalR;

namespace Queo.Boards.Controllers
{
    /// <summary>
    /// Controller für <see cref="Task"/>
    /// </summary>
    [System.Web.Http.RoutePrefix("api")]
    public class TaskController : AuthorizationRequiredApiController {
        private readonly ITaskService _taskService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskService"></param>
        public TaskController(ITaskService taskService) {
            _taskService = taskService;
        }

        /// <summary>
        /// Aktualisiert ob ein Task abgeschlossen ist, oder nicht.
        /// </summary>
        /// <param name="task">Business Id des Tasks als Guid</param>
        /// <param name="isDoneDto">Dto mit bool Wert, ob der Task abgeschlossen ist..</param>
        /// <returns></returns>
        [HttpPut]
        [Route("task/{task:Guid}/isDone")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(TaskModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult UpdateDone([ModelBinder] Task task, BoolValueDto isDoneDto) {
            Task updatedTask = _taskService.UpdateDone(task, isDoneDto.Value);
            return Ok(TaskModelBuilder.Build(updatedTask));
        }

        /// <summary>
        /// Löscht den Task
        /// </summary>
        /// <param name="task">Business Id des Tasks als Guid</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("task/{task:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(TaskModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult Delete([ModelBinder] Task task) {
            _taskService.Delete(task);
            return Ok(TaskModelBuilder.Build(task));
        }
    }
}
