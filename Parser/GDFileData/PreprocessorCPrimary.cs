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
        private string @string;
        private char? @char;
        private IPreprocessorCLogicalOrConditionExp preCLogicalOrExp;
        private int? number;

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
        public PreprocessorCPrimary(string @string, int column, int line, long position)
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
        public PreprocessorCPrimary(char @char, int column, int line, long position)
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
        public PreprocessorCPrimary(IPreprocessorCLogicalOrConditionExp preCLogicalOrExp, int column, int line, long position)
            : base(column, line, position)
        {
            this.rule = 3;
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
        public PreprocessorCPrimary(int number, int column, int line, long position)
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

        public string String
        {
            get { return this.@string; }
        }

        public char? Char
        {
            get { return this.@char;}
        }

        public IPreprocessorCLogicalOrConditionExp PreCLogicalOrExp
        {
            get { return preCLogicalOrExp; }
        }

        public int? Number
        {
            get
            {
                return this.number;
            }
        }

        #endregion

        public override string ToString()
        {
            switch (rule)
            {
                case 1:
                    return this.String.Encode();
                case 2:
                    if (this.Char.HasValue)
                        return string.Format("'{0}'", this.Char.Value.ToString());
                    break;
                case 3:
                    return string.Format("({0})", this.preCLogicalOrExp.ToString());
                case 4:
                    return this.identifier.Name;
                case 5 :
                    if (this.Number.HasValue)
                        return this.Number.ToString();
                    break;
            }
            return "<null>";
        }
    }
}
