using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Slf.Ast;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    partial class OilexerProvider
    {
        public class AltParserBuilder :
            ParserCompiler,
            ILanguageCSTTranslator<IOilexerGrammarFile>
        {
            public AltParserBuilder()
            {
            }

            //#region ILanguageCSTTranslator<IOilexerGrammarFile> Members

            public void Process(IOilexerGrammarFile nextInput, IIntermediateAssembly currentAssembly)
            {
                throw new NotSupportedException("Inputs are linked explicitly in grammar files, next input processing is not supported.");
            }

            //#endregion

            //#region ILanguageProcessor<IIntermediateAssembly,IOilexerGrammarFile> Members

            public IIntermediateAssembly Process(IOilexerGrammarFile input)
            {
                this.Initialize(input, new List<string>(), false, false);
                this.BuildProject();
                return this.ResultAssembly;
            }

            //#endregion
        }
    }
}
