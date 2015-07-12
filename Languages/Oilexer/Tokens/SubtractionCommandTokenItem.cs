using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    class SubtractionCommandTokenItem :
        CommandTokenItem,
        ISubtractionCommandTokenItem
    {
        private ITokenExpressionSeries left;
        private ITokenExpressionSeries right;

        public SubtractionCommandTokenItem(ITokenExpressionSeries left, ITokenExpressionSeries right, int column, int line, long position)
            : base(new ITokenExpressionSeries[2] { left, right }, column, line, position)
        {
            this.left = left;
            this.right = right;
        }
        /// <summary>
        /// Returns the <see cref="CommandType"/> associated to the command.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="CommandType.SubtractCommand"/>.
        /// </remarks>
        public override CommandType Type
        {
            get { return CommandType.SubtractCommand; }
        }

        protected override object OnClone()
        {
            var result = new SubtractionCommandTokenItem(left, right, Column, Line, Position);
            base.CloneData(result);
            return result;
        }

        #region ISubtractionCommandTokenItem Members

        public ITokenExpressionSeries Left
        {
            get { return this.left; }
        }

        public ITokenExpressionSeries Right
        {
            get { return this.right; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Subtract({0}, {1}){2}", left, right, base.ToString());
        }
    }
}
