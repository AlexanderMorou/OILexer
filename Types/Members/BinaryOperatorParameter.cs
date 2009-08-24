using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Comments;
using System.CodeDom;
using Oilexer.Expression;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    public sealed partial class BinaryOperatorParameter :
        MemberParentExpression<CodeVariableReferenceExpression>,
        IBinaryOperatorParameter
    {
        internal enum OperatorSide
        {
            Left,
            Right
        }
        private IBinaryOperatorOverloadMember parent;
        private IParameterReferenceComment referenceComment;
        private OperatorSide side;
        /// <summary>
        /// Creates a new <see cref="BinaryOperatorParameter"/> with the <paramref name="parent"/>
        /// provided.
        /// </summary>
        /// <param name="parent">The <see cref="IBinaryOperatorOverloadMember"/> 
        /// which the <see cref="BinaryOperatorParameter"/> is derived from.</param>
        internal BinaryOperatorParameter(IBinaryOperatorOverloadMember parent, OperatorSide side)
            : base()
        {
            this.parent = parent;
            this.side = side;
        }
        #region IParameterReferenceExpression Members

        public IParameterReferenceComment GetReferenceParticle()
        {
            if (this.referenceComment == null)
                this.referenceComment = new ReferenceParticle(this);
            return this.referenceComment;
        }

        #endregion

        #region IVariableReferenceExpression Members

        IMemberReferenceComment IVariableReferenceExpression.GetReferenceParticle()
        {
            return this.GetReferenceParticle();
        }

        #endregion

        #region IExpression<CodeVariableReferenceExpression> Members

        /// <summary>
        /// Generates the <see cref="CodeVariableReferenceExpression"/> for the <see cref="BinaryOperatorParameter"/>.
        /// </summary>
        /// <param name="options">The <see cref="ICodeDOMTranslationOptions"/> which guide the
        /// generation process.</param>
        /// <returns>A new <see cref="CodeVariableReferenceExpression"/> which relates to the 
        /// <see cref="BinaryOperatorParameter"/>.</returns>
        public new CodeVariableReferenceExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return new CodeVariableReferenceExpression(this.Name);
        }

        #endregion

        #region IExpression Members

        CodeExpression IExpression.GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return this.GenerateCodeDom(options);
        }

        #endregion

        #region ITypeReferenceable Members

        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            return;
        }

        #endregion


        #region IMemberReferenceExpression Members

        /// <summary>
        /// Returns the name of the <see cref="BinaryOperatorParameter"/>.
        /// </summary>
        public string Name
        {
            get
            {
                switch (side)
                {
                    case OperatorSide.Left:
                        return "leftSide";
                    case OperatorSide.Right:
                        return "rightSide";
                }
                return null;
            }
        }

        public IMemberParentExpression Reference
        {
            get { return null; }
        }

        IMemberReferenceComment IMemberReferenceExpression.GetReferenceParticle()
        {
            return this.GetReferenceParticle();
        }

        #endregion
    }
}
