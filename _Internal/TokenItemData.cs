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
    internal class TokenItemData :
        ITokenItemData
    {
        #region ITokenItemData Members
        /// <summary>
        /// Returns the <see cref="IFieldMember"/> relative to the primary
        /// exit state for the self ambiguous token item.
        /// </summary>
        public IFieldMember PrimaryExitState { get; private set; }

        /// <summary>
        /// Returns the <see cref="IFieldMember"/> which denotes 
        /// which subset within the state machine to compare 
        /// against.
        /// </summary>
        public IFieldMember AllowedField { get; private set; }

        /// <summary>
        /// Returns the <see cref="IFieldMember"/> within the 
        /// current subset enumeration that represents the current
        /// entry.
        /// </summary>
        public IFieldMember SubsetField { get; private set; }

        /// <summary>
        /// Returns the <see cref="IFieldReferenceExpression"/>
        /// relative to the current subset's 'none' entry
        /// for set allowance comparison purposes.
        /// </summary>
        public IFieldReferenceExpression NoneReference { get; private set; }

        #endregion

        public TokenItemData(IFieldMember primaryExitState, IFieldMember allowedField, IFieldMember subsetField, IFieldReferenceExpression noneReference)
        {
            this.PrimaryExitState = primaryExitState;
            this.AllowedField = allowedField;
            this.SubsetField = subsetField;
            this.NoneReference = noneReference;
        }
    }
}
