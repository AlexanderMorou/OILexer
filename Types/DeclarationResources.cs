using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oilexer.Types.Members;
using Oilexer.Properties;
using System.CodeDom;
using System.Resources;
using Oilexer.Statements;
using Oilexer.Expression;
using System.ComponentModel;
using Oilexer.FileModel;
using System.IO;
using res = Oilexer.Properties.Resources;
namespace Oilexer.Types
{
    public sealed class DeclarationResources :
        ClassType,
        IDeclarationResources
    {
        #region DeclarationResources data members

        /// <summary>
        /// Data member for <see cref="GenerationType"/>
        /// </summary>
        private ResourceGenerationType generationType;

        /// <summary>
        /// Data member for <see cref="StringTable"/>.
        /// </summary>
        private IDeclarationResourcesStringTable stringTable;

        /// <summary>
        /// Data member for <see cref="ResourceManager"/>.
        /// </summary>
        private IPropertyMember resourceManager;

        /// <summary>
        /// Data member for the <see cref="ResourceManager"/>'s access.
        /// </summary>
        private IFieldMember resourceManagerDataMember;

        #endregion
        /// <summary>
        /// Creates a new instance of <see cref="DeclarationResources"/> with the <paramref name="name"/>
        /// and <paramref name="parentTarget"/> provided.
        /// </summary>
        /// <param name="parentTarget">The <see cref="ITypeParent"/> which contains the <see cref="DeclarationResources"/>.</param>
        public DeclarationResources(ITypeParent parentTarget)
            : base(GetName(parentTarget), parentTarget)
        {
            base.IsStatic = true;
            base.IsSealed = false;
            base.IsAbstract = false;
            this.AccessLevel = DeclarationAccessLevel.Internal;
        }

        private static string GetName(ITypeParent parentTarget)
        {
            string baseName = res.ResourcesAutoClassName;
            if (parentTarget is IDeclaredType)
            {
                Guid g = Guid.NewGuid();
                return string.Format("{0}_{1}", baseName, g.ToString("N").Substring(0, 6));
            }
            else
                return baseName;
        }

        public DeclarationResources(IDeclarationResources basePartial, ITypeParent parentTarget)
            : base(basePartial, parentTarget)
        {
        }

        #region DeclarationResources Overrides
        /// <summary>
        /// Returns the name of the <see cref="DeclarationResources"/>.
        /// </summary>
        public override sealed string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                throw new ReadOnlyException("The name of the resources class is locked.");
            }
        }

        /// <summary>
        /// Returns a <see cref="ITypeReference"/> for <see cref="System.Object"/>,
        /// <see cref="DeclarationResources"/> represent a static class which cannot inherit
        /// any type other than <see cref="System.Object"/>.
        /// </summary>
        public override ITypeReference BaseType
        {
            get
            {
                return base.BaseType;
            }
            set
            {
                throw new ReadOnlyException("Base type on a resource class is locked.");
            }
        }

        /// <summary>
        /// Returns zero because <see cref="DeclarationResources"/> contain no nested
        /// types.
        /// </summary>
        /// <param name="includePartials">Wheter to include the partials.  Ignored in this
        /// implementation.</param>
        /// <returns>A <see cref="System.Int32"/> equaling zero.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override int GetTypeCount(bool includePartials)
        {
            return 0;
        }

        public override bool IsStatic
        {
            get
            {
                return base.IsStatic;
            }
            set
            {
                throw new ReadOnlyException("Resource classes are always static.");
            }
        }

        public override bool IsAbstract
        {
            get
            {
                return base.IsAbstract;
            }
            set
            {
                throw new ReadOnlyException("Resource classes are always abstract and sealed, therefore static.");
            }
        }

        public override bool IsSealed
        {
            get
            {
                return base.IsSealed;
            }
            set
            {
                throw new ReadOnlyException("Resource classes are always sealed and abstract, therefore static.");
            }
        }

        #region DeclarationResources Initialization Members
        /// <summary>
        /// Returns null, <see cref="DeclarationResources"/> have no type-parameters.
        /// </summary>
        /// <returns>null</returns>
        protected override ITypeParameterMembers<ITypeParameterMember<CodeTypeDeclaration>, CodeTypeDeclaration> InitializeTypeParameters()
        {
            return null;
        }

        /// <summary>
        /// Returns null, <see cref="DeclarationResources"/> have no nested classes.
        /// </summary>
        /// <returns>null</returns>
        protected override IClassTypes InitializeClasses()
        {
            return null;
        }

        /// <summary>
        /// Returns null, <see cref="DeclarationResources"/> have no nested delegates.
        /// </summary>
        /// <returns>null</returns>
        protected override IDelegateTypes InitializeDelegates()
        {
            return null;
        }

        /// <summary>
        /// Returns null, <see cref="DeclarationResources"/> have no nested enumerators.
        /// </summary>
        /// <returns>null</returns>
        protected override IEnumeratorTypes InitializeEnumerators()
        {
            return null;
        }

        /// <summary>
        /// Returns null, <see cref="DeclarationResources"/> have no nested interfaces.
        /// </summary>
        /// <returns>null</returns>
        protected override IInterfaceTypes InitializeInterfaces()
        {
            return null;
        }

        /// <summary>
        /// Returns null, <see cref="DeclarationResources"/> have no nested structures.
        /// </summary>
        /// <returns>null</returns>
        protected override IStructTypes InitializeStructures()
        {
            return null;
        }

        /// <summary>
        /// Returns null, <see cref="DeclarationResources"/> have no resources.
        /// </summary>
        /// <returns>null</returns>
        protected override IDeclarationResources InitializeResources()
        {
            return null;
        }

        #endregion
        #endregion

        #region IDeclarationResources Members

        public ResourceGenerationType GenerationType
        {
            get
            {
                return this.generationType;
            }
            set
            {
                bool rebuild = generationType != value;
                if (rebuild)
                {
                    this.generationType = value;
                    this.StringTable.Rebuild();
                }
            }
        }

        public IDeclarationResourcesStringTable StringTable
        {
            get {
                if (this.stringTable == null)
                    this.stringTable = this.InitializeStringTable();
                return this.stringTable;
            }
        }

        #endregion

        private IPropertyMember InitializeResourceManager()
        {
            //Declare the resource manager field.
            this.resourceManagerDataMember = this.Fields.AddNew(new TypedName("_resourceManager", typeof(ResourceManager)));
            //The property...
            this.resourceManager = this.Properties.AddNew(new TypedName("ResourceManager", resourceManagerDataMember.FieldType), true, false);
            //Full resolution.
            resourceManagerDataMember.FieldType.ResolutionOptions = TypeReferenceResolveOptions.GlobalType;
            IFieldReferenceExpression resourceManagerDataMemberRef = resourceManagerDataMember.GetReference();
            //Check if it's null,
            IConditionStatement ics = resourceManager.GetPart.IfThen((Expression.Expression)resourceManagerDataMemberRef == PrimitiveExpression.NullValue);
            //If it is and the parent is a namespace, then use the assembly's resource.
            if (this.ParentTarget is INameSpaceDeclaration || this.parentTarget is IIntermediateProject)
                ics.Assign(resourceManagerDataMemberRef, new CreateNewObjectExpression(resourceManagerDataMember.FieldType, new PrimitiveExpression("Resources"), (new TypeOfExpression(this)).GetProperty("Assembly")));
            //Otherwise the resource is for the contianing type.
            else if (this.parentTarget is IDeclaredType)
                ics.Assign(resourceManagerDataMemberRef, new CreateNewObjectExpression(resourceManagerDataMember.FieldType, new TypeOfExpression(this)));
            //Yield the return of the data member.
            resourceManager.GetPart.Return(resourceManagerDataMemberRef);
            return this.resourceManager;
        }

        private IDeclarationResourcesStringTable InitializeStringTable()
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().StringTable.GetPartialClone(this);
            return new DeclarationResourcesStringTable(this);
        }


        #region IDeclarationResources Members

        public new IDeclarationResources GetRootDeclaration()
        {
            if (base.basePartial == null)
                return this;
            else
                return (IDeclarationResources)base.basePartial;
        }

        #endregion

        #region IDeclarationResources Members
        /// <summary>
        /// Returns the resource manager property which the cached properties retrieve their values off of.
        /// </summary>
        /// <remarks>
        /// Only applies when <see cref="GenerationType"/> is 
        /// <see cref="ResourceGenerationType.GeneratedClassWithCache"/>.
        /// </remarks>
        public IPropertyMember ResourceManager
        {
            get
            {
                if (this.IsPartial)
                    return this.GetRootDeclaration().ResourceManager;
                if (this.resourceManager == null)
                    this.resourceManager = this.InitializeResourceManager();
                return this.resourceManager;
            }
        }

        #endregion


        #region IDeclarationResources Members

        public string WriteResources()
        {
            IDeclarationTarget idt = null;
            if (this.IsPartial)
                return null;
            for (idt = this; idt != null && (!(idt is INameSpaceDeclaration)); idt = idt.ParentTarget)
                ;
            if ((idt == null) || (!(idt is INameSpaceDeclaration)))
                throw new InvalidOperationException("Object in an invalid state.");
            INameSpaceDeclaration insd = (INameSpaceDeclaration)idt;
            TemporaryFile tf;
            if (this == this.Project.Resources)
                tf = TemporaryFileHelper.GetTemporaryDirectory("", true).Files.GetTemporaryFile(string.Format("Resources.resources", insd.FullName, this.Name));
            else
                tf = TemporaryFileHelper.GetTemporaryDirectory("", true).Files.GetTemporaryFile(string.Format("{0}.{1}.resources", insd.FullName, this.Name));

            tf.OpenStream(FileMode.Create);
            ResourceWriter rw = new ResourceWriter(tf.FileStream);
            foreach (IDeclarationResourcesStringTableEntry idrste in this.StringTable.Values)
            {
                rw.AddResource(idrste.Name, idrste.Value);
            }
            rw.Close();
            tf.CloseStream();
            return tf.FileName;
        }

        #endregion

    }
}
