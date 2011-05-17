using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public interface ILiteralReferenceTokenItem :
        ITokenItem
    {
        /// <summary>
        /// Returns the source of the literal that the <see cref="ILiteralReferenceTokenItem"/>
        /// relates to.
        /// </summary>
        ITokenEntry Source { get; }
        /// <summary>
        /// Returns the literal the <see cref="ILiteralReferenceTokenItem"/> references.
        /// </summary>
        ILiteralTokenItem Literal { get; }
    }
}
