using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="ILiteralReferenceTokenItem"/> with
    /// a speific data-type.
    /// </summary>
    /// <typeparam name="T">The type of value stored in the <typeparamref name="TLiteral"/>.</typeparam>
    /// <typeparam name="TLiteral">The type of literal.</typeparam>
    public interface ILiteralReferenceTokenItem<T, TLiteral> :
        ILiteralReferenceTokenItem
        where TLiteral :
            ILiteralTokenItem<T>
    {
        /// <summary>
        /// Returns the literal the <see cref="ILiteralReferenceTokenItem{T, TLiteral}"/> references.
        /// </summary>
        new TLiteral Literal { get; }
    }
}
