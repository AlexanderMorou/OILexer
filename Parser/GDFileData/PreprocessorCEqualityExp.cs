using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public class PreprocessorCEqualityExp :
        PreprocessorCExp,
        IPreprocessorCEqualityExp
    {
        private int rule = -1;
        private IPreprocessorCEqualityExp preCEqualityExp;
        private IPreprocessorCPrimary preCPrimary;
        /// <summary>
        /// Creates a new <see cref="PreprocessorCEqualityExp"/> with the <paramref name="preCEqualityExp"/>,
        /// <paramref name="preCPrimary"/>, <paramref name="termIsEq"/>, <paramref name="column"/>, 
        /// <paramref name="line"/>, and <paramref name="position"/> provided.
        /// </summary>
        /// <param name="termIsEq">Whether the term encountered is '==' (true) or '!=' (false).</param>
        /// <param name="preCEqualityExp">The previous <see cref="IPreprocessorCEqualityExp"/>.</param>
        /// <param name="preCPrimary">The <see cref="IPreprocessorCPrimary"/> 
        /// that is next.</param>
        /// <remarks>Rule 1 or 2</remarks>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorCEqualityExp"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorCEqualityExp"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorCEqualityExp"/> 
        /// was declared at.</param>
        public PreprocessorCEqualityExp(IPreprocessorCEqualityExp preCEqualityExp, bool termIsEq, IPreprocessorCPrimary preCPrimary, int column, int line, long position)
            : base(column, line, position)
        {
            rule = (termIsEq ? 1 : 2);
            this.preCPrimary = preCPrimary;
            this.preCEqualityExp = preCEqualityExp;
        }

        /// <summary>
        /// Creates a new <see cref="PreprocessorCEqualityExp"/> with the
        /// <paramref name="preCPrimary"/>, <paramref name="column"/>, 
        /// <paramref name="line"/>, and <paramref name="position"/> provided.
        /// </summary>
        /// <param name="preCPrimary">The <see cref="IPreprocessorCPrimary"/> 
        /// that is next.</param>
        /// <remarks>Rule 3</remarks>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorCEqualityExp"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorCEqualityExp"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorCEqualityExp"/> 
        /// was declared at.</param>
        public PreprocessorCEqualityExp(IPreprocessorCPrimary preCPrimary, int column, int line, long position)
            : base(column, line, position)
        {
            this.preCPrimary = preCPrimary;
            this.rule = 3;
        }

        #region IPreprocessorCEqualityExp Members

        public int Rule
        {
            get { return rule; }
        }

        public IPreprocessorCEqualityExp PreCEqualityExp
        {
            get { return this.preCEqualityExp; }
        }

        public IPreprocessorCPrimary PreCPrimary
        {
            get { return this.preCPrimary; }
        }

        #endregion

        public override string ToString()
        {
            switch (rule)
            {
                case 1:
                    return string.Format("{0} == {1}", this.preCEqualityExp.ToString(), this.preCPrimary.ToString());
                case 2:
                    return string.Format("{0} != {1}", this.preCEqualityExp.ToString(), this.preCPrimary.ToString());
                case 3:
                    return preCPrimary.ToString();
            }
            return null;
        }
    }
}
