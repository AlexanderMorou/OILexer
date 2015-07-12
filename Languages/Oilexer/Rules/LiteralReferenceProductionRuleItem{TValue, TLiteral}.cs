using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    public abstract class LiteralReferenceProductionRuleItem<TValue, TLiteral> :
        ProductionRuleItem,
        ILiteralReferenceProductionRuleItem<TValue, TLiteral>
        where TLiteral :
            ILiteralTokenItem<TValue>
    {
        /// <summary>
        /// Data member for <see cref="TLiteral"/>.
        /// </summary>
        private TLiteral literal;
        /// <summary>
        /// Data member for <see cref="Source"/>.
        /// </summary>
        private IOilexerGrammarTokenEntry source;
        private bool isFlag;
        private bool counter;
        /// <summary>
        /// Creates a new <see cref="LiteralReferenceProductionRuleItem{TValue, TLiteral}"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="literal">The <typeparamref name="TLiteral"/> which the <see cref="LiteralReferenceProductionRuleItem{TValue, TLiteral}"/>
        /// references.</param>
        /// <param name="source">The <see cref="IOilexerGrammarTokenEntry"/> which the contains the <paramref name="literal"/> the <see cref="LiteralReferenceProductionRuleItem{TValue, TLiteral}"/>
        /// references.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="LiteralReferenceProductionRuleItem{TValue, TLiteral}"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="LiteralReferenceProductionRuleItem{TValue, TLiteral}"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="LiteralReferenceProductionRuleItem{TValue, TLiteral}"/> was declared.</param>
        public LiteralReferenceProductionRuleItem(TLiteral literal, IOilexerGrammarTokenEntry source, int column, int line, long position)
            : base(column, line, position)
        {
            this.literal = literal;
            this.source = source;
        }
        public LiteralReferenceProductionRuleItem(TLiteral literal, IOilexerGrammarTokenEntry source, int column, int line, long position, bool wasFlag, bool wasCounter)
            : base(column, line, position)
        {
            this.literal = literal;
            this.source = source;
            this.isFlag = wasFlag;
            this.counter = wasCounter;
        }

        //#region ILiteralReferenceProductionRuleItem<TValue,TLiteral> Members

        /// <summary>
        /// Returns the literal the <see cref="LiteralReferenceProductionRuleItem{TValue, TLiteral}"/> references.
        /// </summary>
        public TLiteral Literal
        {
            get { return this.literal; }
            internal set { this.literal = value; }
        }

        /// <summary>
        /// Creates a copy of the current <see cref="LiteralReferenceProductionRuleItem{TValue, TLiteral}"/>.
        /// </summary>
        /// <returns>A new <see cref="LiteralReferenceProductionRuleItem{TValue, TLiteral}"/> with the data
        /// members of the current <see cref="LiteralReferenceProductionRuleItem{TValue, TLiteral}"/>.</returns>
        public new ILiteralReferenceProductionRuleItem<TValue, TLiteral> Clone()
        {
            return ((ILiteralReferenceProductionRuleItem<TValue, TLiteral>)(base.Clone()));
        }

        //#endregion

        //#region ILiteralReferenceProductionRuleItem Members

        /// <summary>
        /// Returns the source of the literal that the <see cref="LiteralReferenceProductionRuleItem{TValue, TLiteral}"/>
        /// relates to.
        /// </summary>
        public IOilexerGrammarTokenEntry Source
        {
            get { return this.source; }
            internal set { this.source = value; }
        }

        ILiteralTokenItem ILiteralReferenceProductionRuleItem.Literal
        {
            get { return this.Literal; }
        }

        ILiteralReferenceProductionRuleItem ILiteralReferenceProductionRuleItem.Clone()
        {
            return this.Clone();
        }

        //#endregion


        public override string ToString()
        {
            return string.Format("{0}.{1}{2}", this.Source.Name, this.Literal.Name, base.ToString());
        }

        protected override string ToStringFurtherOptions()
        {
            return this.RepeatOptions.ToString();
        }

        protected internal override void CloneData(IScannableEntryItem target)
        {
            LiteralReferenceProductionRuleItem<TValue, TLiteral> tTarget = target as LiteralReferenceProductionRuleItem<TValue, TLiteral>;
            if (tTarget != null)
            {
                tTarget.counter = this.counter;
                tTarget.isFlag = this.isFlag;
            }
            base.CloneData(target);
        }

        public IOilexerGrammarProductionRuleEntry Rule { get; internal set; }

        #region ILiteralReferenceProductionRuleItem Members

        public bool IsFlag
        {
            get { return this.isFlag; }
        }

        public bool Counter
        {
            get
            {
                return this.counter;
            }
        }
        #endregion
    }
}
