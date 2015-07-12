using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with a language's
    /// symbols.
    /// </summary>
    public interface IGrammarSymbolSet :
        IControlledCollection<IGrammarSymbol>
    {
        IGrammarSymbol this[IOilexerGrammarTokenEntry entry] { get; }
        IGrammarSymbol this[ILiteralTokenItem entry] { get; }
        IGrammarSymbol this[IOilexerGrammarProductionRuleEntry entry] { get; }

        //void EstablishContextualAmbiguity(IOilexerGrammarTokenEntry ambiguousTrigger, Tuple<IOilexerGrammarTokenEntry, ITokenItem[]>[] ambiguities);
    }
}
