using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Slf.Translation;
using AllenCopeland.Abstraction.Slf.Ast;
using System.Reflection;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public sealed partial class OilexerProvider :
        LanguageProvider<OilexerLanguage, OilexerProvider, IIntermediateCliManager, Type, Assembly>,
        ILanguageProvider<OilexerLanguage, OilexerProvider>
    {
        private AltParser parser;
        private AltParserBuilder builder;

        internal OilexerProvider(IIntermediateCliManager identityManager) : base(identityManager) { }

        public ILanguageParser<IOilexerGrammarFile> Parser
        {
            get
            {
                if (this.parser == null)
                    this.parser = new AltParser();
                return this.parser;
            }
        }

        public AltParserBuilder CSTTranslator
        {
            get
            {
                if (this.builder == null)
                    this.builder = new AltParserBuilder();
                return this.builder;
            }
        }

        public IIntermediateCodeTranslator Translator
        {
            get { throw new NotSupportedException(); }
        }

        protected override OilexerLanguage OnGetLanguage()
        {
            return OilexerLanguage.LanguageInstance;
        }
    }
}
