using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    internal class TokenEqualityLevel :
        List<OilexerGrammarTokenEntry>
    {
        public TokenEqualityLevel(IEnumerable<OilexerGrammarTokenEntry> set)
            : base(set)
        {
        }

        public TokenEqualityLevel()
        {

        }
        
        new public void Sort()
        {
            OilexerGrammarTokenEntry[] items = (from i in this
                                         orderby i.Name
                                         select i).ToArray();
            this.Clear();
            this.AddRange(items);
        }
        public int Index { get; internal set; }
    }
}
