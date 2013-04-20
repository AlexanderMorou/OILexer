using AllenCopeland.Abstraction.Slf.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    internal class OilexerAssembly :
        IntermediateAssembly<OilexerLanguage, OilexerProvider, OilexerAssembly, IIntermediateCliManager, Type, Assembly>
    {
        private OilexerProvider provider;
        internal OilexerAssembly(string name, OilexerProvider provider)
            : base(name)
        {
            this.provider = provider;
        }
        internal OilexerAssembly(OilexerAssembly rootAssembly)
            : base(rootAssembly)
        {
        }
        protected override OilexerAssembly GetNewPart()
        {
            return new OilexerAssembly(this);
        }

        public override OilexerLanguage Language
        {
            get { return OilexerLanguage.LanguageInstance; }
        }

        public override sealed OilexerProvider Provider
        {
            get
            {
                if (this.IsRoot)
                    return this.provider;
                else
                    return this.GetRoot().provider;
            }
        }
    }
}
