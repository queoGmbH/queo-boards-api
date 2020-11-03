using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Builder für <see cref="AttachmentModel" />
    /// </summary>
    public static class DocumentModelBuilder {
        /// <summary>
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static AttachmentModel Build(Document document, User user) {
            Require.NotNull(document, "document");
            Require.NotNull(user, "user");

            return new AttachmentModel(BreadCrumbsModelBuilder.Build(document.Card), document.BusinessId, document.Name, AttachmentDownloadTokenizer.GetToken(new AttachmentDownload(user, document, DateTime.Now.AddHours(2))));
        }
    }
}