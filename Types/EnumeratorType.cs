using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// An enumerator that will be translated into a <see cref="CodeTypeDeclaration"/>.
    /// </summary>
    [Serializable]
    public class EnumeratorType :
        DeclaredType<CodeTypeDeclaration>,
        IEnumeratorType
    {
        /// <summary>
        /// Data member for <see cref="EnumeratorType"/>
        /// </summary>
        private EnumeratorBaseType baseType = EnumeratorBaseType.Default;
        /// <summary>
        /// Data member for <see cref="Fields"/>.
        /// </summary>
        private IEnumTypeFieldMembers fields;

        public override CodeTypeDeclaration GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            CodeTypeDeclaration ctd = base.GenerateCodeDom(options);
            ctd.IsEnum = true;
            ctd.Members.AddRange(this.Fields.GenerateCodeDom(options));
            switch (this.BaseType)
            {
                case EnumeratorBaseType.UByte:
                    ctd.BaseTypes.Add(typeof(byte));
                    break;
                case EnumeratorBaseType.SByte:
                    ctd.BaseTypes.Add(typeof(sbyte));
                    break;
                case EnumeratorBaseType.UShort:
                    ctd.BaseTypes.Add(typeof(ushort));
                    break;
                case EnumeratorBaseType.Short:
                    ctd.BaseTypes.Add(typeof(short));
                    break;
                case EnumeratorBaseType.UInt:
                    ctd.BaseTypes.Add(typeof(uint));
                    break;
                case EnumeratorBaseType.ULong:
                    ctd.BaseTypes.Add(typeof(ulong));
                    break;
                case EnumeratorBaseType.SLong:
                    ctd.BaseTypes.Add(typeof(long));
                    break;
            }
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            return ctd;
        }

        /// <summary>
        /// Creates a new instance of <see cref="EnumeratorType"/> with the <paramref name="name"/>
        /// and <paramref name="parent"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="EnumeratorType"/>.</param>
        /// <param name="parentTarget">The <see cref="ITypeParent"/> which contains the <see cref="EnumeratorType"/>.</param>
        protected internal EnumeratorType(string name, ITypeParent parentTarget)
            : base(name, parentTarget)
        {
            CheckModule();
        }

        /// <summary>
        /// Returns whether the <see cref="EnumeratorType"/> is a generic.
        /// </summary>
        /// <returns>false if the <see cref="EnumeratorType"/> is top-level, otherwise value is based upon
        /// the <see cref="DeclaredType{TDom}.ParentTarget"/>.</returns>
        public override bool IsGeneric
        {
            get
            {
                if (this.ParentTarget is IDeclaredType)
                    return ((IDeclaredType)(this.ParentTarget)).IsGeneric;
                else
                    return false;
            }
        }

        #region IEnumeratorType Members


        public IFieldMember GetMemberByName(string memberName)
        {
            foreach (IFieldMember ifm in this.Fields.Values)
                if (ifm.Name == memberName)
                    return ifm;
            return null;
        }


        /// <summary>
        /// Returns/sets the base type of the <see cref="EnumeratorType"/>.
        /// </summary>
        public EnumeratorBaseType BaseType
        {
            get
            {
                return this.baseType;
            }
            set
            {
                this.baseType = value;
            }
        }

        #endregion

        /// <summary>
        /// Returns whether the <see cref="EnumeratorType"/> is in generic form when <see cref="IsGeneric"/> is true.
        /// </summary>
        public override bool IsGenericTypeDefinition
        {
            get
            {
                if (this.parentTarget is IDeclaredType)
                    return ((IDeclaredType)(this.ParentTarget)).IsGenericTypeDefinition;
                else
                    return false;
            }
        }

        #region IEnumeratorType Members


        public IEnumTypeFieldMembers Fields
        {
            get {
                if (this.fields == null)
                    this.InitializeFields();
                return this.fields;
            }
        }

        #endregion

        #region IFieldParentType Members

        IFieldMembersBase IFieldParentType.Fields
        {
            get { return this.Fields; }
        }

        #endregion

        /// <summary>
        /// Initializes the fields data member.
        /// </summary>
        private void InitializeFields()
        {
            //Initialize the fields.
            this.fields = new EnumTypeFieldMembers(this);
        }

        /// <summary>
        /// Performs a look-up on the <see cref="EnumeratorType"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="EnumeratorType"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.fields != null)
                this.Fields.GatherTypeReferences(ref result, options);
        }

        #region IFieldParentType Members


        public int GetMemberCount()
        {
            if (this.fields != null)
                return this.fields.Count;
            else
                return 0;
        }

        #endregion
    }
}
