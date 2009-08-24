using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using Oilexer.Expression;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary> 
    /// An interface declaration.
    /// </summary>
    [Serializable]
    public partial class InterfaceType :
        SegmentableParameteredType<IInterfaceType, CodeTypeDeclaration, InterfacePartials>,
        IInterfaceType
    {
        #region InterfaceType Data Members
        /// <summary>
        /// data member for <see cref="Methods"/>
        /// </summary>
        private IMethodSignatureMembers methods;
        /// <summary>
        /// Data member for <see cref="Properties"/>,
        /// </summary>
        private IPropertySignatureMembers properties;

        /// <summary>
        /// Data member for <see cref="ImplementsList"/>.
        /// </summary>
        private ITypeReferenceCollection implementsList;
        
        #endregion

        #region InterfaceType Constructors
        /// <summary>
        /// Creates a new instance of <see cref="InterfaceType"/> with the name and
        /// <paramref name="parentTarget"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="InterfaceType"/>.</param>
        /// <param name="parentTarget">The <see cref="ITypeParent"/> which contains the <see cref="InterfaceType"/>.</param>
        protected internal InterfaceType(string name, ITypeParent parentTarget)
            : base(name, parentTarget)
        {
        }

        protected internal InterfaceType(IInterfaceType basePartial, ITypeParent parentTarget)
            : base(basePartial, parentTarget)
        {
            
        }

        #endregion

        /// <summary>
        /// Generates the <see cref="CodeTypeDeclaration"/> that represents the <see cref="InterfaceType"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="CodeTypeDeclaration"/> if successful.-null- otherwise.</returns>
        public override CodeTypeDeclaration GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            CodeTypeDeclaration result = base.GenerateCodeDom(options);
            ITypeReference[] impls = this.ImplementsList.ToArray();
            bool[] duplicate = new bool[impls.Length];
            List<string> names = new List<string>();
            for (int i = 0; i < impls.Length; i++)
            {
                string currentName = impls[i].TypeInstance.GetTypeName(options);
                if (impls[i].TypeParameters.Count > 0 && impls[i].TypeInstance.IsGeneric)
                    currentName += string.Format("`{0}", impls[i].TypeParameters.Count);
                duplicate[i] = names.Contains(currentName);
                if (duplicate[i])
                {
                    duplicate[names.IndexOf(currentName)] = true;
                }
                names.Add(currentName);
            }
            bool autoResolve = options.AutoResolveReferences;
            for (int i = 0; i < impls.Length; i++)
            {
                ITypeReference typeRef = impls[i];
                if (duplicate[i] && autoResolve)
                    options.AutoResolveReferences = false;
                result.BaseTypes.Add(typeRef.GenerateCodeDom(options));
                if (duplicate[i] && autoResolve)
                    options.AutoResolveReferences = autoResolve;
            }
            if (options.AllowPartials)
            {
                foreach (IMethodSignatureMember memberMethod in this.Methods.Values)
                    if (memberMethod.ParentTarget == this)
                        result.Members.Add(memberMethod.GenerateCodeDom(options));
                foreach (IPropertySignatureMember memberProperty in this.Properties.Values)
                    if (memberProperty.ParentTarget == this)
                        result.Members.Add(memberProperty.GenerateCodeDom(options));
            }
            else if (this.IsRoot)
            {
                result.Members.AddRange(this.Methods.GenerateCodeDom(options));
                result.Members.AddRange(this.Properties.GenerateCodeDom(options));
            }
            result.IsInterface = true;
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            return result;
        }

        #region IType Members

        /// <summary>
        /// Returns the <see cref="CodeTypeReference"/> which denotes the <see cref="InterfaceType"/>.
        /// </summary>
        public new CodeTypeReference Reference
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IInterfaceType Members

        public new IInterfacePartials Partials
        {
            get { return (IInterfacePartials)base.Partials; }
        }

        public ITypeReferenceCollection ImplementsList
        {
            get
            {
                if (this.IsPartial)
                    return this.GetRootDeclaration().ImplementsList;
                if (this.implementsList == null)
                    this.implementsList = new TypeReferenceCollection();
                return this.implementsList;
            }
        }

        #endregion


        protected override InterfacePartials InitializePartials()
        {
            return new InterfacePartials(this);
        }

        #region ISignatureMemberParentType Members

        public IMethodSignatureMembers Methods
        {
            get {
                if (this.methods == null)
                    this.InitializeMethods();
                return this.methods;
            }
        }

        public IPropertySignatureMembers Properties
        {
            get {
                if (this.properties == null)
                    this.InitializeProperties();
                return this.properties;
            }
        }

        public ITypeReferenceExpression GetTypeExpression(ITypeReferenceCollection typeArguments)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region Initialization Members

        private void InitializeMethods()
        {
            if (this.IsPartial)
                this.methods = base.GetRootDeclaration().Methods.GetPartialClone(this);
            else
                this.methods = new MethodSignatureMembers(this);
        }

        private void InitializeProperties()
        {
            if (this.IsPartial)
                this.properties = base.GetRootDeclaration().Properties.GetPartialClone(this);
            else
                this.properties = new PropertySignatureMembers(this);
        }
        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="InterfaceType"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="InterfaceType"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.IsRoot && this.implementsList != null)
                    foreach (ITypeReference itr in this.ImplementsList)
                            result.Add(itr);
            if (options.AllowPartials)
            {
                this.Properties.GatherTypeReferences(ref result, options);
                this.Methods.GatherTypeReferences(ref result, options);
            }
            else
            {
                if (this.properties != null && this.IsRoot)
                    this.Properties.GatherTypeReferences(ref result, options);
                if (this.methods != null && this.IsRoot)
                    this.Methods.GatherTypeReferences(ref result, options);
            }
        }

        #region ISignatureMemberParentType Members

        public int GetMemberCount()
        {
            return this.GetMemberCount(true);
        }

        #endregion

        public override int GetTypeCount(bool includePartials)
        {
            return 0;
        }

        public override int GetMemberCount(bool includePartials)
        {
            int result = 0;
            if (includePartials)
            {
                if (this.methods != null || this.IsPartial)
                    result += this.Methods.Count;
                if (this.properties != null || this.IsPartial)
                    result += this.Properties.Count;
            }
            else
            {
                if (this.methods != null || this.IsPartial)
                    result += this.Methods.GetCountForTarget(this);
                if (this.properties != null || this.IsPartial)
                    result += this.Properties.GetCountForTarget(this);
            }
            return result;
        }
    }
}
