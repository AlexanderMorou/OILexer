using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Oilexer.VSIntegration.Classifications
{
    internal class GDTokenClassifications
    {
        [Export]
        [FileExtension(".oilexer")]
        [ContentType("oilexer")]
        internal static FileExtensionToContentTypeDefinition OILexerFileType = null;
        
        [Export]
        [Name("Oilexer")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition OILexerContentType = null;
        
        /// <summary>
        /// Defines the character literal classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Literal: Character")]
        internal static ClassificationTypeDefinition CharacterLiteral = null;

        /// <summary>
        /// Defines the character range classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Character Range")]
        internal static ClassificationTypeDefinition CharacterRange = null;

        /// <summary>
        /// Defines the comment classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Comment")]
        internal static ClassificationTypeDefinition Comment = null;

        /// <summary>
        /// Defines the identifier classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Identifier")]
        internal static ClassificationTypeDefinition Identifier = null;

        /// <summary>
        /// Defines the rule reference classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Rule Reference")]
        internal static ClassificationTypeDefinition RuleReference = null;

        /// <summary>
        /// Defines the token reference classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Token Reference")]
        internal static ClassificationTypeDefinition TokenReference = null;

        /// <summary>
        /// Defines the number literal classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Literal: Number")]
        internal static ClassificationTypeDefinition Number = null;

        /// <summary>
        /// Defines the operator classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Operator")]
        internal static ClassificationTypeDefinition Operator = null;

        /// <summary>
        /// Defines the preprocessor classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Preprocessor Directive")]
        internal static ClassificationTypeDefinition Preprocessor = null;

        /// <summary>
        /// Defines the string literal classification type
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Literal: String")]
        internal static ClassificationTypeDefinition String = null;

        /// <summary>
        /// Defines the whitespace classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Whitespace")]
        internal static ClassificationTypeDefinition Whitespace = null;

        /// <summary>
        /// Defines the error classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Error")]
        internal static ClassificationTypeDefinition Error = null;

        /// <summary>
        /// Defines the error classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Rule Template Reference")]
        internal static ClassificationTypeDefinition RuleTemplateReference = null;

        /// <summary>
        /// Defines the error classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("OILexer: Rule Template Parameter Reference")]
        internal static ClassificationTypeDefinition RuleTemplateParameterReference = null;

    }
}
