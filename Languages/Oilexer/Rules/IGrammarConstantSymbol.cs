using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a 
    /// token which is a literal value.
    /// </summary>
    public interface IGrammarConstantSymbol :
        IGrammarTokenSymbol
    {
        /// <summary>
        /// Returns the <see cref="GrammarConstantType"/> which determines the kind of constant the 
        /// <see cref="IGrammarConstantSymbol"/> is.
        /// </summary>
        GrammarConstantType Type { get; }
    }
    /// <summary>
    /// The kind of constant the <see cref="IGrammarConstantSymbol"/>
    /// is.
    /// </summary>
    public enum GrammarConstantType
    {
        /// <summary>
        /// The constant is derived from a full token which consists of exactly one unnamed literal
        /// entry only.
        /// </summary>
        Entry,
        /// <summary>
        /// The constant is derived from an element of an enumerator token, which consists of a series
        /// of named literals under a unified name.
        /// </summary>
        Item
    }
}
