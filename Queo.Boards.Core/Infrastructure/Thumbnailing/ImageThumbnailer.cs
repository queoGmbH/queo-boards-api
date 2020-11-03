using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Com.QueoFlow.Thumbnailer;
using Com.QueoFlow.Thumbnailer.Converter;
using Com.QueoFlow.Thumbnailer.Converter.ImageMagick;
using Com.QueoFlow.Thumbnailer.Converter.Shell;
using Com.QueoFlow.Thumbnailer.Output;
using Com.QueoFlow.Thumbnailer.Shared;
using Com.QueoFlow.Thumbnailer.Shared.Options;
using Common.Logging;
using QueoCsharpThumbnailer.Input.Stream;

namespace Queo.Boards.Core.Infrastructure.Thumbnailing {
    /// <summary>
    /// Thumbnailer, der ein Vorschaubild für ein <see cref="Image"/> erstellt und dieses im <see cref="OutputPath"/> ablegt.
    /// </summary>
    public class ImageThumbnailer {
        private readonly DirectoryInfo _outputPath;
        private readonly GenericThumbnailer<StreamWrapper, FileInfo> _thumbnailer;
        private DirectoryInfo _tempPath;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ImageThumbnailer));

        public ImageThumbnailer(DirectoryInfo mediaPath, DirectoryInfo ghostScriptPath) {
            _outputPath = new DirectoryInfo(mediaPath.FullName + "\\Thumbnails");
            _tempPath = new DirectoryInfo(mediaPath.FullName + "\\ThumbnailTemp");

            _logger.Info("Thumbnailer Output: " + _outputPath);
            _logger.Info("Thumbnailer Temp: " + _tempPath);
            _logger.Info("Thumbnailer GhostScript: " + ghostScriptPath);
            
            /*Aus dem Image eine Datei machen.*/
            StreamInputConverter inputConverter = new StreamInputConverter(_tempPath);

            /*Liste der zur Verfügung stehenden Converter die das Vorschaubild erzeugen.*/
            List<IConverter> converters = new List<IConverter>{ new ImageMagickConverter(ghostScriptPath), new ShellConverter() };

            SimpleFallbackConverter fallbackConverter = new SimpleFallbackConverter();
            FileInfoOutputConverter outputConverter = new FileInfoOutputConverter(_outputPath);

            _thumbnailer = new GenericThumbnailer<StreamWrapper, FileInfo>(inputConverter, null, converters, fallbackConverter, outputConverter);
        }

        public DirectoryInfo OutputPath {
            get { return _outputPath; }
        }

        public async Task<FileInfo> GetThumbnailAsync(StreamWrapper input, ConvertOptions options) {
            return await _thumbnailer.GetThumbnailAsync(input, options);
        }
    }
}