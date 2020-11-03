using System;

namespace Queo.Boards.Core.Dtos {
    /// <summary>
    ///     Dto für PasswortResteRequests
    /// </summary>
    public class PasswordResetRequestDto {
        /// <summary>
        /// </summary>
        /// <param name="passwordResetRequestValidTo"></param>
        /// <param name="passwordResetRequestId"></param>
        public PasswordResetRequestDto(DateTime? passwordResetRequestValidTo, Guid? passwordResetRequestId) {
            PasswordResetRequestValidTo = passwordResetRequestValidTo;
            PasswordResetRequestId = passwordResetRequestId;
        }

        /// <summary>
        ///     Liefert oder setzt die eine Guid für einen Passwort-Reset
        /// </summary>
        public Guid? PasswordResetRequestId { get; set; }

        /// <summary>
        ///     Liefert oder setzt das Datum an dem ein Passwort-Reset-Request abläuft.
        /// </summary>
        public DateTime? PasswordResetRequestValidTo { get; set; }
    }
}