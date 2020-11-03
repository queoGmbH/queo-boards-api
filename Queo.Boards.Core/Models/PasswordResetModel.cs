using System;
using Newtonsoft.Json;
using Spring.Objects.Factory.Attributes;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model welches Informationen für ei PaswortRest enthält.
    /// </summary>
    public class PasswordResetModel {
        /// <summary>
        /// </summary>
        /// <param name="passwortResetRequestId"></param>
        /// <param name="newPassword"></param>
        public PasswordResetModel(Guid passwortResetRequestId, string newPassword) {
            PasswortResetRequestId = passwortResetRequestId;
            NewPassword = newPassword;
        }

        /// <summary>
        ///     Liefert oder setzt das neue Passwort
        /// </summary>
        [JsonProperty("newPassword")]
        [Required]
        public string NewPassword { get; set; }

        /// <summary>
        ///     Liefert oder setzt die PasswortResetRequestId für die das neue Passwort gesetzt werden soll.
        /// </summary>
        [JsonProperty("passwordResetRequestId")]
        [Required]
        public Guid PasswortResetRequestId { get; set; }
    }
}