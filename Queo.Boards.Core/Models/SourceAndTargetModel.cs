namespace Queo.Boards.Core.Models {
    /// <summary>
    /// Model, welches in der Lage ist, eine Quelle und ein Ziel vom gleichen Typ abzubilden.
    /// </summary>
    /// <typeparam name="TParent">Der Typ des Parents, auf welches das Objekt verschoben wurde.</typeparam>
    /// <typeparam name="TMoved"></typeparam>
    
    public class SourceAndTargetModel<TParent, TMoved> {

        /// <param name="source">Von wo aus, wurde das Objekt verschoben.</param>
        /// <param name="target">Wohin wurde das Objekt verschoben.</param>
        /// <param name="moved"></param>
        public SourceAndTargetModel(TParent source, TParent target, TMoved moved) {
            Source = source;
            Target = target;
            Moved = moved;
        }

        /// <summary>
        /// Ruft die Quelle ab.
        /// </summary>
        public TParent Source { get; private set; }

        /// <summary>
        /// Ruft das Ziel ab.
        /// </summary>
        public TParent Target {
            get; private set;
        }

        /// <summary>
        /// Ruft das verschobene Objekt ab.
        /// </summary>
        public TMoved Moved { get; private set; }
    }
}