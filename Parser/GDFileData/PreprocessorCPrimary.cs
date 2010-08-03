using System;
using System.Collections.Generic;
using System.Text;
using Oilexer._Internal;

namespace Oilexer.Parser.GDFileData
{
    public class PreprocessorCPrimary :
        PreprocessorCExp,
        IPreprocessorCPrimary
    {
        private int rule;
        private GDTokens.IdentifierToken identifier;
        private GDTokens.StringLiteralToken @string;
        private GDTokens.CharLiteralToken @char;
        private GDTokens.OperatorToken openingParenthesis;
        private IPreprocessorCLogicalOrConditionExp preCLogicalOrExp;
        private GDTokens.NumberLiteral number;

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
        public PreprocessorCPrimary(GDTokens.StringLiteralToken @string, int column, int line, long position)
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
        public PreprocessorCPrimary(GDTokens.CharLiteralToken @char, int column, int line, long position)
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
        public PreprocessorCPrimary(GDTokens.OperatorToken openingParenthesis, IPreprocessorCLogicalOrConditionExp preCLogicalOrExp, int column, int line, long position)
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
        public PreprocessorCPrimary(GDTokens.IdentifierToken identifier, int column, int line, long position)
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
        public PreprocessorCPrimary(GDTokens.NumberLiteral number, int column, int line, long position)
            : base(column, line, position)
        {
            this.rule = 5;
            this.number = number;
        }

        #region IPreprocessorCPrimary Members

        public int Rule
        {
            get { return this.rule; }
        }

        public GDTokens.IdentifierToken Identifier
        {
            get { return this.identifier; }
        }

        public GDTokens.StringLiteralToken String
        {
            get { return this.@string; }
        }

        public GDTokens.CharLiteralToken Char
        {
            get { return this.@char;}
        }

        public IPreprocessorCLogicalOrConditionExp PreCLogicalOrExp
        {
            get { return preCLogicalOrExp; }
        }

        public GDTokens.NumberLiteral Number
        {
            get
            {
                return this.number;
            }
        }


        public IGDToken Token
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

        #endregion

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
