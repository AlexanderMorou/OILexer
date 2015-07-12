using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    internal class UnicodeTargetGraph :
        ControlledDictionary<RegularLanguageDFAState, IUnicodeTarget>,
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
            base._Add(target, result);
            return result;
        }

        #endregion

        #region IEquatable<IUnicodeTargetGraph> Members

        public bool Equals(IUnicodeTargetGraph other)
        {
            return this.Equals(other, false);
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

        public override int GetHashCode()
        {
            int result = this.Count;
            foreach (var kvp in this)
                result ^= kvp.Key.GetHashCode() ^ kvp.Value.GetHashCode();
            return result;
        }


        public bool Equals(IUnicodeTargetGraph other, bool relaxOriginatingState)
        {
            if (other == null)
                return false;
            if (other.Count != this.Count)
                return false;
            return other.All(p =>
                this.ContainsKey(p.Key) && this[p.Key].Equals(p.Value, relaxOriginatingState));
        }

    }
}
