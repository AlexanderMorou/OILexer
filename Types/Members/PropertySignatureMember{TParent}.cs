using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    public abstract class PropertySignatureMember<TParent> :
        Member<TParent, CodeMemberProperty>,
        IPropertySignatureMember<TParent>
        where TParent :
            IDeclarationTarget
    {
        #region PropertySignatureMember<TParent> Data members
        /// <summary>
        /// Data member for <see cref="PropertyType"/>.
        /// </summary>
        ITypeReference propertyType;
        /// <summary>
        /// Data member for <see cref="HasGet"/>.
        /// </summary>
        private bool hasGet;
        /// <summary>
        /// Data member for <see cref="HasSet"/>.
        /// </summary>
        private bool hasSet;
        /// <summary>
        /// Data member for <see cref="Summary"/>.
        /// </summary>
        private string summary;
        /// <summary>
        /// Data member for <see cref="Remarks"/>.
        /// </summary>
        private string remarks;
        /// <summary>
        /// Data member for <see cref="HidesPrevious"/>.
        /// </summary>
        private bool hidesPrevious;
        #endregion

        #region PropertySignatureMember<TParent> Constructors
        public PropertySignatureMember(string name, TParent parentTarget)
            : base(name, parentTarget)
        {

        }
        public PropertySignatureMember(TypedName nameAndType, TParent parentTarget)
            : base(nameAndType.Name, parentTarget)
        {
            this.PropertyType = nameAndType.TypeReference;
        }
        #endregion 

        public override CodeMemberProperty GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            CodeMemberProperty result = new CodeMemberProperty();
            result.Attributes = MemberAttributes.Final | AccessLevelAttributes(this.AccessLevel);
            result.Type = this.PropertyType.GenerateCodeDom(options);
            result.HasGet = this.HasGet;
            result.HasSet = this.HasSet;
            result.Name = this.Name;
            if (this.hidesPrevious)
                result.Attributes |= MemberAttributes.New;
            if (this.summary != null && this.summary != string.Empty)
                result.Comments.Add(new CodeCommentStatement(new CodeComment(this.summary, true)));
            if (this.remarks != null && this.remarks != string.Empty)
                result.Comments.Add(new CodeCommentStatement(new CodeComment(this.remarks, true)));
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            return result;
        }

        #region IPropertySignatureMember<TParent> Members

        /// <summary>
        /// Returns/sets whether the <see cref="PropertySignatureMember{TParent}"/> has a get area.
        /// </summary>
        public bool HasGet
        {
            get
            {
                return this.hasGet;
            }
            set
            {
                this.hasGet = value;
            }
        }

        /// <summary>
        /// Returns/sets whether the <see cref="PropertySignatureMember{TParent}"/> has a set area.
        /// </summary>
        public bool HasSet
        {
            get
            {
                return this.hasSet;
            }
            set
            {
                this.hasSet = value;
            }
        }


        /// <summary>
        /// Returns/sets the type reference the property uses.
        /// </summary>
        public ITypeReference PropertyType
        {
            get
            {
                return this.propertyType;
            }
            set
            {
                this.propertyType = value;
            }
        }

        #endregion

        protected override IMemberReferenceExpression OnGetReference()
        {
            throw new NotSupportedException("Signatures cannot be referenced in this manner");
        }

        /// <summary>
        /// Performs a look-up on the <see cref="PropertySignatureMember{TParent}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="PropertySignatureMember{TParent}"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.propertyType != null)
                result.Add(this.propertyType);
        }
        #region IAutoCommentMember Members

        /// <summary>
        /// Returns/sets a string related to the auto-documentation summary comment generated by the system.
        /// </summary>
        public string Summary
        {
            get
            {
                return this.summary;
            }
            set
            {
                this.summary = value;
            }
        }

        /// <summary>
        /// Returns/sets a string related to the auto-documentation remarks comment generated by the system.
        /// </summary>
        public string Remarks
        {
            get
            {
                return this.remarks;
            }
            set
            {
                this.remarks = value;
            }
        }

        #endregion
        #region ISignatureTableMember Members

        public virtual bool HidesPrevious
        {
            get
            {
                return this.hidesPrevious;
            }
            set
            {
                this.hidesPrevious = value;
            }
        }

        #endregion

    }
}
