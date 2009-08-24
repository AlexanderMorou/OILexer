using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using Oilexer.Expression;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal
{
    internal interface ITokenItemData
    {
        /// <summary>
        /// Returns the <see cref="IFieldMember"/> relative to the primary
        /// exit state for the self ambiguous token item.
        /// </summary>
        IFieldMember PrimaryExitState { get; }
        /// <summary>
        /// Returns the <see cref="IFieldMember"/> which denotes 
        /// which subset within the state machine to compare 
        /// against.
        /// </summary>
        IFieldMember AllowedField { get; }
        /// <summary>
        /// Returns the <see cref="IFieldMember"/> within the 
        /// current subset enumeration that represents the current
        /// entry.
        /// </summary>
        IFieldMember SubsetField { get; }
        /// <summary>
        /// Returns the <see cref="IFieldReferenceExpression"/>
        /// relative to the current subset's 'none' entry
        /// for set allowance comparison purposes.
        /// </summary>
        IFieldReferenceExpression NoneReference { get; }
    }
}
