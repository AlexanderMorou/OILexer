using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types.Members;
using Oilexer.Translation;

namespace Oilexer.Types
{
    [Serializable]
    public abstract class SegmentableParameteredType<TItem, TDom, TPartials> :
        ParameteredDeclaredType<TItem, TDom>,
        ISegmentableDeclaredType<TItem, TDom>
        where TItem :
            ISegmentableDeclaredType<TItem, TDom>,
            IParameteredDeclaredType<TDom>
        where TDom :
            CodeTypeDeclaration,
            new()
        where TPartials :
            ISegmentableDeclaredTypePartials<TItem, TDom>
    {
        #region SegmentableParameteredType<TItem, TDom, TPartials> Data Members
        /// <summary>
        /// Data member for <see cref="Partials"/>.
        /// </summary>
        private TPartials partials;
        /// <summary>
        /// Data member for <see cref="GetRootDeclaration()"/>.
        /// </summary>
        internal TItem basePartial;
        #endregion

        #region SegmentableParameteredType<TItem, TDom, TPartials> Constructors
        /// <summary>
        /// Creates a new instance of <see cref="SegmentableParameteredType{TItem, TDom, TPartials}"/> with the 
        /// <paramref name="name"/> and <paramref name="parentTarget"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="SegmentableParameteredType{TItem, TDom, TPartials}"/>.</param>
        /// <param name="parentTarget">The <see cref="ITypeParent"/> that contains
        /// the <see cref="SegmentableParameteredType{TItem, TDom, TPartials}"/>.</param>
        public SegmentableParameteredType(string name, ITypeParent parentTarget)
            : base(name, parentTarget)
        {
            CheckModule();
        }

        /// <summary>
        /// Creates a new <see cref="SegmentableParameteredType{TItem, TDom, TPartials}"/> with
        /// the <paramref name="basePartial"/> and <paramref name="parentTarget"/> provided.
        /// </summary>
        /// <param name="basePartial">The base partial which created the <see cref="SegmentableParameteredType{TItem, TDom, TPartials}"/>.</param>
        /// <param name="parentTarget">The <see cref="ITypeParent"/> which could potentially be a segment of the
        /// original parent.</param>
        internal SegmentableParameteredType(TItem basePartial, ITypeParent parentTarget)
            : base(parentTarget)
        {
            this.basePartial = basePartial;
            if (parentTarget is INameSpaceDeclaration)
                CheckModule();
        }
        #endregion

        protected override void CheckModule()
        {
            if (this.IsRoot)
                base.CheckModule();
            else
            {
                this.Module = this.GetRootDeclaration().Module;
            }
        }

        /// <summary>
        /// Returns the name of the <see cref="SegmentableParameteredType{TItem, TDom, TPartials}"/>.
        /// </summary>
        public override string Name
        {
            get
            {
                if (this.IsPartial)
                    return basePartial.Name;
                else
                    return base.Name;
            }
            set
            {
                if (this.basePartial == null)
                    base.Name = value;
                else
                    this.basePartial.Name = value;
            }
        }

        /// <summary>
        /// Generates the <typeparamref name="TDom"/> that represents the <see cref="SegmentableParameteredType{TItem, TDom, TPartials}"/>.
        /// </summary>
        /// <returns>A new instance of a <typeparamref name="TDom"/> if successful.-null- otherwise.</returns>
        public override TDom GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            TDom result = base.GenerateCodeDom(options);
            if (options.AllowPartials && (this.IsPartial || (this.IsRoot && this.Partials.Count > 0)))
                result.IsPartial = true;

            return result;
        }

        public override IAttributeDeclarations Attributes
        {
            get
            {
                if (this.IsPartial)
                    return this.GetRootDeclaration().Attributes;
                return base.Attributes;
            }
        }

        public override DeclarationAccessLevel AccessLevel
        {
            get
            {
                if (this.IsPartial)
                    return this.GetRootDeclaration().AccessLevel;
                return base.AccessLevel;
            }
            set
            {
                if (this.IsPartial)
                    this.GetRootDeclaration().AccessLevel = value;
                else
                    base.AccessLevel = value;
            }
        }

        public override IIntermediateProject Project
        {
            get
            {
                if (this.IsPartial)
                    return this.GetRootDeclaration().Project;
                return base.Project;
            }
        }

        public override IIntermediateModule Module
        {
            get
            {
                if (this.IsPartial)
                    return this.GetRootDeclaration().Module;
                return base.Module;
            }
            set
            {
                if (this.IsPartial)
                {
                    this.GetRootDeclaration().Module = value;
                    return;
                }
                base.Module = value;
            }
        }

        protected override ITypeParameterMembers<ITypeParameterMember<TDom>, TDom> InitializeTypeParameters()
        {
            if (this.IsPartial)
                return this.basePartial.TypeParameters;
            return base.InitializeTypeParameters();
        }
        
        #region ISegmentableDeclarationTarget Members

        /// <summary>
        /// Returns whether the <see cref="SegmentableParameteredType{TItem, TDom, TPartials}"/> is
        /// segmented into multiple instances.
        /// </summary>
        public bool IsPartial
        {
            get { return this.basePartial != null; }
        }

        /// <summary>
        /// Returns whether the <see cref="SegmentableParameteredType{TItem, TDom, TPartials}"/> is the root
        /// instance.
        /// </summary>
        public bool IsRoot
        {
            get { return this.basePartial == null; ; }
        }

        ISegmentableDeclaredTypePartials ISegmentableDeclaredType.Partials
        {
            get
            {
                return (ISegmentableDeclaredTypePartials)this.Partials;
            }
        }

        ISegmentableDeclaredType ISegmentableDeclaredType.GetRootDeclaration()
        {
            return (ISegmentableDeclaredType)this.GetRootDeclaration();
        }

        /// <summary>
        /// Returns the root <typeparamref name="TItem"/> which is responsible for managing
        /// the data elements of the <see cref="ISegmentableDeclaredType{TItem, TDom}"/>.
        /// </summary>
        /// <returns>An instance of <typeparamref name="TItem"/> which reflects the root instance of the 
        /// <see cref="SegmentableParameteredType{TItem, TDom, TPartials}"/>.</returns>
        public virtual TItem GetRootDeclaration()
        {
            if (this.IsRoot)
                return (TItem)(object)this;
            else
                return this.basePartial;
        }

        #endregion

        #region ISegmentableDeclaredType<TItem,TDom> Members

        TItem ISegmentableDeclaredType<TItem, TDom>.GetRootDeclaration()
        {
            return this.GetRootDeclaration();
        }

        ISegmentableDeclaredTypePartials<TItem, TDom> ISegmentableDeclaredType<TItem, TDom>.Partials
        {
            get { return this.Partials; }
        }

        #endregion

        public TPartials Partials
        {
            get
            {
                if (this.partials == null)
                    if (this.IsRoot)
                        this.partials = InitializePartials();
                    else if (this.IsPartial)
                        return (TPartials)this.basePartial.Partials;
                return this.partials;
            }
        }

        protected abstract TPartials InitializePartials();

        #region ISegmentableDeclarationTarget Members


        ISegmentableDeclarationTarget ISegmentableDeclarationTarget.GetRootDeclaration()
        {
            return this.GetRootDeclaration();
        }

        ISegmentableDeclarationTargetPartials ISegmentableDeclarationTarget.Partials
        {
            get { return (ISegmentableDeclarationTargetPartials)this.Partials; }
        }

        #endregion

        #region ISegmentableDeclarationTarget<TItem> Members


        ISegmentableDeclarationTargetPartials<TItem> ISegmentableDeclarationTarget<TItem>.Partials
        {
            get { return this.Partials; }
        }

        #endregion

        public abstract int GetTypeCount(bool includePartials);

        public abstract int GetMemberCount(bool includePartials);

        public override IDeclaredTypeReference<TDom> GetTypeReference(ITypeReferenceCollection typeParameters)
        {
            if (this.IsPartial)
                return this.GetRootDeclaration().GetTypeReference(typeParameters);
            return base.GetTypeReference(typeParameters);
        }

    }
}
