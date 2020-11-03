using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Builder für <see cref="CommentModel" />
    /// </summary>
    public static class CommentModelBuilder {
        /// <summary>
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static CommentModel Build(Comment comment) {
            string commentText = null;
            if (comment.IsDeleted) {
                commentText = "";
            }
            else {
                commentText = comment.Text;
            }
            return new CommentModel(comment.BusinessId, UserModelBuilder.BuildUser(comment.Creator), commentText, comment.CreationDate, comment.IsDeleted, BreadCrumbsModelBuilder.Build(comment.Card));
        }
    }
}