using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for a member which overloads the interpretation of the
    /// containing type with regards to binary operation expressions.
    /// </summary>
    public interface IBinaryOperatorOverloadMember :
        IOperatorOverloadMember<OverloadableBinaryOperators>
    {
        /// <summary>
        /// Returns/sets the side the containing type the <see cref="IBinaryOperatorOverloadMember"/>
        /// is contained within matches.
        /// </summary>
        BinaryOperatorOverloadContainingSide ContainingSide { get; set; }
        /// <summary>
        /// The other type used in the overload.
        /// </summary>
        /// <remarks>If <see cref="ContainingSide"/> is <see cref="BinaryOperatorOverloadContainingSide.Both"/>
        /// then this property is read-only.</remarks>
        ITypeReference OtherSide { get; set; }
        /// <summary>
        /// Returns the left parameter variable.
        /// </summary>
        IBinaryOperatorParameter LeftParameter { get; }
        /// <summary>
        /// Returns the right parameter variable.
        /// </summary>
        IBinaryOperatorParameter RightParameter { get; }
    }
}
