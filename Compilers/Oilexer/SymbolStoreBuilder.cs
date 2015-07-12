using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class SymbolStoreBuilder
    {
        private IIntermediateAssembly _targetAssembly;
        private ParserCompiler _compiler;
        private IIntermediateClassType _symbolStore;
        private IIntermediateClassType _singletons;
        private IIntermediateEnumType _identityEnum;

        public void Build(ParserCompiler compiler, IIntermediateAssembly targetAssembly)
        {
            this._targetAssembly = targetAssembly;
            this._compiler = compiler;
            this._symbolStore = targetAssembly.DefaultNamespace.Parts.Add().Classes.Add("{0}SymbolStore", this._compiler.Source.Options.AssemblyName);
            this._singletons = _symbolStore.Parts.Add().Classes.Add("Singletons");
            this._identityEnum = targetAssembly.DefaultNamespace.Parts.Add().Enums.Add("{0}Symbols", this._compiler.Source.Options.AssemblyName);
            _symbolStore.AccessLevel = AccessLevelModifiers.Internal;
            _symbolStore.SpecialModifier = SpecialClassModifier.Static;
            _singletons.AccessLevel = AccessLevelModifiers.Internal;
            _symbolStore.SummaryText = string.Format("Defines the used symbols for the {0} language, effectively each unique set of combinations is outlined within this symbol store.", compiler.Source.Options.GrammarName);
        }
        public IIntermediateClassType Singletons { get { return this._singletons; } }
        public IIntermediateClassType SymbolStore { get { return this._symbolStore; } }

        public IIntermediateEnumType Identities
        {
            get
            {
                return this._identityEnum;
            }
        }
    }
}
