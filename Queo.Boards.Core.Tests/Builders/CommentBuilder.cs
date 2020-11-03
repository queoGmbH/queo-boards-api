using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;

namespace Queo.Boards.Core.Tests.Builders {
    public class CommentBuilder : Builder<Comment> {
        private readonly ICommentDao _commentDao;
        private readonly CardBuilder _cardBuilder;
        private readonly UserBuilder _userBuilder;
        private string _text;
        private Card _card;
        private User _creator;
        private DateTime? _creationDate;
        private bool _isDeleted;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public CommentBuilder(ICommentDao commentDao, CardBuilder cardBuilder, UserBuilder userBuilder) {
            _commentDao = commentDao;
            _cardBuilder = cardBuilder;
            _userBuilder = userBuilder;
        }

        public CommentBuilder Creator(User creator) {
            _creator = creator;
            return this;
        }

        public CommentBuilder CreationDate(DateTime creationDate) {
            _creationDate = creationDate;
            return this;
        }

        public CommentBuilder WithText(string text) {
            _text = text;
            return this;
        }

        public CommentBuilder OnCard(Card card) {
            _card = card;
            return this;
        }

        public override Comment Build() {
            if (_card == null) {
                _card = _cardBuilder.Build();
            }
            if (!_creationDate.HasValue) {
                _creationDate = DateTime.Now;
            }
            if (_creator == null) {
                _creator = _userBuilder.Build();
            }
            Comment comment = new Comment(_card, _text, _creator, _creationDate.Value);
            _card.Comments.Add(comment);
            comment.UpdateIsDeleted(_isDeleted);

            if (_commentDao != null) {
                _commentDao.Save(comment);
                _commentDao.Flush();
            }
            return comment;
        }

        public CommentBuilder LargeText() {
            _text = Texts.LargeText;
            return this;
        }

        public CommentBuilder Deleted(bool isDeleted = true) {
            _isDeleted = isDeleted;

            return this;
        }
    }
}