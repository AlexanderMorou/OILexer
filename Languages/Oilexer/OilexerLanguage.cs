using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Cli;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
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

        public OilexerProvider GetProvider()
        {
            return new OilexerProvider(IntermediateCliGateway.CreateIdentityManager(CliGateway.CurrentPlatform, CliGateway.CurrentVersion));
        }

        public Guid Guid
        {
            get { return OilexerLanguage._LanguageGuid; }
        }

        //#region ILanguage Members

        public string Name
        {
            get { return "OILexer Grammar Description Language"; }
        }

        ILanguageProvider ILanguage.GetProvider()
        {
            return this.GetProvider();
        }

        public CompilerSupport CompilerSupport
        {
            get 
            {
                return CompilerSupport.FullSupport      ^
                    (
                        CompilerSupport.DebuggerSupport |
                        CompilerSupport.COMInterop      |
                        CompilerSupport.Unsafe          |
                        CompilerSupport.Win32Resources
                    );
            }
        }

        public ILanguageVendor Vendor
        {
            get { return LanguageVendors.AllenCopeland; }
        }

        public IAssembly CreateAssembly(string name)
        {
            return new OilexerAssembly(name, GetProvider());
        }

        //#endregion

        //#region ILanguage<OilexerLanguage,OilexerProvider> Members

        OilexerProvider ILanguage<OilexerLanguage,OilexerProvider>.GetProvider(IIdentityManager identityManager)
        {
            if (!(identityManager is IIntermediateCliManager))
                throw new ArgumentException("Wrong kind of identity manager", "identityManager");
            return new OilexerProvider((IIntermediateCliManager)identityManager);
        }

        //#endregion

        public OilexerProvider GetProvider(IIntermediateCliManager identityManager)
        {
            if (!(identityManager is IIntermediateCliManager))
                throw new ArgumentException("Wrong kind of identity manager", "identityManager");
            return new OilexerProvider((IIntermediateCliManager)identityManager);
        }
    }
}
