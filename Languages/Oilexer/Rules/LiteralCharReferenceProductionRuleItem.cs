using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{

    /// <summary>
    /// Provides a base implementation of <see cref="ILiteralCharReferenceProductionRuleItem"/>
    /// which is a reference to a literal character from a <see cref="IOilexerGrammarTokenEntry"/>.
    /// </summary>
    public class LiteralCharReferenceProductionRuleItem :
        LiteralReferenceProductionRuleItem<char, ILiteralCharTokenItem>,
        ILiteralCharReferenceProductionRuleItem
    {

        /// <summary>
        /// Creates a new <see cref="LiteralCharReferenceProductionRuleItem"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="literal">The <see cref="ILiteralCharTokenItem"/> which the <see cref="LiteralCharReferenceProductionRuleItem"/>
        /// references.</param>
        /// <param name="source">The <see cref="IOilexerGrammarTokenEntry"/> which the contains the <paramref name="literal"/> the <see cref="LiteralCharReferenceProductionRuleItem"/>
        /// references.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="LiteralCharReferenceProductionRuleItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="LiteralCharReferenceProductionRuleItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="LiteralCharReferenceProductionRuleItem"/> was declared.</param>
        public LiteralCharReferenceProductionRuleItem(ILiteralCharTokenItem literal, IOilexerGrammarTokenEntry source, int column, int line, long position)
            : base(literal, source, column, line, position)
        {
        }
        public LiteralCharReferenceProductionRuleItem(ILiteralCharTokenItem literal, IOilexerGrammarTokenEntry source, int column, int line, long position, bool wasFlag, bool wasCounter)
            : base(literal, source, column, line, position, wasFlag, wasCounter)
        {
        }

        /// <summary>
        /// Type-inspecific 'onclone' to allowfor hiding of <see cref="Clone()"/>.
        /// </summary>
        /// <returns>A new <see cref="LiteralCharReferenceProductionRuleItem"/> instance which is a
        /// copy of the current <see cref="LiteralCharReferenceProductionRuleItem"/>.</returns>
        protected override sealed object OnClone()
        {
            LiteralCharReferenceProductionRuleItem result = new LiteralCharReferenceProductionRuleItem(this.Literal, this.Source, this.Column, this.Line, this.Position);
            this.CloneData(result);
            return result;
        }

        //#region ILiteralCharReferenceProductionRuleItem Members

        public new ILiteralCharReferenceProductionRuleItem Clone()
        {
            return ((ILiteralCharReferenceProductionRuleItem)(base.Clone()));
        }

        //#endregion

        public override string ToString()
        {
            if (this.Name != null)
                return string.Format("{0}{3}{4}:{1};{2}{5}", OilexerGrammarCore.EncodePrim(this.Literal.Value), this.Name, this.ToStringFurtherOptions(), this.Counter ? "#" : string.Empty, this.IsFlag ? "!" : string.Empty, this.RepeatOptions);
            else
                return string.Format("{0}{2}{3}{1}{4}", OilexerGrammarCore.EncodePrim(this.Literal.Value), this.ToStringFurtherOptions(), this.Counter ? "#" : string.Empty, this.IsFlag ? "!" : string.Empty, this.RepeatOptions);
        }

        public IOilexerGrammarProductionRuleEntry Rule { get; internal set; }
    }
}
