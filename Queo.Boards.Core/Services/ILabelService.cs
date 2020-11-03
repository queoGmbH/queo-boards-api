using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;

namespace Queo.Boards.Core.Services {
    /// <summary>
    ///     Schnittstelle für <see cref="Label" />
    /// </summary>
    public interface ILabelService {
        /// <summary>
        ///     Erstellt ein neues Label zu einem Board
        /// </summary>
        /// <param name="board"></param>
        /// <param name="labelDto"></param>
        /// <returns></returns>
        Label Create(Board board, LabelDto labelDto);

        /// <summary>
        ///     Löscht ein Label und all seine Kartenzuordnungen
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        Label Delete(Label label);

        /// <summary>
        ///     Aktualisiert ein Label
        /// </summary>
        /// <param name="label"></param>
        /// <param name="labelDto"></param>
        /// <returns></returns>
        Label Update(Label label, LabelDto labelDto);
    }
}