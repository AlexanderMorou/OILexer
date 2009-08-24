using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Inlining
{
    /// <summary>
    /// Provides a base class to inline an <see cref="ITokenEntry"/>.
    /// </summary>
    internal class InlinedTokenEntry :
        TokenEntry
    {
        /// <summary>
        /// Returns the <see cref="Dictionary{TKey, TValue}"/> which 
        /// </summary>
        public Dictionary<ITokenItem, ITokenItem> OldNewLookup { get; private set; }
        /// <summary>
        /// Creates a new <see cref="InlinedTokenEntry"/> with the <paramref name="source"/>
        /// provided.
        /// </summary>
        /// <param name="source">The <see cref="ITokenEntry"/> from which the 
        /// current <see cref="InlinedTokenEntry"/> derives.</param>
        public InlinedTokenEntry(ITokenEntry source)
            : base(source.Name, null, source.ScanMode, source.FileName, source.Column, source.Line, source.Position, source.Unhinged, source.SelfAmbiguous, source.LowerPrecedenceTokens)
        {
            this.OldNewLookup = new Dictionary<ITokenItem, ITokenItem>();
            this.branches = InliningCore.Inline(source.Branches, source, this, this.OldNewLookup);
            this.Source = source;
        }

        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> from which
        /// the current <see cref="InlinedTokenEntry"/> derives.
        /// </summary>
        public ITokenEntry Source { get; private set; }

        internal void ResolveLowerPrecedencesAgain(Dictionary<ITokenEntry, InlinedTokenEntry> originalNewLookup)
        {
            List<ITokenEntry> lowerPrecedences = new List<ITokenEntry>();
            if (this.LowerPrecedenceTokens != null)
                foreach (var token in this.LowerPrecedenceTokens)
                    if (originalNewLookup.ContainsKey(token))
                        lowerPrecedences.Add(originalNewLookup[token]);
            this.LowerPrecedenceTokens = lowerPrecedences.ToArray();
        }
    }
}
