using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public class CommentEntry :
        Entry,
        ICommentEntry
    {
        /// <summary>
        /// Data member for <see cref="Comment"/>
        /// </summary>
        private string comment;

        /// <summary>
        /// Creates a new <see cref="Entry"/> with the <paramref name="column"/>, 
        /// <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="pathExplorationComment">The pathExplorationComment the <see cref="CommentEntry"/> represents.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="CommentEntry"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="CommentEntry"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="CommentEntry"/> 
        /// was declared at.</param>
        public CommentEntry(string comment, string fileName, int column, int line, long position)
            : base(fileName, column, line, position)
        {
            this.comment = comment;
        }

        public override string ToString()
        {
            return this.comment;
        }

        #region ICommentEntry Members

        /// <summary>
        /// Returns the pathExplorationComment the <see cref="CommentEntry"/> represents.
        /// </summary>
        public string Comment
        {
            get { return this.comment; }
        }

        #endregion
    }
}