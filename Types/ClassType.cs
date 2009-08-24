using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using System.Collections;
using System.Reflection;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// Provides a basic implementation of <see cref="IClassType"/> which 
    /// is a class-type generator of code dom.
    /// </summary>
    [Serializable]
    public class ClassType :
        SegmentableParameteredMemberTypeParentType<IClassType, CodeTypeDeclaration, IClassPartials>,
        IClassType
    {

        #region ClassType Data members

        /// <summary>
        /// Data member for <see cref="BaseType"/>.
        /// </summary>
        private ITypeReference baseType;

        /// <summary>
        /// Data member for <see cref="IsStatic"/>.
        /// </summary>
        private bool isStatic;
        private bool isAbstract;
        private bool isSealed;
        ITypeReferenceCollection implementsList;

        #endregion

        #region ClassType Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ClassType"/> with the <paramref name="name"/>
        /// and <paramref name="parentTarget"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="ClassType"/>.</param>
        /// <param name="parentTarget">The <see cref="ITypeParent"/> which contains the <see cref="ClassType"/>.</param>
        protected internal ClassType(string name, ITypeParent parentTarget)
            : base(name, parentTarget)
        {
            if (parentTarget is IType)
                this.AccessLevel = DeclarationAccessLevel.Private;
            else
                this.AccessLevel = DeclarationAccessLevel.Public;
        }

        protected internal ClassType(IClassType basePartial, ITypeParent parentTarget)
            : base(basePartial,parentTarget)
        {
        }

        #endregion

        #region Declaration<TParent, TDom> Members
        /// <summary>
        /// Generates the <see cref="CodeTypeDeclaration"/> that represents the <see cref="ClassType"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="CodeTypeDeclaration"/> if successful.-null- otherwise.</returns>
        public override CodeTypeDeclaration GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            CodeTypeDeclaration result = base.GenerateCodeDom(options);
            if (result == null)
            {
                if (options.BuildTrail != null)
                    options.BuildTrail.Pop();
                return result;
            }
            if (this.IsStatic)
            {
                result.TypeAttributes |= TypeAttributes.Abstract;// | TypeAttributes.Sealed;
                result.Attributes |= MemberAttributes.Static | MemberAttributes.Final;
            }
            result.Attributes |= AccessLevelAttributes(this.AccessLevel);
            if (this.BaseType != null && (!(this.BaseType.Equals(typeof(object).GetTypeReference()))))
            {
                result.BaseTypes.Add(this.BaseType.GenerateCodeDom(options));
            }
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
                if (duplicate[i])
                    if (autoResolve)
                        options.AutoResolveReferences = false;
                result.BaseTypes.Add(typeRef.GenerateCodeDom(options));
                if (duplicate[i] && autoResolve)
                    options.AutoResolveReferences = autoResolve;
            }
            result.IsClass = true;
            if (options.AllowRegions && options.AutoRegionsFor(AutoRegionAreas.Class))
            {
                result.StartDirectives.Add(new CodeRegionDirective(CodeRegionMode.Start, String.Format("Begin {0}", result.Name)));
                result.EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, ""));
            }
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            return result;
        }
        #endregion

        #region IClassType Members
        public virtual ITypeReferenceCollection ImplementsList
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

        /// <summary>
        /// The base-type that the <see cref="ClassType"/> derives from.
        /// </summary>
        /// <remarks>Can be either internal or external.</remarks>
        public virtual ITypeReference BaseType
        {
            get 
            {
                if (this.baseType == null)
                    this.baseType = typeof(object).GetTypeReference();
                return this.baseType; 
            }
            set
            {
                ITypeReference val = value;
                if (val.TypeInstance is Members.ITypeParameterMember)
                    throw new ArgumentException("The base of a class cannot be a type-parameter.", "value");
                else if (val.TypeInstance is IExternType)
                {
                    if (!((IExternType)val.TypeInstance).Type.IsClass)
                        throw new ArgumentException("The base of a class must be another class.", "value");
                    this.baseType = val;
                }
                else if (val.TypeInstance is IClassType)
                    this.baseType = val;
                else
                    throw new ArgumentException("The base of a class must be another class.", "value");
            }
        }
        /// <summary>
        /// Returns/sets whether the <see cref="ClassType"/> and its members are static.
        /// </summary>
        public virtual bool IsStatic
        {
            get
            {
                if (this.IsRoot)
                    return this.isStatic;
                else
                    return this.GetRootDeclaration().IsStatic;
            }
            set
            {
                if (this.IsRoot)
                {
                    this.isStatic = value;
                    if (value)
                    {
                        if (this.isSealed)
                            this.isSealed = false;
                        if (this.isAbstract)
                            this.isAbstract = false;
                    }
                }
                else
                    this.GetRootDeclaration().IsStatic = value;
            }
        }

        #endregion

        public new IClassType GetRootDeclaration()
        {
            return (IClassType)base.GetRootDeclaration();
        }


        #region ISegmentableDeclaredType<IClassType,CodeTypeDeclaration> Members


        ISegmentableDeclaredTypePartials<IClassType, CodeTypeDeclaration> ISegmentableDeclaredType<IClassType,CodeTypeDeclaration>.Partials
        {
            get { return this.Partials; }
        }

        #endregion

        protected override IClassPartials InitializePartials()
        {
            return new ClassPartials(this);
        }

        public override void Dispose()
        {
            this.baseType = null;
            if (this.implementsList != null)
            {
                if (this.IsRoot)
                    this.implementsList.Clear();
                this.implementsList = null;
            }
            base.Dispose();
        }

        #region IClassType Members


        public virtual bool IsAbstract
        {
            get
            {
                if (this.IsRoot)
                    return this.isAbstract;
                else
                    return this.GetRootDeclaration().IsAbstract;
            }
            set
            {
                if (this.IsRoot)
                {
                    this.isAbstract = value;
                    if (value)
                    {
                        if (this.isStatic)
                            this.isStatic = false;
                        if (this.isSealed)
                            this.isSealed = false;
                    }
                }
                else
                    this.GetRootDeclaration().IsAbstract = value;
            }
        }

        public virtual bool IsSealed
        {
            get
            {
                if (this.IsRoot)
                    return this.isSealed;
                else
                    return this.GetRootDeclaration().IsSealed;
            }
            set
            {
                if (this.IsRoot)
                {
                    this.isSealed = value;
                    if (value)
                    {
                        if (this.isAbstract)
                            this.isAbstract = false;
                        if (this.isStatic)
                            this.isStatic = false;
                    }
                }
                else
                    this.GetRootDeclaration().IsSealed = value;
            }
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="ClassType"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="ClassType"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.IsRoot && (this.BaseType != null) && (!(this.BaseType.Equals(typeof(object).GetTypeReference()))))
                result.Add(this.BaseType);
            if (this.implementsList != null)
                foreach (ITypeReference itr in this.ImplementsList)
                        result.Add(itr);
        }

    }
}
