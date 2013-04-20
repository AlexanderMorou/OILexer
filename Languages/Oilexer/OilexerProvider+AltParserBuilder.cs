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
            ParserBuilder,
            ILanguageCSTTranslator<IGDFile>
        {
            public AltParserBuilder()
            {
            }


            #region ILanguageCSTTranslator<IGDFile> Members

            public void Process(IGDFile nextInput, IIntermediateAssembly currentAssembly)
            {
                throw new NotSupportedException("Inputs are linked explicitly in grammar files, next input processing is not supported.");
            }

            #endregion

            #region ILanguageProcessor<IIntermediateAssembly,IGDFile> Members

            public IIntermediateAssembly Process(IGDFile input)
            {
                this.Initialize(input, new List<string>());
                this.BuildProject();
                return this.Project;
            }

            #endregion
        }
    }
}
