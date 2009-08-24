using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public class PreprocessorCLogicalOrConditionExp :
        PreprocessorCExp,
        IPreprocessorCLogicalOrConditionExp
    {
        /// <summary>
        /// Data member for <see cref="Left"/>
        /// </summary>
        private IPreprocessorCLogicalOrConditionExp left;
        /// <summary>
        /// Data member for <see cref="Right"/>
        /// </summary>
        private IPreprocessorCLogicalAndConditionExp right;

        /// <summary>
        /// Creates a new <see cref="PreprocessorCLogicalOrConditionExp"/> with the <paramref name="left"/>
        /// <paramref name="right"/>, <paramref name="column"/>, 
        /// <paramref name="line"/>, and <paramref name="position"/> provided.
        /// </summary>
        /// <param name="left">The previous <see cref="IPreprocessorCLogicalOrConditionExp"/>.</param>
        /// <param name="right">The <see cref="IPreprocessorCLogicalAndConditionExp"/> 
        /// that is next.</param>
        /// <remarks>Rule 1</remarks>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorCLogicalOrConditionExp"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorCLogicalOrConditionExp"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorCLogicalOrConditionExp"/> 
        /// was declared at.</param>
        public PreprocessorCLogicalOrConditionExp(IPreprocessorCLogicalOrConditionExp left, IPreprocessorCLogicalAndConditionExp right, int column, int line, long position)
            : base(column, line, position)
        {
            this.left = left;
            this.right = right;
        }
        
        /// <summary>
        /// Creates a new <see cref="PreprocessorCLogicalOrConditionExp"/> with the <paramref name="right"/>, <paramref name="column"/>, 
        /// <paramref name="line"/>, and <paramref name="position"/> 
        /// provided.
        /// </summary>
        /// <param name="right">The <see cref="IPreprocessorCLogicalAndConditionExp"/> the 
        /// <see cref="PreprocessorCLogicalOrConditionExp"/> links to.</param>
        /// <remarks>Rule 2</remarks>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorCLogicalOrConditionExp"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorCLogicalOrConditionExp"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorCLogicalOrConditionExp"/> 
        /// was declared at.</param>
        public PreprocessorCLogicalOrConditionExp(IPreprocessorCLogicalAndConditionExp right, int column, int line, long position)
            : base(column, line, position)
        {
            this.right = right;
        }

        #region IPreprocessorCLogicalOrConditionExp Members

        public IPreprocessorCLogicalOrConditionExp Left
        {
            get { return this.left; }
        }

        public IPreprocessorCLogicalAndConditionExp Right
        {
            get { return this.right; }
        }

        #endregion
        public override string ToString()
        {
            if (left == null)
                return this.right.ToString();
            else
                return string.Format("{0} || {1}",this.left, this.right);
        }
    }
}
