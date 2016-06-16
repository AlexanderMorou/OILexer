using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with an entry to a grammar description file.
    /// </summary>
    public interface IOilexerGrammarEntry
    {
        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="IOilexerGrammarEntry"/> was declared at.
        /// </summary>
        int Column { get; }
        /// <summary>
        /// Returns the line index the <see cref="IOilexerGrammarEntry"/> was declared at.
        /// </summary>
        int Line { get; }
        /// <summary>
        /// Returns the position the <see cref="IOilexerGrammarEntry"/> was declared at.
        /// </summary>
        long Position { get; }
        /// <summary>
        /// Returns the file the <see cref="IOilexerGrammarEntry"/> was declared in.
        /// </summary>
        string FileName { get; }
    }
}
