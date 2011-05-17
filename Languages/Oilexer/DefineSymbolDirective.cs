using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public class PreprocessorDefineSymbolDirective :
        PreprocessorDirectiveBase,
        IPreprocessorDefineSymbolDirective
    {

        public PreprocessorDefineSymbolDirective(string symbolName, string Value, int column, int line, long position)
            : this(symbolName, column, line, position)
        {
            this.Value = Value;
        }

        public PreprocessorDefineSymbolDirective(string symbolName, int column, int line, long position)
            : base(column, line, position)
        {
            this.SymbolName = symbolName;
        }

        public override EntryPreprocessorType Type
        {
            get { return EntryPreprocessorType.DefineSymbol; }
        }

        #region IPreprocessorDefineSymbolDirective Members

        public string SymbolName { get; private set; }

        public string Value { get; private set; }

        #endregion
    }
}
