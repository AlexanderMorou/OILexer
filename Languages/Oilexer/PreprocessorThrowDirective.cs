using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
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
        IErrorEntry reference;
        private IGDFile source;
        private IGDToken[] arguments;
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
        public PreprocessorThrowDirective(IGDFile source, string error, IGDToken[] arguments, int column, int line, long position)
            : base(column, line, position)
        {
            this.source = source;
            this.error = error;
            this.arguments = arguments;
        }

        #region IPreprocessorThrowDirective Members

        public IErrorEntry Reference
        {
            get
            {
                if (this.source != null && this.reference == null && this.error != null && this.error != string.Empty)
                {
                    foreach (IEntry ie in source)
                        if (ie is IErrorEntry && ((IErrorEntry)(ie)).Name == error)
                        {
                            this.reference = (IErrorEntry)ie;
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
        public IGDToken[] Arguments
        {
            get { return this.arguments; }
        }

        #endregion

        public override EntryPreprocessorType Type
        {
            get { return EntryPreprocessorType.Throw; }
        }
    }
}
