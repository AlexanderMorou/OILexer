using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="IOilexerGrammarEntry"/> production rule.
    /// Used to express a part of syntax for a <see cref="IOilexerGrammarFile"/>.
    /// </summary>
    public interface IOilexerGrammarProductionRuleEntry :
        IProductionRuleSeries,
        IOilexerGrammarScannableEntry,
        INamedProductionRuleSource
    {
        new string Name { get; }
        /// <summary>
        /// Returns/sets whether the elements of 
        /// the <see cref="IOilexerGrammarProductionRuleEntry"/>
        /// should inherit from the current
        /// <see cref="IOilexerGrammarProductionRuleEntry"/>.
        /// </summary>
        /// <remarks>Collapse points represent a point
        /// within the hierarchy where the context of the rule is purely 
        /// a series of alternations, and the supplemental
        /// node that is created by the <see cref="IOilexerGrammarProductionRuleEntry"/>
        /// is superfluous, and should be reduced to a type label.</remarks>
        bool IsRuleCollapsePoint { get; set; }
        /// <summary>
        /// Denotes whether the <see cref="IOilexerGrammarProductionRuleEntry"/> should be
        /// collapsed to its minimal deterministic form.
        /// </summary>
        /// <remarks>May have unintended side-effects.</remarks>
        bool MaxReduce { get; set; }
        /// <summary>Returns/sets the nullable <see cref="Int32"/> value denoting the number of tokens, max, to look-ahead before defaulting to back-tracking methods.</summary>
        int? LookaheadTokenLimit { get; set; }
        /// <summary>Returns the <see cref="String"/> value denoting the definition of the <see cref="IOilexerGrammarProductionRuleEntry"/> prior to expanding the templates.</summary>
        string PreexpansionText { get; }

        void CreatePreexpansionText();
    }
}
