using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public class PreprocessorCPrimary :
        PreprocessorCExp,
        IPreprocessorCPrimary
    {
        private int rule;
        private OilexerGrammarTokens.IdentifierToken identifier;
        private OilexerGrammarTokens.StringLiteralToken @string;
        private OilexerGrammarTokens.CharLiteralToken @char;
        private OilexerGrammarTokens.OperatorToken openingParenthesis;
        private IPreprocessorCLogicalOrConditionExp preCLogicalOrExp;
        private OilexerGrammarTokens.NumberLiteral number;

        /// <summary>
        /// Creates a new <see cref="PreprocessorCExp"/> with the <paramref name="string"/>,
        /// <paramref name="column"/>,  <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="string">The string represented by the <see cref="PreprocessorCExp"/>.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorCExp"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorCExp"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorCExp"/> 
        /// was declared at.</param>
        public PreprocessorCPrimary(OilexerGrammarTokens.StringLiteralToken @string, int column, int line, long position)
            : base(column, line, position)
        {
            this.rule = 1;
            this.@string = @string;
        }
        
        /// <summary>
        /// Creates a new <see cref="PreprocessorCExp"/> with the <paramref name="char"/>,
        /// <paramref name="column"/>,  <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="char">The character represented by the <see cref="PreprocessorCPrimary"/>.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorCExp"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorCExp"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorCExp"/> 
        /// was declared at.</param>
        public PreprocessorCPrimary(OilexerGrammarTokens.CharLiteralToken @char, int column, int line, long position)
            : base(column, line, position)
        {
            this.rule = 2;
            this.@char = @char;
        }

        /// <summary>
        /// Creates a new <see cref="PreprocessorCExp"/> with the <paramref name="preCLogicalOrExp"/>,
        /// <paramref name="column"/>,  <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="preCLogicalOrExp">The logical or expression that was in parameters that was encountered by the <see cref="PreprocessorCPrimary"/>.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorCExp"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorCExp"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorCExp"/> 
        /// was declared at.</param>
        public PreprocessorCPrimary(OilexerGrammarTokens.OperatorToken openingParenthesis, IPreprocessorCLogicalOrConditionExp preCLogicalOrExp, int column, int line, long position)
            : base(column, line, position)
        {
            this.rule = 3;
            this.openingParenthesis = openingParenthesis;
            this.preCLogicalOrExp = preCLogicalOrExp;
        }


        /// <summary>
        /// Creates a new <see cref="PreprocessorCExp"/> with the <paramref name="identifier"/>,
        /// <paramref name="column"/>,  <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="identifier">The identifier represented by the <see cref="PreprocessorCExp"/></param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorCExp"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorCExp"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorCExp"/> 
        /// was declared at.</param>
        public PreprocessorCPrimary(OilexerGrammarTokens.IdentifierToken identifier, int column, int line, long position)
            : base(column, line, position)
        {
            this.rule = 4;
            this.identifier = identifier;
        }
        /// <summary>
        /// Creates a new <see cref="PreprocessorCExp"/> with the <paramref name="number"/>,
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="number">The <see cref="System.Int32"/> represented by the <see cref="PreprocessorCExp"/></param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorCExp"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorCExp"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorCExp"/> 
        /// was declared at.</param>
        public PreprocessorCPrimary(OilexerGrammarTokens.NumberLiteral number, int column, int line, long position)
            : base(column, line, position)
        {
            this.rule = 5;
            this.number = number;
        }

        //#region IPreprocessorCPrimary Members

        public int Rule
        {
            get { return this.rule; }
        }

        public OilexerGrammarTokens.IdentifierToken Identifier
        {
            get { return this.identifier; }
        }

        public OilexerGrammarTokens.StringLiteralToken String
        {
            get { return this.@string; }
        }

        public OilexerGrammarTokens.CharLiteralToken Char
        {
            get { return this.@char;}
        }

        public IPreprocessorCLogicalOrConditionExp PreCLogicalOrExp
        {
            get { return preCLogicalOrExp; }
        }

        public OilexerGrammarTokens.NumberLiteral Number
        {
            get
            {
                return this.number;
            }
        }


        public IOilexerGrammarToken Token
        {
            get
            {
                switch (this.rule)
                {
                    case 1:
                        return this.@string;
                    case 2:
                        return this.@char;
                    case 3:
                        return this.openingParenthesis;
                    case 4:
                        return this.identifier;
                    case 5:
                        return this.number;
                }
                return null;
            }
        }

        //#endregion

        public override string ToString()
        {
            switch (rule)
            {
                case 1:
                    return this.String.Value;
                case 2:
                    return this.Char.Value;
                case 3:
                    return string.Format("({0})", this.preCLogicalOrExp.ToString());
                case 4:
                    return this.identifier.Name;
                case 5 :
                    return this.Number.Value;
            }
            return "<null>";
        }
    }
}
