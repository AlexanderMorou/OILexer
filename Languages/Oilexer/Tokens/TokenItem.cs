using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public abstract class TokenItem :
        ScannableEntryItem,
        ITokenItem
    {
        /// <summary>
        /// Data member for <see cref="DefaultSoftRefOrValue"/>.
        /// </summary>
        private string defaultSoftRefOrValue;

        /// <summary>
        /// Creates a new <see cref="TokenItem"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="TokenItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="TokenItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="TokenItem"/> was declared.</param>
        protected TokenItem(int column, int line, long position)
            : base(column, line, position)
        {

        }

        #region ITokenItem Members

        public new ITokenItem Clone()
        {
            return ((ITokenItem)(base.Clone()));
        }

        public string DefaultSoftRefOrValue
        {
            get { return this.defaultSoftRefOrValue; }
            internal set { this.defaultSoftRefOrValue = value; }
        }

        #endregion

        protected override string ToStringFurtherOptions()
        {
            if (this.DefaultSoftRefOrValue != null)
            {
                return string.Format("Default={0};", this.DefaultSoftRefOrValue);
            }
            return null;
        }
    }
}
