using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public class RegularLanguageDFARootState :
        RegularLanguageDFAState
    {
        private IOilexerGrammarTokenEntry entry;

        public RegularLanguageDFARootState(IOilexerGrammarTokenEntry entry)
        {
            this.entry = entry;
        }

        internal IOilexerGrammarTokenEntry Entry
        {
            get
            {
                return this.entry;
            }
        }
    }
}
