using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public class PreprocessorEntryContainer :
        PreprocessorDirectiveBase,
        IPreprocessorEntryContainer
    {
        public PreprocessorEntryContainer(IOilexerGrammarEntry contained, int column, int line, long position)
            : base(column, line, position)
        {
            this.Contained = contained;
        }

        //#region IPreprocessorEntryContainer Members

        public IOilexerGrammarEntry Contained { get; private set; }

        //#endregion

        public override EntryPreprocessorType Type
        {
            get { return EntryPreprocessorType.EntryContainer; }
        }
    }
}
