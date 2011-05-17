using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    /// <summary>
    /// Provides a base implementation of <see cref="ILiteralStringTokenItem"/> which is
    /// a <see cref="System.String"/> literal defined in an <see cref="ITokenEntry"/>.
    /// </summary>
    public class LiteralStringTokenItem :
        LiteralTokenItem<string>,
        ILiteralStringTokenItem
    {
        /// <summary>
        /// Data member for <see cref="CaseInsensitive"/>.
        /// </summary>
        private bool caseInsensitive;
        private bool siblingAmbiguity;
        /// <summary>
        /// Creates a new <see cref="LiteralStringTokenItem"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="value">The value the <see cref="LiteralStringTokenItem"/> represents.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="LiteralStringTokenItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="LiteralStringTokenItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="LiteralStringTokenItem"/> was declared.</param>
        public LiteralStringTokenItem(String value, bool caseInsensitive, int column, int line, long position, bool siblingAmbiguity)
            : base(value, column, line, position)
        {
            this.siblingAmbiguity = siblingAmbiguity;
            this.caseInsensitive = caseInsensitive;
        }

        /// <summary>
        /// Creates a copy of the current <see cref="LiteralStringTokenItem"/>.
        /// </summary>
        /// <returns>A new <see cref="LiteralStringTokenItem"/> with the data
        /// members of the current <see cref="LiteralStringTokenItem"/>.</returns>
        protected override object OnClone()
        {
            LiteralStringTokenItem lsti = new LiteralStringTokenItem(base.Value, this.CaseInsensitive, base.Column, base.Line, base.Position, siblingAmbiguity);
            base.CloneData((IScannableEntryItem)lsti);
            return lsti;
        }

        #region ILiteralStringTokenItem Members

        /// <summary>
        /// Creates a copy of the current <see cref="LiteralStringTokenItem"/>.
        /// </summary>
        /// <returns>A new <see cref="LiteralStringTokenItem"/> with the data
        /// members of the current <see cref="LiteralStringTokenItem"/>.</returns>
        public new ILiteralStringTokenItem Clone()
        {
            return ((ILiteralStringTokenItem)(base.Clone()));
        }

        /// <summary>
        /// Returns whether the <see cref="LiteralStringTokenItem"/>'s value is
        /// case-insensitive.
        /// </summary>
        public bool CaseInsensitive
        {
            get { return this.caseInsensitive; }
        }

        public bool SiblingAmbiguity
        {
            get { return this.siblingAmbiguity; }
            internal set { this.siblingAmbiguity = value; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("@{3}{0}{1}{2}", GrammarCore.EncodePrim(this.Value), base.ToString(), this.siblingAmbiguity ? "**" : string.Empty, this.CaseInsensitive ? "@" : string.Empty);
        }

    }
}
