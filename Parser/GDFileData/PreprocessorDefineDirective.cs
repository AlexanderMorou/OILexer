using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// Provides a base implementation of <see cref="IPreprocessorDefineDirective"/>
    /// which is a conditional return directive that yields the result of the 
    /// conditional for the current iteration or run-through.
    /// </summary>
    public class PreprocessorDefineDirective :
        PreprocessorDirectiveBase,
        IPreprocessorDefineDirective
    {
        private string declareTarget;
        private IProductionRule[] definedRules;

        /// <summary>
        /// Creates a new <see cref="PreprocessorDefineDirective"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="fileName">The file in which the <see cref="PreprocessorDefineDirective"/> was declared
        /// in.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorDefineDirective"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorDefineDirective"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorDefineDirective"/> 
        /// was declared at.</param>
        public PreprocessorDefineDirective(string declareTarget, IProductionRule[] definedRules, int column, int line, long position)
            : base(column, line, position)
        {
            this.declareTarget = declareTarget;
            this.definedRules = definedRules;
        }

        public override EntryPreprocessorType Type
        {
            get { return EntryPreprocessorType.Define; }
        }

        #region IPreprocessorDefineDirective Members

        /// <summary>
        /// Returns the target of the <see cref="IPreprocessorDefineDirective"/>.
        /// </summary>
        public string DeclareTarget
        {
            get { return this.declareTarget; }
        }

        /// <summary>
        /// Returns the array of <see cref="IProductionRule"/> which result.
        /// </summary>
        public IProductionRule[] DefinedRules
        {
            get { return this.definedRules; }
        }

        #endregion
    }
}
