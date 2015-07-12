using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public class BaseEncodeGraphCommand :
        CommandTokenItem,
        IBaseEncodeGraphCommand
    {
        private OilexerGrammarTokens.NumberLiteral numericBase;
        private OilexerGrammarTokens.StringLiteralToken stringBase;
        private ITokenExpressionSeries encodeTarget;
        private OilexerGrammarTokens.NumberLiteral digits;

        public BaseEncodeGraphCommand(ITokenExpressionSeries searchTarget, OilexerGrammarTokens.NumberLiteral numericBase, OilexerGrammarTokens.NumberLiteral digits, int column, int line, long position)
            : base(new ITokenExpressionSeries[1] { searchTarget }, column, line, position)
        {
            this.encodeTarget = searchTarget;
            this.digits = digits;
            this.numericBase = numericBase;
        }

        public BaseEncodeGraphCommand(ITokenExpressionSeries searchTarget, OilexerGrammarTokens.StringLiteralToken stringBase, OilexerGrammarTokens.NumberLiteral digits, int column, int line, long position)
            : base(new ITokenExpressionSeries[1] { searchTarget }, column, line, position)
        {
            this.encodeTarget = searchTarget;
            this.digits = digits;
            this.stringBase = stringBase;
        }

        /// <summary>
        /// Returns the <see cref="CommandType"/> associated to the command.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="CommandType.ScanCommand"/>.
        /// </remarks>
        public override CommandType Type
        {
            get { return CommandType.EncodeUnicode; }
        }

        #region IScanCommandTokenItem Members

        public ITokenExpressionSeries EncodeTarget
        {
            get { return this.encodeTarget; }
        }

        #endregion

        public override string ToString()
        {
            if (this.stringBase == null)
            {
                return string.Format("UnicodeGraph({0}, {1}, {2})", this.EncodeTarget, NumericBase, Digits);
            }
            else
            {
                return string.Format("UnicodeGraph({0}, {1}, {2})", this.EncodeTarget, StringBase, Digits);
            }
        }

        protected override object OnClone()
        {
            if (this.stringBase == null)
            {
                BaseEncodeGraphCommand ugc = new BaseEncodeGraphCommand(this.encodeTarget, this.NumericBase, Digits, Column, Line, Position);
                base.CloneData(ugc);
                return ugc;
            }
            else
            {
                BaseEncodeGraphCommand ugc = new BaseEncodeGraphCommand(this.encodeTarget, this.StringBase, Digits, Column, Line, Position);
                base.CloneData(ugc);
                return ugc;
            }
        }

        public OilexerGrammarTokens.NumberLiteral Digits { get { return this.digits; } }

        public OilexerGrammarTokens.NumberLiteral NumericBase { get { return this.numericBase; } }
        public OilexerGrammarTokens.StringLiteralToken StringBase { get { return this.stringBase; } }
    }
}
