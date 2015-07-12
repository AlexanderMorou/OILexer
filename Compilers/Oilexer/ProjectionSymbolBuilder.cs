using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class ProjectionSymbolBuilder :
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
            this.resultInterface = this.assembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}PredictionContext", compiler.Source.Options.AssemblyName);
            resultInterface.AccessLevel = AccessLevelModifiers.Public;
            return this.resultInterface;
        }

        public IIntermediateInterfaceType ILanguageProjectionContext { get { return this.resultInterface; } }
    }
}
