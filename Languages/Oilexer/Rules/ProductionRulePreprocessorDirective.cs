using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
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

        protected override object OnClone()
        {
            throw new NotImplementedException();
        }

        //#region IProductionRulePreprocessorDirective Members

        /// <summary>
        /// Returns the <see cref="IPreprocessorDirective"/> which was parsed.
        /// </summary>
        public IPreprocessorDirective Directive
        {
            get { return this.directive; }
        }

        public new IProductionRulePreprocessorDirective Clone()
        {
            return new ProductionRulePreprocessorDirective(this.directive, Column, Line, Position);
        }

        public IOilexerGrammarProductionRuleEntry Rule { get; internal set; }
        //#endregion
    }
}
