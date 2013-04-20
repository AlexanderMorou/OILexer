using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Cli;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    /// <summary>
    /// Provides information about the OILexer language which reads in 
    /// a series of grammar description files and produces an assembly as
    /// a result.
    /// </summary>
    public class OilexerLanguage :
        ILanguage<OilexerLanguage, OilexerProvider>
    {
        private static Guid _LanguageGuid = LanguageGuids.Oilexer;
        private OilexerLanguage() {
        }
        /// <summary>
        /// The singleton instance which which provides information about the language.
        /// </summary>
        internal static readonly OilexerLanguage LanguageInstance = new OilexerLanguage();
        #region IHighLevelLanguage<IGDFile> Members

        public OilexerProvider GetProvider()
        {
            return new OilexerProvider(IntermediateGateway.CreateIdentityManager(CliGateway.CurrentPlatform, CliGateway.CurrentVersion));
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

        public IAssembly CreateAssembly(string name)
        {
            return new OilexerAssembly(name, GetProvider());
        }

        #endregion
    }
}
