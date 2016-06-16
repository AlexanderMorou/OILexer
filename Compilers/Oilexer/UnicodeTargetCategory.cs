using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    internal class UnicodeTargetCategory :
        IUnicodeTargetCategory
    {
        public UnicodeTargetCategory(UnicodeCategory targetedCategory)
        {
            this.TargetedCategory = targetedCategory;
        }

        #region IUnicodeTargetCategory Members

        /// <summary>
        /// Returns the <see cref="UnicodeCategory"/> 
        /// referred to by the <see cref="UnicodeTargetCategory"/>.
        /// </summary>
        public UnicodeCategory TargetedCategory { get; private set; }

        #endregion

        #region IEquatable<IUnicodeTargetCategory> Members

        public virtual bool Equals(IUnicodeTargetCategory other)
        {
            if (other is IUnicodeTargetPartialCategory)
                return false;
            return EqualsInternal(other);
        }

        protected bool EqualsInternal(IUnicodeTargetCategory other)
        {
            return this.TargetedCategory == other.TargetedCategory;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is IUnicodeTargetCategory)
                return this.Equals((IUnicodeTargetCategory)(obj));
            return false;
        }

        public override int GetHashCode()
        {
            return this.TargetedCategory.GetHashCode();
        }
    }
}
