using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;

namespace Oilexer.Translation
{
    public class IntermediateCodeTranslatorOptions :
        CodeTranslationOptions,
        IIntermediateCodeTranslatorOptions
    {
        /// <summary>
        /// Data member for <see cref="Formatter"/>.
        /// </summary>
        private IIntermediateCodeTranslatorFormatter formatter;

        /// <summary>
        /// Creates a new <see cref="IntermediateCodeTranslatorOptions"/> instance.
        /// </summary>
        public IntermediateCodeTranslatorOptions()
            : base()
        {
        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCodeTranslatorOptions"/> instance
        /// which resolves references automatically based upon <paramref name="autoResolveReferences"/>.
        /// </summary>
        /// <param name="autoResolveReferences">Whether or not the 
        /// <see cref="IIntermediateCodeTranslator"/> should resolve the references itself.</param>
        public IntermediateCodeTranslatorOptions(bool autoResolveReferences)
            : base(autoResolveReferences)
        {
        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCodeTranslatorOptions"/> instance
        /// which resolves references automatically based upon <paramref name="autoResolveReferences"/>
        /// and formats the code according to <paramref name="formatter"/>.
        /// </summary>
        /// <param name="autoResolveReferences">Whether or not the 
        /// <see cref="IIntermediateCodeTranslator"/> should resolve the references itself.</param>
        /// <param name="formatter">Intermediary by which the code is formatted.</param>
        public IntermediateCodeTranslatorOptions(bool autoResolveReferences, IIntermediateCodeTranslatorFormatter formatter)
            : base(autoResolveReferences)
        {
            this.formatter = formatter;
        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCodeTranslatorOptions"/> instance
        /// which resolves references automatically based upon <paramref name="autoResolveReferences"/>
        /// and translates the names by <paramref name="nameHandler"/>,
        /// </summary>
        /// <param name="autoResolveReferences">Whether or not the 
        /// <see cref="IIntermediateCodeTranslator"/> should resolve the references itself.</param>
        /// <param name="nameHandler">A <see cref="ICodeGeneratorNameHandler"/> implementation instance
        /// which processes the <see cref="IDeclaration"/> names before translation into
        /// Code.</param>
        public IntermediateCodeTranslatorOptions(bool autoResolveReferences, ICodeGeneratorNameHandler nameHandler)
            : base(autoResolveReferences, nameHandler)
        {
        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCodeTranslatorOptions"/> instance
        /// which resolves references automatically based upon <paramref name="autoResolveReferences"/>,
        /// translates the names by <paramref name="nameHandler"/>,
        /// and formats the code according to <paramref name="formatter"/>.
        /// </summary>
        /// <param name="autoResolveReferences">Whether or not the 
        /// <see cref="IIntermediateCodeTranslator"/> should resolve the references itself.</param>
        /// <param name="nameHandler">A <see cref="ICodeGeneratorNameHandler"/> implementation instance
        /// which processes the <see cref="IDeclaration"/> names before translation into
        /// Code.</param>
        /// <param name="formatter">Intermediary by which the code is formatted.</param>
        public IntermediateCodeTranslatorOptions(bool autoResolveReferences, ICodeGeneratorNameHandler nameHandler, IIntermediateCodeTranslatorFormatter formatter)
            : base(autoResolveReferences, nameHandler)
        {
            this.formatter = formatter;
        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCodeTranslatorOptions"/> instance which 
        /// formats the code according to <paramref name="formatter"/>.
        /// </summary>
        /// <param name="formatter">Intermediary by which the code is formatted.</param>
        public IntermediateCodeTranslatorOptions(IIntermediateCodeTranslatorFormatter formatter)
            : base()
        {
            this.formatter = formatter;
        }

        /// <summary>
        /// Creates a new <see cref="IntermediateCodeTranslatorOptions"/> instance
        /// which translates the names by <paramref name="nameHandler"/>.
        /// </summary>
        /// <param name="nameHandler">A <see cref="ICodeGeneratorNameHandler"/> implementation instance
        /// which processes the <see cref="IDeclaration"/> names before translation into
        /// Code.</param>
        public IntermediateCodeTranslatorOptions(ICodeGeneratorNameHandler nameHandler)
            : base(false, nameHandler)
        {
        }
        
        /// <summary>
        /// Creates a new <see cref="IntermediateCodeTranslatorOptions"/> instance
        /// which translates the names by <paramref name="nameHandler"/>,
        /// and formats the code according to <paramref name="formatter"/>.
        /// </summary>
        /// <param name="nameHandler">A <see cref="ICodeGeneratorNameHandler"/> implementation instance
        /// which processes the <see cref="IDeclaration"/> names before translation into
        /// Code.</param>
        /// <param name="formatter">Intermediary by which the code is formatted.</param>
        public IntermediateCodeTranslatorOptions(ICodeGeneratorNameHandler nameHandler, IIntermediateCodeTranslatorFormatter formatter)
            : base(false, nameHandler)            
        {
            this.formatter = formatter;
        }

        #region IIntermediateCodeTranslatorOptions Members

        /// <summary>
        /// Returns the code formatter used to manage the special format for the generation process.
        /// </summary>
        public IIntermediateCodeTranslatorFormatter Formatter
        {
            get
            {
                return this.formatter;
            }
            set
            {
                this.formatter = value;
            }
        }

        #endregion
    }
}
