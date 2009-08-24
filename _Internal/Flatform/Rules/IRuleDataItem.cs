using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Rules
{
    internal enum RuleDataItemType
    {
        Enumeration,
        Capture,
        Rule,
    }
    /// <summary>
    /// Defines properties and methods for working with a rule data item.
    /// </summary>
    internal interface IRuleDataItem
    {
        /// <summary>
        /// Returns the rank of the element, representing the level to which
        /// the elements are structured.
        /// </summary>
        /// <remarks>In cases where a rank of 1 is repeated its rank is increased to 2,
        /// if a rank of 2 is repeated, it is increased to 3, and so on.</remarks>
        int Rank { get; set; }

        /// <summary>
        /// Returns the <see cref="RuleDataItemType"/> of the current element.
        /// </summary>
        RuleDataItemType ElementType { get; }

        /// <summary>
        /// Returns whether the <paramref name="other"/>
        /// is of equivalent nature to the current 
        /// <see cref="IRuleDataItem"/>.
        /// </summary>
        /// <param name="other">The <see cref="IRuleDataItem"/> to compare.</param>
        /// <returns>True if the <see cref="IRuleDataItem"/> is of equal nature
        /// to the <paramref name="other"/> provided.</returns>
        bool AreEqualNatured(IRuleDataItem other);

        /// <summary>
        /// Obtains the best natured <see cref="IRuleDataItem"/> from the
        /// <paramref name="equalNaturedSet"/> provided.
        /// </summary>
        /// <param name="equalNaturedSet">An array of <see cref="IRuleDataItem"/> 
        /// elements which are equally natured to the current
        /// <see cref="IRuleDataItem"/>.</param>
        /// <returns>The best natured <see cref="IRuleDataItem"/> of the
        /// <paramref name="equalNaturedSet"/>.</returns>
        IRuleDataItem ObtainBestNatured(IRuleDataItem[] equalNaturedSet);

        /// <summary>
        /// Merges the <paramref name="other"/> <see cref="IRuleDataItem"/>
        /// with the current <see cref="IRuleDataItem"/>.
        /// </summary>
        /// <param name="other">The <see cref="IRuleDataItem"/> to merge 
        /// with the current element.</param>
        /// <returns>A <see cref="IRuleDataItem"/> which represents the merge
        /// between the current <paramref name="IRuleDataItem"/> and the
        /// <paramref name="other"/>.</returns>
        IRuleDataItem MergeWith(IRuleDataItem other);
    }
}
