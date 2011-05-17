﻿using System;
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
    /// <summary>
    /// Defines properties and methods for working with a token item that links to another 
    /// <see cref="ITokenEntry"/>.
    /// </summary>
    public interface ITokenReferenceTokenItem :
        ITokenItem
    {
        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> that the <see cref="ITokenReferenceTokenItem"/>
        /// references.
        /// </summary>
        ITokenEntry Reference { get; }
    }
}
