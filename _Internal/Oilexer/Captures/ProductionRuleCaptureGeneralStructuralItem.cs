using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal class ProductionRuleCaptureGeneralStructuralItem :
        ProductionRuleCaptureReferenceStructuralItem<IProductionRuleSource>
    {
        public ProductionRuleCaptureGeneralStructuralItem(IProductionRuleSource initialReference, IOilexerGrammarProductionRuleEntry rule, ResultedDataType defaultResultType)
            : base(initialReference, rule, defaultResultType)
        {

        }
        public ProductionRuleCaptureGeneralStructuralItem(IProductionRuleSource[] references, IOilexerGrammarProductionRuleEntry rule, ResultedDataType defaultResultType)
            : base(references, rule, defaultResultType)
        {

        }

        protected override IProductionRuleCaptureStructuralItem PerformUnion(IProductionRuleCaptureStructuralItem rightElement)
        {
            return new ProductionRuleCaptureGeneralStructuralItem(this.Sources.Concat(rightElement.Sources).ToArray(), this.Rule, this.ResultType);
        }
    }
}
