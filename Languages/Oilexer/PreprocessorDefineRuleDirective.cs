using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    /// <summary>
    /// Provides a base implementation of <see cref="IPreprocessorDefineRuleDirective"/>
    /// which is a conditional return directive that yields the result of the 
    /// conditional for the current iteration or run-through.
    /// </summary>
    public class PreprocessorDefineRuleDirective :
        PreprocessorDirectiveBase,
        IPreprocessorDefineRuleDirective
    {
        private string declareTarget;
        private IProductionRule[] definedRules;

        /// <summary>
        /// Creates a new <see cref="PreprocessorDefineRuleDirective"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="fileName">The file in which the <see cref="PreprocessorDefineRuleDirective"/> was declared
        /// in.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorDefineRuleDirective"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorDefineRuleDirective"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorDefineRuleDirective"/> 
        /// was declared at.</param>
        public PreprocessorDefineRuleDirective(string declareTarget, IProductionRule[] definedRules, int column, int line, long position)
            : base(column, line, position)
        {
            this.declareTarget = declareTarget;
            this.definedRules = definedRules;
        }

        public override EntryPreprocessorType Type
        {
            get { return EntryPreprocessorType.DefineRule; }
        }

        //#region IPreprocessorDefineRuleDirective Members

        /// <summary>
        /// Returns the target of the <see cref="IPreprocessorDefineRuleDirective"/>.
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

        //#endregion
    }
}
