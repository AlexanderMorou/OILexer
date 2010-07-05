using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Oilexer.Parser.Builder
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
