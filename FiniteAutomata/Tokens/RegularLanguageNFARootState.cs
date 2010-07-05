using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;

namespace Oilexer.FiniteAutomata.Tokens
{
    public class RegularLanguageNFARootState :
        RegularLanguageNFAState
    {
        private ITokenEntry entry;
        public RegularLanguageNFARootState(ITokenEntry entry)
            : base()
        {
            this.entry = entry;
        }
        protected override RegularLanguageDFAState GetRootDFAState()
        {
            return new RegularLanguageDFARootState(this.entry);
        }

        /// <summary>
        /// Creates a version of the current
        /// <see cref="RegularLanguageNFARootState"/> which is 
        /// deterministic by creating a left-side union on elements
        /// which overlap on their <see cref="RegularLanguageSet"/> 
        /// transition requirements.
        /// </summary>
        /// <returns>A new <see cref="RegularLanguageDFARootState"/> 
        /// instance which represents the current automation
        /// in a deterministic manner.</returns>
        public new RegularLanguageDFARootState DeterminateAutomata()
        {
            return (RegularLanguageDFARootState)base.DeterminateAutomata();
        }

        public ITokenEntry Source { get { return this.entry; } }
    }
}
