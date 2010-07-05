using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer._Internal.Inlining;

namespace Oilexer.Parser.Builder
{
    internal class TokenPrecedenceTable :
        List<TokenEqualityLevel>
    {
        public TokenPrecedenceTable(IEnumerable<InlinedTokenEntry> items)
        {
            this.Add(new TokenEqualityLevel(items));
            SortPrecedences();
        }

        private void SortPrecedences()
        {
            while (NeedsSorting())
            {
                for (int i = 0; i < this.Count; i++)
                {
                    var level = this[i];
                    for (int e = 0; e < level.Count; e++)
                    {
                        var entry = level[e];
                        foreach (var lower in entry.LowerPrecedenceTokens.Cast<InlinedTokenEntry>())
                        {
                            bool lowerIsHigherOrEqual = false;
                            TokenEqualityLevel heLevel = null;
                            foreach (var higherLevel in this)
                            {
                                if (higherLevel.Contains(lower))
                                {
                                    heLevel = higherLevel;
                                    lowerIsHigherOrEqual = true;
                                    break;
                                }
                                if (higherLevel == level)
                                    break;
                            }
                            if (lowerIsHigherOrEqual)
                            {
                                bool currentHit = false;
                                TokenEqualityLevel lowerTarget = null;
                                foreach (var lowerLevel in this)
                                {
                                    if (lowerLevel == level)
                                        currentHit = true;
                                    else if (currentHit)
                                    {
                                        bool noLower = true;
                                        foreach (var lowerEntry in lowerLevel)
                                        {
                                            if (lowerEntry.LowerPrecedenceTokens.Contains(lower))
                                                noLower = false;
                                        }
                                        if (noLower)
                                            lowerTarget = lowerLevel;
                                        else if (lowerTarget != null)
                                            lowerTarget = null;
                                    }
                                }
                                bool inserted = false;
                                if (currentHit)
                                {
                                    if (lowerTarget != null)
                                    {
                                        if (level.IndexOf(lower) < e)
                                            e--;
                                        level.Remove(lower);
                                        inserted = true;
                                        lowerTarget.Add(lower);
                                    }
                                }
                                if (!inserted)
                                {
                                    if (level.IndexOf(lower) < e)
                                        e--;
                                    level.Remove(lower);
                                    lowerTarget = new TokenEqualityLevel();
                                    lowerTarget.Add(lower);
                                    this.Add(lowerTarget);
                                }
                            }
                        }
                    }
                }
            }
            this.SortIndividual();
        }

        private void SortIndividual()
        {
            foreach (var level in this)
                level.Sort();
        }

        private bool NeedsSorting()
        {
            foreach (var level in this)
                foreach (var entry in level)
                    foreach (var lower in entry.LowerPrecedenceTokens.Cast<InlinedTokenEntry>())
                        foreach (var higherLevel in this)
                        {
                            if (higherLevel.Contains(lower))
                                return true;
                            if (higherLevel == level)
                                break;
                        }
            return false;
        }
    }
}
