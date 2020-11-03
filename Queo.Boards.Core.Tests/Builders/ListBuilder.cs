using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;

namespace Queo.Boards.Core.Tests.Builders {
    public class ListBuilder : Builder<List> {
        private readonly BoardBuilder _boardBuilder;
        private readonly IListDao _listDao;
        private DateTime? _archivedAt;
        private Board _board;
        private int? _positionOnBoard;
        private string _title = "list title";

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public ListBuilder(IListDao listDao, BoardBuilder boardBuilder) {
            _listDao = listDao;
            _boardBuilder = boardBuilder;
        }

        public ListBuilder ArchivedAt(DateTime archivedAt) {
            _archivedAt = archivedAt;
            return this;
        }

        public override List Build() {
            if (_board == null) {
                _board = _boardBuilder.Build();
            }
            List list = new List(_board, _title);
            if (_positionOnBoard.HasValue) {
                _board.Lists.Insert(_positionOnBoard.Value, list);
            } else {
                _board.Lists.Add(list);
            }

            if (_archivedAt.HasValue) {
                list.Archive(_archivedAt.Value.ToUniversalTime());
            }

            if (_listDao != null) {
                _listDao.Save(list);
                _listDao.Flush();
            }

            try {
                return list;
            } finally {
                SetDefaults();
            }
        }

        public ListBuilder OnBoard(Board board) {
            _board = board;
            return this;
        }

        public ListBuilder Position(int position) {
            _positionOnBoard = position;
            return this;
        }

        public ListBuilder Title(string title) {
            _title = title;
            return this;
        }

        private void SetDefaults() {
            _board = null;
            _positionOnBoard = null;
            _archivedAt = null;
            _title = "list title";
        }
    }
}