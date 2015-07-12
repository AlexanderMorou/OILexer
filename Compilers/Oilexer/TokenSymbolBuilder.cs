using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Cli;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    public class TokenSymbolBuilder :
        IConstructBuilder<Tuple<ParserCompiler, CommonSymbolBuilder, IIntermediateAssembly>, IIntermediateInterfaceType>
    {
        private ParserCompiler compiler;
        private CommonSymbolBuilder rootSymbol;
        private IIntermediateAssembly assembly;
        private IIntermediateInterfaceType resultInterface;
        private IIntermediateCliManager _identityManager;
        
        public IIntermediateInterfaceType Build(Tuple<ParserCompiler, CommonSymbolBuilder, IIntermediateAssembly> input)
        {
            this.compiler = input.Item1;
            this.rootSymbol = input.Item2;
            this.assembly = input.Item3;
            this._identityManager = (IIntermediateCliManager)this.assembly.IdentityManager;
            INamespaceDeclaration targetSpace;
            var targetSpaceName = TypeSystemIdentifiers.GetDeclarationIdentifier(string.Format("{0}.Cst", this.assembly.DefaultNamespace.FullName));
            if (!assembly.Namespaces.PathExists(targetSpaceName.Name))
                targetSpace = this.assembly.DefaultNamespace.Namespaces.Add("Cst");
            else
                targetSpace = this.assembly.DefaultNamespace.Namespaces[targetSpaceName];
            var mutableTargetSpace = (IIntermediateNamespaceDeclaration)targetSpace;
            this.resultInterface = mutableTargetSpace.Parts.Add().Interfaces.Add("I{0}Token", compiler.Source.Options.AssemblyName);
            this.resultInterface.ImplementedInterfaces.Add(this.rootSymbol.ILanguageSymbol);
            this.resultInterface.AccessLevel = AccessLevelModifiers.Public;
            this.BuildStartPosition();
            this.BuildEndPosition();
            this.BuildStartTokenIndex();
            return this.resultInterface;
        }

        private void BuildStartTokenIndex()
        {
            this.StartTokenIndex = this.resultInterface.Properties.Add(
                new TypedName("StartTokenIndex", RuntimeCoreType.Int32, this._identityManager), true, true);
        }

        private void BuildStartPosition()
        {
            this.StartPosition = this.resultInterface.Properties.Add(
                new TypedName("StartPosition", RuntimeCoreType.Int32, this._identityManager), true, false);
        }

        private void BuildEndPosition()
        {
            this.EndPosition = this.resultInterface.Properties.Add(
                new TypedName("EndPosition", RuntimeCoreType.Int32, this._identityManager), true, false);
        }

        public IIntermediateInterfaceType ILanguageToken { get { return this.resultInterface; } }

        public CommonSymbolBuilder CommonSymbolBuilder { get { return this.rootSymbol; } }


        public IIntermediateInterfacePropertyMember StartTokenIndex { get; set; }

        public IIntermediateInterfacePropertyMember StartPosition { get; set; }

        public IIntermediateInterfacePropertyMember EndPosition { get; set; }
    }
}
