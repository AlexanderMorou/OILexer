using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// Provides a base implementation of <see cref="IPreprocessorConditionalReturnDirective"/>
    /// which is a conditional return directive that yields the result of the 
    /// conditional for the current iteration or run-through.
    /// </summary>
    public class PreprocessorConditionalReturnDirective :
        PreprocessorDirectiveBase,
        IPreprocessorConditionalReturnDirective
    {
        IProductionRule[] result;
        /// <summary>
        /// Creates a new <see cref="PreprocessorConditionalReturnDirective"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="fileName">The file in which the <see cref="PreprocessorConditionalReturnDirective"/> was declared
        /// in.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorConditionalReturnDirective"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorConditionalReturnDirective"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorConditionalReturnDirective"/> 
        /// was declared at.</param>
        public PreprocessorConditionalReturnDirective(IProductionRule[] result, int column, int line, long position) 
            : base(column, line, position)
        {
            this.result = result;
        }

        public override EntryPreprocessorType Type
        {
            get { return EntryPreprocessorType.Return; }
        }

        #region IPreprocessorConditionalReturnDirective Members

        /// <summary>
        /// Returns the array of <see cref="IProductionRule"/> which result.
        /// </summary>
        public IProductionRule[] Result
        {
            get { return this.result; }
        }

        #endregion
    }
}
