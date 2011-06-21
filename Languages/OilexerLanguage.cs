using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Oil;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages
{
    /// <summary>
    /// Provides information about the OILexer language which reads in 
    /// a series of grammar description files and produces an assembly as
    /// a result.
    /// </summary>
    public class OilexerLanguage :
        IHighLevelLanguage<IGDFile>
    {
        private static Guid _LanguageGuid = new Guid(0xED13FCAD, 0xE20F, 0x4C81, 0xA6, 0xFE, 0xAF, 0xAD, 0xE2, 0x99, 0xB9, 0xC0);
        private OilexerLanguage() {
        }
        /// <summary>
        /// The singleton instance which which provides information about the language.
        /// </summary>
        internal static OilexerLanguage LanguageInstance = new OilexerLanguage();
        #region IHighLevelLanguage<IGDFile> Members

        public IHighLevelLanguageProvider<IGDFile> GetProvider()
        {
            return OilexerProvider.ProviderInstance;
        }

        public Guid Guid
        {
            get { return OilexerLanguage._LanguageGuid; }
        }

        #endregion

        #region ILanguage Members

        public string Name
        {
            get { return "OILexer Grammar Description Language"; }
        }

        ILanguageProvider ILanguage.GetProvider()
        {
            return this.GetProvider();
        }

        #endregion

        #region ILanguage Members

        public CompilerSupport CompilerSupport
        {
            get { return CompilerSupport.FullSupport ^ (CompilerSupport.DebuggerSupport | CompilerSupport.COMInterop | CompilerSupport.Unsafe | CompilerSupport.Win32Resources); }
        }

        public ILanguageVendor Vendor
        {
            get { return LanguageVendors.AllenCopeland; }
        }

        #endregion

        #region ILanguage Members

        public IIntermediateAssembly CreateAssembly(string name)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
