using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllenCopeland.Abstraction.Utilities.Collections;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal abstract class ProductionRuleCaptureReferenceStructuralItem<TRef> :
        ProductionRuleCaptureStructuralItem,
        IProductionRuleCaptureReferenceStructuralItem<TRef>
    {
        private ControlledCollection<IProductionRuleSource> sources;
        private TRef[] references;

        public ProductionRuleCaptureReferenceStructuralItem(TRef initialReference, IOilexerGrammarProductionRuleEntry rule, ResultedDataType defaultResultType)
            : this(new TRef[1] { initialReference }, rule, defaultResultType)
        {
        }

        public ProductionRuleCaptureReferenceStructuralItem(TRef[] references, IOilexerGrammarProductionRuleEntry rule, ResultedDataType defaultResultType)
            : base(rule)
        {
            this.ResultType = defaultResultType;
            this.references = references;
        }

        public TRef[] References
        {
            get
            {
                return this.references;
            }
        }


        protected override sealed IControlledCollection<IProductionRuleSource> OnGetSources()
        {
            return sources ?? (this.sources = new ControlledCollection<IProductionRuleSource>(this.references.Cast<IProductionRuleSource>().ToArray()));
        }

    }

    internal class ProductionRuleCaptureReferenceStructuralItem :
        ProductionRuleCaptureReferenceStructuralItem<IRuleReferenceProductionRuleItem>,
        IProductionRuleCaptureReferenceStructuralItem
    {
        internal ProductionRuleCaptureReferenceStructuralItem(IRuleReferenceProductionRuleItem initialReference, IOilexerGrammarProductionRuleEntry rule)
            : base(initialReference, rule, ResultedDataType.ImportType)
        {
        }

        internal ProductionRuleCaptureReferenceStructuralItem(IRuleReferenceProductionRuleItem[] references, IOilexerGrammarProductionRuleEntry rule)
            : base(references, rule, ResultedDataType.ImportType)
        {
        }

        protected override IProductionRuleCaptureStructuralItem PerformUnion(IProductionRuleCaptureStructuralItem rightElement)
        {
            if (rightElement is IProductionRuleCaptureReferenceStructuralItem)
                return new ProductionRuleCaptureReferenceStructuralItem(this.References.Concat(rightElement.Sources.Cast<IRuleReferenceProductionRuleItem>()).ToArray(), this.Rule)
                    {
                        ResultType = UnionResultTypes(rightElement)
                    };
            return new ProductionRuleCaptureGeneralStructuralItem(this.Sources.Concat(rightElement.Sources).ToArray(), this.Rule, this.ResultType);
        }
    }

    internal class ProductionRuleTokenReferenceStructuralItem :
        ProductionRuleCaptureReferenceStructuralItem<ITokenReferenceProductionRuleItem>,
        IProductionRuleTokenReferenceStructuralItem
    {
        internal ProductionRuleTokenReferenceStructuralItem(ITokenReferenceProductionRuleItem initialReference, IOilexerGrammarProductionRuleEntry rule)
            : base(initialReference, rule, ResultedDataType.ImportType)
        {
        }
        internal ProductionRuleTokenReferenceStructuralItem(ITokenReferenceProductionRuleItem[] references, IOilexerGrammarProductionRuleEntry rule)
            : base(references, rule, ResultedDataType.ImportType)
        {
        }

        protected override IProductionRuleCaptureStructuralItem PerformUnion(IProductionRuleCaptureStructuralItem rightElement)
        {
            if (rightElement is IProductionRuleTokenReferenceStructuralItem)
                return new ProductionRuleTokenReferenceStructuralItem(this.References.Concat(rightElement.Sources.Cast<ITokenReferenceProductionRuleItem>()).ToArray(), this.Rule)
                    {
                        ResultType = UnionResultTypes(rightElement),
                    };
            return new ProductionRuleCaptureGeneralStructuralItem(this.Sources.Concat(rightElement.Sources).ToArray(), this.Rule, this.ResultType);
        }

    }

    internal class ProductionRuleLiteralTokenItemReferenceStructuralItem :
        ProductionRuleCaptureReferenceStructuralItem<ILiteralReferenceProductionRuleItem>,
        IProductionRuleLiteralTokenItemReferenceStructuralItem
    {
        internal ProductionRuleLiteralTokenItemReferenceStructuralItem(IOilexerGrammarTokenEntry initialSourceEntry, ILiteralReferenceProductionRuleItem initialReference, IOilexerGrammarProductionRuleEntry rule)
            : base(initialReference, rule, ResultedDataType.ImportType)
        {
            this.SourceEntries = new IOilexerGrammarTokenEntry[1] { initialSourceEntry };
        }
        internal ProductionRuleLiteralTokenItemReferenceStructuralItem(IOilexerGrammarTokenEntry[] sourceEntries, ILiteralReferenceProductionRuleItem[] references, IOilexerGrammarProductionRuleEntry rule)
            : base(references, rule, ResultedDataType.ImportType)
        {
            this.SourceEntries = sourceEntries;
        }

        protected override IProductionRuleCaptureStructuralItem PerformUnion(IProductionRuleCaptureStructuralItem rightElement)
        {
            if (rightElement is IProductionRuleLiteralTokenItemReferenceStructuralItem)
                return new ProductionRuleLiteralTokenItemReferenceStructuralItem(
                    this.SourceEntries.Concat(((IProductionRuleLiteralTokenItemReferenceStructuralItem)(rightElement)).SourceEntries).ToArray(), 
                    this.References.Concat(rightElement.Sources.Cast<ILiteralReferenceProductionRuleItem>()).ToArray(), 
                    this.Rule)
                {
                    ResultType = UnionResultTypes(rightElement),
                };
            return new ProductionRuleCaptureGeneralStructuralItem(this.Sources.Concat(rightElement.Sources).ToArray(), this.Rule, this.ResultType);
        }

        public IOilexerGrammarTokenEntry[] SourceEntries { get; private set; }
    };
}
