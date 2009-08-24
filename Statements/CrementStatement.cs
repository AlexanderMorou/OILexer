using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;
using Oilexer.Types;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace Oilexer.Statements
{
    public class CrementStatement :
        Statement<CodeSnippetStatement>,
        ICrementStatement
    {
        /// <summary>
        /// Data member for <see cref="CrementType"/>.
        /// </summary>
        private CrementType crementType;
        /// <summary>
        /// Data member for <see cref="Operation"/>.
        /// </summary>
        private CrementOperation operation;
        /// <summary>
        /// Data member for <see cref="Target"/>.
        /// </summary>
        private IAssignStatementTarget target;

        public CrementStatement(CrementType crementType, CrementOperation operation, IAssignStatementTarget target)
        {
            this.crementType = crementType;
            this.operation = operation;
            this.target = target;
        }

        public override CodeSnippetStatement GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.LanguageProvider != null)
            {
                if (options.LanguageProvider is CSharpCodeProvider)
                    return new CodeSnippetStatement(this.ConvertToString(TranslationLanguage.CSharp));
                //else if (options.LanguageProvider is VBCodeProvider)
                //    return new CodeSnippetStatement(this.ConvertToString(TranslationLanguage.VisualBasic));
            }
            return new CodeSnippetStatement(this.ConvertToString(TranslationLanguage.CSharp) + " /* Unknown language. */");
        }

        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (this.Target != null)
                this.Target.GatherTypeReferences(ref result, options);
        }

        #region ICrementStatement Members

        /// <summary>
        /// Returns/sets whether the crement statement is prefix based or postfix based.
        /// </summary>
        public CrementType CrementType
        {
            get
            {
                return this.crementType;
            }
            set
            {
                this.crementType = value;
            }
        }

        /// <summary>
        /// Returns/sets the crement operation that is applied to <see cref="Target"/>.
        /// </summary>
        public CrementOperation Operation
        {
            get
            {
                return this.operation;
            }
            set
            {
                this.operation = value;
            }
        }

        /// <summary>
        /// Returns the <see cref="IAssignStatementTarget"/> which has the <see cref="Operation"/> applied to it.
        /// </summary>
        public IAssignStatementTarget Target
        {
            get
            {
                return this.target;
            }
            set
            {
                this.target = value;
            }
        }

        #endregion
    }
}
