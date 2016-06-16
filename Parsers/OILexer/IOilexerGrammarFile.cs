using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Cst;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with a grammar description file.
    /// </summary>
    public interface IOilexerGrammarFile :
        ICollection<IOilexerGrammarEntry>,
        IConcreteNode
    {
        /// <summary>Returns the <see cref="IControlledCollection{T}"/> of files that the <see cref="IOilexerGrammarFile"/> was created from.</summary>
        IControlledCollection<string> Files { get; }
        /// <summary>Returns the <see cref="IOilexerGrammarFileOptions"/> which determines the options related to the resulted generation process.</summary>
        IOilexerGrammarFileOptions Options { get; }

        IList<string> Includes { get; }

        IControlledCollection<IOilexerGrammarRegion> Regions { get; }
        /// <summary>Returns the <see cref="String"/> value which denotes the common path shared by all filenames.</summary>
        string RelativeRoot { get; }

        IDictionary<string, string> DefinedSymbols { get; }
    }
}
