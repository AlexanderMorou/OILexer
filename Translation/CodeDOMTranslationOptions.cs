using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Oilexer.Types;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System.Data;

namespace Oilexer.Translation
{
    /// <summary>
    /// Determinant for the CodeDom object hierarchy 
    /// generation process.
    /// </summary>
    public sealed class CodeDOMTranslationOptions :
        CodeTranslationOptions,
        ICodeDOMTranslationOptions
    {
        /// <summary>
        /// Data member for <see cref="LanguageProvider"/>.
        /// </summary>
        private CodeDomProvider languageProvider;
        /// <summary>
        /// Data member for <see cref="Options"/>.
        /// </summary>
        private CodeGeneratorOptions options;

        /// <summary>
        /// Creates a new instance of <see cref="CodeDOMTranslationOptions"/>.
        /// </summary>
        public CodeDOMTranslationOptions() 
            : base()
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="CodeDOMTranslationOptions"/> with the <paramref name="autoResolveReferences"/>.
        /// </summary>
        /// <param name="autoResolveReferences">Whether the <see cref="CodeDOMTranslationOptions"/> 
        /// should resolve import references when types are encountered.</param>
        /// <remarks>The <see cref="NameHandler"/> will default to a passive in==out name handler.</remarks>
        public CodeDOMTranslationOptions(bool autoResolveReferences)
            : this(autoResolveReferences, new PassiveNameHandler())
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="CodeDOMTranslationOptions"/> with the <paramref name="autoResolveReferences"/> and
        /// <paramref name="nameHandler"/> provided.
        /// </summary>
        /// <param name="autoResolveReferences">Whether the <see cref="CodeDOMTranslationOptions"/> 
        /// should resolve import references when types are encountered.</param>
        /// <param name="nameHandler">A <see cref="ICodeGeneratorNameHandler"/> implementation instance
        /// which processes the <see cref="IDeclaration"/> names before translation into
        /// CodeDOM.</param>
        public CodeDOMTranslationOptions(bool autoResolveReferences, ICodeGeneratorNameHandler nameHandler)
            : base(autoResolveReferences, nameHandler)
        {

        }

        public override bool AllowPartials
        {
            get
            {
                if (this.LanguageProvider != null)
                    return (this.LanguageProvider.Supports(GeneratorSupport.PartialTypes)) && base.AllowPartials;
                return base.AllowPartials;
            }
            set
            {
                base.AllowPartials = value;
            }
        }

        /// <summary>
        /// Returns/sets the generation language.
        /// </summary>
        /// <remarks>Used to create better transitory comments and fixes common bugs
        /// in the specific <see cref="CodeDomProvider"/>.</remarks>
        public CodeDomProvider LanguageProvider
        {
            get
            {
                return this.languageProvider;
            }
            set
            {
                if (this.locked)
                    throw new InvalidOperationException("Cannot change the LanguageProvider on the DefaultOptions.");
                this.languageProvider = value;
            }
        }

        /// <summary>
        /// Returns/sets the original codedom's generator options.
        /// </summary>
        public CodeGeneratorOptions Options
        {
            get
            {
                return options;
            }
            set
            {
                this.options = value;
            }
        }
    }
}
