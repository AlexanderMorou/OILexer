using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
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
            return false;
        }

        #region IEquatable<IUnicodeTargetPartialCategory> Members

        public bool Equals(IUnicodeTargetPartialCategory other)
        {
            return this.NegativeAssertion.Equals(other.NegativeAssertion) && base.EqualsInternal(other);
        }

        #endregion

        public override int GetHashCode()
        {
            return this.NegativeAssertion.GetHashCode() ^ base.GetHashCode();
        }
    }
}
