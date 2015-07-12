using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    internal class OilexerGrammarProductionRuleRegion :
        IOilexerGrammarRegion
    {
        private IOilexerGrammarProductionRuleEntry entry;

        public OilexerGrammarProductionRuleRegion(IOilexerGrammarProductionRuleEntry entry, long bodyStart, long bodyEnd)
        {
            this.entry = entry;
            this.Start = (int)bodyStart;
            this.End = (int)bodyEnd;
        }

        #region IOilexerGrammarRegion Members

        public int Start { get; private set; }

        public int End { get; private set; }

        public string Description
        {
            get { return this.entry.GetBodyString(); }
        }

        public string CollapseForm
        {
            get { return string.Format("..."); }
        }

        #endregion
    }
}
