using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    public abstract class LiteralProductionRuleItem<T> :
        ProductionRuleItem,
        ILiteralProductionRuleItem<T>
    {
        /// <summary>
        /// Data member for <see cref="Value"/>.
        /// </summary>
        private T value;
        private bool flag;
        private bool counter;
        /// <summary>
        /// Creates a new <see cref="LiteralProductionRuleItem{T}"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="value">The value defined by the <see cref="LiteralProductionRuleItem{T}"/>.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="LiteralProductionRuleItem{T}"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="LiteralProductionRuleItem{T}"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="LiteralProductionRuleItem{T}"/> was declared.</param>
        public LiteralProductionRuleItem(T value, int column, int line, long position, bool flag, bool counter)
            : base(column, line, position)
        {
            this.value = value;
            this.flag = flag;
            this.counter = counter;
        }

        #region ILiteralProductionRuleItem<T> Members

        /// <summary>
        /// Returns the value defined by the <see cref="LiteralProductionRuleItem{T}"/>.
        /// </summary>
        public T Value
        {
            get { return this.value; }
        }

        /// <summary>
        /// Creates a copy of the current <see cref="ILiteralProductionRuleItem{T}"/>.
        /// </summary>
        /// <returns>A new <see cref="LiteralProductionRuleItem{T}"/> with the data
        /// members of the current <see cref="LiteralProductionRuleItem{T}"/>.</returns>
        public new ILiteralProductionRuleItem<T> Clone()
        {
            return ((ILiteralProductionRuleItem<T>)(base.Clone()));
        }

        #endregion

        #region ILiteralProductionRuleItem Members

        object ILiteralProductionRuleItem.Value
        {
            get { return this.Value; }
        }

        ILiteralProductionRuleItem ILiteralProductionRuleItem.Clone()
        {
            return this.Clone();
        }

        #endregion

        #region ILiteralProductionRuleItem Members


        public bool Flag
        {
            get { return this.flag; }
        }

        #endregion

        #region ILiteralProductionRuleItem Members

        public bool Counter
        {
            get { return this.counter;; }
        }

        #endregion
    }
}
