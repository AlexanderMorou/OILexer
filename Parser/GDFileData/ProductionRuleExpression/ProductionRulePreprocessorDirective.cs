using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.ProductionRuleExpression
{
    public class ProductionRulePreprocessorDirective :
        ProductionRuleItem,
        IProductionRulePreprocessorDirective
    {
        private IPreprocessorDirective directive;
        /// <summary>
        /// Creates a new <see cref="ProductionRulePreprocessorDirective"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="ProductionRulePreprocessorDirective"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="ProductionRulePreprocessorDirective"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="ProductionRulePreprocessorDirective"/> was declared.</param>
        public ProductionRulePreprocessorDirective(IPreprocessorDirective directive, int column, int line, long position)
            : base(column, line, position)
        {
            this.directive = directive;
        }

        #region IProductionRulePreprocessorDirective Members

        /// <summary>
        /// Returns the <see cref="IPreprocessorDirective"/> which was parsed.
        /// </summary>
        public IPreprocessorDirective Directive
        {
            get { return this.directive; }
        }

        #endregion

        protected override object OnClone()
        {
            throw new NotImplementedException();
        }

        #region IProductionRulePreprocessorDirective Members


        public new IProductionRulePreprocessorDirective Clone()
        {
            return new ProductionRulePreprocessorDirective(this.directive, Column, Line, Position);
        }

        #endregion
    }
}
