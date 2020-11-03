using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;

namespace Queo.Boards.Core.Tests.Builders {
    public class LabelBuilder : Builder<Label> {
        private readonly BoardBuilder _boardBuilder;
        private readonly ILabelDao _labelDao;
        private Board _board;
        private string _color = "ne Farbe";
        private string _name = "Labelname";

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public LabelBuilder(ILabelDao labelDao, BoardBuilder boardBuilder) {
            _labelDao = labelDao;
            _boardBuilder = boardBuilder;
        }

        public LabelBuilder ForBoard(Board board) {
            _board = board;
            return this;
        }

        public override Label Build() {
            if (_board == null) {
                _board = _boardBuilder.Build();
            }
            Label label = new Label(_board, _color, _name);
            _board.Labels.Add(label);
            if (_labelDao != null) {
                _labelDao.Save(label);
                _labelDao.Flush();
            }
            return label;
        }

        public LabelBuilder Color(string color) {
            _color = color;
            return this;
        }

        public LabelBuilder WithName(string name) {
            _name = name;
            return this;
        }
    }
}