using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public class PreprocessorCLogicalAndConditionExp :
        PreprocessorCExp,
        IPreprocessorCLogicalAndConditionExp
    {
        private IPreprocessorCLogicalAndConditionExp left;
        private IPreprocessorCEqualityExp right;

        /// <summary>
        /// Creates a new <see cref="PreprocessorCLogicalAndConditionExp"/> with the <paramref name="left"/>,
        /// <paramref name="right"/>, <paramref name="column"/>, 
        /// <paramref name="line"/>, and <paramref name="position"/> provided.
        /// </summary>
        /// <param name="left">The previous <see cref="IPreprocessorCLogicalAndConditionExp"/>.</param>
        /// <param name="right">The <see cref="IPreprocessorCEqualityExp"/> 
        /// that is next.</param>
        /// <remarks>Rule 1</remarks>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorCLogicalOrConditionExp"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorCLogicalOrConditionExp"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorCLogicalOrConditionExp"/> 
        /// was declared at.</param>
        public PreprocessorCLogicalAndConditionExp(IPreprocessorCLogicalAndConditionExp left, IPreprocessorCEqualityExp right, int column, int line, long position)
            : base(column, line, position)
        {
            this.left = left;
            this.right = right;
        }
        
        /// <summary>
        /// Creates a new <see cref="PreprocessorCLogicalAndConditionExp"/> with the <paramref name="right"/>, 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>
        /// provided.
        /// </summary>
        /// <param name="right">The <see cref="IPreprocessorCEqualityExp"/> the 
        /// <see cref="PreprocessorCLogicalAndConditionExp"/> links to.</param>
        /// <remarks>Rule 2</remarks>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorCLogicalOrConditionExp"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorCLogicalOrConditionExp"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorCLogicalOrConditionExp"/> 
        /// was declared at.</param>
        public PreprocessorCLogicalAndConditionExp(IPreprocessorCEqualityExp right, int column, int line, long position)
            : base(column, line, position)
        {
            this.right = right;
        }

        #region IPreprocessorCLogicalAndConditionExp Members

        public IPreprocessorCLogicalAndConditionExp Left
        {
            get { return this.left; }
        }

        public IPreprocessorCEqualityExp Right
        {
            get { return this.right; }
        }

        #endregion

        public override string ToString()
        {
            if (left == null)
                return this.right.ToString();
            else
                return string.Format("{0} && {1}", this.left, this.right);
        }
    }
}
