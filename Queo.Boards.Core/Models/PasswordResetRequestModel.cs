using Newtonsoft.Json;
using Spring.Objects.Factory.Attributes;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model zum erstellen eines Passwort-Reset-Request
    /// </summary>
    public class PasswordResetRequestModel {
        /// <summary>
        /// </summary>
        /// <param name="userName"></param>
        public PasswordResetRequestModel(string userName) {
            UserName = userName;
        }

        /// <summary>
        ///     Liefert den Namen des Nutzers für den das Passwort zurückgesetzt werden soll.
        /// </summary>
        [Required]
        [JsonProperty("userName")]
        public string UserName { get; }
    }
}