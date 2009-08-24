using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Statements;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for an overload member which denotes type 
    /// coercion from/to the containing type.
    /// </summary>
    public interface ITypeConversionOverloadMember :
        IExpressionCoercionMember
    {
        /// <summary>
        /// Returns/sets whether the conversion overload is implicit or explicit.
        /// </summary>
        TypeConversionRequirement Requirement { get; set; }
        /// <summary>
        /// Returns/sets whether the conversion overload is from the containing type or 
        /// to the containing type.
        /// </summary>
        TypeConversionDirection Direction { get; set; }
        /// <summary>
        /// Returns/sets the type which is coerced by the overload.
        /// </summary>
        ITypeReference CoercionType { get; set; }
        /// <summary>
        /// Returns the <see cref="ITypeConversionOverloadSource"/> which relates to a reference to 
        /// the source parameter for the conversion process.
        /// </summary>
        ITypeConversionOverloadSource Source { get; }
    }
}
