using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    public class ExpressionCoercionMembers :
        Members<IExpressionCoercionMember, IMemberParentType, CodeSnippetTypeMember>,
        IExpressionCoercionMembers
    {
        public ExpressionCoercionMembers(IMemberParentType targetDeclaration)
            : base(targetDeclaration)
        {

        }
        public ExpressionCoercionMembers(IMemberParentType targetDeclaration, ExpressionCoercionMembers sibling)
            : base(targetDeclaration, sibling)
        {

        }

        protected override IMembers<IExpressionCoercionMember, IMemberParentType, CodeSnippetTypeMember> OnGetPartialClone(IMemberParentType parent)
        {
            return this.GetPartialClone(parent);
        }

        #region IExpressionCoercionMembers Members

        /// <summary>
        /// Creates a new <see cref="ExpressionCoercionMembers"/> instance 
        /// which denotes its parent as the one specified.
        /// </summary>
        /// <param name="parent">The partial of the <see cref="IMembers{TItem, TParent, TDom}.TargetDeclaration"/> which
        /// needs a <see cref="ExpressionCoercionMembers"/> instance.</param>
        /// <returns>A new <see cref="ExpressionCoercionMembers"/> instance
        /// which wraps the original dictionary, but refers to the proper target.</returns>
        public new IExpressionCoercionMembers GetPartialClone(IMemberParentType parent)
        {
            if (this.TargetDeclaration is ISegmentableDeclaredType)
                return new ExpressionCoercionMembers(parent, this);
            else
                throw new NotSupportedException("The parent type is non-segmentable.");
        }

        /// <summary>
        /// Adds a new <see cref="IBinaryOperatorOverloadMember"/> with the <paramref name="side"/>,
        /// <paramref name="otherSide"/> and <paramref name="overloadedOperator"/> provided.
        /// </summary>
        /// <param name="side">The side the containing type is contained on.</param>
        /// <param name="otherSide">The other side of the operation.</param>
        /// <param name="overloadedOperator">The operator overloaded by the new <see cref="IBinaryOperatorOverloadMember"/>.</param>
        /// <returns>A new <see cref="IBinaryOperatorOverloadMember"/> implementation.</returns>
        public IBinaryOperatorOverloadMember AddNew(BinaryOperatorOverloadContainingSide side, ITypeReference otherSide, OverloadableBinaryOperators overloadedOperator)
        {
            IBinaryOperatorOverloadMember binOpOvrMember = new BinaryOperatorOverloadMember(this.TargetDeclaration);
            binOpOvrMember.ContainingSide = side;
            if (!(side == BinaryOperatorOverloadContainingSide.Both && otherSide == null))
                binOpOvrMember.OtherSide = otherSide;
            binOpOvrMember.Operator = overloadedOperator;
            this._Add(binOpOvrMember.GetUniqueIdentifier(), binOpOvrMember);
            return binOpOvrMember;
        }

        /// <summary>
        /// Adds a new <see cref="IBinaryOperatorOverloadMember"/> with the <paramref name="overloadedOperator"/> provided which 
        /// is contained on both sides.
        /// </summary>
        /// <param name="overloadedOperator">The operator overloaded by the new <see cref="IBinaryOperatorOverloadMember"/>.</param>
        /// <returns>A new <see cref="IBinaryOperatorOverloadMember"/> implementation.</returns>
        public IBinaryOperatorOverloadMember AddNew(OverloadableBinaryOperators overloadedOperator)
        {
            return this.AddNew(BinaryOperatorOverloadContainingSide.Both, null, overloadedOperator);
        }

        /// <summary>
        /// Adds a new <see cref="IBinaryOperatorOverloadMember"/> with the containing type as the 
        /// left side.
        /// </summary>
        /// <param name="rightSide">The right-side type of the binary operation overload.</param>
        /// <param name="overloadedOperator">The operator overloaded by the new <see cref="IBinaryOperatorOverloadMember"/> implementation.</param>
        /// <returns>A new <see cref="IBinaryOperatorOverloadMember"/> implementation.</returns>
        public IBinaryOperatorOverloadMember AddNewLeft(ITypeReference rightSide, OverloadableBinaryOperators overloadedOperator)
        {
            return this.AddNew(BinaryOperatorOverloadContainingSide.LeftSide, rightSide, overloadedOperator);
        }

        /// <summary>
        /// Adds a new <see cref="IBinaryOperatorOverloadMember"/> with the containing type as the 
        /// right side.
        /// </summary>
        /// <param name="leftSide">The left-side type of the binary operation overload.</param>
        /// <param name="overloadedOperator">The operator overloaded by the new <see cref="IBinaryOperatorOverloadMember"/> implementation.</param>
        /// <returns>A new <see cref="IBinaryOperatorOverloadMember"/> implementation.</returns>
        public IBinaryOperatorOverloadMember AddNewRight(ITypeReference leftSide, OverloadableBinaryOperators overloadedOperator)
        {
            return this.AddNew(BinaryOperatorOverloadContainingSide.RightSide, leftSide, overloadedOperator);
        }

        /// <summary>
        /// Adds a new <see cref="IUnaryOperatorOverloadMember"/> which overloads the <paramref name="overloadedOperator"/> provided.
        /// </summary>
        /// <param name="overloadedOperator">The unary operator overloaded by the new <see cref="IUnaryOperatorOverloadMember"/>.</param>
        /// <returns></returns>
        public IUnaryOperatorOverloadMember AddNew(OverloadableUnaryOperators overloadedOperator)
        {
            IUnaryOperatorOverloadMember unaryOp = new UnaryOperatorOverloadMember(this.TargetDeclaration);
            unaryOp.Operator = overloadedOperator;
            this._Add(unaryOp.GetUniqueIdentifier(), unaryOp);
            return unaryOp;
        }

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
        public ITypeConversionOverloadMember AddNew(TypeConversionRequirement requirement, TypeConversionDirection direction, ITypeReference coercionType)
        {
            ITypeConversionOverloadMember typeConv = new TypeConversionOverloadMember(this.TargetDeclaration);
            typeConv.CoercionType = coercionType;
            typeConv.Requirement = requirement;
            typeConv.Direction = direction;
            this._Add(typeConv.GetUniqueIdentifier(), typeConv);
            return typeConv;
        }

        /// <summary>
        /// Adds a new explicit <see cref="ITypeConversionOverloadMember"/> with the <paramref name="direction"/> and <paramref name="coercionType"/>
        /// provided.
        /// </summary>
        /// <param name="direction">The direction of the conversion.</param>
        /// <param name="coercionType">The type the type coercion is translated from/to based upon
        /// <paramref name="direction"/>.</param>
        /// <returns>A new explicit <see cref="ITypeConversionOverloadMember"/> implementation.</returns>
        public ITypeConversionOverloadMember AddNewExplicit(TypeConversionDirection direction, ITypeReference coercionType)
        {
            return this.AddNew(TypeConversionRequirement.Explicit, direction, coercionType);
        }

        /// <summary>
        /// Adds a new explicit <see cref="ITypeConversionOverloadMember"/> with the <paramref name="coercionType"/> provided
        /// that translates <paramref name="coercionType"/> to the containing type.
        /// </summary>
        /// <param name="coercionType">The type to be coerced from.</param>
        /// <returns>A new explicit <see cref="ITypeConversionOverloadMember"/> implementation which
        /// translates to the containing type.</returns>
        public ITypeConversionOverloadMember AddNewExplicitTo(ITypeReference coercionType)
        {
            return this.AddNewExplicit(TypeConversionDirection.ToContainingType, coercionType);
        }

        /// <summary>
        /// Adds a new explicit <see cref="ITypeConversionOverloadMember"/> with the <paramref name="coercionType"/> provided
        /// that translates from the containing type to the <paramref name="coercionType"/>.
        /// </summary>
        /// <param name="coercionType">The type to be coerced to.</param>
        /// <returns>A new explicit <see cref="ITypeConversionOverloadMember"/> implementation which
        /// translates from the containing type.</returns>
        public ITypeConversionOverloadMember AddNewExplicitFrom(ITypeReference coercionType)
        {
            return this.AddNewExplicit(TypeConversionDirection.FromContainingType, coercionType);
        }

        /// <summary>
        /// Adds a new implicit <see cref="ITypeConversionOverloadMember"/> with the <paramref name="direction"/> and <paramref name="coercionType"/>
        /// provided.
        /// </summary>
        /// <param name="direction">The direction of the conversion.</param>
        /// <param name="coercionType">The type the type coercion is translated from/to based upon
        /// <paramref name="direction"/>.</param>
        /// <returns>A new implicit <see cref="ITypeConversionOverloadMember"/> implementation.</returns>
        public ITypeConversionOverloadMember AddNewImplicit(TypeConversionDirection direction, ITypeReference coercionType)
        {
            return this.AddNew(TypeConversionRequirement.Implicit, direction, coercionType);
        }

        /// <summary>
        /// Adds a new implicit <see cref="ITypeConversionOverloadMember"/> with the <paramref name="coercionType"/> provided
        /// that translates <paramref name="coercionType"/> to the containing type.
        /// </summary>
        /// <param name="coercionType">The type to be coerced from.</param>
        /// <returns>A new implicit <see cref="ITypeConversionOverloadMember"/> implementation which
        /// translates to the containing type.</returns>
        public ITypeConversionOverloadMember AddNewImplicitTo(ITypeReference coercionType)
        {
            return this.AddNewImplicit(TypeConversionDirection.ToContainingType, coercionType);
        }

        /// <summary>
        /// Adds a new implicit <see cref="ITypeConversionOverloadMember"/> with the <paramref name="coercionType"/> provided
        /// that translates from the containing type to the <paramref name="coercionType"/>.
        /// </summary>
        /// <param name="coercionType">The type to be coerced to.</param>
        /// <returns>A new implicit <see cref="ITypeConversionOverloadMember"/> implementation which
        /// translates from the containing type.</returns>
        public ITypeConversionOverloadMember AddNewImplicitFrom(ITypeReference coercionType)
        {
            return this.AddNewImplicit(TypeConversionDirection.FromContainingType, coercionType);
        }

        #endregion
    }
}
