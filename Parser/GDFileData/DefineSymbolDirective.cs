using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.GDFileData
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
