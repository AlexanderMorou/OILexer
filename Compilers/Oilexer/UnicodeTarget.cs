using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    internal class UnicodeTarget :
        ControlledDictionary<UnicodeCategory, IUnicodeTargetCategory>,
        IUnicodeTarget
    {
        public UnicodeTarget(RegularLanguageDFAState target, bool targetIsOrigin)
        {
            this.TargetIsOrigin = targetIsOrigin;
            this.Target = target;
        }
        #region IUnicodeTarget Members

        public RegularLanguageDFAState Target { get; private set; }

        public IUnicodeTargetCategory Add(UnicodeCategory category)
        {
            UnicodeTargetCategory result = new UnicodeTargetCategory(category);
            base._Add(category, result);
            return result;
        }

        public IUnicodeTargetPartialCategory Add(UnicodeCategory category, RegularLanguageSet negativeAssertion)
        {
            UnicodeTargetPartialCategory result = new UnicodeTargetPartialCategory(category, negativeAssertion);
            base._Add(category, result);
            return result;
        }

        #endregion

        #region IEquatable<IUnicodeTarget> Members

        public bool Equals(IUnicodeTarget other)
        {
            if (other.TargetIsOrigin != this.TargetIsOrigin)
                return false;
            if (other.Count != this.Count)
                return false;
            if (other.Target != this.Target)
                return false;
            return other.All(p =>
                this.ContainsKey(p.Key) && this[p.Key].Equals(p.Value));
        }

        #endregion

        public override int GetHashCode()
        {
            int result = 0;
            foreach (var category in this)
                result ^= category.GetHashCode();
            return result ^ this.Count;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is IUnicodeTarget)
                return this.Equals((IUnicodeTarget)obj);
            return false;
        }
        public bool TargetIsOrigin { get; private set; }
    }
}
