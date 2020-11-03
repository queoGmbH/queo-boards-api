namespace Queo.Boards.Core.Infrastructure.Templating {
    /// <summary>
    /// Schnittstelle für einen RenderContext.
    /// </summary>
    public interface IRenderContext {
        /// <summary>
        /// Bereitet eine Vorlage für das Rendering vor. <see cref="ITemplate.Render"/>
        /// </summary>
        /// <param name="template"></param>
        /// <returns>Ein vorbereitetes Template, das gerendert werden kann.</returns>
        ITemplate Parse(string template);

        /// <summary>
        /// Bereitet eine Vorlage vor und rendert sie dann anhand der übergebenen Modelldaten.
        /// </summary>
        /// <param name="template">Die Vorlage</param>
        /// <param name="modelMap">Die Daten zum Rendern</param>
        /// <returns>Die gerenderte Vorlage</returns>
        string ParseAndRender(string template, ModelMap modelMap);
    }
}