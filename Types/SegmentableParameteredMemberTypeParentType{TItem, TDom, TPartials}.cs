using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.CodeDom;
using Oilexer.Types.Members;
using System.Reflection;
using Oilexer.Expression;
using Oilexer._Internal;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// A parameterized type that may contain type-arguments, members, and sub-types.
    /// </summary>
    [Serializable]
    public abstract partial class SegmentableParameteredMemberTypeParentType<TItem, TDom, TPartials> :
        SegmentableParameteredType<TItem, TDom, TPartials>,
        IParameteredParentType<TDom>
        where TItem :
            class,
            ISegmentableDeclaredType<TItem, TDom>,
            IParameteredParentType<TDom>
        where TDom :
            CodeTypeDeclaration,
            new()
        where TPartials :
            ISegmentableDeclaredTypePartials<TItem, TDom>
    {

        #region SegmentableParameteredMemberTypeParentType Data members
        /// <summary>
        /// Data member fro <see cref="Interfaces"/>.
        /// </summary>
        private IInterfaceTypes interfaces;

        /// <summary>
        /// Data member for <see cref="Resources"/>.
        /// </summary>
        private IDeclarationResources resources;

        /// <summary>
        /// Data member for <see cref="SnippetMembers"/>.
        /// </summary>
        private ISnippetMembers snippetMembers;

        /// <summary>
        /// Data member for <see cref="Coercions"/>.
        /// </summary>
        private IExpressionCoercionMembers coercions;

        /// <summary>
        /// Data member for <see cref="Classes"/>.
        /// </summary>
        private IClassTypes classes;
        /// <summary>
        /// Data member for <see cref="Delegates"/>.
        /// </summary>
        private IDelegateTypes delegates;
        /// <summary>
        /// Data member for <see cref="Structures"/>.
        /// </summary>
        private IStructTypes structures;
        /// <summary>
        /// Data member for <see cref="Enumerators"/>.
        /// </summary>
        private IEnumeratorTypes enumerators;
        /// <summary>
        /// Data member for <see cref="Methods"/>.
        /// </summary>
        private IMethodMembers methods;
        /// <summary>
        /// Data member for <see cref="Fields"/>.
        /// </summary>
        private IFieldMembers fields;
        /// <summary>
        /// Data member for <see cref="Properties"/>.
        /// </summary>
        private IPropertyMembers properties;

        /// <summary>
        /// Data member for <see cref="Constructors"/>.
        /// </summary>
        private IConstructorMembers constructors;

        #endregion 

        #region SegmentableParameteredMemberTypeParentType Constructors
        /// <summary>
        /// Creates a new instance of <see cref="SegmentableParameteredMemberTypeParentType{TItem, TDom, TPartials}"/> with the 
        /// <paramref name="name"/> and <paramref name="parentTarget"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="SegmentableParameteredMemberTypeParentType{TItem, TDom, TPartials}"/>.</param>
        /// <param name="parentTarget">The <see cref="ITypeParent"/> that contains
        /// the <see cref="SegmentableParameteredMemberTypeParentType{TItem, TDom, TPartials}"/>.</param>
        public SegmentableParameteredMemberTypeParentType(string name, ITypeParent parentTarget)
            : base(name, parentTarget)
        {
        }

        internal SegmentableParameteredMemberTypeParentType(TItem basePartial, ITypeParent parentTarget)
            : base(basePartial, parentTarget)
        {
        }

        #endregion

        #region ITypeParent Members

        /// <summary>
        /// Returns the <see cref="IClassTypes"/> which denote the <see cref="IClassType"/> instances
        /// of the <see cref="ITypeParent"/>.
        /// </summary>
        public IClassTypes Classes
        {
            get {
                if (this.classes == null)
                    this.classes = InitializeClasses();
                return this.classes; 
            }
        }

        /// <summary>
        /// Returns the <see cref="IDelegateTypes"/> which denote the <see cref="IDelegateType"/> instances
        /// of the <see cref="ITypeParent"/>.
        /// </summary>
        public IDelegateTypes Delegates
        {
            get
            {
                if (this.delegates == null)
                    this.delegates = InitializeDelegates();
                return this.delegates;
            }
        }

        /// <summary>
        /// Returns the <see cref="IStructTypes"/> which denote the <see cref="IStructType"/>
        /// instances of the <see cref="ITypeParent"/>.
        /// </summary>
        public IStructTypes Structures
        {
            get
            {
                if (this.structures == null)
                    this.structures = InitializeStructures();
                return this.structures;
            }
        }

        /// <summary>
        /// Returns the <see cref="IEnumeratorTypes"/> which denote the <see cref="IEnumeratorType"/>
        /// instances of the <see cref="ITypeParent"/>.
        /// </summary>
        public IEnumeratorTypes Enumerators
        {
            get {
                if (this.enumerators == null)
                    this.enumerators = this.InitializeEnumerators();
                return this.enumerators;
            }
        }

        /// <summary>
        /// Returns the <see cref="IInterfaceTypes"/> which denote the <see cref="IInterfaceType"/>
        /// instances of the <see cref="ITypeParent"/>.
        /// </summary>
        public IInterfaceTypes Interfaces
        {
            get {
                if (this.interfaces == null)
                    this.interfaces = InitializeInterfaces();
                return this.interfaces;
            }
        }

        #endregion

        #region IFieldParentType Members

        IFieldMembersBase IFieldParentType.Fields
        {
            get
            {
                return this.Fields;
            }
        }

        #endregion

        #region IMemberParentType Members
        public IFieldMembers Fields
        {
            get
            {
                if (this.fields == null)
                    this.fields = this.InitializeFields();
                return this.fields;
            }
        }
        public IPropertyMembers Properties
        {
            get
            {
                if (this.properties == null)
                    this.properties = this.InitializeProperties();
                return this.properties;
            }
        }

        public IMethodMembers Methods
        {
            get {
                if (this.methods == null)
                    this.methods = InitializeMethods();
                return this.methods;
            }
        }

        #endregion

        /// <summary>
        /// Generates the <see cref="CodeTypeDeclaration"/> that represents the <see cref="SegmentableParameteredMemberTypeParentType"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="CodeTypeDeclaration"/> if successful.-null- otherwise.</returns>
        public override TDom GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            TDom result = base.GenerateCodeDom(options);
            _OIL._Core.InsertPartialDeclarationTypes<TItem, TDom, CodeTypeMemberCollection>(result.Name, (TItem)(object)this, result.Members, options);
            if (options.AllowPartials)
            {
                ///*
                foreach (ISnippetMember subTypeSnippet in this.SnippetMembers.Values)
                {
                    if (subTypeSnippet.ParentTarget == this)
                        result.Members.Add(subTypeSnippet.GenerateCodeDom(options));
                }

                int regionMemberIndex = result.Members.Count;
                foreach (IConstructorMember memberConstructor in this.Constructors.Values)
                    if (memberConstructor.ParentTarget == this)
                        result.Members.Add(memberConstructor.GenerateCodeDom(options));
                if (options.AllowRegions && options.AutoRegionsFor(AutoRegionAreas.Constructors) && this.Constructors.GetCountForTarget(this) > 0)
                {
                    result.Members[regionMemberIndex].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Constructors", result.Name)));
                    result.Members[result.Members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                }
                regionMemberIndex = result.Members.Count;
                foreach (IFieldMember memberField in this.Fields.Values)
                    if (memberField.ParentTarget == this)
                        result.Members.Add(memberField.GenerateCodeDom(options));
                if (options.AllowRegions && options.AutoRegionsFor(AutoRegionAreas.Fields) && this.Fields.GetCountForTarget(this) > 0)
                {
                    result.Members[regionMemberIndex].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Data Members", result.Name)));
                    result.Members[result.Members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                }
                regionMemberIndex = result.Members.Count;
                foreach (IMethodMember memberMethod in this.Methods.Values)
                    if (memberMethod.ParentTarget == this)
                        result.Members.Add(memberMethod.GenerateCodeDom(options));
                if (options.AllowRegions && options.AutoRegionsFor(AutoRegionAreas.Methods) && this.Methods.GetCountForTarget(this) > 0)
                {
                    result.Members[regionMemberIndex].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Methods", result.Name)));
                    result.Members[result.Members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                }
                regionMemberIndex = result.Members.Count;
                foreach (IPropertyMember memberProperty in this.Properties.Values)
                    if (memberProperty.ParentTarget == this)
                        result.Members.Add(memberProperty.GenerateCodeDom(options));
                if (options.AllowRegions && options.AutoRegionsFor(AutoRegionAreas.Properties) && this.Properties.GetCountForTarget(this) > 0)
                {
                    result.Members[regionMemberIndex].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Properties", result.Name)));
                    result.Members[result.Members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                }
                //*/
                /*
                result.Members.AddRange(this.Classes.GenerateCodeDom(options));
                result.Members.AddRange(this.Structures.GenerateCodeDom(options));
                result.Members.AddRange(this.Methods.GenerateCodeDom(options));
                //*/
            }
            else if (this.IsRoot)
            {
                foreach (ISnippetMember subTypeSnippet in this.SnippetMembers.Values)
                {
                    if (subTypeSnippet.ParentTarget == this)
                        result.Members.Add(subTypeSnippet.GenerateCodeDom(options));
                }
                int regionMemberIndex = result.Members.Count;
                result.Members.AddRange(this.Constructors.GenerateCodeDom(options));
                if (options.AllowRegions && options.AutoRegionsFor(AutoRegionAreas.Constructors) && this.Constructors.Count > 0)
                {
                    result.Members[regionMemberIndex].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Constructors", result.Name)));
                    result.Members[result.Members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                }
                regionMemberIndex = result.Members.Count;
                result.Members.AddRange(this.Fields.GenerateCodeDom(options));
                if (options.AllowRegions && options.AutoRegionsFor(AutoRegionAreas.Fields) && this.Fields.Count > 0)
                {
                    result.Members[regionMemberIndex].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Data Members", result.Name)));
                    result.Members[result.Members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                }
                regionMemberIndex = result.Members.Count;
                result.Members.AddRange(this.Methods.GenerateCodeDom(options));
                if (options.AllowRegions && options.AutoRegionsFor(AutoRegionAreas.Methods) && this.Methods.Count > 0)
                {
                    result.Members[regionMemberIndex].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Methods", result.Name)));
                    result.Members[result.Members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                }
                regionMemberIndex = result.Members.Count;
                result.Members.AddRange(this.Properties.GenerateCodeDom(options));
                if (options.AllowRegions && options.AutoRegionsFor(AutoRegionAreas.Properties) && this.Properties.Count > 0)
                {
                    result.Members[regionMemberIndex].StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, string.Format("{0} Properties", result.Name)));
                    result.Members[result.Members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
                }
            }
            else
                return null;

            return result;
        }

        #region ITypeMemberParent Members

        public IDeclarationResources Resources
        {
            get 
            {
                if (this.resources == null)
                    this.resources = this.InitializeResources();
                return this.resources;
            }
        }

        #endregion

        #region IMemberParentType Members


        public IThisReferenceExpression GetThisExpression()
        {
            //Pointless, but typical?
            return new ThisReferenceExpression();
        }

        public ITypeReferenceExpression GetTypeExpression()
        {
            return new TypeReferenceExpression(this.GetTypeReference());
        }

        #endregion


        #region Initialization Members
        protected virtual IFieldMembers InitializeFields()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().Fields.GetPartialClone(this);
            return new FieldMembers(this);
        }


        protected virtual IMethodMembers InitializeMethods()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().Methods.GetPartialClone(this);
            return new MethodMembers(this);
        }

        protected virtual IDeclarationResources InitializeResources()
        {
            if (this.IsPartial)
                return (IDeclarationResources)this.GetRootDeclaration().Resources.Partials.AddNew(this);
            return new DeclarationResources(this);
        }

        protected virtual IEnumeratorTypes InitializeEnumerators()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().Enumerators.GetPartialClone(this);
            return new EnumeratorTypes(this);
        }

        protected virtual IInterfaceTypes InitializeInterfaces()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().Interfaces.GetPartialClone(this);
            return new InterfaceTypes(this);
        }


        protected virtual IClassTypes InitializeClasses()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().Classes.GetPartialClone(this);
            return new ClassTypes(this);
        }

        protected virtual IDelegateTypes InitializeDelegates()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().Delegates.GetPartialClone(this);
            return new DelegateTypes(this);
        }

        protected virtual IStructTypes InitializeStructures()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().Structures.GetPartialClone(this);
            return new StructTypes(this);
        }
        private IPropertyMembers InitializeProperties()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().Properties.GetPartialClone(this);
            return new PropertyMembers(this);
        }

        protected virtual IConstructorMembers InitializeConstructors()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().Constructors.GetPartialClone(this);
            return new ConstructorMembers(this);
        }

        protected virtual IExpressionCoercionMembers InitializeCoercions()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().Coercions.GetPartialClone(this);
            return new ExpressionCoercionMembers(this);
        }

        #endregion

        #region IMemberParentType Members

        public IConstructorMembers Constructors
        {
            get
            {
                if (this.constructors == null)
                    this.constructors = InitializeConstructors();
                return this.constructors;
            }
        }

        public IExpressionCoercionMembers Coercions
        {
            get
            {
                if (this.coercions == null)
                    this.coercions = InitializeCoercions();
                return this.coercions;
            }
        }

        #endregion



        #region IMemberParentType Members


        public ISnippetMembers SnippetMembers
        {
            get {
                if (this.snippetMembers == null)
                    this.snippetMembers = InitializeSnippetMembers();
                return this.snippetMembers;
            }
        }

        private ISnippetMembers InitializeSnippetMembers()
        {
            return new SnippetMembers(this);
        }

        #endregion

        public override void Dispose()
        {
            if (this.classes != null)
                this.classes.Dispose();
            if (this.delegates != null)
                this.delegates.Dispose();
            if (this.enumerators != null)
                this.enumerators.Dispose();
            if (this.interfaces != null)
                this.interfaces.Dispose();
            if (this.structures != null)
                this.structures.Dispose();
            if (this.snippetMembers != null)
                this.snippetMembers.Dispose();
            if (this.properties != null)
                this.properties.Dispose();
            if (this.methods != null)
                this.methods.Dispose();
            if (this.fields != null)
                this.fields.Dispose();
            if (this.constructors != null)
                this.constructors.Dispose();
            this.classes = null;
            this.delegates = null;
            this.enumerators = null;
            this.interfaces = null;
            this.structures = null;
            if (this.resources != null)
            {
                this.resources.Dispose();
                this.resources = null;
            }
            this.fields = null;
            this.constructors = null;
            base.Dispose();
        }

        /// <summary>
        /// Performs a look-up on the <see cref="SegmentableParameteredMemberTypeParentType{TItem, TDom, TPartials}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="SegmentableParameteredMemberTypeParentType{TItem, TDom, TPartials}"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (options.AllowPartials)
            {
                if (this.fields != null)
                    this.Fields.GatherTypeReferences(ref result, options);
                if (this.properties != null)
                    this.Properties.GatherTypeReferences(ref result, options);
                if (this.methods != null)
                    this.Methods.GatherTypeReferences(ref result, options);
                if (this.constructors != null)
                    this.Constructors.GatherTypeReferences(ref result, options);
                if (this.Classes != null)
                    this.Classes.GatherTypeReferences(ref result, options);
                if (this.Delegates != null)
                    this.Delegates.GatherTypeReferences(ref result, options);
                if (this.Enumerators != null)
                    this.Enumerators.GatherTypeReferences(ref result, options);
                if (this.Interfaces != null)
                    this.Interfaces.GatherTypeReferences(ref result, options);
                if (this.Structures != null)
                    this.Structures.GatherTypeReferences(ref result, options);
                if (this.resources != null)
                    this.resources.GatherTypeReferences(ref result, options);
            }
            else
            {
                if (this.fields != null && this.IsRoot)
                    this.Fields.GatherTypeReferences(ref result, options);
                if (this.properties != null && this.IsRoot)
                    this.Properties.GatherTypeReferences(ref result, options);
                if (this.methods != null && this.IsRoot)
                    this.Methods.GatherTypeReferences(ref result, options);
                if (this.constructors != null && this.IsRoot)
                    this.Constructors.GatherTypeReferences(ref result, options);
                if (this.IsRoot)
                {
                    if (this.Classes != null)
                        this.Classes.GatherTypeReferences(ref result, options);
                    if (this.Delegates != null)
                        this.Delegates.GatherTypeReferences(ref result, options);
                    if (this.Enumerators != null)
                        this.Enumerators.GatherTypeReferences(ref result, options);
                    if (this.Interfaces != null)
                        this.Interfaces.GatherTypeReferences(ref result, options);
                    if (this.Structures != null)
                        this.Structures.GatherTypeReferences(ref result, options);
                    if (this.resources != null)
                        this.resources.GatherTypeReferences(ref result, options);
                }
            }
        }

        #region ITypeParent Members

        public int GetTypeCount()
        {
            return GetTypeCount(true);
        }

        #endregion

        #region IMemberParentType Members

        public int GetMemberCount()
        {
            return this.GetMemberCount(true);
        }

        #endregion


        public override int GetTypeCount(bool includePartials)
        {
            int result = 0;
            if (includePartials)
            {
                if (this.classes != null || this.IsPartial)
                    result += this.Classes.Count;
                if (this.delegates != null || this.IsPartial)
                    result += this.Delegates.Count;
                if (this.enumerators != null || this.IsPartial)
                    result += this.Enumerators.Count;
                if (this.interfaces != null || this.IsPartial)
                    result += this.Interfaces.Count;
                if (this.structures != null || this.IsPartial)
                    result += this.Structures.Count;
            }
            else
            {
                if (this.classes != null || (this.IsPartial && this.Classes != null))
                    result += this.Classes.GetCountForTarget(this);
                if (this.delegates != null || (this.IsPartial && this.Delegates != null))
                    result += this.Delegates.GetCountForTarget(this);
                if (this.enumerators != null || (this.IsPartial && this.Enumerators != null))
                    result += this.Enumerators.GetCountForTarget(this);
                if (this.interfaces != null || (this.IsPartial && this.Interfaces != null))
                    result += this.Interfaces.GetCountForTarget(this);
                if (this.structures != null || (this.IsPartial && this.Structures != null))
                    result += this.Structures.GetCountForTarget(this);
            }
            return result;
        }

        public override int GetMemberCount(bool includePartials)
        {
            int result = 0;
            if (includePartials)
            {
                if (this.fields != null || this.IsPartial)
                    result += this.Fields.Count;
                if (this.methods != null || this.IsPartial)
                    result += this.Methods.Count;
                if (this.properties != null || this.IsPartial)
                    result += this.Properties.Count;
                if (this.constructors != null || this.IsPartial)
                    result += this.Constructors.Count;
            }
            else
            {
                if (this.fields != null || this.IsPartial)
                    result += this.Fields.GetCountForTarget(this);
                if (this.methods != null || this.IsPartial)
                    result += this.Methods.GetCountForTarget(this);
                if (this.properties != null || this.IsPartial)
                    result += this.Properties.GetCountForTarget(this);
                if (this.constructors != null || this.IsPartial)
                    result += this.Constructors.GetCountForTarget(this);
            }
            return result;
        }
    }
}
