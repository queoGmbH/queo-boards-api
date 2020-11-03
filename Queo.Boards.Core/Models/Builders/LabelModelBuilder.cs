using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Builder für <see cref="LabelModel" />
    /// </summary>
    public static class LabelModelBuilder {
        /// <summary>
        ///     Erstellt ein neues <see cref="LabelModel" />
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static LabelModel Build(Label label) {
            return new LabelModel(label.BusinessId, label.Name, label.Color, BreadCrumbsModelBuilder.Build(label.Board));
        }
    }
}