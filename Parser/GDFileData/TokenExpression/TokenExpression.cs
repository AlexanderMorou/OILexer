using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer._Internal;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    /// <summary>
    /// Provides a base implementation of <see cref="ITokenExpression"/> 
    /// </summary>
    public class TokenExpression :
        ReadOnlyCollection<ITokenItem>,
        ITokenExpression
    {
        /// <summary>
        /// Data member for <see cref="Column"/>.
        /// </summary>
        private int column;
        /// <summary>
        /// Data member for <see cref="Line"/>.
        /// </summary>
        private int line;
        /// <summary>
        /// Data member for <see cref="Position"/>.
        /// </summary>
        private long position;
        /// <summary>
        /// Data member for <see cref="FileName"/>.
        /// </summary>
        private string fileName;

        /// <summary>
        /// Creates a new <see cref="TokenExpression"/> with the <paramref name="fileName"/>,
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="fileName">The file in which the <see cref="TokenExpression"/> was declared
        /// in.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="TokenExpression"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="TokenExpression"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="TokenExpression"/> 
        /// was declared at.</param>
        public TokenExpression(ICollection<ITokenItem> items, string fileName, int column, int line, long position)
        {
            this.fileName = fileName;
            this.column = column;
            this.line = line;
            this.position = position;
            foreach (ITokenItem iti in items)
                this.baseCollection.Add(iti);
        }
        #region IScannableEntryItem Members

        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="TokenExpression"/> was declared at.
        /// </summary>
        public int Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Returns the line index the <see cref="TokenExpression"/> was declared at.
        /// </summary>
        public int Line
        {
            get { return this.line; }
        }

        /// <summary>
        /// Returns the position in the file the <see cref="TokenExpression"/> was declared at.
        /// </summary>
        public long Position
        {
            get { return this.position; }
        }

        /// <summary>
        /// Returns the file the <see cref="TokenExpression"/> was declared in.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        #endregion

        #region IAmbiguousGDEntity Members

        public void Disambiguify(IGDFile context, IEntry root)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (ITokenItem ite in this.baseCollection)
            {
                if (first)
                    first = false;
                else
                    sb.Append(" ");
                sb.Append(ite.ToString());
            }
            return sb.ToString();
        }

        internal ICollection<ITokenItem> BaseCollection
        {
            get
            {
                return base.baseCollection;
            }
        }
    }
}
