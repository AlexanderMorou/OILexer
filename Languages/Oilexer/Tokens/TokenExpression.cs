using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    /// <summary>
    /// Provides a base implementation of <see cref="ITokenExpression"/> 
    /// </summary>
    public class TokenExpression :
        ControlledCollection<ITokenItem>,
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
                this.baseList.Add(iti);
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (ITokenItem ite in this.baseList)
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
                return base.baseList;
            }
        }

        public string Name
        {
          get { return null; }
        }
    }
}
