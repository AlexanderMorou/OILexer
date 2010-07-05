using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.FiniteAutomata.Tokens;
using System.Globalization;

namespace Oilexer.Parser.Builder
{
    internal class UnicodeTargetPartialCategory :
        UnicodeTargetCategory,
        IUnicodeTargetPartialCategory
    {
        public UnicodeTargetPartialCategory(UnicodeCategory targetedCategory, RegularLanguageSet negativeAssertion)
            : base(targetedCategory)
        {
            this.NegativeAssertion = negativeAssertion;
        }

        #region IUnicodeTargetPartialCategory Members

        public RegularLanguageSet NegativeAssertion { get; private set; }

        #endregion

        public override bool Equals(IUnicodeTargetCategory other)
        {
            if (other is IUnicodeTargetPartialCategory)
                return this.Equals((IUnicodeTargetPartialCategory)other);
            return base.Equals(other);
        }

        #region IEquatable<IUnicodeTargetPartialCategory> Members

        public bool Equals(IUnicodeTargetPartialCategory other)
        {
            return this.NegativeAssertion.Equals(other.NegativeAssertion) && base.Equals(other);
        }

        #endregion

        public override int GetHashCode()
        {
            return this.NegativeAssertion.GetHashCode() ^ base.GetHashCode();
        }
    }
}
