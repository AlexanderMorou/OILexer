using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    internal class TokenPrecedenceTable :
        List<TokenEqualityLevel>
    {
        public TokenPrecedenceTable(IEnumerable<OilexerGrammarTokenEntry> items)
        {
            this.AddRange(CreateLevels(items));
            SortIndividual();
        }

        //private void SortPrecedences()
        //{

        //}

        private IEnumerable<TokenEqualityLevel> CreateLevels(IEnumerable<OilexerGrammarTokenEntry> series)
        {
            var lower = (from entry in series
                         where series.Any(k => k.LowerPrecedenceTokens != null && k.LowerPrecedenceTokens.Contains(entry))
                         select entry).ToArray();
            var higher = series.Except(lower).ToArray();
            yield return new TokenEqualityLevel(higher);
            if (lower.Length > 0)
                foreach (var lowerSet in CreateLevels(lower))
                    yield return lowerSet;
        }

        private void SortIndividual()
        {
            int index = 0;
            foreach (var level in this)
            {
                level.Index = ++index;
                level.Sort();
            }
        }

        private bool NeedsSorting()
        {
            foreach (var level in this)
                foreach (var entry in level)
                    foreach (var lower in entry.LowerPrecedenceTokens.Cast<OilexerGrammarTokenEntry>())
                        foreach (var higherLevel in this)
                        {
                            if (higherLevel.Contains(lower))
                                return true;
                            if (higherLevel == level)
                                break;
                        }
            return false;
        }

        #region IEnumerable<InlinedTokenEntry> Members

        /// <summary>
        /// Obtains an <see cref="IEnumerable{T}"/> instance which 
        /// yields the tokens in order by precedence.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<InlinedTokenEntry> GetTokens()
        {
            foreach (var level in ((IList<TokenEqualityLevel>)this))
                foreach (var entry in level)
                    yield return (InlinedTokenEntry)entry;
        }

        #endregion
    }
}
