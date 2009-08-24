using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Oilexer.Expression;
using Oilexer.Types.Members;
using System.Diagnostics;
using System.Reflection;
using Oilexer.Utilities.Collections;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// Provides a generically delared type that yields <typeparamref name="TDom"/> instances.
    /// </summary>
    /// <typeparam name="TDom">The <see cref="CodeTypeDeclaration"/> which is yielded
    /// by the <see cref="DeclaredType{TDom}"/></typeparam>
    [Serializable]
    public abstract partial class DeclaredType<TDom> :
        Declaration<ITypeParent, TDom>,
        IDeclaredType<TDom>,
        IType
        where TDom :
            CodeTypeDeclaration,
            new()
    {
        private IAttributeDeclarations attributes = null;
        private IIntermediateModule module;

        #region DeclaredType constructors

        /// <summary>
        /// Creates a new instance of <see cref="DeclaredType{TDom}"/> with the <paramref name="name"/> and
        /// <paramref name="parentTarget"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="DeclaredType{TDom}"/>.</param>
        /// <param name="parentTarget">The <see cref="ITypeParent"/> which contains the <see cref="DeclaredType{TDom}"/>.</param>
        public DeclaredType(string name, ITypeParent parentTarget)
            : base(name, parentTarget)
        {
        }
        public DeclaredType(ITypeParent parentTarget)
            : base(parentTarget)
        {
        }

        /// <summary>
        /// If the <see cref="Declaration{TDom, TParent}.ParentTarget"/> is an <see cref="INameSpaceDeclaration"/> then
        /// the <see cref="Module"/> is set to the 
        /// <see cref="IIntermediateProject.CurrentDefaultModule"/>.
        /// </summary>
        protected virtual void CheckModule()
        {
            if (this.ParentTarget is INameSpaceDeclaration || this.ParentTarget is IIntermediateProject)
            {
                IIntermediateProject iip = null;
                if (this.ParentTarget is INameSpaceDeclaration)
                    iip = ((INameSpaceDeclaration)(this.ParentTarget)).Project;
                else
                    iip = ((IIntermediateProject)this.ParentTarget);
                if (iip != null)
                    this.module = iip.CurrentDefaultModule;
            }
            else if (this.parentTarget is IDeclaredType)
                this.module = ((IDeclaredType)(this.parentTarget)).Module;
        }
        #endregion

        #region IType Members
        /// <summary>
        /// Returns the <see cref="CodeTypeReference"/> the <see cref="DeclaredType{TDom}"/>
        /// represents.
        /// </summary>
        ITypeReference IType.GetTypeReference()
        {
            return this.GetTypeReference();
        }
        ITypeReference IType.GetTypeReference(ITypeReferenceCollection typeParameters)
        {
            return this.GetTypeReference(typeParameters);
        }

        ITypeReference IType.GetTypeReference(params ITypeReference[] typeReferences)
        {
            return this.GetTypeReference(new TypeReferenceCollection(typeReferences));
        }

        ITypeReference IType.GetTypeReference(params IType[] typeReferences)
        {
            return this.GetTypeReference(new TypeReferenceCollection(typeReferences));
        }

        ITypeReference IType.GetTypeReference(params object[] typeReferences)
        {
            return this.GetTypeReference(new TypeReferenceCollection(typeReferences));
        }

        /// <summary>
        /// Returns whether the <see cref="DeclaredType{TDom}"/> is a class.
        /// </summary>
        public bool IsClass
        {
            get { return this is IClassType; }
        }

        /// <summary>
        /// Returns whether the <see cref="DeclaredType{TDom}"/> is a delegate.
        /// </summary>
        public bool IsDelegate
        {
            get { return this is IDelegateType; }
        }

        /// <summary>
        /// Returns whether the <see cref="DeclaredType{TDom}"/> is an interface.
        /// </summary>
        public bool IsInterface
        {
            get { return this is IInterfaceType; }
        }

        /// <summary>
        /// Returns whether the <see cref="DeclaredType{TDom}"/> is an enumerator.
        /// </summary>
        public bool IsEnumerator
        {
            get { return this is IEnumeratorType; }
        }

        /// <summary>
        /// Returns whether the <see cref="DeclaredType{TDom}"/> is a structure.
        /// </summary>
        public bool IsStructure
        {
            get { return this is IStructType; }
        }

        /// <summary>
        /// Returns/sets whether the <see cref="DeclaredType{TDom}"/> is a generic type.
        /// </summary>
        /// <returns>True, if the <see cref="DeclaredType{TDom}"/> is a generic, false otherwise.</returns>
        /// <remarks>If it is a generic and the value is changed to false, the generic
        /// type-parameters are cleared.</remarks>
        public abstract bool IsGeneric { get; }

        /// <summary>
        /// Returns whether the <see cref="IType"/> is in generic form when <see cref="IsGeneric"/> is true.
        /// </summary>
        public abstract bool IsGenericTypeDefinition { get; }

        /// <summary>
        /// Returns the type name of the <see cref="IType"/>.
        /// </summary>
        /// <param name="options">The code-dom generator options that direct the 
        /// generation process.</param>
        /// <returns>A <see cref="System.String"/> which is a qualified name relative to the <see cref="IType"/>.</returns>
        public string GetTypeName(ICodeTranslationOptions options)
        {
            return this.GetTypeName(options, false, this.FillInTypeParameters().ToArray());
        }

        /// <summary>
        /// Returns the type name of the <see cref="IType"/>.
        /// </summary>
        /// <param name="options">The code-dom generator options that direct the 
        /// generation process.</param>
        /// <param name="commentStyle">Whether or not the type name is represented in 
        /// comment style with the type-parameters expanded and encased in curly 
        /// braces ('{' and '}').</param>
        /// <returns>A <see cref="System.String"/> which is a qualified name relative to the <see cref="IType"/>.</returns>
        public string GetTypeName(
                        ICodeTranslationOptions 

                options, bool commentStyle)
        {
            return this.GetTypeName(options, commentStyle, this.FillInTypeParameters().ToArray());
        }

        /// <summary>
        /// Returns the type name of the <see cref="IType"/>.
        /// </summary>
        /// <param name="options">The code-dom generator options that direct the 
        /// generation process.</param>
        /// <param name="typeParameterValues">The series of <see cref="ITypeReference"/> instance
        /// implementations which relate to the generic-parameters of the <see cref="IType"/>.</param>
        /// <returns>A <see cref="System.String"/> which is a qualified name relative to the <see cref="IType"/>.</returns>
        public string GetTypeName(
                        ICodeTranslationOptions 

                options, ITypeReference[] typeParameterValues)
        {
            return this.GetTypeName(options, false, typeParameterValues);
        }

        /// <summary>
        /// Returns the type name of the <see cref="IType"/>.
        /// </summary>
        /// <param name="options">The code-dom generator options that direct the 
        /// generation process.</param>
        /// <param name="commentStyle">Whether or not the type name is represented in 
        /// comment style with the type-parameters expanded and encased in curly 
        /// braces ('{' and '}').</param>
        /// <param name="typeParameterValues">The series of <see cref="ITypeReference"/> instance
        /// implementations which relate to the generic-parameters of the <see cref="IType"/>.</param>
        /// <returns>A <see cref="System.String"/> which is a qualified name relative to the <see cref="IType"/>.</returns>
        public string GetTypeName(ICodeTranslationOptions options, bool commentStyle, ITypeReference[] typeParameterValues)
        {
            INameSpaceDeclaration nsdType = GetNamespace();
            if (nsdType == null)
                return this.Name;
            IList<IDeclarationTarget> hierarchy = GetDeclarationHierarchy(this);
            string[] typeArgNames = new string[0];
            string tArgNames = "";
            int tpChainArgs = 0;
            string[] names = new string[hierarchy.Count];
            for (int i = 0; i < hierarchy.Count; i++)
            {
                string currentName = null;
                if (options.NameHandler.HandlesName((IDeclaration)hierarchy[i]))
                    currentName = options.NameHandler.HandleName((IDeclaration)hierarchy[i]);
                else
                    currentName = hierarchy[i].Name;
                if (hierarchy[i] is IParameteredDeclaredType && ((IParameteredDeclaredType)hierarchy[i]).TypeParameters != null && ((IParameteredDeclaredType)hierarchy[i]).TypeParameters.Count > 0)
                {
                    IParameteredDeclaredType pdt = (IParameteredDeclaredType)hierarchy[i];
                    //string[] tpNam = new string[pdt.TypeParameters.Count];
                    if (!commentStyle)
                        currentName += string.Format("`{0}", pdt.TypeParameters.Count);
                    else
                    {
                        typeArgNames = new string[pdt.TypeParameters.Count];
                        for (int tpArgIndex = 0; tpArgIndex < pdt.TypeParameters.Count; tpArgIndex++)
                            typeArgNames[tpArgIndex] = ((ITypeParameterMember)pdt.TypeParameters.Values[tpArgIndex]).Name;
                        currentName += string.Format("{{{0}}}", String.Join(", ", typeArgNames));
                    }
                    if (!commentStyle)
                        tpChainArgs += pdt.TypeParameters.Count;
                }
                names[i] = currentName;
            }
            if ((!(commentStyle)) && (tpChainArgs == typeParameterValues.Length && tpChainArgs > 0))
            {
                typeArgNames = new string[tpChainArgs];
                for (int tpArgIndex = 0; tpArgIndex < typeParameterValues.Length; tpArgIndex++)
                {
                    if (((typeParameterValues[tpArgIndex].ResolutionOptions & TypeReferenceResolveOptions.FullType) == TypeReferenceResolveOptions.FullType) || ((typeParameterValues[tpArgIndex].ResolutionOptions & TypeReferenceResolveOptions.GlobalType) == TypeReferenceResolveOptions.GlobalType))
                    {
                        bool autoResolve = options.AutoResolveReferences;
                        if (autoResolve)
                            options.AutoResolveReferences = false;
                        typeArgNames[tpArgIndex] = typeParameterValues[tpArgIndex].TypeInstance.GetTypeName(options, typeParameterValues[tpArgIndex].TypeParameters.ToArray());
                        if (autoResolve)
                            options.AutoResolveReferences = autoResolve;

                    }
                    else
                        typeArgNames[tpArgIndex] = typeParameterValues[tpArgIndex].TypeInstance.GetTypeName(options, typeParameterValues[tpArgIndex].TypeParameters.ToArray());
                    typeArgNames[tpArgIndex] = string.Format("[{0}]", typeArgNames[tpArgIndex]);
                }
                tArgNames = String.Format("[{0}]", String.Join(",", typeArgNames));
            }

            if (options.CurrentNameSpace != null)
            {
                //The namespace the generator is 'in' contains the this's namespace.
                //Thus checking the imports list is un-necessary.
                if (options.CurrentNameSpace.FullName.Contains(String.Format("{0}.", nsdType.FullName)))
                    return String.Join("+", names);
                if (options.AutoResolveReferences)
                {
                    if (!(options.ImportList.Contains(nsdType.Name)))
                        options.ImportList.Add(nsdType.FullName);
                    if (commentStyle)
                        return String.Format("{0}{1}", String.Join(".", names), tArgNames);
                    return String.Format("{0}{1}", String.Join("+", names), tArgNames);
                }
            }
            if (options.ImportList.Contains(nsdType.FullName) && options.AutoResolveReferences)
                if (commentStyle)
                    return String.Join(".", names);
                else
                    return String.Format("{0}{1}", String.Join("+", names), tArgNames);
            if (commentStyle)
                return string.Format("{0}.{1}", nsdType.FullName, String.Join(".", names));
            return string.Format("{0}.{1}{2}", nsdType.FullName, String.Join("+", names), tArgNames);
        }


        #endregion


        #region IDeclaredType<TDom> Members

        /// <summary>
        /// Returns the <see cref="IAttributeDeclarations"/> defined on the <see cref="DeclaredType{TDom}"/>.
        /// </summary>
        public virtual IAttributeDeclarations Attributes
        {
            get {
                if (this.attributes == null)
                    this.InitializeAttributes();
                return this.attributes;
            }
        }

        private void InitializeAttributes()
        {
            this.attributes = new AttributeDeclarations();
        }

        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the <see cref="DeclaredType{TDom}"/>.
        /// </summary>
        public IDeclaredTypeReference<TDom> GetTypeReference()
        {
            return this.GetTypeReference(new TypeReferenceCollection());
        }

        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the generic
        /// type.
        /// </summary>
        /// <param name="typeParameters">The <see cref="ITypeReferenceCollection"/> that relates to the
        /// type-parameters of the <see cref="IDeclaredType{TDom}"/>.</param>
        public virtual IDeclaredTypeReference<TDom> GetTypeReference(ITypeReferenceCollection typeParameters)
        {
            return new Reference(this, FillInTypeParameters(typeParameters));
        }

        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        /// <param name="typeParameters">The array of <see cref="ITypeReference"/> elements
        /// that relates to the type-parameters of the <see cref="IDeclaredType{TDom}"/>.</param>
        public IDeclaredTypeReference<TDom> GetTypeReference(params ITypeReference[] typeReferences)
        {
            return this.GetTypeReference(new TypeReferenceCollection(typeReferences));
        }

        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        /// <param name="typeParameters">The array of <see cref="IType"/> elements
        /// that relates to the type-parameters of the <see cref="IDeclaredType{TDom}"/>.</param>
        public IDeclaredTypeReference<TDom> GetTypeReference(params IType[] typeReferences)
        {
            return this.GetTypeReference(new TypeReferenceCollection(typeReferences));
        }

        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        /// <param name="typeParameters">The array of <see cref="object"/> elements
        /// that relates to the type-parameters of the <see cref="IDeclaredType{TDom}"/>.</param>
        public IDeclaredTypeReference<TDom> GetTypeReference(ICollection<ITypeReference> typeReferences)
        {
            ITypeReference[] trc= new ITypeReference[typeReferences.Count];
            int i = 0;
            foreach (ITypeReference itr in typeReferences)
                trc[i] = itr;
            return this.GetTypeReference(trc);
        }

        /// <summary>
        /// Returns the <see cref="IDeclaredTypeReference{T}"/> that refers to the type given the types
        /// for the type-parameters.
        /// </summary>
        /// <param name="typeReferences">The array of <see cref="object"/> elements
        /// that relates to the type-parameters of the <see cref="IDeclaredType{TDom}"/>.</param>
        public IDeclaredTypeReference<TDom> GetTypeReference(params object[] typeReferences)
        {
            return this.GetTypeReference(new TypeReferenceCollection(typeReferences));
        }

        #endregion

        #region Utility Members
        protected virtual ITypeReferenceCollection FillInTypeParameters()
        {
            return this.FillInTypeParameters(null);
        }
        protected virtual ITypeReferenceCollection FillInTypeParameters(ITypeReferenceCollection originalSet)
        {
            List<IDeclarationTarget> parents = new List<IDeclarationTarget>();
            ITypeReferenceCollection resultSet;
            if (originalSet == null)
                resultSet = new TypeReferenceCollection();
            else
                resultSet = new TypeReferenceCollection(originalSet.ToArray());
            for (IDeclarationTarget parent = this; ((parent != null) && (!(parent is INameSpaceDeclaration))); parent = parent.ParentTarget)
                parents.Add(parent);
            parents.Reverse();
            int skip = 0;
            for (IDeclarationTarget parent = parents[0]; parents.Count > 0; parents.RemoveAt(0), parent = parents.Count > 0 ? parents[0] : null)
            {
                if (parent is IParameteredDeclaredType)
                {
                    if (((IParameteredDeclaredType)parent).TypeParameters == null)
                        continue;
                    foreach (ITypeParameterMember itpm in ((IParameteredDeclaredType)parent).TypeParameters.Values)
                        if (skip++ >= resultSet.Count)
                            resultSet.Add(itpm);
                }
            }
            return resultSet;
        }

        private static IList<IDeclarationTarget> GetDeclarationHierarchy(IDeclarationTarget source)
        {
            Stack<IDeclarationTarget> stack = new Stack<IDeclarationTarget>();
            List<IDeclarationTarget> result = new List<IDeclarationTarget>();
            IDeclarationTarget current = source;
            while (!((current is INameSpaceDeclaration) || (current == null)))
            {
                stack.Push(current);
                current = current.ParentTarget;
            }
            while (stack.Count > 0)
                result.Add(stack.Pop());
            return result;
        }
        private INameSpaceDeclaration GetNamespace()
        {
            IDeclarationTarget current = this;
            while (!((current is INameSpaceDeclaration) || (current == null)))
            {
                current = current.ParentTarget;
            }
            /* *
             * The condition of the iteration determines whether this is null.
             * If the parent item is null, or a namespace, it should be valued 
             * accordingly.
             * */
            return (INameSpaceDeclaration)current;
        }

        #endregion

        #region Override Members
        /// <summary>
        /// Generates the <typeparamref name="TDom"/> that represents the <see cref="DeclaredType{TDom}"/>.
        /// </summary>
        /// <returns>A new instance of a <typeparamref name="TDom"/> if successful.-null- otherwise.</returns>
        public override TDom GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            //No Constructor initializer, because there are no constraint with
            //constructor arguments yet.
            TDom result = new TDom();
            result.CustomAttributes = this.Attributes.GenerateCodeDom(options);
            switch (this.AccessLevel)
            {
                case DeclarationAccessLevel.Public:
                    if (this.ParentTarget is IDeclaredType)
                        result.TypeAttributes = TypeAttributes.NestedPublic;
                    else
                        result.TypeAttributes = TypeAttributes.Public;
                    break;
                case DeclarationAccessLevel.Private:
                    if (this.ParentTarget is IDeclaredType)
                        result.TypeAttributes = TypeAttributes.NestedPrivate;
                    else
                        result.TypeAttributes = TypeAttributes.NotPublic;
                    break;
                case DeclarationAccessLevel.Protected:
                    if (this.ParentTarget is IDeclaredType)
                        result.TypeAttributes = TypeAttributes.NestedFamily;
                    else
                        result.TypeAttributes = TypeAttributes.Public;
                    break;
                case DeclarationAccessLevel.ProtectedInternal:
                    if (this.ParentTarget is IDeclaredType)
                        result.TypeAttributes = TypeAttributes.NestedFamORAssem;
                    else
                        result.TypeAttributes = TypeAttributes.NotPublic;
                    break;
            }
            if (options.NameHandler.HandlesName(this))
                result.Name = options.NameHandler.HandleName(this);
            else
                result.Name = this.Name;
            return result;
        }
        #endregion

        #region IEquatable<IType> Members

        public bool Equals(IType other)
        {
            if (!(other is DeclaredType<TDom>))
                return false;
            return other == this;
        }

        #endregion

        #region IDeclaredType Members

        public virtual IIntermediateProject Project
        {
            get
            {
                if (this.ParentTarget is IIntermediateProject)
                    return (IIntermediateProject)this.ParentTarget;
                INameSpaceDeclaration insd = this.GetRootNameSpace();
                if (insd == null || (!(insd.ParentTarget is IIntermediateProject)))
                    return null;
                return ((IIntermediateProject)(insd.ParentTarget));
            }
        }

        public virtual IIntermediateModule Module
        {
            get
            {
                if (this.module == null)
                    this.CheckModule();
                return this.module;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!(this.ParentTarget is INameSpaceDeclaration || this.ParentTarget is IIntermediateProject))
                    throw new InvalidOperationException("Nested types cannot exist in a module other than their container's module.");
                if (this.module == null)
                    CheckModule();
                this.module = value;
            }
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="DeclaredType{TDom}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="DeclaredType{TDom}"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.Attributes != null)
                this.Attributes.GatherTypeReferences(ref result, options);
        }

        public override ITypeParent ParentTarget
        {
            get
            {
                return base.ParentTarget;
            }
            set
            {
                base.ParentTarget = value;
                if (this.module == null)
                    CheckModule();
            }
        }

    }
}
