using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
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
