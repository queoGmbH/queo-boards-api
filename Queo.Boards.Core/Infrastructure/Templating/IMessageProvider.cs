namespace Queo.Boards.Core.Infrastructure.Templating {
    public interface IMessageProvider {
        /// <summary>
        /// Rendert eine Mailmessage aus dem angegebenen Template und verwendet dabei die Daten aus dem Model.
        /// </summary>
        /// <param name="templateName">Name des Templates</param>
        /// <param name="model">Daten f�r das Template</param>
        /// <returns></returns>
        string RenderMessage(string templateName, ModelMap model);
    }
}