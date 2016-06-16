using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    public interface IOilexerGrammarRegion
    {
        /// <summary>
        /// Returns the <see cref="Int32"/> value representing the point at which
        /// the <see cref="IOilexerGrammarRegion"/> starts.
        /// </summary>
        int Start { get; }
        /// <summary>
        /// Returns the <see cref="Int32"/> value representing the point at which
        /// the <see cref="IOilexerGrammarRegion"/> ends.
        /// </summary>
        int End { get; }
        /// <summary>
        /// The <see cref="String"/> representing the collapsed
        /// tip shown for the region.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The <see cref="String"/> representing the collapsed form
        /// of the region.
        /// </summary>
        string CollapseForm { get; }
    }
}
