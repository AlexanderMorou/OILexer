using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Provides a base implementation of <see cref="ITokenReferenceProductionRuleItem"/>
    /// which is a <see cref="IProductionRuleItem"/> that references an <see cref="ITokenEntry"/>.
    /// </summary>
    public class TokenReferenceProductionRuleItem :
        ProductionRuleItem,
        ITokenReferenceProductionRuleItem
    {
        /// <summary>
        /// Creates a new <see cref="TokenReferenceProductionRuleItem"/> with the <paramref name="reference"/>,
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/> provided.
        /// </summary>
        /// <param name="reference">The <see cref="ITokenEntry"/> the <see cref="TokenReferenceProductionRuleItem"/> 
        /// references.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="TokenReferenceProductionRuleItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="TokenReferenceProductionRuleItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="TokenReferenceProductionRuleItem"/> was declared.</param>
        public TokenReferenceProductionRuleItem(ITokenEntry reference, int column, int line, long position)
            : base(column, line, position)
        {
            this.reference = reference;
        }

        /// <summary>
        /// Data member for <see cref="Reference"/>.
        /// </summary>
        private ITokenEntry reference;

        #region ITokenReferenceProductionRuleItem Members

        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> which the <see cref="TokenReferenceProductionRuleItem"/> 
        /// relates to.
        /// </summary>
        public ITokenEntry Reference
        {
            get { return this.reference; }
            internal set { this.reference = value; }
        }

        #endregion

        protected override sealed object OnClone()
        {
            TokenReferenceProductionRuleItem result = new TokenReferenceProductionRuleItem(this.Reference, this.Column, this.Line, this.Position);
            this.CloneData(result);
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", this.Reference.Name, base.ToString());
        }
    }
}
