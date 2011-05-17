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
    /// a <see cref="System.Char"/> literal defined in an <see cref="ITokenEntry"/>.
    /// </summary>
    public class LiteralCharTokenItem :
        LiteralTokenItem<char>,
        ILiteralCharTokenItem
    {
        /// <summary>
        /// Data member for <see cref="CaseInsensitive"/>.
        /// </summary>
        private bool caseInsensitive;
        /// <summary>
        /// Creates a new <see cref="LiteralCharTokenItem"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="value">The value the <see cref="LiteralCharTokenItem"/> represents.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="LiteralCharTokenItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="LiteralCharTokenItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="LiteralCharTokenItem"/> was declared.</param>
        public LiteralCharTokenItem(char value, bool caseInsensitive, int column, int line, long position)
            : base(value, column, line, position)
        {
            this.caseInsensitive=caseInsensitive;
        }

        /// <summary>
        /// Creates a copy of the current <see cref="LiteralCharTokenItem"/>.
        /// </summary>
        /// <returns>A new <see cref="LiteralCharTokenItem"/> with the data
        /// members of the current <see cref="LiteralCharTokenItem"/>.</returns>
        protected override object OnClone()
        {
            LiteralCharTokenItem lsti = new LiteralCharTokenItem(base.Value, this.CaseInsensitive, base.Column, base.Line, base.Position);
            base.CloneData((IScannableEntryItem)lsti);
            return lsti;
        }

        #region ILiteralCharTokenItem Members

        /// <summary>
        /// Creates a copy of the current <see cref="LiteralCharTokenItem"/>.
        /// </summary>
        /// <returns>A new <see cref="LiteralCharTokenItem"/> with the data
        /// members of the current <see cref="LiteralCharTokenItem"/>.</returns>
        public new ILiteralCharTokenItem Clone()
        {
            return ((ILiteralCharTokenItem)(base.Clone()));
        }
        /// <summary>
        /// Returns whether the <see cref="LiteralCharTokenItem"/>'s value is
        /// case-insensitive.
        /// </summary>
        public bool CaseInsensitive
        {
            get { return this.caseInsensitive; }
        }

        #endregion
        public override string ToString()
        {
            return string.Format("{2}{0}{1}", GrammarCore.EncodePrim(this.Value), base.ToString(), this.CaseInsensitive ? "@" : string.Empty);
        }

    }
}
