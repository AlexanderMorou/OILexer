using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer._Internal.Flatform.Tokens;
using Oilexer._Internal.Flatform.Rules.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a flattened rule
    /// item, which provides large scale analysis results in a single point.
    /// </summary>
    internal interface IFlattenedRuleItem 
    {
        /// <summary>
        /// Returns the <see cref="IProductionRuleItem"/>
        /// the <see cref="IFlattenedRuleItem"/> is based upon.
        /// </summary>
        IProductionRuleItem Source { get; }
        /// <summary>
        /// Returns the <see cref="IProductionRuleEntry"/>
        /// from which the <see cref="Source"/> is derived.
        /// </summary>
        IProductionRuleEntry SourceRoot { get; }
        /// <summary>
        /// Returns the <see cref="FlattenedRuleExpression"/>
        /// which directly contains the <see cref="IFlattenedRuleItem"/>
        /// </summary>
        FlattenedRuleExpression Parent { get; }
        /// <summary>
        /// Returns the <see cref="FlattenedRuleEntry"/> 
        /// which contains the <see cref="IFlattenedRuleItem"/>
        /// </summary>
        FlattenedRuleEntry Root { get; }
        /// <summary>
        /// Initializes the <see cref="IFlattenedRuleItem"/>
        /// </summary>
        /// <param name="tokenLookup">A <see cref="IDictionary{TKey, TValue}"/> 
        /// which contains final build data about the tokens contained within
        /// the language in the active scope.</param>
        void Initialize(TokenFinalDataSet tokenLookup, IProductionRuleEntry[] productionRules);
#if false
        /// <summary>
        /// Returns the <see cref="IRuleDataNode"/> associated to the
        /// current <see cref="IFlattenedRuleItem"/> which is responsible
        /// for the resultant data structure aspect of the rule item.
        /// </summary>
        //IRuleDataNode DataNode { get; }
#endif
        /// <summary>
        /// Returns the <see cref="IRuleDataItem"/>
        /// associated to the <see cref="IFlattenedRuleItem"/> which is responsible
        /// for the resultant data structure aspect of the rule item.
        /// </summary>
        IRuleDataItem DataItem { get; }
        /// <summary>
        /// Returns the <see cref="SimpleLanguageState"/>
        /// which represents the current transition information necessary
        /// to advance to the next point in the parse of the current
        /// rule.
        /// </summary>
        SimpleLanguageState State { get; }
        /// <summary>
        /// Returns the <see cref="ScannableEntryItemRepeatOptions"/> 
        /// for the <see cref="Source"/>.
        /// </summary>
        ScannableEntryItemRepeatOptions RepeatOptions { get; }
        /// <summary>
        /// Returns whether the <see cref="IFlattenedRuleItem"/>
        /// is optional.
        /// </summary>
        /// <remarks>Used in state creation.</remarks>
        bool Optional { get; }
    }
}
