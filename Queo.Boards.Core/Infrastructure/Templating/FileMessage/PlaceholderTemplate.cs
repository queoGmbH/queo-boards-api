namespace Queo.Boards.Core.Infrastructure.Templating.FileMessage {
    /// <summary>
    /// Implementierung des <see cref="ITemplate"/> für den <see cref="PlaceholderRenderContext"/>.
    /// </summary>
    /// <remarks>
    /// Die Klasse wird vom <see cref="PlaceholderRenderContext"/> intern genutzt und darf nicht direkt verwendet werden.
    /// </remarks>
    internal class PlaceholderTemplate:ITemplate {
        private readonly string _template;
        private readonly PlaceholderRenderContext _placeholderRenderContext;

        public PlaceholderTemplate(string template, PlaceholderRenderContext placeholderRenderContext) {
            _template = template;
            _placeholderRenderContext = placeholderRenderContext;
        }

        /// <summary>
        /// Rendert das Template mit den Angegebenen Daten.
        /// </summary>
        /// <param name="modelMap">Die Daten die im Template verwendet werden. Siehe: <see cref="ModelMap"/></param>
        /// <returns>Den gerenderten Text</returns>
        public string Render(ModelMap modelMap) {
            return _placeholderRenderContext.ParseAndRender(_template, modelMap);
        }
    }
}