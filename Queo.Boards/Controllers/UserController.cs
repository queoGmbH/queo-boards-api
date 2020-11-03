using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using FluentValidation.Attributes;
using log4net;
using log4net.Core;
using Microsoft.AspNet.Identity;
using NSwag.Annotations;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Exceptions;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Validators.Users;
using Queo.Boards.Infrastructure.Controller;

namespace Queo.Boards.Controllers {
    /// <summary>
    ///     Controller for <see cref="User" />
    /// </summary>
    [RoutePrefix("api/user")]
    public class UserController : AuthorizationRequiredApiController {
        private readonly INotificationService _notificationService;
        private readonly ISecurityService _securityService;

        private readonly IUserService _userService;

        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="notificationService"></param>
        /// <param name="securityService"></param>
        public UserController(IUserService userService, INotificationService notificationService, ISecurityService securityService) {
            _userService = userService;
            _notificationService = notificationService;
            _securityService = securityService;
        }

        /// <summary>
        ///     Creates a new local user
        /// </summary>
        /// <param name="createData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(UserListModel), Description = "If the user could successfully be created.")]
        [SwaggerResponse((HttpStatusCode)420, typeof(void), Description = "If the limit of users is exceeded.")]
        [SwaggerResponse((HttpStatusCode)422, typeof(void), Description = "If there are any validation errors in the data.")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "If another user with same username already exists.")]
        [Queo.Boards.Infrastructure.Http.Authorize(Roles = UserRole.SYSTEM_ADMINISTRATOR)]
        public IHttpActionResult CreateLocalUser(
            [Validator(typeof(UserCreateModelValidator))]
            UserCreateModel createData) {
            User existingUser = _userService.FindByUsername(createData.Name);
            if (existingUser != null) {
                return Conflict("Another user with same username already exists.");
            }

            UserProfileDto profileDto = new UserProfileDto(
                createData.Mail,
                createData.Firstname,
                createData.Lastname,
                createData.Company,
                createData.Department,
                createData.Phone);
            UserAdministrationDto administrationDto = new UserAdministrationDto(createData.Roles, createData.IsEnabled);
            string hash = _securityService.HashPassword(createData.Password);

            try {
                UserListModel userListModel =
                    UserListModelBuilder.Build(_userService.Create(createData.Name, hash, administrationDto, profileDto, createData.CanWrite));

                return Ok(userListModel);
            } catch (UserLimitReachedException) {
                return PolicyNotFulfilled("User limit exceeded.");
            }
        }

        /// <summary>
        ///     Liefert alle Nutzer der Anwendung.
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<UserModel>))]
        public IHttpActionResult GetAll() {
            return Ok(_userService.GetAll(PageRequest.All).ToList().Select(UserModelBuilder.BuildUser));
        }

        /// <summary>
        ///     Liefert alle Nutzer für den Sys-Admin mit ausfühlichen Informationen
        /// </summary>
        /// <returns></returns>
        [Route("with-details")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<UserListModel>))]
        [Queo.Boards.Infrastructure.Http.Authorize(Roles = UserRole.SYSTEM_ADMINISTRATOR)]
        public IHttpActionResult GetAllForSysAdmin() {
            return Ok(_userService.GetAll(PageRequest.All).ToList().Select(UserListModelBuilder.Build));
        }

        /// <summary>
        ///     Liefert die Rollen, die ein Nutzer bei einem Board hat.
        ///     Mögliche Rollen:
        ///     <see cref="BoardRole.BOARD_ROLE_OWNER">BOARD_OWNER</see>: Der Nutzer ist als Besitzer des Boards hinterlegt
        ///     <see cref="BoardRole.BOARD_ROLE_USER">BOARD_User</see>: Der Nutzer ist ein Nutzer des Boards, da er Besitzer,
        ///     explizites Mitglied oder über ein Team am Board hinterlegt ist.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="currentUser">Der Nutzer, dessen Rollen für ein Board abgefragt werden sollen.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("me/roles/boards/{board:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string[]))]
        public IHttpActionResult GetUsersBoardRoles([ModelBinder] Board board, [ModelBinder] [SwaggerIgnore] User currentUser) {
            Require.NotNull(board, "board");
            Require.NotNull(currentUser, "currentUser");

            IList<string> roles = new List<string>();
            if (board.Owners.Contains(currentUser)) {
                /*Wenn der Nutzer einer der Besitzer des Boards ist, hat er u.a. die Rolle "BOARD_OWNER"*/
                roles.Add(BoardRole.BOARD_ROLE_OWNER);
            }

            if (board.Accessibility == Accessibility.Public || board.GetBoardUsers().Contains(currentUser)) {
                /*Wenn das Board öffentlich ist oder der Nutzer unter den Nutzern des Boards ist, hat er u.a. die Rolle "BOARD_USER"*/
                roles.Add(BoardRole.BOARD_ROLE_USER);
            }

            return Ok(roles.ToArray());
        }

        /// <summary>
        ///     Ruft den aktuell angemeldeten Nutzer ab.
        /// </summary>
        /// <returns></returns>
        [Route("me")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(UserModel))]
        public IHttpActionResult Me([SwaggerIgnore] [ModelBinder] User currentUser) {
            Require.NotNull(currentUser, "currentUser");

            return Ok(UserModelBuilder.BuildUser(currentUser));
        }

        [Route("me/password")]
        [HttpPut]
        public IHttpActionResult MeChangePassword([FromBody] PasswordUpdateModel passwordUpdateModel,
            [SwaggerIgnore] [ModelBinder] User currentUser) {
            PasswordVerificationResult passwordVerificationResult =
                new PasswordHasher().VerifyHashedPassword(currentUser.PasswordHash, passwordUpdateModel.OldPassword);
            if (passwordVerificationResult == PasswordVerificationResult.Failed) {
                return BadRequest("current password and old password confirmation are not equal.");
            }

            if (!passwordUpdateModel.NewPassword.Equals(passwordUpdateModel.NewPasswordConfirmation)) {
                return BadRequest("password and password confirmation are not equal.");
            }

            _userService.Update(currentUser, passwordUpdateModel.OldPassword, passwordUpdateModel.NewPassword);
            return Ok();
        }

        /// <summary>
        ///     Ruft die Rolle(n) des aktuell angemeldeten Nutzers ab.
        /// </summary>
        /// <returns>Die Rollen als String-Array</returns>
        [Route("me/roles")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string[]))]
        public IHttpActionResult MeRoles([SwaggerIgnore] [ModelBinder] User currentUser) {
            Require.NotNull(currentUser, "currentUser");

            return Ok(currentUser.Roles);
        }

        /// <summary>
        ///     Führt ein Passwortreset aus / stößt ihn an.
        /// </summary>
        /// <param name="passwordResetModel"></param>
        /// <returns></returns>
        [Route("me/passwordreset")]
        [AllowAnonymous]
        [HttpPut]
        public IHttpActionResult PasswordReset([FromBody] PasswordResetModel passwordResetModel) {
            LogManager.GetLogger(GetType()).Info($"Reset für RequestId: {passwordResetModel.PasswortResetRequestId}");

            if (ModelState.IsValid) {
                try {
                    _userService.ResetPassword(passwordResetModel.PasswortResetRequestId, passwordResetModel.NewPassword);
                } catch (Exception e) {
                    return UnprocessableEntity(e.Message);
                }
                return Ok();
            }
            return BadRequest("Neues Passwort darf nicht leer sein.");
        }

        /// <summary>
        ///     Setzt einen Passwort-Reset-Request für den übergebenen Nutzer
        /// </summary>
        /// <param name="passwordResetRequestModel"></param>
        /// <returns></returns>
        [Route("me/passwordresetrequest")]
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult PasswordResetRequest([FromBody] PasswordResetRequestModel passwordResetRequestModel) {
            LogManager.GetLogger(GetType()).Info($"ResetRequest für Nutzer: {passwordResetRequestModel.UserName}");
            if (ModelState.IsValid) {
                try {
                    _userService.PasswordResetRequest(passwordResetRequestModel.UserName);
                } catch (Exception e) {
                    return UnprocessableEntity(e.Message);
                }
                return Ok();
            }
            return BadRequest("Nutzername darf nicht leer sein");
        }

        /// <summary>
        ///     Updates an existing user with data from the <see cref="UserUpdateModel" />.
        /// </summary>
        /// <param name="userToUpdate">Guid -> Model bound to existing user</param>
        /// <param name="updateData">Updated values</param>
        /// <returns>Updated user for list view.</returns>
        [HttpPatch]
        [Route("{userToUpdate:guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(UserListModel), Description = "If the user could succesfully be updated.")]
        [SwaggerResponse((HttpStatusCode)422, typeof(void), Description = "If there are any validation errors in the data.")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "If another user with same username already exists.")]
        [Queo.Boards.Infrastructure.Http.Authorize(Roles = UserRole.SYSTEM_ADMINISTRATOR)]
        public IHttpActionResult Update(
            [ModelBinder] User userToUpdate, [FromBody] [Validator(typeof(UserUpdateModelValidator))]
            UserUpdateModel updateData) {
            User existingUser = _userService.FindByUsername(updateData.Name);
            if (existingUser != null && !Equals(existingUser, userToUpdate)) {
                return Conflict("Another user with same username already exists.");
            }

            User updatedLocalUser = _userService.UpdateLocalUser(userToUpdate, updateData);
            return Ok(UserListModelBuilder.Build(updatedLocalUser));
        }

        /// <summary>
        ///     Updates the password for an existing user.
        /// </summary>
        /// <param name="userToUpdate"></param>
        /// <param name="userUpdatePasswordModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{userToUpdate:guid}/password")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(UserListModel), Description = "If the user could succesfully be updated.")]
        [Queo.Boards.Infrastructure.Http.Authorize(Roles = UserRole.SYSTEM_ADMINISTRATOR)]
        public IHttpActionResult UpdatePassword([ModelBinder] User userToUpdate, [FromBody] UserUpdatePasswordModel userUpdatePasswordModel) {
            User updatedLocalUser = _userService.Update(userToUpdate, userUpdatePasswordModel.NewPassword);

            return Ok(UserListModelBuilder.Build(updatedLocalUser));
        }
    }
}