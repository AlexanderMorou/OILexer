using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Oilexer.Translation;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Utilities.Arrays;

namespace Oilexer
{
    /// <summary>
    /// An intermediate project which contains members spanned across a series of instances.
    /// </summary>
    public class IntermediateProject :
        IIntermediateProject,
        IAttributeDeclarationTarget
    {
        #region IntermediateProject data members.
        /// <summary>
        /// Data member for <see cref="Classes"/>.
        /// </summary>
        private IClassTypes classes;
        /// <summary>
        /// Data member for <see cref="Delegates"/>.
        /// </summary>
        private IDelegateTypes delegates;
        /// <summary>
        /// Data member for <see cref="Interfaces"/>.
        /// </summary>
        private IInterfaceTypes interfaces;
        /// <summary>
        /// Data member for <see cref="Structures"/>.
        /// </summary>
        private IStructTypes structures;
        /// <summary>
        /// Data member for <see cref="Enumerators"/>.
        /// </summary>
        private IEnumeratorTypes enumerators;

        /// <summary>
        /// Data member for <see cref="EntryPoint"/>.
        /// </summary>
        private IMethodMember entryPoint;

        /// <summary>
        /// Data member containing the root declaration.
        /// </summary>
        private IIntermediateProject baseDeclaration;
        /// <summary>
        /// Data member for <see cref="DefaultNameSpace"/>.
        /// </summary>
        private INameSpaceDeclaration defaultNameSpace = null;
        /// <summary>
        /// Data member for <see cref="Name"/>.
        /// </summary>
        private string name;
        /// <summary>
        /// Data member for <see cref="NameSpaces"/>.
        /// </summary>
        private INameSpaceDeclarations nameSpaces;
        /// <summary>
        /// Data member denoting the specific instance which is the root instance for this
        /// <see cref="IntermediateProject"/>.
        /// </summary>
        IIntermediateProjectPartials partials;
        /// <summary>
        /// Data member for <see cref="Resources"/>.
        /// </summary>
        private IDeclarationResources resources = null;

        /// <summary>
        /// Data member for <see cref="CurrentDefaultModule"/>.
        /// </summary>
        private IIntermediateModule currentDefaultModule;
        /// <summary>
        /// Data member for <see cref="RootModule"/>.
        /// </summary>
        private IIntermediateModule rootModule;
        /// <summary>
        /// Data member for <see cref="OutputType"/>.
        /// </summary>
        private ProjectOutputType outputType;
        /// <summary>
        /// Data member for <see cref="Modules"/>.
        /// </summary>
        private IIntermediateModules modules;

        /// <summary>
        /// Data member for <see cref="AssemblyInformation"/>.
        /// </summary>
        private IIntermediateProjectInformation assemblyInformation;

        /// <summary>
        /// Data member for <see cref="Attributes"/>.
        /// </summary>
        private IAttributeDeclarations attributes;

        #endregion

        #region IntermediateProject Constructors

        /// <summary>
        /// Creates a new <see cref="IntermediateProject"/> instance with the
        /// <paramref name="name"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="IntermediateProject"/>.</param>
        /// <param name="defaultNameSpace">The name of the default namespace.</param>
        public IntermediateProject(string name, string defaultNameSpace)
        {
            if ((defaultNameSpace == null) || (defaultNameSpace == string.Empty))
                throw new ArgumentNullException("defaultNameSpace");
            this.name = name;
            this.defaultNameSpace = this.NameSpaces.AddNew(defaultNameSpace);
        }

        internal IntermediateProject(IIntermediateProject baseDeclaration)
        {
            this.baseDeclaration = baseDeclaration;
        }

        #endregion

        #region ISegmentableDeclarationTarget<IIntermediateProject> Members

        /// <summary>
        /// Returns the root <see cref="IIntermediateProject"/> which is responsible for managing
        /// the data elements of the <see cref="IntermediateProject"/>.
        /// </summary>
        /// <returns>An instance of an <see cref="IIntermediateProject"/> implementation which reflects the root instance of the 
        /// <see cref="IntermediateProject"/>.</returns>
        public IIntermediateProject GetRootDeclaration()
        {
            if (this.baseDeclaration == null)
                return this;
            return this.baseDeclaration;
        }

        ISegmentableDeclarationTargetPartials<IIntermediateProject> ISegmentableDeclarationTarget<IIntermediateProject>.Partials
        {
            get
            {
                return this.Partials;
            }
        }

        #endregion

        #region ISegmentableDeclarationTarget Members

        /// <summary>
        /// Returns whether the <see cref="IntermediateProject"/> is
        /// a segment of a series.
        /// </summary>
        public bool IsPartial
        {
            get
            {
                return this.baseDeclaration != null;
            }
        }

        /// <summary>
        /// Returns whether the <see cref="ISegmentableDeclarationTarget"/> is the root
        /// instance.
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return this.baseDeclaration == null;
            }
        }

        ISegmentableDeclarationTarget ISegmentableDeclarationTarget.GetRootDeclaration()
        {
            if (this.baseDeclaration == null)
                return this;
            return this.baseDeclaration;
        }

        ISegmentableDeclarationTargetPartials ISegmentableDeclarationTarget.Partials
        {
            get { return (ISegmentableDeclarationTargetPartials)this.Partials; }
        }

        #endregion

        #region IDeclarationTarget Members

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDeclarationTarget ParentTarget
        {
            get
            {
                return null;
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        /// <summary>
        /// Returns/sets the name of the <see cref="IntermediateProject"/>.
        /// </summary>
        public string Name
        {
            get
            {
                if (this.IsPartial)
                    return this.GetRootDeclaration().Name;
                return this.name;
            }
            set
            {
                if (this.IsPartial)
                    this.GetRootDeclaration().Name = value;
                else
                    this.name = value;
            }
        }

        public INameSpaceDeclaration GetRootNameSpace()
        {
            return null;
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// Releases any data associated with the <see cref="IntermediateProject"/>.
        /// </summary>
        public void Dispose()
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
            if (!this.IsPartial && (this.partials != null))
            {
                this.partials.Dispose();
                this.partials = null;
            }
            else
            {
                this.baseDeclaration = null;
            }
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
        }

        #endregion

        #region IIntermediateProject Members

        public IIntermediateProjectInformation AssemblyInformation
        {
            get
            {
                if (!this.IsRoot)
                    return this.GetRootDeclaration().AssemblyInformation;
                if (this.assemblyInformation == null)
                    this.assemblyInformation = this.InitializeAssemblyInformation();
                return this.assemblyInformation;
            }
        }

        private IIntermediateProjectInformation InitializeAssemblyInformation()
        {
            return new IntermediateProjectInformation(this, this.OnDeclarationChanged);
        }

        /// <summary>
        /// Returns/sets the type of assembly that is resulted from the 
        /// <see cref="IntermediateProject"/>.
        /// </summary>
        public ProjectOutputType OutputType
        {
            get
            {
                return this.outputType;
            }
            set
            {
                this.outputType = value;
            }
        }


        /// <summary>
        /// Returns/sets the current module that newly created <see cref="IDeclaredType"/> 
        /// instance implementations default to.
        /// </summary>
        /// <remarks>Applies to top-level types only.</remarks>
        public IIntermediateModule CurrentDefaultModule
        {
            get
            {
                if (this.IsPartial)
                    return this.GetRootDeclaration().CurrentDefaultModule;
                if (this.currentDefaultModule == null)
                    this.currentDefaultModule = this.Modules.Values[0];
                return this.currentDefaultModule;
            }
            set
            {
                if (this.IsPartial)
                    this.GetRootDeclaration().CurrentDefaultModule = value;
                else
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    this.currentDefaultModule = value;
                }
            }
        }

        /// <summary>
        /// Returns the series of modules that makes up the (potentially) multi-file assembly.
        /// </summary>
        public IIntermediateModules Modules
        {
            get
            {
                if (this.modules == null)
                    this.modules = this.InitializeModules();
                return this.modules;
            }
        }

        /// <summary>
        /// Returns/sets the default namespace used.
        /// </summary>
        public INameSpaceDeclaration DefaultNameSpace
        {
            get
            {
                if (this.IsPartial)
                    if (this.defaultNameSpace == null)
                        this.defaultNameSpace = this.GetRootDeclaration().DefaultNameSpace.Partials.AddNew(this);
                return this.defaultNameSpace;
            }
            set
            {
                if (this.IsPartial)
                    this.GetRootDeclaration().DefaultNameSpace = value;
                else
                    this.defaultNameSpace = value;
            }
        }
        /// <summary>
        /// Returns the partial elements of the <see cref="IntermediateProject"/>.
        /// </summary>
        public IIntermediateProjectPartials Partials
        {
            get
            {
                if (this.partials == null)
                    this.partials = InitializePartials();
                return this.partials;
            }
        }

        /// <summary>
        /// Returns the namespaces contained within the <see cref="IntermediateProject"/>.
        /// </summary>
        public INameSpaceDeclarations NameSpaces
        {
            get
            {
                if (this.nameSpaces == null)
                    this.nameSpaces = this.InitializeNameSpaces();
                return this.nameSpaces;
            }
        }

        public IMethodMember EntryPoint
        {
            get
            {
                if (this.entryPoint == null)
                    this.entryPoint = this.GetEntryPoint();
                return this.entryPoint;
            }
            set
            {
                if (!(this.ValidateEntryPoint(value)))
                    throw new ArgumentOutOfRangeException("Entrypoint is not valid.");
                this.entryPoint = value;
            }
        }

        private bool ValidateEntryPoint(IMethodMember value)
        {
            if (((IDeclaredType)value.ParentTarget).IsGeneric)
                return false;
            if (value.Name != "Main")
                return false;
            if (value.Parameters.Count < 0 || value.Parameters.Count > 1)
                return false;
            if (value.Parameters.Count == 0)
                return value.ReturnType.Equals(typeof(int).GetTypeReference()) || value.ReturnType.Equals(typeof(void).GetTypeReference());
            IMethodParameterMember impm = value.Parameters[value.Parameters.Keys[0]];
            if (impm.ParameterType.ArrayElementType == null || impm.ParameterType.ArrayRank == 0)
                return (impm.ParameterType.Equals(typeof(string).GetTypeReference())) && (value.ReturnType.Equals(typeof(int).GetTypeReference()) || value.ReturnType.Equals(typeof(void).GetTypeReference()));
            else if (impm.ParameterType.ArrayRank == 1 && impm.ParameterType.ArrayElementType != null)
                return ((impm.ParameterType.ArrayElementType.ArrayRank == 0 || impm.ParameterType.ArrayElementType.ArrayElementType == null) && impm.ParameterType.ArrayElementType.Equals(typeof(string).GetTypeReference())) && (value.ReturnType.Equals(typeof(int).GetTypeReference()) || value.ReturnType.Equals(typeof(void).GetTypeReference()));
            return false;
        }

        private IMethodMember GetEntryPoint()
        {
            foreach (IDeclaredType idt in this.CurrentDefaultModule.DeclaredTypes)
                if (idt is IClassType)
                    foreach (IMethodMember imm in ((IClassType)idt).Methods.Values)
                        if (ValidateEntryPoint(imm))
                            return imm;
            foreach (IIntermediateModule iim in this.Modules.Values)
                if (iim == this.CurrentDefaultModule)
                    continue;
                else
                    foreach (IDeclaredType idt in iim.DeclaredTypes)
                        if (idt is IClassType)
                            foreach (IMethodMember imm in ((IClassType)idt).Methods.Values)
                                if (ValidateEntryPoint(imm))
                                    return imm;
            return null;
        }

        public IIntermediateModule RootModule
        {
            get {
                if (this.rootModule == null)
                    this.rootModule = this.Modules[this.Modules.Keys[0]];
                return this.rootModule;
            }
        }

        public CodeCompileUnit GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            CodeCompileUnit ccu = new CodeCompileUnit();
            ProjectDependencyReport pdr = new ProjectDependencyReport(this, options);
            ccu.ReferencedAssemblies.AddRange(Tweaks.TranslateArray<Assembly, string>(pdr.CompiledAssemblyReferences.ToArray(), delegate(Assembly a)
            {
                return a.Location;
            }));
            ccu.Namespaces.AddRange(this.NameSpaces.GenerateCodeDom(options));
            return ccu;
        }

        #endregion

        #region IntermediateProject Initialization Members 

        
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

        /// <summary>
        /// Initializes the <see cref="Partials"/>' data member.
        /// </summary>
        /// <returns>A new <see cref="IIntermediateProjectPartials"/> instance
        /// implementation.</returns>
        protected virtual IIntermediateProjectPartials InitializePartials()
        {
            return new IntermediateProjectPartials(this);
        }

        protected virtual IIntermediateModules InitializeModules()
        {
            //There is no partial system for modules.
            if (this.IsPartial) 
                return this.GetRootDeclaration().Modules;
            //Create and return.
            return new IntermediateModules(this);
        }

        /// <summary>
        /// Initializes the <see cref="NameSpaces"/>' data member.
        /// </summary>
        /// <returns>A new <see cref="INameSpaceDeclarations"/> instance implementation.</returns>
        protected virtual INameSpaceDeclarations InitializeNameSpaces()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().NameSpaces.GetPartialClone(this);
            return new NameSpaceDeclarations(this);
        }

        /// <summary>
        /// Initializes the <see cref="IDeclarationResources"/> for the <see cref="IntermediateProject"/>.
        /// </summary>
        protected virtual IDeclarationResources InitializeResources()
        {
            INameSpaceDeclaration defaultNameSpace = FindOrGetChildOf<INameSpaceDeclaration, CodeNamespace, INameSpaceParent>(this.DefaultNameSpace, this);
            if (defaultNameSpace.ChildSpaces.ContainsKey("Properties"))
            {
                INameSpaceDeclaration insd = defaultNameSpace.ChildSpaces["Properties"];
                if (insd.ParentTarget != defaultNameSpace)
                    defaultNameSpace = insd.Partials.AddNew(defaultNameSpace);
                else
                    defaultNameSpace = insd;
            }
            else
                defaultNameSpace = defaultNameSpace.ChildSpaces.AddNew("Properties");
            if (this.IsPartial)
            {
                return (IDeclarationResources)this.GetRootDeclaration().Resources.Partials.AddNew(defaultNameSpace);
            }
            IDeclarationResources idrs = new DeclarationResources(defaultNameSpace);
            defaultNameSpace.Classes.Add(idrs);
            return idrs;
        }

        private static TChild FindOrGetChildOf<TChild, TChildDom, TParent>(TChild child, TParent parent)
            where TChild :
                class,
                IDeclaration<TParent, TChildDom>,
                ISegmentableDeclarationTarget<TChild>
            where TChildDom :
                CodeObject
            where TParent :
                class,
                IDeclarationTarget
        {
            if (child.ParentTarget == parent)
                return child;
            foreach (TChild tc in child.Partials)
                if (tc.ParentTarget == parent)
                    return tc;
            return child.Partials.AddNew(parent);
        }

        #endregion 
    
        #region ITypeReferenceable Members

        /// <summary>
        /// Performs a look-up on the <see cref="IntermediateProject"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="IntermediateProject"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (options.AllowPartials)
            {
                this.Classes.GatherTypeReferences(ref result, options);
                this.Delegates.GatherTypeReferences(ref result, options);
                this.Enumerators.GatherTypeReferences(ref result, options);
                this.Interfaces.GatherTypeReferences(ref result, options);
                this.Structures.GatherTypeReferences(ref result, options);
                this.NameSpaces.GatherTypeReferences(ref result, options);
                if (this.attributes != null)
                    if (this.Attributes.Count > 0)
                        this.Attributes.GatherTypeReferences(ref result, options);
            }
            else
            {
                if (this.IsRoot)
                {
                    this.Classes.GatherTypeReferences(ref result, options);
                    this.Delegates.GatherTypeReferences(ref result, options);
                    this.Enumerators.GatherTypeReferences(ref result, options);
                    this.Interfaces.GatherTypeReferences(ref result, options);
                    this.Structures.GatherTypeReferences(ref result, options);
                    this.NameSpaces.GatherTypeReferences(ref result, options);
                    if (this.attributes != null)
                        if (this.Attributes.Count > 0)
                            this.Attributes.GatherTypeReferences(ref result, options);
                }
            }
            if (this.resources != null)
                this.resources.GatherTypeReferences(ref result, options);
        }

        #endregion

        public override string ToString()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().ToString();
            else
                if (this.DefaultNameSpace != null)
                    return string.Format("{0}, {1}", this.name, this.defaultNameSpace);
                else
                    return string.Format("{0}, {{No Default NameSpace}}", this.name);
        }

        /// <summary>
        /// Returns whether a namespace exits and denotes which specifically it is.
        /// </summary>
        /// <param name="nameSpace">The full path of the namespace to scan for.</param>
        /// <param name="foundSpace">The reference to the namespace to yield if a match
        /// is found.</param>
        /// <returns>true if a namespace with the <see cref="INameSpaceDeclaration.FullName"/> of <paramref name="nameSpace"/>
        /// exists in the <see cref="IntermediateProject"/> scope; false, otherwise.</returns>
        public bool NameSpaceExists(string nameSpace, ref INameSpaceDeclaration foundSpace)
        {
            return NameSpaceExists(this.NameSpaces, nameSpace, ref foundSpace);
        }

        private static bool NameSpaceExists(INameSpaceDeclarations nameSpaces, string nameSpace, ref INameSpaceDeclaration foundSpace)
        {
            foreach (INameSpaceDeclaration insd in nameSpaces.Values)
            {
                bool childrenContain = false;
                if (insd.FullName == nameSpace)
                {
                    foundSpace = insd;
                    return true;
                }
                childrenContain = NameSpaceExists(insd.ChildSpaces, nameSpace, ref foundSpace);
                if (childrenContain)
                    return true;
            }
            return false;
        }

        public INameSpaceDeclaration GetLocalPartial(INameSpaceDeclaration targetSpace)
        {
            INameSpaceDeclaration searchPoint = targetSpace.GetRootDeclaration();
            INameSpaceDeclaration searchSpace;
            Stack<INameSpaceDeclaration> createTrail = new Stack<INameSpaceDeclaration>();
            for (searchSpace = searchPoint; searchSpace != null; searchSpace = (INameSpaceDeclaration)searchSpace.ParentTarget)
            {
                if (searchSpace.Project == this)
                    break;
                foreach (INameSpaceDeclaration insd in searchSpace.Partials)
                    if (insd.Project == this)
                    {
                        searchSpace = insd;
                        break;
                    }
                createTrail.Push(searchSpace);
                if (!(searchSpace.ParentTarget is INameSpaceDeclaration))
                {
                    searchSpace = null;
                    break;
                }
            }
            while (createTrail.Count > 0)
                if (searchSpace == null)
                    searchSpace = createTrail.Pop().Partials.AddNew(this);
                else
                    searchSpace = createTrail.Pop().Partials.AddNew(searchSpace);
            return searchSpace;
        }

        public INameSpaceDeclaration GetClosestMatchingSpace(string nameSpace)
        {
            List<string> parts = new List<string>(nameSpace.Split('.'));

            //Scan through until there are no more parts.
            while (parts.Count > 0)
            {
                string currentSpace = string.Join(".", parts.ToArray());
                /* *
                 * If the current full namespace matches something, then the closest
                 * match has been found.  Because the process started at the largest
                 * and worked to the smallest.
                 * */
                INameSpaceDeclaration currentSpaceItem = null;
                if (NameSpaceExists(currentSpace, ref currentSpaceItem))
                    return currentSpaceItem;
                parts.RemoveAt(parts.Count - 1);
            }
            //No result.
            return null;
        }

        public INameSpaceDeclaration BuildOrMatchNameSpace(string nameSpace)
        {
            INameSpaceDeclaration insd = GetClosestMatchingSpace(nameSpace);
            //There was no match even close.
            if (insd == null)
                //Yield a new entry alltogether.
                return this.NameSpaces.AddNew(nameSpace);
            /* *
             * Yield a new namespace entity, while keeping the member names and 
             * namespace states.
             * */
            insd = insd.Partials.AddNew();
            if (insd.FullName == nameSpace)
                return insd;
            else
            {
                //Next truncate everything before the current location.
                //Building the path is next.
                string remainingPath = nameSpace.Substring(insd.FullName.Length + 1);
                insd = insd.ChildSpaces.AddNew(remainingPath);
                return insd;
            }
        }

        #region IResourceable Members

        public virtual IDeclarationResources Resources
        {
            get {
                if (this.resources == null)
                    this.resources = this.InitializeResources();
                return this.resources;
            }
        }

        #endregion

        #region ITypeParent Members

        public IClassTypes Classes
        {
            get
            {
                if (this.classes == null)
                    this.classes = this.InitializeClasses();
                return this.classes;
            }
        }

        public IDelegateTypes Delegates
        {
            get
            {
                if (this.delegates == null)
                    this.delegates = this.InitializeDelegates();
                return this.delegates;
            }
        }

        public IEnumeratorTypes Enumerators
        {
            get
            {
                if (this.enumerators == null)
                    this.enumerators = this.InitializeEnumerators();
                return this.enumerators;
            }
        }

        public IInterfaceTypes Interfaces
        {
            get
            {
                if (this.interfaces == null)
                    this.interfaces = this.InitializeInterfaces();
                return this.interfaces;
            }
        }

        public IStructTypes Structures
        {
            get
            {
                if (this.structures == null)
                    this.structures = this.InitializeStructures();
                return this.structures;
            }
        }

        public int GetTypeCount(bool includePartials)
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

        public int GetTypeCount()
        {
            int result = 0;
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
            return result;
        }

        #endregion

        #region IDeclaration Members

        public string GetUniqueIdentifier()
        {
            return this.Name;
        }

        public DeclarationAccessLevel AccessLevel
        {
            get
            {
                return DeclarationAccessLevel.Public;
            }
            set
            {
                throw new NotSupportedException("Assemblies/projects are always public.");
            }
        }

        public event EventHandler<DeclarationChangeArgs> DeclarationChange;

        CodeObject IDeclaration.GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return this.GenerateCodeDom(options);
        }

        #endregion

        #region INameSpaceParent Members

        public int GetNameSpaceCount(bool includePartials)
        {
            if (includePartials)
                return this.NameSpaces.Count;
            else
                return this.NameSpaces.GetCountForTarget(this);
        }

        #endregion


        #region IAttributeDeclarationTarget Members

        /// <summary>
        /// Returns the <see cref="IAttributeDeclarations"/> defined on the <see cref="IntermediateProject"/>.
        /// </summary>
        public IAttributeDeclarations Attributes
        {
            get
            {
                if (this.attributes == null)
                    this.attributes = InitializeAttributes();
                return this.attributes;
            }
        }

        private static IAttributeDeclarations InitializeAttributes()
        {
            return new AttributeDeclarations();
        }

        #endregion

        protected virtual void OnDeclarationChanged()
        {
            if (this.DeclarationChange != null)
                this.DeclarationChange(this, new DeclarationChangeArgs(this));
        }
    }
}
