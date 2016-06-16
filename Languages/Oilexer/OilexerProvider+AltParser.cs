using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    partial class OilexerProvider
    {
        internal class AltParser :
            OilexerParser,
            ILanguageParser<IOilexerGrammarFile>
        {
            internal AltParser(bool parseIncludes = true, bool captureRegions = false, IList<IToken> originalFormTokens = null)
                : base(parseIncludes, captureRegions, originalFormTokens)
            {
                
            }

            //#region ILanguageProcessor<IParserResults<IOilexerGrammarFile>,Stream,string> Members

            public IParserResults<IOilexerGrammarFile> Process(Stream input, string context)
            {
                return base.Parse(input, context);
            }

            //#endregion

            //#region ILanguageProcessor<IParserResults<IOilexerGrammarFile>,FileInfo> Members

            public IParserResults<IOilexerGrammarFile> Process(FileInfo input)
            {
                return base.Parse(input.FullName);
            }

            //#endregion
        }
    }
}
