using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class StackContextBuilder :
        IConstructBuilder<Tuple<ParserCompiler, CommonSymbolBuilder, IIntermediateAssembly>, IIntermediateInterfaceType>
    {
        private ParserCompiler compiler;
        private CommonSymbolBuilder commonSymbol;
        private IIntermediateAssembly assembly;
        private IIntermediateInterfaceType resultInterface;

        public IIntermediateInterfaceType Build(Tuple<ParserCompiler, CommonSymbolBuilder, IIntermediateAssembly> input)
        {
            this.compiler = input.Item1;
            this.commonSymbol = input.Item2;
            this.assembly = input.Item3;
            this.resultInterface = this.assembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}StackContext", compiler.Source.Options.AssemblyName);
            this.resultInterface.AccessLevel = AccessLevelModifiers.Public;
            this.resultInterface.ImplementedInterfaces.Add(commonSymbol.ILanguageSymbol);
            this.resultInterface.ImplementedInterfaces.Add(((IGenericType)this.assembly.IdentityManager.ObtainTypeReference(this.assembly.IdentityManager.ObtainTypeReference(RuntimeCoreType.RootType).Assembly.UniqueIdentifier.GetTypeIdentifier("System.Collections.Generic", "IEnumerable", 1))).MakeGenericClosure(commonSymbol.ILanguageSymbol));
            this.IndexerImpl = this.resultInterface.Indexers.Add(this.compiler.CommonSymbolBuilder.ILanguageSymbol, new TypedNameSeries(new TypedName("index", RuntimeCoreType.Int32, input.Item3.IdentityManager)), true, false);

            return resultInterface;
        }

        public IIntermediateInterfaceType ILanguageStackContext { get { return this.resultInterface; } }

        public Ast.Members.IIntermediateInterfaceIndexerMember IndexerImpl { get; set; }
    }
}
