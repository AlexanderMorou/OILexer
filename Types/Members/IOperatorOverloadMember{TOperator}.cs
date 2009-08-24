using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for a member which overloads the interpretation of the
    /// containing type with regards to <typeparamref name="TOperator"/> type expressions.
    /// </summary>
    /// <typeparam name="TOperator">The type of operator that is overloadable by the
    /// <see cref="IOperatorOverloadMember{TOperator}"/>.</typeparam>
    public interface IOperatorOverloadMember<TOperator> :
        IExpressionCoercionMember
        where TOperator :
            struct
    {
        /// <summary>
        /// The operator overloaded.
        /// </summary>
        TOperator Operator { get; set; }
    }
}
