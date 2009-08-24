using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;

namespace Oilexer.Translation
{
    public partial class CodeTranslationOptions :
        ICodeTranslationOptions
    {
        #region CodeDOMGeneratorOptions Data members
        /// <summary>
        /// Data member for <see cref="AutoResolveReferences"/>.
        /// </summary>
        private bool autoResolveReferences;

        /// <summary>
        /// Data member for <see cref="ImportList"/>.
        /// </summary>
        private ICollection<string> importList;

        /// <summary>
        /// Data member for <see cref="NameHandler"/>.
        /// </summary>
        private  ICodeGeneratorNameHandler nameHandler;

        /// <summary>
        /// Data member for <see cref="CurrentNameSpace"/>
        /// </summary>
        private INameSpaceDeclaration currentNameSpace;

        /// <summary>
        /// Data member for <see cref="AutoRegions"/>.
        /// </summary>
        private AutoRegionAreas autoRegions = AutoRegionAreas.Standard;

        /// <summary>
        /// Data member for <see cref="AllowRegions"/>.
        /// </summary>
        private bool allowRegions = true;

        /// <summary>
        /// Data member for <see cref="AllowPartials"/>.
        /// </summary>
        private bool allowPartials = true;

        /// <summary>
        /// Data member for <see cref="AutoComments"/>.
        /// </summary>
        private bool autoComments = false;
        
        /// <summary>
        /// Data member for <see cref="BuildTrail"/>.
        /// </summary>
        private Stack<IDeclarationTarget> buildTrail = new Stack<IDeclarationTarget>();

        internal bool locked;

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="CodeTranslationOptions"/>.
        /// </summary>
        public CodeTranslationOptions() :
            this(false)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="CodeTranslationOptions"/> with the <paramref name="autoResolveReferences"/> and
        /// <paramref name="nameHandler"/> provided.
        /// </summary>
        /// <param name="autoResolveReferences">Whether the <see cref="CodeTranslationOptions"/> 
        /// should resolve import references when types are encountered.</param>
        /// <param name="nameHandler">A <see cref="ICodeGeneratorNameHandler"/> implementation instance
        /// which processes the <see cref="IDeclaration"/> names before translation into
        /// Code.</param>
        public CodeTranslationOptions(bool autoResolveReferences, ICodeGeneratorNameHandler nameHandler)
        {
            if (nameHandler == null)
                throw new ArgumentNullException("nameHandler");
            this.autoResolveReferences = autoResolveReferences;
            this.nameHandler = nameHandler;
            this.importList = new Collection<string>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="CodeDOMGeneratorOptions"/> with the <paramref name="autoResolveReferences"/>.
        /// </summary>
        /// <param name="autoResolveReferences">Whether the <see cref="CodeDOMGeneratorOptions"/> 
        /// should resolve import references when types are encountered.</param>
        /// <remarks>The <see cref="NameHandler"/> will default to a passive in==out name handler.</remarks>
        public CodeTranslationOptions(bool autoResolveReferences)
            : this(autoResolveReferences, new PassiveNameHandler())
        {
        }

        #region ICodeDOMTranslationOptions Members

        /// <summary>
        /// Returns whether the <see cref="CodeTranslationOptions"/> supports regions for the given
        /// <paramref name="area"/>.
        /// </summary>
        /// <param name="area">The <see cref="AutoRegionAreas"/> that the <see cref="CodeTranslationOptions"/>
        /// supports given the current state of the <see cref="CodeTranslationOptions"/>.</param>
        /// <returns>true, if the <see cref="CodeTranslationOptions"/> supports
        /// auto-regioning for the <paramref name="area"/> given; false, otherwise.</returns>
        public bool AutoRegionsFor(AutoRegionAreas area)
        {
            return ((this.AutoRegions & area) == area);
        }

        /// <summary>
        /// Returns whether to resolve import references when types are encountered.
        /// </summary>
        public bool AutoResolveReferences
        {
            get { return this.autoResolveReferences; }
            set {
                if (this.locked)
                    throw new InvalidOperationException("The CodeDOMGeneratorOptions is in an invalid state.");
                this.autoResolveReferences = value; }
        }

        /// <summary>
        /// Returns the <see cref="ICodeDOMGeneratorNameHandler"/> that translates the <see cref="IDeclaration"/>
        /// names as necessary.
        /// </summary>
        public ICodeGeneratorNameHandler NameHandler
        {
            get { return this.nameHandler; }
        }

        /// <summary>
        /// Returns the namespace import list.
        /// </summary>
        public ICollection<string> ImportList
        {
            get { return this.importList; }
        }

        /// <summary>
        /// Returns/sets the current scope of the generation process.
        /// </summary>
        public INameSpaceDeclaration CurrentNameSpace
        {
            get
            {
                return this.currentNameSpace;
            }
            set
            {
                this.currentNameSpace = value;
            }
        }

        /// <summary>
        /// Returns/sets whether regions are allowed in the generation process.
        /// </summary>
        public bool AllowRegions
        {
            get
            {
                return this.allowRegions;
            }
            set
            {
                this.allowRegions = value;
            }
        }

        /// <summary>
        /// Returns/sets whether types are allowed to be spanned across multiple instances.
        /// </summary>
        public virtual bool AllowPartials
        {
            get
            {
                return this.allowPartials;
            }
            set
            {
                this.allowPartials = value;
            }
        }

        /// <summary>
        /// Returns/sets whether auto-regions are generated based upon member types.
        /// </summary>
        public AutoRegionAreas AutoRegions
        {
            get
            {
                return this.autoRegions;
            }
            set
            {
                this.autoRegions = value;
            }
        }

        public Stack<IDeclarationTarget> BuildTrail
        {
            get
            {
                return this.buildTrail;
            }
        }

        public bool AutoComments
        {
            get
            {
                return this.autoComments;
            }
            set
            {
                this.autoComments = value;
            }
        }

        #endregion
    }
}
