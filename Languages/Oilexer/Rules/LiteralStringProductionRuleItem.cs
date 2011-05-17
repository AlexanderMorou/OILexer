using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public class LiteralStringProductionRuleItem :
        LiteralProductionRuleItem<string>,
        ILiteralStringProductionRuleItem
    {
        private bool caseInsensitive;
        /// <summary>
        /// Creates a new <see cref="LiteralStringProductionRuleItem"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="LiteralStringProductionRuleItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="LiteralStringProductionRuleItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="LiteralStringProductionRuleItem"/> was declared.</param>
        public LiteralStringProductionRuleItem(string value, bool caseInsensitive, int column, int line, long position, bool flag, bool counter)
            : base(value, column, line, position, flag, counter)
        {
            this.caseInsensitive = caseInsensitive;
        }

        protected override object OnClone()
        {
            LiteralStringProductionRuleItem result = new LiteralStringProductionRuleItem(Value, this.CaseInsensitive, Column, Line, Position, this.Flag, this.Counter);

            base.CloneData(result);
            return result;
        }

        #region ILiteralStringProductionRuleItem Members

        /// <summary>
        /// Creates a copy of the current <see cref="LiteralStringProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="LiteralStringProductionRuleItem"/> with the data
        /// members of the current <see cref="LiteralStringProductionRuleItem"/>.</returns>
        public new ILiteralStringProductionRuleItem Clone()
        {
            return ((ILiteralStringProductionRuleItem)(base.Clone()));
        }

        /// <summary>
        /// Returns whether the <see cref="LiteralStringProductionRuleItem"/>'s value is
        /// case-insensitive.
        /// </summary>
        public bool CaseInsensitive
        {
            get { return this.caseInsensitive; }
        }

        #endregion

        public override string ToString()
        {
            return GrammarCore.EncodePrim(this.Value);
        }

    }
}
