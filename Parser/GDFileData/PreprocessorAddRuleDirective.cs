using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;

namespace Oilexer.Parser.GDFileData
{
    public class PreprocessorAddRuleDirective :
        PreprocessorDirectiveBase,
        IPreprocessorAddRuleDirective
    {
        private IProductionRule[] rules;
        private string insertTarget;
        /// <summary>
        /// Creates a new <see cref="PreprocessorAddRuleDirective"/> with the <paramref name="rules"/>,
        /// <paramref name="insertTarget"/>, <paramref name="column"/>, <paramref name="line"/>, and 
        /// <paramref name="position"/>.
        /// </summary>
        /// <param name="insertTarget">The target that is to receive the <paramref name="rules"/>.</param>
        /// <param name="rules">The <see cref="IProductionRule"/> array of rules to add to
        /// <paramref name="insertTarget"/>.</param>
        /// <param name="fileName">The file in which the <see cref="PreprocessorAddRuleDirective"/> was declared
        /// in.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorAddRuleDirective"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorAddRuleDirective"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorAddRuleDirective"/> 
        /// was declared at.</param>
        public PreprocessorAddRuleDirective(string insertTarget, IProductionRule[] rules, int column, int line, long position)
            : base(column, line, position)
        {
            this.insertTarget = insertTarget;
            this.rules = rules;
        }
        public override EntryPreprocessorType Type
        {
            get { return EntryPreprocessorType.AddRule; }
        }

        #region IPreprocessorAddRuleDirective Members

        public string InsertTarget
        {
            get { return this.insertTarget; }
        }

        /// <summary>
        /// Returns the rules added by the <see cref="IPreprocessorAddRuleDirective"/>.
        /// </summary>
        public IProductionRule[] Rules
        {
            get { return this.rules; }
        }

        #endregion
    }
}
