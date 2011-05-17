using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining
{
    /// <summary>
    /// Defines properties and methods for working with an inlined token item.
    /// </summary>
    internal interface IInlinedTokenItem :
        ITokenItem
    {
        /// <summary>
        /// Returns the <see cref="ITokenItem"/> from which the current
        /// <see cref="IInlinedTokenItem"/> is derived.
        /// </summary>
        ITokenItem Source { get; }
        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> which contains
        /// the <see cref="Source"/>.
        /// </summary>
        ITokenEntry SourceRoot { get; }

        /// <summary>
        /// Returns the <see cref="InlinedTokenEntry"/> which contains the current
        /// <see cref="IInlinedTokenItem"/>.
        /// </summary>
        InlinedTokenEntry Root { get; }

        /// <summary>
        /// Returns the <see cref="RegularLanguageNFAState"/> which denotes
        /// the nondeterministic representation of the current
        /// <see cref="IInlinedTokenItem"/>.
        /// </summary>
        RegularLanguageNFAState State { get; }
    }
}
