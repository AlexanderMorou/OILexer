using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.FiniteAutomata.Tokens;

namespace Oilexer.Parser.Builder
{
    internal class UnicodeTargetGraph :
        ControlledStateDictionary<RegularLanguageDFAState, IUnicodeTarget>,
        IUnicodeTargetGraph
    {
        #region IUnicodeTargetGraph Members

        /// <summary>
        /// Inserts and returns a new <see cref="IUnicodeTarget"/>
        /// implementation with the <paramref name="target"/> specified.
        /// </summary>
        /// <param name="target">The <see cref="RegularLanguageDFAState"/>
        /// to which the <see cref="IUnicodeTarget"/> is directed towards.</param>
        /// <returns>A new <see cref="IUnicodeTarget"/> implementation
        /// with the <paramref name="target"/> specified.</returns>
        public IUnicodeTarget Add(RegularLanguageDFAState target, bool targetIsOrigin)
        {
            UnicodeTarget result = new UnicodeTarget(target, targetIsOrigin);
            base.dictionaryCopy.Add(target, result);
            return result;
        }

        #endregion


        #region IEquatable<IUnicodeTargetGraph> Members

        public bool Equals(IUnicodeTargetGraph other)
        {
            if (other == null)
                return false;
            if (other.Count != this.Count)
                return false;
            return other.All(p =>
                {
                    return this.ContainsKey(p.Key) && this[p.Key].Equals(p.Value);
                });
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is IUnicodeTargetGraph)
                return this.Equals((IUnicodeTargetGraph)obj);
            return false;
        }
    }
}
