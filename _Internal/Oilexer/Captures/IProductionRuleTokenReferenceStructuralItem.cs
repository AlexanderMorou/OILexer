using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal interface IProductionRuleCaptureReferenceStructuralItem<TRef> :
        IProductionRuleCaptureStructuralItem
    {
        TRef[] References { get; }
    }
    internal interface IProductionRuleCaptureReferenceStructuralItem :
        IProductionRuleCaptureReferenceStructuralItem<IRuleReferenceProductionRuleItem>
    {

    }
    internal interface IProductionRuleTokenReferenceStructuralItem :
        IProductionRuleCaptureReferenceStructuralItem<ITokenReferenceProductionRuleItem>
    {
    }
    internal interface IProductionRuleLiteralTokenItemReferenceStructuralItem :
        IProductionRuleCaptureReferenceStructuralItem<ILiteralReferenceProductionRuleItem>
    {
        IOilexerGrammarTokenEntry[] SourceEntries { get; }
    }

}
