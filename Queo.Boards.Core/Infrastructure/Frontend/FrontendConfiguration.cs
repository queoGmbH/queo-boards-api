namespace Queo.Boards.Core.Infrastructure.Frontend {

    /// <summary>
    /// Enthält die Konfiguration für das Frontend.
    /// </summary>
    public class FrontendConfiguration {
        
        /// <summary>
        /// Ruft die Url der Startseite des Frontends ab.
        /// 
        /// Die Base-Url wird unter anderem beim E-Mail-Versand verwendet.
        /// </summary>
        public string BaseUrl { get; set; }

    }
}