using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    /// <summary>
    /// Provides a base implementation of <see cref="ISoftReferenceTokenItem"/> 
    /// which is a soft reference whose validity is unconfirmed.
    /// </summary>
    public class SoftReferenceTokenItem :
        TokenItem,
        ISoftReferenceTokenItem
    {
        /// <summary>
        /// Data member for <see cref="PrimaryName"/>.
        /// </summary>
        private string primaryName;
        /// <summary>
        /// Data member for <see cref="SecondaryName"/>.
        /// </summary>
        private string secondaryName;

        /// <summary>
        /// Creates a new <see cref="SoftReferenceTokenItem"/> with the <paramref name="primaryName"/>.
        /// <paramref name="secondaryName"/>, <paramref name="column"/>, <paramref name="line"/>, 
        /// and <paramref name="position"/> provided.
        /// </summary>
        /// <param name="primaryName">The token, declaration rule, or error that the <see cref="SoftReferenceTokenItem"/>
        /// refers to.</param>
        /// <param name="secondaryName">The member of the token or production rule the <see cref="SoftReferenceTokenItem"/>
        /// refers to.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="SoftReferenceTokenItem"/> was
        /// defined.</param>
        /// <param name="line">The line at which the <see cref="SoftReferenceTokenItem"/> was defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="SoftReferenceTokenItem"/> was declared.</param>
        /// <remarks><paramref name="secondaryName"/> does not relate to errors because
        /// errors have no members.</remarks>
        public SoftReferenceTokenItem(string primaryName, string secondaryName, int column, int line, long position)
            : base(column, line, position)
        {
            this.primaryName = primaryName;
            this.secondaryName = secondaryName;
        }

        /// <summary>
        /// Creates a copy of the current <see cref="SoftReferenceTokenItem"/>.
        /// </summary>
        /// <returns>A new <see cref="SoftReferenceTokenItem"/> with the data
        /// members of the current <see cref="SoftReferenceTokenItem"/>.</returns>
        protected override object OnClone()
        {
            SoftReferenceTokenItem srpri = new SoftReferenceTokenItem(this.PrimaryName, this.SecondaryName, this.Column, this.Line, this.Position)
            {
                PrimaryToken = this.PrimaryToken,
                SecondaryToken = this.SecondaryToken
            };
            base.CloneData(srpri);
            return srpri;
        }

        #region ISoftReferenceTokenItem Members
        /// <summary>
        /// Returns the name of the target the <see cref="SoftReferenceTokenItem"/> refers
        /// to.
        /// </summary>
        public string PrimaryName
        {
            get { return this.primaryName; }
        }

        /// <summary>
        /// Returns the name of the member in the target (<see cref="PrimaryName"/>).
        /// </summary>
        /// <remarks>Can be null if the <see cref="SoftReferenceTokenItem"/> is
        /// a primary reference.</remarks>
        public string SecondaryName
        {
            get { return this.secondaryName; }
        }


        #endregion

        public override string ToString()
        {
            if (this.SecondaryName != null && this.SecondaryName != string.Empty)
                return string.Format("{0}.{1}{2}", this.PrimaryName, this.SecondaryName, this.RepeatOptions);
            else
                return string.Format("{0}{1}", this.PrimaryName, this.RepeatOptions);
        }

        public GDTokens.IdentifierToken PrimaryToken { get; internal set; }

        public GDTokens.IdentifierToken SecondaryToken { get; internal set; }

    }
}
