using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    /// <summary>
    /// Provides a base implementation of <see cref="ITokenExpressionSeries"/> which provides
    /// a series of <see cref="ITokenExpression"/> instances declared in an <see cref="IGDFile"/>
    /// implementation segmented by <see cref="GDTokens.OperatorType.Pipe"/>
    /// </summary>
    public class TokenExpressionSeries :
        ReadOnlyCollection<ITokenExpression>,
        ITokenExpressionSeries
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
        /// Creates a new <see cref="TokenExpressionSeries"/> with the 
        /// <paramref name="expressions"/>, <paramref name="line"/>, <paramref name="column"/>,
        /// <paramref name="position"/>, and <paramref name="fileName"/> provided.
        /// </summary>
        /// <param name="expressions">The series of <see cref="ITokenExpression"/> instances
        /// which make up the <see cref="TokenExpressionSeries"/>.</param>
        /// <param name="line">The line at which the <see cref="TokenExpressionSeries"/> was
        /// defined.</param>
        /// <param name="column">The column on <paramref name="line"/> on which the
        /// <see cref="TokenExpressionSeries"/> declaration started.</param>
        /// <param name="position">The byte in the file/stream the <see cref="TokenExpressionSeries"/>
        /// started.</param>
        /// <param name="fileName">The file in which the <see cref="TokenExpressionSeries"/> was defined.</param>
        public TokenExpressionSeries(ITokenExpression[] expressions, int line, int column, long position, string fileName)
        {
            foreach (ITokenExpression ite in expressions)
                this.baseList.Add(ite);
            this.column = column; 
            this.line = line;
            this.position = position;
            this.fileName = fileName;
        }

        #region ITokenExpressionSeries Members

        /// <summary>
        /// Returns the column on the current <see cref="Line"/> the 
        /// <see cref="TokenExpressionSeries"/> was declared at.
        /// </summary>
        public int Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Returns the line index the <see cref="TokenExpressionSeries"/> was declared at.
        /// </summary>
        public int Line
        {
            get { return this.line; }
        }

        /// <summary>
        /// Returns the byte in the file the <see cref="TokenExpressionSeries"/> was declared at.
        /// </summary>
        public long Position
        {
            get { return this.position; }
        }

        /// <summary>
        /// Returns the file the <see cref="TokenExpressionSeries"/> was declared in.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (ITokenExpression ite in this.baseList)
            {
                if (first)
                    first = false;
                else
                {
                    sb.AppendLine(" | ");
                    sb.Append("\t");
                }
                sb.Append(ite.ToString().Replace("\r\n", "\r\n\t"));
            }
            return sb.ToString();
        }

        internal void Add(ITokenExpression ite)
        {
            this.baseList.Add(ite);
        }

        public string GetBodyString()
        {
            return this.ToString();
        }

        #endregion
    }
}
