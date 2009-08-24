using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a series of members which coerce either 
    /// expression execution or interpretation.
    /// </summary>
    public interface IExpressionCoercionMembers :
        IMembers<IExpressionCoercionMember, IMemberParentType, CodeSnippetTypeMember>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IExpressionCoercionMembers"/> implementation
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="parent">The partial of the <see cref="IMembers{TItem, TParent, TDom}.TargetDeclaration"/> which
        /// needs a <see cref="IExpressionCoercionMembers"/> implementation instance.</param>
        /// <returns>A new <see cref="IExpressionCoercionMembers"/> implementation instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        new IExpressionCoercionMembers GetPartialClone(IMemberParentType parent);
        /// <summary>
        /// Adds a new <see cref="IBinaryOperatorOverloadMember"/> with the <paramref name="side"/>,
        /// <paramref name="otherSide"/> and <paramref name="overloadedOperator"/> provided.
        /// </summary>
        /// <param name="side">The side the containing type is contained on.</param>
        /// <param name="otherSide">The other side of the operation.</param>
        /// <param name="overloadedOperator">The operator overloaded by the new <see cref="IBinaryOperatorOverloadMember"/>.</param>
        /// <returns>A new <see cref="IBinaryOperatorOverloadMember"/> implementation.</returns>
        IBinaryOperatorOverloadMember AddNew(BinaryOperatorOverloadContainingSide side, ITypeReference otherSide, OverloadableBinaryOperators overloadedOperator);
        /// <summary>
        /// Adds a new <see cref="IBinaryOperatorOverloadMember"/> with the <paramref name="overloadedOperator"/> provided which 
        /// is contained on both sides.
        /// </summary>
        /// <param name="overloadedOperator">The operator overloaded by the new <see cref="IBinaryOperatorOverloadMember"/>.</param>
        /// <returns>A new <see cref="IBinaryOperatorOverloadMember"/> implementation.</returns>
        IBinaryOperatorOverloadMember AddNew(OverloadableBinaryOperators overloadedOperator);
        /// <summary>
        /// Adds a new <see cref="IBinaryOperatorOverloadMember"/> with the containing type as the 
        /// left side.
        /// </summary>
        /// <param name="rightSide">The right-side type of the binary operation overload.</param>
        /// <param name="overloadedOperator">The operator overloaded by the new <see cref="IBinaryOperatorOverloadMember"/> implementation.</param>
        /// <returns>A new <see cref="IBinaryOperatorOverloadMember"/> implementation.</returns>
        IBinaryOperatorOverloadMember AddNewLeft(ITypeReference rightSide, OverloadableBinaryOperators overloadedOperator);
        /// <summary>
        /// Adds a new <see cref="IBinaryOperatorOverloadMember"/> with the containing type as the 
        /// right side.
        /// </summary>
        /// <param name="leftSide">The left-side type of the binary operation overload.</param>
        /// <param name="overloadedOperator">The operator overloaded by the new <see cref="IBinaryOperatorOverloadMember"/> implementation.</param>
        /// <returns>A new <see cref="IBinaryOperatorOverloadMember"/> implementation.</returns>
        IBinaryOperatorOverloadMember AddNewRight(ITypeReference leftSide, OverloadableBinaryOperators overloadedOperator);

        /// <summary>
        /// Adds a new <see cref="IUnaryOperatorOverloadMember"/> which overloads the <paramref name="overloadedOperator"/> provided.
        /// </summary>
        /// <param name="overloadedOperator">The unary operator overloaded by the new <see cref="IUnaryOperatorOverloadMember"/>.</param>
        /// <returns></returns>
        IUnaryOperatorOverloadMember AddNew(OverloadableUnaryOperators overloadedOperator);

        /// <summary>
        /// Adds a new <see cref="ITypeConversionOverloadMember"/> with the <paramref name="requirement"/>, <paramref name="direction"/> and <paramref name="coercionType"/>
        /// provided.
        /// </summary>
        /// <param name="requirement">Whether the new <see cref="ITypeConversionOverloadMember"/> is 
        /// implicit or explicit</param>
        /// <param name="direction">The direction of the conversion.</param>
        /// <param name="coercionType">The type the type coercion is translated from/to based upon
        /// <paramref name="direction"/>.</param>
        /// <returns>A new <see cref="ITypeConversionOverloadMember"/> implementation.</returns>
        ITypeConversionOverloadMember AddNew(TypeConversionRequirement requirement, TypeConversionDirection direction, ITypeReference coercionType);
        /// <summary>
        /// Adds a new explicit <see cref="ITypeConversionOverloadMember"/> with the <paramref name="direction"/> and <paramref name="coercionType"/>
        /// provided.
        /// </summary>
        /// <param name="direction">The direction of the conversion.</param>
        /// <param name="coercionType">The type the type coercion is translated from/to based upon
        /// <paramref name="direction"/>.</param>
        /// <returns>A new explicit <see cref="ITypeConversionOverloadMember"/> implementation.</returns>
        ITypeConversionOverloadMember AddNewExplicit(TypeConversionDirection direction, ITypeReference coercionType);
        /// <summary>
        /// Adds a new explicit <see cref="ITypeConversionOverloadMember"/> with the <paramref name="coercionType"/> provided
        /// that translates <paramref name="coercionType"/> to the containing type.
        /// </summary>
        /// <param name="coercionType">The type to be coerced from.</param>
        /// <returns>A new explicit <see cref="ITypeConversionOverloadMember"/> implementation which
        /// translates to the containing type.</returns>
        ITypeConversionOverloadMember AddNewExplicitTo(ITypeReference coercionType);
        /// <summary>
        /// Adds a new explicit <see cref="ITypeConversionOverloadMember"/> with the <paramref name="coercionType"/> provided
        /// that translates from the containing type to the <paramref name="coercionType"/>.
        /// </summary>
        /// <param name="coercionType">The type to be coerced to.</param>
        /// <returns>A new explicit <see cref="ITypeConversionOverloadMember"/> implementation which
        /// translates from the containing type.</returns>
        ITypeConversionOverloadMember AddNewExplicitFrom(ITypeReference coercionType);
        /// <summary>
        /// Adds a new implicit <see cref="ITypeConversionOverloadMember"/> with the <paramref name="direction"/> and <paramref name="coercionType"/>
        /// provided.
        /// </summary>
        /// <param name="direction">The direction of the conversion.</param>
        /// <param name="coercionType">The type the type coercion is translated from/to based upon
        /// <paramref name="direction"/>.</param>
        /// <returns>A new implicit <see cref="ITypeConversionOverloadMember"/> implementation.</returns>
        ITypeConversionOverloadMember AddNewImplicit(TypeConversionDirection direction, ITypeReference coercionType);
        /// <summary>
        /// Adds a new implicit <see cref="ITypeConversionOverloadMember"/> with the <paramref name="coercionType"/> provided
        /// that translates <paramref name="coercionType"/> to the containing type.
        /// </summary>
        /// <param name="coercionType">The type to be coerced from.</param>
        /// <returns>A new implicit <see cref="ITypeConversionOverloadMember"/> implementation which
        /// translates to the containing type.</returns>
        ITypeConversionOverloadMember AddNewImplicitTo(ITypeReference coercionType);
        /// <summary>
        /// Adds a new implicit <see cref="ITypeConversionOverloadMember"/> with the <paramref name="coercionType"/> provided
        /// that translates from the containing type to the <paramref name="coercionType"/>.
        /// </summary>
        /// <param name="coercionType">The type to be coerced to.</param>
        /// <returns>A new implicit <see cref="ITypeConversionOverloadMember"/> implementation which
        /// translates from the containing type.</returns>
        ITypeConversionOverloadMember AddNewImplicitFrom(ITypeReference coercionType);
    }
}
