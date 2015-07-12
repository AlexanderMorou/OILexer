using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Ast.Members;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf.Compilers.Oilexer
{
    /// <summary>
    /// Provides a common symbol builder which acts as the point of
    /// commonality between tokens, rules and predictions.
    /// </summary>
    public class CommonSymbolBuilder :
        IConstructBuilder<Tuple<ParserCompiler, GrammarVocabularyModelBuilder, IIntermediateAssembly>, IIntermediateInterfaceType>
    {
        private ParserCompiler compiler;
        private GrammarVocabularyModelBuilder grammarModel;
        private IIntermediateAssembly initialAssembly;
        private IIntermediateInterfacePropertyMember startTokenIndex;
        private IIntermediateInterfacePropertyMember tokenCount;
        private IIntermediateInterfacePropertyMember identity;
        private IIntermediateInterfacePropertyMember length;
        private IIntermediateInterfaceType result;

        public IIntermediateInterfaceType Build(Tuple<ParserCompiler, GrammarVocabularyModelBuilder, IIntermediateAssembly> input)
        {
            this.compiler = input.Item1;
            this.grammarModel = input.Item2;
            this.initialAssembly = input.Item3;

            var result = initialAssembly.DefaultNamespace.Parts.Add().Interfaces.Add("I{0}Symbol", this.compiler.Source.Options.AssemblyName);
            this.startTokenIndex = BuildStartTokenIndex(result);
            this.tokenCount = result.Properties.Add(new TypedName("TokenCount", RuntimeCoreType.Int32, initialAssembly.IdentityManager, initialAssembly), true, false);
            this.identity = result.Properties.Add(new TypedName("Identity", grammarModel.IdentityEnum), true, false);
            this.length = result.Properties.Add(new TypedName("Length", RuntimeCoreType.Int32, initialAssembly.IdentityManager, initialAssembly), true, false);
            this.tokenCount.SummaryText = string.Format("Returns the @s:Int32; value denoting the number of tokens consumed by the @s:{0};", result.Name);
            this.identity.SummaryText = string.Format("Returns the @s:{0}; value denoting the identity represented by the @s:{1};", grammarModel.IdentityEnum.Name, result.Name);
            this.identity.RemarksText = string.Format("Yields @s:{0}.None; when the symbol represents a @s:I{1}ProjectionContext;.", grammarModel.IdentityEnum.Name, this.compiler.Source.Options.AssemblyName);
            this.length.SummaryText = string.Format("Returns the @s:UInt64; value which denotes the length, in characters, represented by the @s:{0};.", result.Name);
            result.AccessLevel = AccessLevelModifiers.Public;
            this.result = result;
            return result;
        }

        private IIntermediateInterfacePropertyMember BuildStartTokenIndex(IIntermediateInterfaceType result)
        {
            var startTokenIndex = result.Properties.Add(new TypedName("StartTokenIndex", RuntimeCoreType.Int32, initialAssembly.IdentityManager, initialAssembly), true, false);
            startTokenIndex.SummaryText = string.Format("Returns the @s:Int32; value denoting the index within the @s:I{1}SymbolStream; the @s:{0}; resides within.", result.Name, this.compiler.Source.Options.AssemblyName);
            return startTokenIndex;
        }

        public IIntermediateInterfaceType ILanguageSymbol { get { return this.result; } }

        public IIntermediateInterfacePropertyMember StartTokenIndex
        {
            get
            {
                return this.startTokenIndex;
            }
        }

        public IIntermediateInterfacePropertyMember TokenCount
        {
            get
            {
                return this.tokenCount;
            }
        }

        public IIntermediateInterfacePropertyMember Identity
        {
            get
            {
                return this.identity;
            }
        }

        public IIntermediateInterfacePropertyMember Length
        {
            get
            {
                return this.length;
            }
        }
    }
}
