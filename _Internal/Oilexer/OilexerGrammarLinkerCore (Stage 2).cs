using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Slf.Abstract;
using System.Threading.Tasks;
/*---------------------------------------------------------------------\
| Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
|----------------------------------------------------------------------|
| The Abstraction Project's code is provided under a contract-release  |
| basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
\-------------------------------------------------------------------- */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    partial class OilexerGrammarLinkerCore
    {
        internal static bool NeedsExpansion(this IOilexerGrammarFile target)
        {
            foreach (var item in target)
            {
                if (item is IOilexerGrammarProductionRuleEntry)
                    if (((IOilexerGrammarProductionRuleEntry)(item)).NeedsExpansion())
                        return true;
            }
            return false;
        }
        internal static bool NeedsExpansion(this IOilexerGrammarProductionRuleEntry entry)
        {
            return entry.NeedsExpansion(entry);
        }

        internal static bool NeedsExpansion(this IProductionRuleSeries series, IOilexerGrammarProductionRuleEntry entry)
        {
            if (series.Count == 0)
                return true;
            if (series is IProductionRuleGroupItem)
            {
                var gSeries = (IProductionRuleGroupItem)series;
                if (gSeries.Count == 1 && gSeries[0].Count == 1 && gSeries.Name.IsEmptyOrNull() && gSeries.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                    return true;
            }
            foreach (IProductionRule rule in series)
            {
                if (rule.NeedsExpansion(entry))
                    return true;
            }
            return false;
        }

        internal static bool NeedsExpansion(this IProductionRule series, IOilexerGrammarProductionRuleEntry entry)
        {
            foreach (IProductionRuleItem item in series)
            {
                if (item is ITemplateReferenceProductionRuleItem)
                    return true;
                else if (item is ITemplateParamReferenceProductionRuleItem)
                    return true;
                else if (item is IProductionRuleGroupItem)
                {
                    var gItem = (IProductionRuleGroupItem)item;
                    if (gItem.Count == 0)
                        return true;
                    else if (gItem.Name.IsEmptyOrNull() && gItem.Count == 1 && gItem[0].Count == 1 && gItem.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                        return true;
                    if (gItem.NeedsExpansion(entry))
                        return true;
                }
            }
            return false;
        }

        internal static void ExpandTemplates(this IOilexerGrammarProductionRuleEntry entry, IList<IOilexerGrammarTokenEntry> availableStock, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            while (entry.NeedsExpansion())
            {
                OilexerGrammarProductionRuleEntry e = ((OilexerGrammarProductionRuleEntry)(entry));
                IProductionRuleSeries iprs = entry.ExpandTemplates(availableStock, entry, file, errors);
                e.Clear();
                foreach (IProductionRule ipr in iprs)
                    e.Add(ipr);
            }
        }

        internal static IProductionRuleSeries ExpandTemplates(this IProductionRuleSeries series, IList<IOilexerGrammarTokenEntry> availableStock, IOilexerGrammarProductionRuleEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            List<IProductionRule> result = new List<IProductionRule>();
            foreach (IProductionRule rule in series.ToArray())
            {
                if (!series.NeedsExpansion(entry))
                    result.Add(rule);
                else
                    result.Add(rule.ExpandTemplates(availableStock, entry, file, errors));
            }
            if (EachSubitemIsNamelessGroup(result))
            {
                List<IProductionRule> minimizedResult = new List<IProductionRule>();
                foreach (var rule in result)
                    foreach (IProductionRuleGroupItem item in rule)
                        foreach (var subRule in item)
                            minimizedResult.Add(subRule);
                result = minimizedResult;
            }
            return new ProductionRuleSeries(result);
        }

        internal static IProductionRuleGroupItem ExpandTemplates(this IProductionRuleGroupItem groupItem, IList<IOilexerGrammarTokenEntry> availableStock, IOilexerGrammarProductionRuleEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            ProductionRuleGroupItem result = new ProductionRuleGroupItem(((IProductionRuleSeries)(groupItem)).ExpandTemplates(availableStock, entry, file, errors).ToArray(), groupItem.Column, groupItem.Line, groupItem.Position);
            result.RepeatOptions = groupItem.RepeatOptions;
            result.Name = groupItem.Name;
            return result;
        }
        internal static IProductionRule ExpandTemplates(this IProductionRule rule, IList<IOilexerGrammarTokenEntry> availableStock, IOilexerGrammarProductionRuleEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            List<IProductionRuleItem> result = new List<IProductionRuleItem>();
            foreach (IProductionRuleItem ruleItem in rule)
            {
                if (ruleItem is IProductionRuleGroupItem)
                {
                    var gItem = (IProductionRuleGroupItem)ruleItem;
                    if (!gItem.NeedsExpansion(entry))
                        result.Add(ruleItem);
                    else if (gItem.Count == 0)
                        continue;
                    else if (gItem.Count == 1 && gItem[0].Count == 1 && gItem.Name.IsEmptyOrNull() && gItem.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                        result.Add(gItem[0][0]);
                    else
                        result.Add(gItem.ExpandTemplates(availableStock, entry, file, errors));
                }
                else if (ruleItem is ITemplateReferenceProductionRuleItem)
                {
                    ITemplateReferenceProductionRuleItem tRef = (ITemplateReferenceProductionRuleItem)ruleItem;
                    result.Add(tRef.Expand(availableStock, entry, file, errors));
                }
                else
                    result.Add(ruleItem);
            }
        rebuildResult:
            List<IProductionRuleItem> rebuiltResult = new List<IProductionRuleItem>();
            foreach (IProductionRuleItem ipri in result)
            {
                IProductionRuleGroupItem group = null;
                if ((group = ipri as IProductionRuleGroupItem) != null && group.Count == 1 && group.Name.IsEmptyOrNull() && group.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                    foreach (IProductionRuleItem iprii in group[0])
                        rebuiltResult.Add(iprii);
                else
                    rebuiltResult.Add(ipri);
            }
            if (rebuiltResult.Count != result.Count)
            {
                result = rebuiltResult;
                goto rebuildResult;
            }
            return new ProductionRule(rebuiltResult, rule.FileName, rule.Column, rule.Line, rule.Position);
        }

        private static bool EachSubitemIsNamelessGroup(IEnumerable<IProductionRule> series)
        {
            foreach (var item in series)
            {
                IProductionRuleGroupItem groupItem = null;
                if (!(item.Count == 1 &&
                    (groupItem = item[0] as IProductionRuleGroupItem) != null &&
                    groupItem.Name.IsEmptyOrNull() &&
                    groupItem.RepeatOptions == ScannableEntryItemRepeatInfo.None))
                    return false;
            }
            return true;
        }

        internal static IProductionRuleGroupItem Expand(this ITemplateReferenceProductionRuleItem ruleItem, IList<IOilexerGrammarTokenEntry> availableStock, IOilexerGrammarProductionRuleEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {

            IProductionRuleGroupItem result = new ProductionRuleGroupItem(ruleItem.Reference.Expand(availableStock, ruleItem, file, errors).ToArray(), ruleItem.Column, ruleItem.Line, ruleItem.Position);
            result.RepeatOptions = ruleItem.RepeatOptions;
            result.Name = ruleItem.Name;
            return result;
        }

        internal static bool HasExpansion(this IProductionRuleSeries series)
        {
            foreach (IProductionRule rule in series)
                if (rule.HasExpansion())
                    return true;
            return false;
        }

        internal static bool HasExpansion(this IProductionRule rule)
        {
            foreach (IProductionRuleItem ruleItem in rule)
                if (ruleItem.HasExpansion())
                    return true;
            return false;
        }

        internal static bool HasExpansion(this IProductionRuleItem rule)
        {
            if (rule is IProductionRulePreprocessorDirective)
                return true;
            else if (rule is ITemplateParamReferenceProductionRuleItem)
                return true;
            else if (rule is ITemplateReferenceProductionRuleItem)
                return true;
            else if (rule is IProductionRuleGroupItem)
                return ((IProductionRuleSeries)(rule)).HasExpansion();
            return false;
        }

        internal static IProductionRuleSeries Expand(this IOilexerGrammarProductionRuleTemplateEntry entry, IList<IOilexerGrammarTokenEntry> availableStock, ITemplateReferenceProductionRuleItem dataSource, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            List<IProductionRuleSeries> result = new List<IProductionRuleSeries>();
            foreach (ProductionRuleTemplateArgumentSeries prtas in new ProductionRuleTemplateArgumentSeries(entry, dataSource))
            {
                IProductionRuleSeries item = entry.Expand(entry, availableStock, prtas, entry, file, errors);
                if (item == null)
                    continue;
                if (item.Count == 1 && item[0] == null)
                    continue;
                else if (item.Count == 1 && item[0].Count == 1 && item[0][0] == null)
                    continue;
                result.Add(item);
            }

            if (dataSource.RepeatOptions == ScannableEntryItemRepeatInfo.None && (dataSource.Name == null || dataSource.Name == string.Empty))
                return new ProductionRuleSeries(result);
            else
            {
                ProductionRuleGroupItem rResult = new ProductionRuleGroupItem(result, dataSource.Column, dataSource.Line, dataSource.Position);
                rResult.RepeatOptions = dataSource.RepeatOptions;
                rResult.Name = dataSource.Name;
                return rResult;
            }
        }

        internal static IProductionRuleSeries Expand(this IProductionRuleSeries series, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            List<IProductionRule> result = new List<IProductionRule>();
            foreach (IProductionRule ipr in series)
            {
                IProductionRule resultedItem = null;
                if (ipr.HasExpansion())
                    resultedItem = ipr.Expand(currentEntry, availableStock, argumentLookup, entry, file, errors);
                else
                    resultedItem = ipr;
                if (resultedItem == null)
                    continue;
                result.Add(resultedItem);
            }
            if (result.Count == 0)
                return null;
            return new ProductionRuleSeries(result);
        }

        internal static IProductionRule Expand(this IProductionRule rule, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (rule.HasExpansion())
            {
                List<IProductionRuleItem> result = new List<IProductionRuleItem>();
                foreach (IProductionRuleItem item in rule)
                {
                    IProductionRuleItem ipri = null;
                    if (item.HasExpansion())
                        ipri = item.Expand(currentEntry, availableStock, argumentLookup, entry, file, errors);
                    else
                        ipri = item.Clone();
                    if (ipri == null)
                        continue;
                    result.Add(ipri);
                }
            rebuildResult:
                List<IProductionRuleItem> rebuiltResult = new List<IProductionRuleItem>();
                foreach (IProductionRuleItem ipri in result)
                    if (ipri is IProductionRuleGroupItem && ((IProductionRuleGroupItem)(ipri)).Count == 1 && (ipri.Name == null || ipri.Name == string.Empty) && ipri.RepeatOptions == ScannableEntryItemRepeatInfo.None)
                        foreach (IProductionRuleItem iprii in ((IProductionRuleGroupItem)(ipri))[0])
                            rebuiltResult.Add(iprii);
                    else
                        rebuiltResult.Add(ipri);
                if (rebuiltResult.Count != result.Count)
                {
                    result = rebuiltResult;
                    goto rebuildResult;
                }
                if (result.Count == 0)
                    return null;
                return new ProductionRule(result, rule.FileName, rule.Column, rule.Line, rule.Position);
            }
            else
                return rule;
        }

        internal static IProductionRuleItem Expand(this IProductionRuleItem ruleItem, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (ruleItem is IProductionRulePreprocessorDirective)
                return ((IProductionRulePreprocessorDirective)(ruleItem)).Expand(currentEntry, availableStock, argumentLookup, entry, file, errors);
            else if (ruleItem is ITemplateParamReferenceProductionRuleItem)
            {
                TemplateParamReferenceProductionRuleItem trpri = (TemplateParamReferenceProductionRuleItem)ruleItem;
                if (argumentLookup.Lookup.ContainsKey(trpri.Reference))
                {
                    IProductionRuleSeries series = argumentLookup.Lookup[trpri.Reference].Replacement;
                    /* *
                     * Fix 4-29-2013
                     * *
                     * Series null check, if an error is thrown by the user's template within a replacement
                     * the result of the replacement is null, thus this is null.
                     * */
                    if (series == null)
                        return null;
                    if (series.Count == 1 && series[0].Count == 1)
                    {
                        IProductionRuleItem ipri = series[0][0].Clone();
                        trpri.CloneData(ipri);
                        return ipri;
                    }
                    else
                    {
                        ProductionRuleGroupItem result = new ProductionRuleGroupItem(series.ToArray(), trpri.Column, trpri.Line, trpri.Position);
                        trpri.CloneData(result);
                        return result;
                    }
                }
                else
                    return trpri.Clone();
            }
            else if (ruleItem is ITemplateReferenceProductionRuleItem)
            {
                ITemplateReferenceProductionRuleItem rI = ruleItem as TemplateReferenceProductionRuleItem;
                List<IProductionRuleSeries> serii = new List<IProductionRuleSeries>();
                foreach (IProductionRuleSeries series in rI)
                {
                    var seriesCopy = series;
                    /* *
                     * Handle deliteralization here to expedite 
                     * phase 3.  If a template yields 5000 literals
                     * then deliteralizing that will waste a massive
                     * chunk of processor cycles.
                     * */
                    if (series.NeedsDeliteralized())
                        seriesCopy = seriesCopy.Deliteralize(currentEntry, availableStock, file, errors);
                    serii.Add(seriesCopy.Expand(currentEntry, availableStock, argumentLookup, entry, file, errors));
                }

                TemplateReferenceProductionRuleItem result = new TemplateReferenceProductionRuleItem(rI.Reference, serii, rI.Column, rI.Line, rI.Position);
                ((TemplateReferenceProductionRuleItem)ruleItem).CloneData(result);
                return result.Expand(availableStock, entry, file, errors);
            }
            else if (ruleItem is IProductionRuleGroupItem)
            {
                if (!((IProductionRuleSeries)(ruleItem)).HasExpansion())
                    return ruleItem.Clone();
                else
                {
                    ProductionRuleGroupItem result = new ProductionRuleGroupItem(((IProductionRuleSeries)ruleItem).Expand(currentEntry, availableStock, argumentLookup, entry, file, errors).ToArray(), ruleItem.Column, ruleItem.Line, ruleItem.Position);
                    result.RepeatOptions = ruleItem.RepeatOptions;
                    result.Name = ruleItem.Name;
                    return result;
                }
            }
            else
                return ruleItem.Clone();
        }

        internal static IProductionRuleItem Expand(this IProductionRulePreprocessorDirective directive, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            return directive.Directive.Expand(currentEntry, availableStock, argumentLookup, entry, file, errors);
        }

        internal static IProductionRuleItem Expand(this IPreprocessorIfDirective directive, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            bool process = true;
            if (directive.Type != EntryPreprocessorType.Else)
            {
                switch (directive.Type)
                {
                    case EntryPreprocessorType.If:
                    case EntryPreprocessorType.ElseIf:
                        process = directive.Condition.Evaluate(currentEntry, availableStock, argumentLookup, entry, file, errors);
                        break;
                    case EntryPreprocessorType.IfNotDefined:
                        process = !directive.Condition.IsDefined(currentEntry, availableStock, argumentLookup, entry, file, errors);
                        break;
                    case EntryPreprocessorType.ElseIfDefined:
                    case EntryPreprocessorType.IfDefined:
                        process = directive.Condition.IsDefined(currentEntry, availableStock, argumentLookup, entry, file, errors);
                        break;
                    default:
                        break;
                }
            }
            if (process)
            {
                List<IProductionRuleItem> result = new List<IProductionRuleItem>();
                foreach (IPreprocessorDirective ipd in directive.Body)
                {
                    IProductionRuleItem ipri = ipd.Expand(currentEntry, availableStock, argumentLookup, entry, file, errors);
                    if (ipri != null)
                        result.Add(ipri);
                }
                if (result.Count > 1)
                    return new ProductionRuleGroupItem(new IProductionRule[] { new ProductionRule(result, entry.FileName, entry.Column, entry.Line, entry.Position) }, directive.Column, directive.Line, directive.Position);
                else if (result.Count > 0)
                    return result[0];
                else
                    return null;
            }
            else if (directive.Next != null)
            {
                return directive.Next.Expand(currentEntry, availableStock, argumentLookup, entry, file, errors);
            }
            return null;
        }

        internal static bool Evaluate(this IPreprocessorCLogicalOrConditionExp expression, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            //rule 2.
            if (expression.Left == null)
            {
                return expression.Right.Evaluate(currentEntry, availableStock, argumentLookup, entry, file, errors);
            }
            //rule 1.
            else
            {
                return expression.Left.Evaluate(currentEntry, availableStock, argumentLookup, entry, file, errors) || expression.Right.Evaluate(currentEntry, availableStock, argumentLookup, entry, file, errors);
            }
        }

        internal static bool Evaluate(this IPreprocessorCLogicalAndConditionExp expression, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            //rule 2.
            if (expression.Left == null)
            {
                try
                {
                    return (bool)expression.Right.Evaluate(currentEntry, availableStock, argumentLookup, entry, file, errors);
                }
                catch
                {
                    errors.SourceError(OilexerGrammarCore.CompilerErrors.InvalidPreprocessorCondition, new LineColumnPair(expression.Line, expression.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), expression.ToString());
                }
            }
            //rule 1.
            else
            {
                return (bool)expression.Left.Evaluate(currentEntry, availableStock, argumentLookup, entry, file, errors) && (bool)expression.Right.Evaluate(currentEntry, availableStock, argumentLookup, entry, file, errors);
            }
            return false;
        }

        internal static object Evaluate(this IPreprocessorCEqualityExp expression, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            IOilexerGrammarTokenEntry lookup;
            ILiteralTokenItem reference = null;
            //Hack #1.
            if (expression.Rule == 1 || expression.Rule == 2)
            {
                string name = null;
                //If the left side is a parameter reference and the right side
                //is an identifier...
                if (expression.PreCEqualityExp.Rule == 3 && expression.PreCPrimary.Rule == 4 && expression.PreCEqualityExp.PreCPrimary.Rule == 4 &&
                    argumentLookup.ContainsParameter(name = expression.PreCEqualityExp.PreCPrimary.Identifier.Name))
                {
                    //Obtain the parameter
                    IProductionRuleTemplatePart part = argumentLookup.GetParameter(name);
                    if (part.SpecialExpectancy == TemplatePartExpectedSpecial.None && part.ExpectedSpecific != null)
                    {
                        //If the specific expectency is a production rule reference...
                        if (part.ExpectedSpecific is ITokenReferenceProductionRuleItem)
                        {
                            //Lookup the expectency.
                            ILiteralTokenItem secondHalf = null;
                            lookup = ((ITokenReferenceProductionRuleItem)(part.ExpectedSpecific)).Reference;
                            //Lookup the right-hand requirement for the condition.
                            ITokenItem sRef = lookup.FindTokenItem(expression.PreCPrimary.Identifier.Name);
                            if (sRef is ILiteralTokenItem)
                                reference = ((ILiteralTokenItem)(sRef));
                            else
                                goto notValidReference;
                            //Obtain the expression series for the left-hand side.
                            IProductionRuleSeries series = argumentLookup[part];
                            //If it's a single unit.
                            if (series.Count == 1 && series[0].Count == 1)
                            {
                                //If it's a soft-reference item...
                                IProductionRuleItem e = series[0][0];
                                if (e is ISoftReferenceProductionRuleItem)
                                {
                                    ISoftReferenceProductionRuleItem sre = ((ISoftReferenceProductionRuleItem)e);
                                    if (((sre.SecondaryName == null || sre.SecondaryName == string.Empty)) && sre.PrimaryName != null)
                                    {
                                        secondHalf = (ILiteralTokenItem)lookup.FindTokenItemByValue(sre.PrimaryName, file, true);
                                    }
                                }
                                //If they used the fully qualified name...
                                else if (e is ILiteralReferenceProductionRuleItem)
                                {
                                    ILiteralReferenceProductionRuleItem lr = ((ILiteralReferenceProductionRuleItem)e);
                                    //So much easier...
                                    secondHalf = lr.Literal;
                                }
                                if (expression.Rule == 1)
                                {
                                    return secondHalf == reference;
                                }
                                else if (expression.Rule == 2)
                                {
                                    return secondHalf != reference;
                                }
                            }
                        }
                    }
                }
                else if (expression.PreCEqualityExp.Rule == 3 && expression.PreCPrimary.Rule == 5 && expression.PreCEqualityExp.PreCPrimary.Rule == 4 &&
                    expression.PreCEqualityExp.PreCPrimary.Identifier.Name.ToLower() == "index")
                {
                    if (expression.Rule == 1)
                    {
                        return expression.PreCPrimary.Number.GetCleanValue() == argumentLookup.Index;
                    }
                    else if (expression.Rule == 2)
                    {
                        return expression.PreCPrimary.Number.GetCleanValue() != argumentLookup.Index;
                    }
                }
            }
        notValidReference:
            switch (expression.Rule)
            {
                case 1: //PreprocessorCEqualityExp "==" PreprocessorCPrimary
                    return expression.PreCEqualityExp.Evaluate(currentEntry, availableStock, argumentLookup, entry, file, errors) == expression.PreCPrimary.Evaluate(currentEntry, availableStock, argumentLookup, entry, file, errors);
                case 2: //PreprocessorCEqualityExp "!=" PreprocessorCPrimary
                    return expression.PreCEqualityExp.Evaluate(currentEntry, availableStock, argumentLookup, entry, file, errors) != expression.PreCPrimary.Evaluate(currentEntry, availableStock, argumentLookup, entry, file, errors);
                case 3: //PreprocessorCPrimary
                    return expression.PreCPrimary.Evaluate(currentEntry, availableStock, argumentLookup, entry, file, errors);
            }
            return false;
        }

        internal static object Evaluate(this IPreprocessorCPrimary expression, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            switch (expression.Rule)
            {
                case 1:
                    return expression.String;
                case 2:
                    return expression.Char;
                case 3:
                    return expression.PreCLogicalOrExp.Evaluate(currentEntry, availableStock, argumentLookup, entry, file, errors);
                case 4:
                    return expression.Identifier.Name;
                case 5:
                    return expression.Number;
            }
            return null;
        }

        internal static bool IsDefined(this IPreprocessorCLogicalOrConditionExp expression, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (expression.Left == null && expression.Right.Left == null && expression.Right.Right.Rule == 3 && expression.Right.Right.PreCPrimary.Rule == 4)
            {
                string name = expression.Right.Right.PreCPrimary.Identifier.Name;
                if (argumentLookup.ContainsParameter(name))
                {
                    IProductionRuleTemplatePart iprtp = argumentLookup.GetParameter(name);
                    if (iprtp.SpecialExpectancy == TemplatePartExpectedSpecial.Rule)
                    {
                        IProductionRuleSeries iprs = argumentLookup[name];
                        if (iprs.Count == 1 && iprs[0].Count == 1)
                        {
                            IProductionRuleItem ipri = iprs[0][0];
                            if (ipri != null)
                                if (ipri is IRuleReferenceProductionRuleItem)
                                    name = ((IRuleReferenceProductionRuleItem)(ipri)).Reference.Name;
                                else if (ipri is ISoftReferenceProductionRuleItem)
                                    //No guarantee that just being a soft-reference guarantees
                                    //lack of definition.

                                    //Reason: if another template defines this later,
                                    //it exists, but hasn't been resolved. -- It will be later in the expansion/resolution phase.
                                    name = ((ISoftReferenceProductionRuleItem)(ipri)).PrimaryName;
                        }
                    }
                    else
                        errors.SourceError(OilexerGrammarCore.CompilerErrors.IsDefinedTemplateParameterMustExpectRule, new LineColumnPair(expression.Line, expression.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), name);
                }
                foreach (IOilexerGrammarEntry ientry in file.ToArray())
                    if (ientry is IOilexerGrammarProductionRuleEntry && (!(ientry is IOilexerGrammarProductionRuleTemplateEntry)))
                        if (((IOilexerGrammarProductionRuleEntry)ientry).Name == name)
                            return true;
                return false;
            }
            errors.SourceError(OilexerGrammarCore.CompilerErrors.InvalidDefinedTarget, new LineColumnPair(expression.Line, expression.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), expression.ToString());
            return false;
        }

        internal static IProductionRuleItem Expand(this IPreprocessorDirective directive, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            switch (directive.Type)
            {
                case EntryPreprocessorType.If:
                case EntryPreprocessorType.IfNotDefined:
                case EntryPreprocessorType.IfDefined:
                case EntryPreprocessorType.ElseIf:
                case EntryPreprocessorType.ElseIfDefined:
                case EntryPreprocessorType.Else:
                    return ((IPreprocessorIfDirective)directive).Expand(currentEntry, availableStock, argumentLookup, entry, file, errors);
                case EntryPreprocessorType.DefineRule:
                    ((IPreprocessorDefineRuleDirective)(directive)).Expand(currentEntry, availableStock, argumentLookup, entry, file, errors);
                    break;
                case EntryPreprocessorType.AddRule:
                    ((IPreprocessorAddRuleDirective)(directive)).Expand(currentEntry, availableStock, argumentLookup, entry, file, errors);
                    break;
                case EntryPreprocessorType.Throw:
                    ((IPreprocessorThrowDirective)(directive)).Expand(currentEntry, availableStock, argumentLookup, entry, file, errors);
                    break;
                case EntryPreprocessorType.Return:
                    return ((IPreprocessorConditionalReturnDirective)(directive)).Expand(currentEntry, availableStock, argumentLookup, entry, file, errors);
            }
            return null;
        }

        internal static void Expand(this IPreprocessorThrowDirective directive, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            string[] errorData = new string[directive.Arguments.Length];
            int index = 0;
            //List<Tuple<string, int, int>> errorLocations = new List<Tuple<string, int, int>>();
            List<Tuple<IProductionRule, ISoftReferenceProductionRuleItem>> errorLocations = new List<Tuple<IProductionRule, ISoftReferenceProductionRuleItem>>();
            foreach (var item in directive.Arguments)
            {
                if (item.TokenType == OilexerGrammarTokenType.Identifier)
                {
                    var idItem = item as OilexerGrammarTokens.IdentifierToken;
                    if (!argumentLookup.ContainsParameter(idItem.Name))
                    {
                        errorData[index++] = idItem.Name;
                        continue;
                    }
                    else
                    {
                        var parameter = argumentLookup.GetParameter(idItem.Name);
                        var parameterDataSeries = argumentLookup[parameter];
                        if (parameterDataSeries.Count == 1 &&
                            parameterDataSeries[0].Count == 1)
                        {
                            var pd0 = parameterDataSeries[0];
                            var parameterData = pd0[0];
                            if (parameterData is ISoftReferenceProductionRuleItem)
                            {

                                var specificData = (ISoftReferenceProductionRuleItem)parameterData;
                                if (specificData.SecondaryToken == null)
                                    errorData[index] = specificData.PrimaryName;
                                else
                                    errorData[index] = string.Format("{0}.{1}", specificData.PrimaryName, specificData.SecondaryName);
                                errorLocations.Add(Tuple.Create(pd0, specificData));
                            }
                        }
                    }
                }
                else if (item.TokenType == OilexerGrammarTokenType.CharacterLiteral)
                    errorData[index] = ((OilexerGrammarTokens.CharLiteralToken)(item)).GetCleanValue().ToString();
                else if (item.TokenType == OilexerGrammarTokenType.StringLiteral)
                    errorData[index] = ((OilexerGrammarTokens.StringLiteralToken)(item)).GetCleanValue();
                index++;
            }
            if (errorLocations.Count > 0)
                foreach (var errorLocation in errorLocations)
                    errors.SourceModelError(OilexerGrammarCore.CompilerErrors.LanguageDefinedError, new LineColumnPair(errorLocation.Item2.Line, errorLocation.Item2.Column), new LineColumnPair(errorLocation.Item2.Line, errorLocation.Item2.Column + (errorLocation.Item2.SecondaryToken == null ? errorLocation.Item2.PrimaryName.Length : errorLocation.Item2.PrimaryName.Length + errorLocation.Item2.SecondaryName.Length)), new Uri(errorLocation.Item1.FileName, UriKind.RelativeOrAbsolute), directive, errorLocation.Item1, errorLocation.Item2, new string[] { directive.Reference.Number.ToString(), string.Format(directive.Reference.Message, errorData) });
            else
                errors.SourceError(OilexerGrammarCore.CompilerErrors.LanguageDefinedError, new LineColumnPair(directive.Line, directive.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), directive.Reference.Number.ToString(), string.Format(directive.Reference.Message, errorData));
        }

        internal static IProductionRuleItem Expand(this IPreprocessorConditionalReturnDirective directive, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            IProductionRule[] result = new IProductionRule[directive.Result.Length];
            for (int i = 0; i < directive.Result.Length; i++)
                result[i] = directive.Result[i].Expand(currentEntry, availableStock, argumentLookup, entry, file, errors);
            var resultG = new ProductionRuleGroupItem(result, directive.Column, directive.Line, directive.Position);
            return resultG;
        }
        internal static void Expand(this IPreprocessorAddRuleDirective directive, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            string search = directive.InsertTarget;
            if (search != null && search != string.Empty)
            {
                OilexerGrammarProductionRuleEntry foundItem = null;
                if (argumentLookup.ContainsParameter(search))
                {
                    IProductionRuleTemplatePart part = argumentLookup.GetParameter(search);
                    IProductionRuleSeries iprs = argumentLookup[part];
                    if (iprs.Count == 1 && iprs[0].Count == 1)
                    {
                        IProductionRuleItem ipri = iprs[0][0];
                        if (ipri is ISoftReferenceProductionRuleItem)
                        {
                            ISoftReferenceProductionRuleItem n = (ISoftReferenceProductionRuleItem)ipri;
                            search = n.PrimaryName;
                        }
                        else if (ipri is IRuleReferenceProductionRuleItem)
                            foundItem = (OilexerGrammarProductionRuleEntry)((IRuleReferenceProductionRuleItem)(ipri)).Reference;
                    }
                }
                if (foundItem == null)
                    foreach (IOilexerGrammarEntry ie in file)
                        if (ie is OilexerGrammarProductionRuleEntry && ((OilexerGrammarProductionRuleEntry)ie).Name == search)
                        {
                            foundItem = ie as OilexerGrammarProductionRuleEntry;
                            break;
                        }
                if (foundItem == null)
                {
                    errors.SourceError(OilexerGrammarCore.CompilerErrors.UndefinedAddRuleTarget, new LineColumnPair(directive.Line, directive.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), string.Join<IProductionRule>(" | ", directive.Rules), search);
                    return;
                }
                foreach (IProductionRule ipr in directive.Rules)
                    foundItem.Add(ipr.Expand(currentEntry, availableStock, argumentLookup, entry, file, errors));
            }
        }
        internal static void Expand(this IPreprocessorDefineRuleDirective directive, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, ProductionRuleTemplateArgumentSeries argumentLookup, IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            string search = directive.DeclareTarget;
            if (search != null && search != string.Empty)
            {
                if (argumentLookup.ContainsParameter(search))
                {
                    IProductionRuleTemplatePart part = argumentLookup.GetParameter(search);
                    IProductionRuleSeries iprs = argumentLookup[part];
                    if (iprs.Count == 1 && iprs[0].Count == 1)
                    {
                        IProductionRuleItem ipri = iprs[0][0];
                        if (ipri is ISoftReferenceProductionRuleItem)
                        {
                            ISoftReferenceProductionRuleItem n = (ISoftReferenceProductionRuleItem)ipri;
                            search = n.PrimaryName;
                        }
                    }
                }
                /* ToDo: Evaluate the depth necessary to institute a lock associated to the base list on the oilexer grammar file type. */
                IOilexerGrammarEntry[] fileElements;
                lock (file)
                    fileElements = file.ToArray();
                foreach (IOilexerGrammarEntry ie in fileElements)
                    if (ie is IOilexerGrammarNamedEntry && ((IOilexerGrammarNamedEntry)ie).Name == search)
                    {
                        errors.SourceError(OilexerGrammarCore.CompilerErrors.DuplicateTermDefined, new LineColumnPair(directive.Line, directive.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), search);
                        return;
                    }
                OilexerGrammarProductionRuleEntry insertedItem = new OilexerGrammarProductionRuleEntry(search, entry.ScanMode, entry.FileName, directive.Column, directive.Line, directive.Position);
                lock (file)
                    file.Add(insertedItem);
                foreach (IProductionRule ipr in directive.DefinedRules)
                {
                    IProductionRule expanded = ipr.Expand(currentEntry, availableStock, argumentLookup, entry, file, errors);
                    insertedItem.Add(expanded);
                }
            }
        }
        public static void ExpandTemplates(this OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            List<IOilexerGrammarProductionRuleEntry> rules = null;
            var toks = (from f in file
                        where f is IOilexerGrammarTokenEntry
                        select (IOilexerGrammarTokenEntry)f).ToList();
            do
            {
                /* *
                 * For grammars which generate a *lot* of changes in their
                 * productions.
                 * */
                GC.Collect();
                GC.WaitForPendingFinalizers();
                rules = new List<IOilexerGrammarProductionRuleEntry>(ruleEntries);
                /* *
                 * Expand the templates of every rule in the file.
                 * Utilize a list to make the operating set
                 * immutable and expansions won't affect the 
                 * enumeration.
                 * */
                var toExpand = (from r in rules
                                where r.NeedsExpansion()
                                select r);
#if ParallelProcessing
                Parallel.ForEach(toExpand, rule=>
#else
                foreach (IOilexerGrammarProductionRuleEntry rule in toExpand)
#endif
                {
                    rule.ExpandTemplates(toks, file, errors);
#if ParallelProcessing
                });
#else
                }
#endif
                /* *
                 * Templates can create rules which rely on 
                 * templates themselves.
                 * *
                 * Repeat the process until the count stabilizes and
                 * none of the rules require further expansion.
                 * */

            } while (ruleEntries.Count() != rules.Count ||
                ruleEntries.Any(entry => entry.NeedsExpansion()));
            //Destroy the templates...
            IList<IOilexerGrammarProductionRuleTemplateEntry> templates = new List<IOilexerGrammarProductionRuleTemplateEntry>(GetTemplateRulesEnumerator(file));
            foreach (IOilexerGrammarProductionRuleTemplateEntry t in templates)
                file.Remove(t);
        }
    }
}
