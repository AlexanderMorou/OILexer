using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    public abstract class LiteralTokenItem<T> :
        TokenItem,
        ILiteralTokenItem<T>
    {
        private T value;
        private bool? isFlag;
        /// <summary>
        /// Creates a new <see cref="LiteralTokenItem{T}"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="value">The value that the <see cref="LiteralTokenItem{T}"/> represents.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="LiteralTokenItem{T}"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="LiteralTokenItem{T}"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="LiteralTokenItem{T}"/> was declared.</param>
        protected LiteralTokenItem(T value, int column, int line, long position)
            : base(column, line, position)
        {
            this.value = value;
        }

        #region ILiteralTokenItem<T> Members

        public T Value
        {
            get { return this.value; }
        }

        #endregion

        #region ILiteralTokenItem<T> Members

        public new ILiteralTokenItem<T> Clone()
        {
            return ((ILiteralTokenItem<T>)(base.Clone()));
        }

        #endregion

        #region ILiteralTokenItem Members

        object ILiteralTokenItem.Value
        {
            get { return this.Value; }
        }


        ILiteralTokenItem ILiteralTokenItem.Clone()
        {
            return this.Clone();
        }

        public bool? IsFlag
        {
            get { return this.isFlag; }
            internal set { this.isFlag = value; }
        }

        #endregion

        protected override string ToStringFurtherOptions()
        {
            if (this.isFlag.HasValue)
            {
                return string.Format("Flag={0};", this.isFlag.Value);
            }
            return null;
        }
    }
}
