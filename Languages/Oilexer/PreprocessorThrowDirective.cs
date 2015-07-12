using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public class PreprocessorThrowDirective :
        PreprocessorDirectiveBase,
        IPreprocessorThrowDirective
    {
        internal string error;
        IOilexerGrammarErrorEntry reference;
        private IOilexerGrammarFile source;
        private IOilexerGrammarToken[] arguments;
        /// <summary>
        /// Creates a new <see cref="PreprocessorThrowDirective"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="arguments">The arguments used for extra information about the error</param>
        /// <param name="fileName">The file in which the <see cref="PreprocessorThrowDirective"/> was declared
        /// in.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorThrowDirective"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorThrowDirective"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorThrowDirective"/> 
        /// was declared at.</param>
        public PreprocessorThrowDirective(IOilexerGrammarFile source, string error, IOilexerGrammarToken[] arguments, int column, int line, long position)
            : base(column, line, position)
        {
            this.source = source;
            this.error = error;
            this.arguments = arguments;
        }

        //#region IPreprocessorThrowDirective Members

        public IOilexerGrammarErrorEntry Reference
        {
            get
            {
                if (this.source != null && this.reference == null && this.error != null && this.error != string.Empty)
                {
                    foreach (IOilexerGrammarEntry ie in source)
                        if (ie is IOilexerGrammarErrorEntry && ((IOilexerGrammarErrorEntry)(ie)).Name == error)
                        {
                            this.reference = (IOilexerGrammarErrorEntry)ie;
                            this.error = null;
                            this.source = null;
                        }
                }
                return this.reference;
            }
        }

        /// <summary>
        /// Returns the arguments associated with the throw directive, all tokens.
        /// </summary>
        public IOilexerGrammarToken[] Arguments
        {
            get { return this.arguments; }
        }

        //#endregion

        public override EntryPreprocessorType Type
        {
            get { return EntryPreprocessorType.Throw; }
        }
    }
}
