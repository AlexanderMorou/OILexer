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

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Provides a base implementation of <see cref="IRuleReferenceProductionRuleItem"/> which
    /// is a <see cref="IProductionRuleItem"/> that references an <see cref="IOilexerGrammarProductionRuleEntry"/>.
    /// </summary>
    public class RuleReferenceProductionRuleItem :
        ProductionRuleItem,
        IRuleReferenceProductionRuleItem
    {
        /// <summary>
        /// Data member for <see cref="Reference"/>.
        /// </summary>
        private IOilexerGrammarProductionRuleEntry reference;
        /// <summary>
        /// Creates a new <see cref="RuleReferenceProductionRuleItem"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="reference">The <see cref="IOilexerGrammarProductionRuleEntry"/> which the 
        /// <see cref="RuleReferenceProductionRuleItem"/> references.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="RuleReferenceProductionRuleItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="RuleReferenceProductionRuleItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="RuleReferenceProductionRuleItem"/> was declared.</param>
        public RuleReferenceProductionRuleItem(IOilexerGrammarProductionRuleEntry reference, int column, int line, long position)
            : base(column, line, position)
        {
            this.reference = reference;
        }

        protected override object OnClone()
        {
            RuleReferenceProductionRuleItem result = new RuleReferenceProductionRuleItem(this.Reference, this.Column, this.Line, this.Position);
            this.CloneData(result);
            return result;
        }

        //#region IRuleReferenceProductionRuleItem Members

        /// <summary>
        /// Returns the <see cref="IOilexerGrammarProductionRuleEntry"/> which the 
        /// <see cref="RuleReferenceProductionRuleItem"/> references.
        /// </summary>
        public IOilexerGrammarProductionRuleEntry Reference
        {
            get { return this.reference; }
        }

        /// <summary>
        /// Creates a copy of the current <see cref="RuleReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="IRuleReferenceProductionRuleItem"/> instance with the data
        /// members of the current <see cref="RuleReferenceProductionRuleItem"/>.</returns>
        public new IRuleReferenceProductionRuleItem Clone()
        {
            return ((IRuleReferenceProductionRuleItem)(base.Clone()));
        }

        //#endregion

        public override string ToString()
        {
            return string.Format("{0}{1}", this.Reference.Name, base.ToString());
        }

        public IOilexerGrammarProductionRuleEntry Rule { get; internal set; }
    }
}
