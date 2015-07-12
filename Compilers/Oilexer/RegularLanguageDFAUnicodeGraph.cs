using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    /// <summary>
    /// Provides a basic scaffolding to generate the logical flow of unicode character sets observed in a lexical state.
    /// </summary>
    /// <remarks>Put simply, you get a mapping between <see cref="UnicodeCategory"/> and a given set of <see cref="RegularLanguageDFAState"/> instances.</remarks>
    internal class RegularLanguageDFAUnicodeGraph :
        Dictionary<RegularLanguageSet, RegularLanguageDFAState>
    {
        public IUnicodeTargetGraph UnicodeGraph { get; internal set; }
    }
}
