using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    [Serializable]
    public class DelegateTypeParameterMember :
        Member<IParameteredDeclaration<IDelegateTypeParameterMember, CodeTypeDelegate, ITypeParent>, CodeParameterDeclarationExpression>,
        IDelegateTypeParameterMember
    {
        /// <summary>
        /// Data member for <see cref="ParameterType"/>.
        /// </summary>
        private ITypeReference parameterType;
        /// <summary>
        /// Data member for <see cref="Direction"/>.
        /// </summary>
        private FieldDirection direction;
        private string documentationComment;

        /// <summary>
        /// Creates a new instance of <see cref="DelegateTypeParameterMember"/>.
        /// </summary>
        /// <param name="nameAndType">The name and data-type of the parameter.</param>
        /// <param name="parentTarget">The parent target of the <see cref="DelegateTypeParameterMember"/>.</param>
        public DelegateTypeParameterMember(TypedName nameAndType, IDelegateType parentTarget)
            : base(nameAndType.Name, parentTarget)
        {
            this.ParameterType = nameAndType.TypeReference;
        }
        public override CodeParameterDeclarationExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            CodeParameterDeclarationExpression result = new CodeParameterDeclarationExpression(parameterType.GenerateCodeDom(options), this.Name);
            if (options.NameHandler.HandlesName(this))
                result.Name = options.NameHandler.HandleName(this);
            result.Direction = this.Direction;
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            return result;
        }
        #region IParameteredParameterMember<IDelegateTypeParameterMember,CodeTypeDelegate,ITypeParent> Members

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

        public FieldDirection Direction
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

        #region IParameteredParameterMember<IDelegateTypeParameterMember,CodeTypeDelegate,ITypeParent> Members


        public new IVariableReferenceExpression GetReference()
        {
            throw new NotSupportedException("Delegate parameters are not referencable.");
        }

        #endregion


        /// <summary>
        /// Performs a look-up on the <see cref="DelegateTypeParameterMember"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="DelegateTypeParameterMember"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.parameterType != null)
                result.Add(this.ParameterType);
        }

        #region IAutoCommentFragmentMember Members

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
