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

namespace Queo.Boards.Controllers {
    /// <summary>
    ///     Controller für Checklisten
    /// </summary>
    [RoutePrefix("api")]
    public class ChecklistController : AuthorizationRequiredApiController {
        private readonly IChecklistService _checklistService;
        private readonly ITaskService _taskService;

        /// <summary>
        /// </summary>
        /// <param name="checklistService"></param>
        /// <param name="taskService"></param>
        public ChecklistController(IChecklistService checklistService, ITaskService taskService) {
            _checklistService = checklistService;
            _taskService = taskService;
        }

        /// <summary>
        ///     Erstellt eine neue Aufgabe an einer Checkliste
        /// </summary>
        /// <param name="checklist">Business Id der Checkliste als Guid</param>
        /// <param name="taskTitleDto">DTO mit dem Titel der Aufgabe</param>
        /// <returns></returns>
        [HttpPost]
        [Route("checklists/{checklist:Guid}/tasks")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(TaskModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult CreateTask([ModelBinder] Checklist checklist, StringValueDto taskTitleDto) {
            Task task = _taskService.Create(checklist, taskTitleDto.Value);
            return Ok(TaskModelBuilder.Build(task));
        }

        /// <summary>
        ///     Löscht eine Checklist inkl. der Tasks
        /// </summary>
        /// <param name="checklist"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("checklists/{checklist:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ChecklistModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult Delete([ModelBinder] Checklist checklist) {
            if (!ModelState.IsValid) {
                if (checklist == null) {
                    return NotFound();
                }
            }
            _checklistService.Delete(checklist);
            return Ok(ChecklistModelBuilder.Build(checklist));
        }

        /// <summary>
        ///     Aktualisiert den Titel einer Karte
        /// </summary>
        /// <param name="checklist">Business Id der Checkliste als Guid</param>
        /// <param name="titleDto">DTO mit dem neuen Titel</param>
        /// <returns></returns>
        [HttpPut]
        [Route("checklists/{checklist:Guid}/title")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ChecklistModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult UpdateTitle([ModelBinder] Checklist checklist, StringValueDto titleDto) {
            Checklist updatedChecklist = _checklistService.UpdateTitle(checklist, titleDto.Value);
            return Ok(ChecklistModelBuilder.Build(updatedChecklist));
        }
    }
}