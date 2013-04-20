using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    internal enum SetExpansionType
    {
        None = 0,
        Complex = 1,
        Enumeration = 2,
        Reference = 4,
        Counter = 8,
        Flag = 16,
        Ignore = 32,
        Set = 64,
    }
    internal static class ExpansionCore
    {
        public static SetExpansionType DiscernProductionRuleSeriesExpansionType(this IProductionRuleSeries series)
        {
            SetExpansionType result = SetExpansionType.None;
            bool rSet = false,
                rCounter = false,
                rFlag = false;
            foreach (var rule in series)
            {
                var ruleExpansionType = rule.DiscernProductionRuleExpansionType();
                bool ruleSet = (ruleExpansionType & SetExpansionType.Set) == SetExpansionType.Set;
                bool ruleCounter = (ruleExpansionType & SetExpansionType.Counter) == SetExpansionType.Counter;
                bool ruleFlag = (ruleExpansionType & SetExpansionType.Flag) == SetExpansionType.Flag;
                ruleExpansionType &= ~(SetExpansionType.Set | SetExpansionType.Counter | SetExpansionType.Flag);


                switch (result)
                {
                    case SetExpansionType.None:
                        result = ruleExpansionType;
                        rSet = ruleSet;
                        rCounter = ruleCounter;
                        rFlag = ruleFlag;
                        break;
                    case SetExpansionType.Enumeration:
                        switch (ruleExpansionType)
                        {
                            case SetExpansionType.Complex:
                                result = SetExpansionType.Complex;
                                break;
                            case SetExpansionType.Reference:
                                result = SetExpansionType.Complex;
                                break;
                            default:
                                rSet = ruleSet;
                                rCounter = ruleCounter;
                                rFlag = ruleFlag;
                                break;
                        }
                        break;
                    case SetExpansionType.Reference:
                        switch (ruleExpansionType)
                        {
                            case SetExpansionType.Complex:
                            case SetExpansionType.Set:
                            case SetExpansionType.Enumeration:
                                result = SetExpansionType.Complex;
                                break;
                            case SetExpansionType.Reference:
                                result = SetExpansionType.Reference;
                                break;
                            default:
                                rSet = ruleSet;
                                rCounter = ruleCounter;
                                rFlag = ruleFlag;
                                break;
                        }
                        break;
                }
            }
            return result;
        }

        public static SetExpansionType DiscernProductionRuleExpansionType(this IProductionRule rule)
        {
            SetExpansionType result = SetExpansionType.None;
            foreach (var item in rule)
            {
                SetExpansionType current = SetExpansionType.None;
                if (item is IProductionRuleGroupItem)
                    current = ((IProductionRuleGroupItem)item).DiscernProductionRuleSeriesExpansionType();
                else
                    current = item.DiscernProductionRuleItemExpansionType();
            }
            return result;
        }

        public static SetExpansionType DiscernProductionRuleItemExpansionType(this IProductionRuleItem target)
        {
            if (target is IProductionRuleGroupItem)
                return ((IProductionRuleGroupItem)target).DiscernProductionRuleSeriesExpansionType();
            else if (target is IRuleReferenceProductionRuleItem ||
                     target is ILiteralCharReferenceProductionRuleItem ||
                     target is ILiteralStringReferenceProductionRuleItem ||
                     target is ITokenReferenceProductionRuleItem)
                if (target.Name == null)
                {
                    var d = target.DiscernProductionRuleItemRepeatType();
                    return d == SetExpansionType.None ? SetExpansionType.Ignore : d;
                }
                else
                    return SetExpansionType.Reference | target.DiscernProductionRuleItemRepeatType();
            return SetExpansionType.None;
        }

        public static SetExpansionType DiscernProductionRuleItemRepeatType(this IProductionRuleItem target)
        {
            ILiteralReferenceProductionRuleItem litRef = target as ILiteralReferenceProductionRuleItem;
            bool isFlag = false;
            bool isCounter = false;
            if (litRef != null)
            {
                isFlag = litRef.IsFlag;
                isCounter = litRef.Counter;
            }
            SetExpansionType result = isFlag ? SetExpansionType.Flag : SetExpansionType.None;
            if (isCounter)
                result |= SetExpansionType.Counter;
            if (target.RepeatOptions.Min == null && target.RepeatOptions.Max == null && target.RepeatOptions.Options == ScannableEntryItemRepeatOptions.None)
                return result;
            else if (target.RepeatOptions.Options != ScannableEntryItemRepeatOptions.None)
                return SetExpansionType.Set | result;
            else if (target.RepeatOptions.Max != null || target.RepeatOptions.Min != null)
                return SetExpansionType.Set | result;
            return result;
        }
    }
}
