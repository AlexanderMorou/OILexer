using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public class PreprocessorStringTerminalDirective :
        PreprocessorDirectiveBase,
        IPreprocessorStringTerminalDirective
    {
        
        public PreprocessorStringTerminalDirective(StringTerminalKind kind, string literal, int column, int line, long position)
            : base(column, line, position)
        {
            this.Kind = kind;
            this.Literal = literal;
        }
        public override EntryPreprocessorType Type
        {
            get { return EntryPreprocessorType.StringTerminal; }
        }

        #region IPreprocessorStringTerminalDirective Members

        public string Literal { get; private set; }

        #endregion

        public StringTerminalKind Kind { get; private set; }
    }
}
