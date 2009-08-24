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
    public class SelfAmbiguousTokenItemData :
        List<SelfAmbiguousTokenItemData.AllowedInfo>,
        ITokenItemData
    {
        public class AllowedInfo
        {
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
            public AllowedInfo(
                IFieldMember allowedField,
                IFieldMember subsetField,
                IFieldReferenceExpression noneReference)
            {
                this.AllowedField = allowedField;
                this.SubsetField = subsetField;
                this.NoneReference = noneReference;
            }
        }
        /// <summary>
        /// Returns the <see cref="IFieldMember"/> relative to the primary
        /// exit state for the self ambiguous token item.
        /// </summary>
        public IFieldMember PrimaryExitState { get; private set; }

        public SelfAmbiguousTokenItemData(IFieldMember primaryExitState)
        {
            this.PrimaryExitState = primaryExitState;
        }

        #region ITokenItemData Members

        IFieldMember ITokenItemData.AllowedField
        {
            get { return this[0].AllowedField; }
        }

        IFieldMember ITokenItemData.SubsetField
        {
            get { return this[0].SubsetField; }
        }

        IFieldReferenceExpression ITokenItemData.NoneReference
        {
            get { return this[0].NoneReference; }
        }

        #endregion
    }
}
