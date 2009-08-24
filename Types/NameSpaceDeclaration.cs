using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer._Internal;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// Provides a basic implementation of a namespace declaration which translates to a CodeDom object.
    /// </summary>
    [Serializable]
    public class NameSpaceDeclaration :
        Declaration<INameSpaceParent, CodeNamespace>,
        INameSpaceDeclaration
    {
        #region NameSpaceDeclaration Data members
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
        /// Data member for <see cref="ChildSpaces"/>.
        /// </summary>
        private INameSpaceDeclarations childSpaces;
        /// <summary>
        /// Data member for <see cref="Paritals"/>.
        /// </summary>
        private INameSpaceDeclarationPartials partials;
        /// <summary>
        /// Data member for <see cref="GetRootDeclaration()"/>.
        /// </summary>
        private INameSpaceDeclaration basePartial;

        #endregion
        public NameSpaceDeclaration(string name)
            : this(name, null)
        {
        }

        public NameSpaceDeclaration(string name, INameSpaceParent parentTarget)
            : base(name, parentTarget)
        {
        }

        internal NameSpaceDeclaration(INameSpaceDeclaration basePartial, INameSpaceParent parentTarget)
            : base(parentTarget)
        {
            this.basePartial = basePartial;
        }

        /// <summary>
        /// Releases the resources associated with the <see cref="NameSpaceDeclaration"/>.
        /// And places the <see cref="NameSpaceDeclaration"/> in a disposed state.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (this.IsPartial)
            {
                this.basePartial = null;
            }
            else
            {
                this.partials.Dispose();
                this.partials = null;
            }
            this.Classes.Dispose();
            this.Delegates.Dispose();
            this.ChildSpaces.Dispose();
            this.Interfaces.Dispose();
            this.Enumerators.Dispose();
            this.Structures.Dispose();
            this.classes = null;
            this.enumerators = null;
            this.delegates = null;
            this.interfaces = null;
            this.structures = null;
        }

        public override string Name
        {
            get
            {
                if (this.basePartial == null)
                    return base.Name;
                else
                    return basePartial.Name;
            }
            set
            {
                if (this.IsRoot)
                    RenameAndMoveChildren(value);
                else
                    this.basePartial.Name = value;
            }
        }

        private void RenameAndMoveChildren(string value)
        {
            if (!value.Contains("."))
                base.Name = value;
            else
            {
                List<string> reverseParts = new List<string>(value.Split('.'));
                reverseParts.Reverse();
                Stack<string> nameParts = new Stack<string>(reverseParts);
                INameSpaceDeclaration insd = this;
                while (nameParts.Count > 0)
                    if (nameParts.Peek() == "")
                    {
                        nameParts.Pop();
                        continue;
                    }
                    else
                    {
                        if (insd.Name != nameParts.Peek())
                            insd.Name = nameParts.Pop();
                        else
                            nameParts.Pop();
                        if (insd.ChildSpaces.ContainsKey(nameParts.Peek()))
                            insd = insd.ChildSpaces[nameParts.Peek()];
                        else
                            insd = insd.ChildSpaces.AddNew(nameParts.Peek());
                    }
                //ToDo:  Add code here to move the types of the namespace.
            }
        }

        /// <summary>
        /// Generates the <see cref="CodeNamespace"/> that represents the <see cref="NameSpaceDeclaration"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="CodeNamespace"/> if successful.-null- otherwise.</returns>
        public override CodeNamespace GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            if (!(options.AllowPartials || this.IsRoot))
                return null;
            CodeNamespace cnResult = new CodeNamespace();
            options.CurrentNameSpace = this;
            cnResult.Name = this.FullName;
            _OIL._Core.InsertPartialDeclarationTypes<INameSpaceDeclaration, CodeNamespace, CodeTypeDeclarationCollection>(cnResult.Name, this, cnResult.Types, options);
            foreach (string import in options.ImportList)
                cnResult.Imports.Add(new CodeNamespaceImport(import));
            options.ImportList.Clear();
            options.CurrentNameSpace = null;
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            if (cnResult.Types.Count == 0)
                return null;
            return cnResult;
        }

        #region INameSpace Members
        public override string GetUniqueIdentifier()
        {
            return this.Name;
        }
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
        public IStructTypes Structures
        {
            get 
            {
                if (this.structures == null)
                    this.structures = this.InitializeStructures();
                return this.structures;
            }
        }

        public IEnumeratorTypes Enumerators
        {
            get {
                if (this.enumerators == null)
                    this.enumerators = this.InitializeEnumerators();
                return this.enumerators;
            }
        }

        public IInterfaceTypes Interfaces
        {
            get {
                if (this.interfaces == null)
                    this.interfaces = this.InitializeInterfaces();
                return this.interfaces;
            }
        }

        public CodeNamespace[] GenerateGroupCodeDom(ICodeDOMTranslationOptions options)
        {
            List<CodeNamespace> result = new List<CodeNamespace>();
            CodeNamespace cnsThisResult = this.GenerateCodeDom(options);
            if (cnsThisResult != null)
                result.Add(cnsThisResult);
            foreach (INameSpaceDeclaration ind in this.ChildSpaces.Values)
            {
                CodeNamespace[] childSpaces;
                if (!options.AllowPartials)
                    //Auto-flattens hierarchhy.
                    childSpaces = ind.GenerateGroupCodeDom(options);
                else
                {
                    //There's extra work to do.
                    List<CodeNamespace> childSpacesList = new List<CodeNamespace>();
                    if (ind.ParentTarget == this)
                    {
                        childSpacesList.AddRange(ind.GenerateGroupCodeDom(options));
                    }
                    foreach (INameSpaceDeclaration indPartial in ind.Partials)
                        if (indPartial.ParentTarget == this)
                            childSpacesList.AddRange(indPartial.GenerateGroupCodeDom(options));
                    childSpaces = childSpacesList.ToArray();
                }
                for (int i = 0; i < childSpaces.Length; i++)
                    if (childSpaces[i] != null)
                        result.Add(childSpaces[i]);
            }
            return result.ToArray();
        }

        public INameSpaceDeclarations ChildSpaces
        {
            get {
                if (this.childSpaces == null)
                    this.childSpaces = InitializeChildSpaces();
                return this.childSpaces;
            }
        }

        /// <summary>
        /// Returns the full name of the <see cref="NameSpaceDeclaration"/> using concatination
        /// and its [potential] parent.
        /// </summary>
        public string FullName
        {
            get
            {
                if (this.ParentTarget == null || this.ParentTarget is IIntermediateProject)
                    return this.Name;
                else
                    return String.Format("{0}.{1}", ((INameSpaceDeclaration)this.ParentTarget).FullName, this.Name);
            }
        }
        #endregion


        #region INameSpaceDeclaration Members


        public void AddType(IDeclaredType newItem)
        {
            if (newItem is IClassType)
                this.Classes.Add((IClassType)newItem);
            else if (newItem is IDelegateType)
                this.Delegates.Add((IDelegateType)newItem);
            else if (newItem is IEnumeratorType)
                this.Enumerators.Add((IEnumeratorType)newItem);
            else if (newItem is IInterfaceType)
                this.Interfaces.Add((IInterfaceType)newItem);
            else if (newItem is IStructType)
                this.Structures.Add((IStructType)newItem);
        }

        #endregion

        #region INameSpaceDeclaration Members


        public INameSpaceDeclarationPartials Partials
        {
            get {
                if (this.IsPartial)
                    return this.GetRootDeclaration().Partials;
                if (this.partials == null)
                    this.partials = this.InitializePartials();
                return this.partials;
            }
        }

        #endregion

        #region ISegmentableDeclarationTarget<INameSpaceDeclaration> Members

        public INameSpaceDeclaration GetRootDeclaration()
        {
            if (this.basePartial == null)
                return this;
            return this.basePartial;
        }

        ISegmentableDeclarationTargetPartials<INameSpaceDeclaration> ISegmentableDeclarationTarget<INameSpaceDeclaration>.Partials
        {
            get { return this.Partials; }
        }

        #endregion

        #region ISegmentableDeclarationTarget Members

        public bool IsPartial
        {
            get { return this.basePartial != null; }
        }

        public bool IsRoot
        {
            get { return this.basePartial == null; ; }
        }

        ISegmentableDeclarationTarget ISegmentableDeclarationTarget.GetRootDeclaration()
        {
            return this.GetRootDeclaration();
        }

        ISegmentableDeclarationTargetPartials ISegmentableDeclarationTarget.Partials
        {
            get
            {
                return (ISegmentableDeclarationTargetPartials)this.Partials;
            }
        }

        #endregion

        #region NameSpaceDeclaration Initialization Members

        private INameSpaceDeclarations InitializeChildSpaces()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().ChildSpaces.GetPartialClone(this);
            return new NameSpaceDeclarations(this);
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

        protected virtual IStructTypes InitializeStructures()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().Structures.GetPartialClone(this);
            return new StructTypes(this);
        }

        private INameSpaceDeclarationPartials InitializePartials()
        {
            return new NameSpaceDeclarationPartials(this);
        }

        #endregion

        #region INameSpaceDeclaration Members

        public IIntermediateProject Project
        {
            get
            {
                INameSpaceDeclaration insd = this.GetRootNameSpace();
                if (insd == null)
                    return null;
                else if (insd.ParentTarget is IIntermediateProject)
                    return ((IIntermediateProject)(insd.ParentTarget));
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="NameSpaceDeclaration"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="NameSpaceDeclaration"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
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
                this.ChildSpaces.GatherTypeReferences(ref result, options);
            }
            else
            {
                if (this.IsRoot)
                    this.Classes.GatherTypeReferences(ref result, options);
                if (this.IsRoot)
                    this.Delegates.GatherTypeReferences(ref result, options);
                if (this.IsRoot)
                    this.Enumerators.GatherTypeReferences(ref result, options);
                if (this.IsRoot)
                    this.Interfaces.GatherTypeReferences(ref result, options);
                if (this.IsRoot)
                    this.Structures.GatherTypeReferences(ref result, options);
                if (this.IsRoot)
                    this.ChildSpaces.GatherTypeReferences(ref result, options);
            }
        }

        #region ITypeParent Members

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

        #region INameSpaceParent Members

        public int GetNameSpaceCount(bool includePartials)
        {
            if (includePartials)
                return this.ChildSpaces.Count;
            else
                return this.ChildSpaces.GetCountForTarget(this);
        }

        #endregion

        #region INameSpaceDeclaration Members


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

        #endregion

    }
}
