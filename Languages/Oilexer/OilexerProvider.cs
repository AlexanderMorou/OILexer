using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Slf.Translation;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public sealed partial class OilexerProvider :
        IHighLevelLanguageProvider<IGDFile>
    {
        private AltParser parser;
        private AltParserBuilder builder;

        internal OilexerProvider() { }

        #region IHighLevelLanguageProvider<IGDFile> Members

        public ILanguageParser<IGDFile> Parser
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
            get {
                if (this.builder == null)
                    this.builder = new AltParserBuilder();
                return this.builder;
            }
        }

        ILanguageCSTTranslator<IGDFile> IHighLevelLanguageProvider<IGDFile>.CSTTranslator
        {
            get
            {
                return this.CSTTranslator;
            }
        }

        public IIntermediateCodeTranslator Translator
        {
            get { throw new NotSupportedException(); }
        }

        public IHighLevelLanguage<IGDFile> Language
        {
            get { return OilexerLanguage.LanguageInstance; }
        }

        #endregion

        #region IHighLevelLanguageProvider<IGDFile> Members

        public IAnonymousTypePatternAid AnonymousTypePattern
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ILanguageProvider Members

        public Oil.IIntermediateAssembly CreateAssembly(string name)
        {
            throw new NotImplementedException();
        }

        ILanguage ILanguageProvider.Language
        {
            get { return this.Language; }
        }

        #endregion
    }
}
