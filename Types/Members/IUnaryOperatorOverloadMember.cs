using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for a member which overloads the interpretation of
    /// the containing type with regards to unary operator expressions.
    /// </summary>
    public interface IUnaryOperatorOverloadMember :
        IOperatorOverloadMember<OverloadableUnaryOperators>
    {
        /// <summary>
        /// Returns the <see cref="IUnaryOperatorOverloadSource"/> which relates to a reference to 
        /// the source parameter for the unary operation.
        /// </summary>
        IUnaryOperatorOverloadSource Source { get; }
    }
}
