using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
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
        
        public PreprocessorStringTerminalDirective(StringTerminalKind kind, string literal, OilexerGrammarTokens.PreprocessorDirective directiveToken, OilexerGrammarTokens.StringLiteralToken literalToken, int column, int line, long position)
            : base(column, line, position)
        {
            this.Kind = kind;
            this.Literal = literal;
            this.LiteralToken = literalToken;
            this.KindToken = directiveToken;
        }
        public override EntryPreprocessorType Type
        {
            get { return EntryPreprocessorType.StringTerminal; }
        }

        //#region IPreprocessorStringTerminalDirective Members

        public string Literal { get; private set; }


        public StringTerminalKind Kind { get; private set; }

        public OilexerGrammarTokens.StringLiteralToken LiteralToken { get; private set; }

        public OilexerGrammarTokens.PreprocessorDirective KindToken { get; private set; }

        public string Target { get; set; }

        //#endregion

    }
}
