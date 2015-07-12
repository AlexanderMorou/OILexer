using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Provides a base implementation of <see cref="ISoftReferenceProductionRuleItem"/> 
    /// which is a soft reference whose validity is unconfirmed.
    /// </summary>
    public class SoftReferenceProductionRuleItem :
        ProductionRuleItem,
        ISoftReferenceProductionRuleItem
    {
        /// <summary>
        /// Data member for <see cref="PrimaryName"/>.
        /// </summary>
        private string primaryName;
        /// <summary>
        /// Data member for <see cref="SecondaryName"/>.
        /// </summary>
        private string secondaryName;

        private bool isFlag;
        private bool isCounter;
       
        /// <summary>
        /// Creates a new <see cref="SoftReferenceProductionRuleItem"/> instance
        /// with the <paramref name="primaryName"/>, <paramref name="secondaryName"/>,
        /// <paramref name="line"/>, <paramref name="column"/>, and <paramref name="position"/>
        /// provided.
        /// </summary>
        /// <param name="primaryName">The token, declaration rule, or error that the <see cref="SoftReferenceProductionRuleItem"/>
        /// refers to.</param>
        /// <param name="secondaryName">The member of the token or production rule the <see cref="SoftReferenceProductionRuleItem"/>
        /// refers to.</param>
        /// <param name="line">The line at which the <see cref="SoftReferenceProductionRuleItem"/> was defined.</param>
        /// <param name="column">The column on <paramref name="line"/> at which the <see cref="SoftReferenceProductionRuleItem"/> was
        /// defined.</param>
        /// <param name="position">The byte in the file at which the <see cref="SoftReferenceProductionRuleItem"/> was declared.</param>
        /// <remarks><paramref name="secondaryName"/> does not relate to errors because
        /// errors have no members.</remarks>
        public SoftReferenceProductionRuleItem(string primaryName, string secondaryName, int line, int column, long position, bool isFlag, bool isCounter)
            : base(column, line, position)
        {
            this.isFlag = isFlag;
            this.isCounter = isCounter;
            this.primaryName = primaryName;
            this.secondaryName = secondaryName;
        }

        /// <summary>
        /// Creates a copy of the current <see cref="SoftReferenceProductionRuleItem"/>.
        /// </summary>
        /// <returns>A new <see cref="SoftReferenceProductionRuleItem"/> with the data
        /// members of the current <see cref="SoftReferenceProductionRuleItem"/>.</returns>
        protected override object OnClone()
        {
            SoftReferenceProductionRuleItem srpri = new SoftReferenceProductionRuleItem(this.PrimaryName, this.SecondaryName, this.Line, this.Column, this.Position, this.isFlag, this.isCounter)
                {
                    PrimaryToken = this.PrimaryToken,
                    SecondaryToken = this.SecondaryToken
                };
            base.CloneData(srpri);
            return srpri;
        }

        //#region ISoftReferenceProductionRuleItem Members

        /// <summary>
        /// Returns the name of the target the <see cref="SoftReferenceProductionRuleItem"/> refers
        /// to.</summary>
        public string PrimaryName
        {
            get { return this.primaryName; }
        }

        /// <summary>
        /// Returns the name of the member in the target (<see cref="PrimaryName"/>).
        /// </summary>
        /// <remarks>Can be null if the <see cref="SoftReferenceProductionRuleItem"/> is
        /// a primary reference.</remarks>
        public string SecondaryName
        {
            get { return this.secondaryName; }
        }

        //#endregion
        public bool IsFlag
        {
            get { return this.isFlag; }
        }

        public bool Counter
        {
            get
            {
                return this.isCounter;
            }
        }

        public override string ToString()
        {
            if (this.SecondaryName != null && this.SecondaryName != string.Empty)
                return string.Format("{0}.{1}", this.PrimaryName, this.SecondaryName);
            else
                return this.PrimaryName;
        }

        public OilexerGrammarTokens.IdentifierToken PrimaryToken { get; internal set; }

        public OilexerGrammarTokens.IdentifierToken SecondaryToken { get; internal set; }

        public IOilexerGrammarProductionRuleEntry Rule { get; internal set; }
    }
}
