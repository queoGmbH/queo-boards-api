namespace Queo.Boards.Core.Infrastructure.Templating {
    /// <summary>
    /// Schnittstelle für ein Template, das mit einem <see cref="IRenderContext"/> verarbeitet werden kann.
    /// </summary>
    public interface ITemplate {
        /// <summary>
        /// Rendert das Template mit den Angegebenen Daten.
        /// </summary>
        /// <param name="modelMap">Die Daten die im Template verwendet werden. Siehe: <see cref="ModelMap"/></param>
        /// <returns>Den gerenderten Text</returns>
        string Render(ModelMap modelMap);
    }
}