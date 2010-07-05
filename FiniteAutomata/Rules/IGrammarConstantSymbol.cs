using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.FiniteAutomata.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a 
    /// token which is a literal value.
    /// </summary>
    public interface IGrammarConstantSymbol :
        IGrammarTokenSymbol
    {
        /// <summary>
        /// Returns the <see cref="GrammarConstantType"/> which
        /// determines the kind of constant the 
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
        /// The constant is derived from a full token which consists
        /// of exactly one unnamed literal entry only.
        /// </summary>
        Entry,
        /// <summary>
        /// The constant is derived from an element of an enumerator
        /// token, which consists of a series of named literals under
        /// a unified name.
        /// </summary>
        Item
    }
}
