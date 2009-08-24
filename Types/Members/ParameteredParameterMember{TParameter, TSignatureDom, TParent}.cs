using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using System.Reflection;
using Oilexer.Utilities.Collections;
using Oilexer._Internal;
using Oilexer.Utilities.Common;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// A generic-form method signature parameter.
    /// </summary>
    /// <typeparam name="TParameter">The type of parameter.</typeparam>
    /// <typeparam name="TSignatureDom">The <see cref="CodeObject"/> the signature yields.</typeparam>
    /// <typeparam name="TParent">The type of parent which holds the method signatures.</typeparam>
    public partial class ParameteredParameterMember<TParameter, TSignatureDom, TParent> :
        Member<IParameteredDeclaration<TParameter, TSignatureDom, TParent>, CodeParameterDeclarationExpression>,
        IParameteredParameterMember<TParameter, TSignatureDom, TParent>
        where TParameter :
            IParameteredParameterMember<TParameter, TSignatureDom, TParent>
        where TParent :
            IDeclarationTarget
        where TSignatureDom :
            CodeObject
    {
        /// <summary>
        /// Data member for <see cref="DocumentationComment"/>.
        /// </summary>
        private string documentationComment;
        /// <summary>
        /// Data member for <see cref="ParameterType"/>
        /// </summary>
        private ITypeReference parameterType;
        /// <summary>
        /// Data member for <see cref="Direction"/>.
        /// </summary>
        private FieldDirection direction;
        /// <summary>
        /// Creates a new instance of <see cref="ParameteredParameterMember{TParameter, TSignatureDom, TParent}"/>
        /// with the parameter type, name and target provided.
        /// </summary>
        /// <param name="nameAndType">The type and name of the parameter.</param>
        /// <param name="parentTarget">The place the parameter exists on.</param>
        public ParameteredParameterMember(TypedName nameAndType, IParameteredDeclaration<TParameter, TSignatureDom, TParent> parentTarget)
            :base(nameAndType.Name, parentTarget)
        {
            this.ParameterType = nameAndType.TypeReference;
        }

        public override CodeParameterDeclarationExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            CodeParameterDeclarationExpression result = new CodeParameterDeclarationExpression(parameterType.GenerateCodeDom(options), this.Name);
            result.CustomAttributes = this.Attributes.GenerateCodeDom(options);
            if (options.NameHandler.HandlesName(this))
                result.Name = options.NameHandler.HandleName(this);
            result.Direction = this.Direction;
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            return result;
        }

        #region IParameteredParameterMember<TParameter,TSignatureDom,TParent> Members

        /// <summary>
        /// Returns/sets the <see cref="ITypeReference"/> that the 
        /// <see cref="ParameteredParameterMember{TParameter, TSignatureDom, TParent}"/> accepts.
        /// </summary>
        public ITypeReference ParameterType
        {
            get
            {
                return this.parameterType;
            }
            set
            {
                this.parameterType = value;
            }
        }

        /// <summary>
        /// Returns/sets the direction the value of the 
        /// <see cref="ParameteredParameterMember{TParameter, TSignatureDom, TParent}"/> will go.
        /// </summary>
        public virtual FieldDirection Direction
        {
            get
            {
                return this.direction;
            }
            set
            {
                this.direction = value;
            }
        }

        #endregion

        protected override IMemberReferenceExpression OnGetReference()
        {
            return this.GetReference();
        }
        public override string GetUniqueIdentifier()
        {
            if (Direction == FieldDirection.In)
                return base.GetUniqueIdentifier();
            else
                return String.Format("{0} {1}", Direction.ToString(), base.GetUniqueIdentifier());
        }
        public new IVariableReferenceExpression GetReference()
        {
            return new ReferenceExpression(this);
        }

        /// <summary>
        /// Performs a look-up on the <see cref="ParameteredParameterMember{TParameter, TSignatureDom, TParent}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="ParameteredParameterMember{TParameter, TSignatureDom, TParent}"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if ((this.parameterType != null))
                result.Add(this.parameterType);
        }

        #region IParameteredParameterMember<TParameter,TSignatureDom,TParent> Members

        /// <summary>
        /// Returns/sets the comment associated with the <see cref="ParameteredParameterMember{TParameter, TSignatureDom, TParent}"/>.
        /// </summary>
        public string DocumentationComment
        {
            get
            {
                return this.documentationComment;
            }
            set
            {
                this.documentationComment = value;
            }
        }

        #endregion

    }
}
