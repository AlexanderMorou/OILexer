using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Provides a base implementation of <see cref="ILiteralCharProductionRuleItem"/> which
    /// is for working with a <see cref="System.Char"/> literal defined in an 
    /// <see cref="IOilexerGrammarProductionRuleEntry"/>.
    /// </summary>
    public class LiteralCharProductionRuleItem :
        LiteralProductionRuleItem<char>,
        ILiteralCharProductionRuleItem
    {
        /// <summary>
        /// Data member for <see cref="CaseInsensitive"/>.
        /// </summary>
        private bool caseInsensitive;
        /// <summary>
        /// Creates a new <see cref="LiteralCharProductionRuleItem"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.Char"/> value defined by the <see cref="LiteralCharProductionRuleItem"/>.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="LiteralCharProductionRuleItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="LiteralCharProductionRuleItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="LiteralCharProductionRuleItem"/> was declared.</param>
        public LiteralCharProductionRuleItem(char value, bool caseInsensitive, int column, int line, long position, bool flag, bool isCounter)
            : base(value, column, line, position, flag, isCounter)
        {
            this.caseInsensitive = caseInsensitive;
        }

        /// <summary>
        /// Type-inspecific 'onclone' to allowfor hiding of <see cref="Clone()"/>.
        /// </summary>
        /// <returns>A copy of the current <see cref="LiteralCharProductionRuleItem"/>.</returns>
        protected override object OnClone()
        {

            LiteralCharProductionRuleItem result = new LiteralCharProductionRuleItem(Value, this.CaseInsensitive, Column, Line, Position, this.Flag, this.Counter);
            this.CloneData(result);
            return result;
        }

        //#region ILiteralCharProductionRuleItem Members

        /// <summary>
        /// Creates a copy of the current <see cref="LiteralCharProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="LiteralCharProductionRuleItem"/> with the data
        /// members of the current <see cref="LiteralCharProductionRuleItem"/>.</returns>
        public new ILiteralCharProductionRuleItem Clone()
        {
            return ((ILiteralCharProductionRuleItem)(base.Clone()));
        }

        /// <summary>
        /// Returns whether the <see cref="LiteralCharProductionRuleItem"/>'s value is
        /// case-insensitive.
        /// </summary>
        public bool CaseInsensitive
        {
            get { return this.caseInsensitive; }
        }

        //#endregion

        public override string ToString()
        {
            return string.Format("{0}{1}", OilexerGrammarCore.EncodePrim(this.Value),base.ToString());
        }

        public IOilexerGrammarProductionRuleEntry Rule { get; internal set; }
    }
}
