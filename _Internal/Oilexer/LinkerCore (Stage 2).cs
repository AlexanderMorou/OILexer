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
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    partial class LinkerCore
    {
        internal static bool NeedsExpansion(this IGDFile target)
        {
            foreach (var item in target)
            {
                if (item is IProductionRuleEntry)
                    if (((IProductionRuleEntry)(item)).NeedsExpansion())
                        return true;
            }
            return false;
        }
        internal static bool NeedsExpansion(this IProductionRuleEntry entry)
        {
            return entry.NeedsExpansion(entry);
        }

        internal static bool NeedsExpansion(this IProductionRuleSeries series, IProductionRuleEntry entry)
        {
            foreach (IProductionRule rule in series)
            {
                if (rule.NeedsExpansion(entry))
                    return true;
            }
            return false;
        }

        internal static bool NeedsExpansion(this IProductionRule series,IProductionRuleEntry entry)
        {
            foreach (IProductionRuleItem item in series)
            {
                if (item is ITemplateReferenceProductionRuleItem)
                    return true;
                else if (item is ITemplateParamReferenceProductionRuleItem)
                    return true;
                else if (item is IProductionRuleGroupItem && ((IProductionRuleGroupItem)(item)).NeedsExpansion(entry))
                    return true;
            }
            return false;
        }

        internal static void ExpandTemplates(this IProductionRuleEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            while (entry.NeedsExpansion())
            {
                ProductionRuleEntry e = ((ProductionRuleEntry)(entry));
                IProductionRuleSeries iprs = entry.ExpandTemplates(entry, file, errors);
                e.Clear();
                foreach (IProductionRule ipr in iprs)
                    e.Add(ipr);
            }
        }

        internal static IProductionRuleSeries ExpandTemplates(this IProductionRuleSeries series, IProductionRuleEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            List<IProductionRule> result = new List<IProductionRule>();
            foreach (IProductionRule rule in series.ToArray())
            {
                if (!series.NeedsExpansion(entry))
                    result.Add(rule);
                else
                    result.Add(rule.ExpandTemplates(entry, file, errors));
            }
            return new ProductionRuleSeries(result);
        }

        internal static IProductionRuleGroupItem ExpandTemplates(this IProductionRuleGroupItem groupItem, IProductionRuleEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            ProductionRuleGroupItem result = new ProductionRuleGroupItem(((IProductionRuleSeries)(groupItem)).ExpandTemplates(entry, file, errors).ToArray(), groupItem.Column, groupItem.Line, groupItem.Position);
            result.RepeatOptions = groupItem.RepeatOptions;
            result.Name = groupItem.Name;
            return result;
        }
        internal static IProductionRule ExpandTemplates(this IProductionRule rule, IProductionRuleEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            List<IProductionRuleItem> result = new List<IProductionRuleItem>();
            foreach (IProductionRuleItem ruleItem in rule)
            {
                if (ruleItem is IProductionRuleGroupItem)
                {
                    if (!((IProductionRuleGroupItem)(ruleItem)).NeedsExpansion(entry))
                        result.Add(ruleItem);
                    else
                        result.Add(((IProductionRuleGroupItem)(ruleItem)).ExpandTemplates(entry, file, errors));
                }
                else if (ruleItem is ITemplateReferenceProductionRuleItem)
                {
                    ITemplateReferenceProductionRuleItem tRef = (ITemplateReferenceProductionRuleItem)ruleItem;
                    result.Add(tRef.Expand(entry, file, errors));
                }
                else
                    result.Add(ruleItem);
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
            return new ProductionRule(rebuiltResult, rule.FileName, rule.Column, rule.Line, rule.Position);
        }

        internal static IProductionRuleGroupItem Expand(this ITemplateReferenceProductionRuleItem ruleItem, IProductionRuleEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            IProductionRuleGroupItem result = new ProductionRuleGroupItem(ruleItem.Reference.Expand(ruleItem, file, errors).ToArray(), ruleItem.Column, ruleItem.Line, ruleItem.Position);
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

        internal static IProductionRuleSeries Expand(this IProductionRuleTemplateEntry entry, ITemplateReferenceProductionRuleItem dataSource, GDFile file, ICompilerErrorCollection errors)
        {
            List<IProductionRuleSeries> result = new List<IProductionRuleSeries>();
            foreach (ProductionRuleTemplateArgumentSeries prtas in new ProductionRuleTemplateArgumentSeries(entry, dataSource))
            {
                IProductionRuleSeries item = entry.Expand(prtas, entry, file, errors);
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

        internal static IProductionRuleSeries Expand(this IProductionRuleSeries series, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            List<IProductionRule> result = new List<IProductionRule>();
            foreach (IProductionRule ipr in series)
            {
                IProductionRule resultedItem = null;
                if (ipr.HasExpansion())
                    resultedItem = ipr.Expand(argumentLookup, entry, file, errors);
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

        internal static IProductionRule Expand(this IProductionRule rule, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            if (rule.HasExpansion())
            {
                List<IProductionRuleItem> result = new List<IProductionRuleItem>();
                foreach (IProductionRuleItem item in rule)
                {
                    IProductionRuleItem ipri = null;
                    if (item.HasExpansion())
                        ipri = item.Expand(argumentLookup, entry, file, errors);
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

        internal static IProductionRuleItem Expand(this IProductionRuleItem ruleItem, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            if (ruleItem is IProductionRulePreprocessorDirective)
                return ((IProductionRulePreprocessorDirective)(ruleItem)).Expand(argumentLookup, entry, file, errors);
            else if (ruleItem is ITemplateParamReferenceProductionRuleItem)
            {
                ITemplateParamReferenceProductionRuleItem itrpri = (ITemplateParamReferenceProductionRuleItem)ruleItem;
                if (argumentLookup.Lookup.ContainsKey(itrpri.Reference))
                {
                    IProductionRuleSeries series = argumentLookup.Lookup[itrpri.Reference].Replacement;
                    if (series.Count == 1 && series[0].Count == 1)
                    {
                        IProductionRuleItem ipri = series[0][0].Clone();
                        ((TemplateParamReferenceProductionRuleItem)(itrpri)).CloneData(ipri);
                        return ipri;
                    }
                    else
                    {
                        ProductionRuleGroupItem result = new ProductionRuleGroupItem(series.ToArray(), itrpri.Column, itrpri.Line, itrpri.Position);
                        ((TemplateParamReferenceProductionRuleItem)(itrpri)).CloneData(result);
                        return result;
                    }
                }
                else
                    return itrpri.Clone();
            }
            else if (ruleItem is ITemplateReferenceProductionRuleItem)
            {
                ITemplateReferenceProductionRuleItem rI = ruleItem as TemplateReferenceProductionRuleItem;
                List<IProductionRuleSeries> serii = new List<IProductionRuleSeries>();
                foreach (IProductionRuleSeries series in rI)
                {
                    serii.Add(series.Expand(argumentLookup, entry, file, errors));
                }

                TemplateReferenceProductionRuleItem result = new TemplateReferenceProductionRuleItem(rI.Reference, serii, rI.Column, rI.Line, rI.Position);
                ((TemplateReferenceProductionRuleItem)ruleItem).CloneData(result);
                return result.Expand(entry, file, errors);
            }
            else if (ruleItem is IProductionRuleGroupItem)
            {
                if (!((IProductionRuleSeries)(ruleItem)).HasExpansion())
                    return ruleItem.Clone();
                else
                {
                    ProductionRuleGroupItem result = new ProductionRuleGroupItem(((IProductionRuleSeries)ruleItem).Expand(argumentLookup, entry, file, errors).ToArray(), ruleItem.Column, ruleItem.Line, ruleItem.Position);
                    result.RepeatOptions = ruleItem.RepeatOptions;
                    result.Name = ruleItem.Name;
                    return result;
                }
            }
            else
                return ruleItem.Clone();
        }

        internal static IProductionRuleItem Expand(this IProductionRulePreprocessorDirective directive, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            return directive.Directive.Expand(argumentLookup, entry, file, errors);
        }

        internal static IProductionRuleItem Expand(this IPreprocessorIfDirective directive, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            bool process = true;
            if (directive.Type != EntryPreprocessorType.Else)
            {
                switch (directive.Type)
                {
                    case EntryPreprocessorType.If:
                    case EntryPreprocessorType.ElseIf:
                        process = directive.Condition.Evaluate(argumentLookup, entry, file, errors);
                        break;
                    case EntryPreprocessorType.IfNotDefined:
                        process = !directive.Condition.IsDefined(argumentLookup, entry, file, errors);
                        break;
                    case EntryPreprocessorType.ElseIfDefined:
                    case EntryPreprocessorType.IfDefined:
                        process = directive.Condition.IsDefined(argumentLookup, entry, file, errors);
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
                    IProductionRuleItem ipri = ipd.Expand(argumentLookup, entry, file, errors);
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
                return directive.Next.Expand(argumentLookup, entry, file, errors);
            }
            return null;
        }

        internal static bool Evaluate(this IPreprocessorCLogicalOrConditionExp expression, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            //rule 2.
            if (expression.Left == null)
            {
                return expression.Right.Evaluate(argumentLookup, entry, file, errors);
            }
            //rule 1.
            else
            {
                return expression.Left.Evaluate(argumentLookup, entry, file, errors) || expression.Right.Evaluate(argumentLookup, entry, file, errors);
            }
        }

        internal static bool Evaluate(this IPreprocessorCLogicalAndConditionExp expression, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            //rule 2.
            if (expression.Left == null)
            {
                try
                {
                    return (bool)expression.Right.Evaluate(argumentLookup, entry, file, errors);
                }
                catch
                {
                    errors.SourceError(GrammarCore.CompilerErrors.InvalidPreprocessorCondition, expression.Line, expression.Column, entry.FileName, expression.ToString());
                }
            }
            //rule 1.
            else
            {
                return (bool)expression.Left.Evaluate(argumentLookup, entry, file, errors) && (bool)expression.Right.Evaluate(argumentLookup, entry, file, errors);
            }
            return false;
        }

        internal static object Evaluate(this IPreprocessorCEqualityExp expression, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            ITokenEntry lookup;
            ILiteralTokenItem reference = null;
            //Hack #1.
            if (expression.Rule == 1 || expression.Rule == 2)
            {
                string name = null;
                //If the left side is a parameter reference and the right side
                //is an identifier...
                if (expression.PreCEqualityExp.Rule == 3  && expression.PreCPrimary.Rule == 4 && expression.PreCEqualityExp.PreCPrimary.Rule == 4 &&
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
                    return expression.PreCEqualityExp.Evaluate(argumentLookup, entry, file, errors) == expression.PreCPrimary.Evaluate(argumentLookup, entry, file, errors);
                case 2: //PreprocessorCEqualityExp "!=" PreprocessorCPrimary
                    return expression.PreCEqualityExp.Evaluate(argumentLookup, entry, file, errors) != expression.PreCPrimary.Evaluate(argumentLookup, entry, file, errors);
                case 3: //PreprocessorCPrimary
                    return expression.PreCPrimary.Evaluate(argumentLookup, entry, file, errors);
            }
            return false;
        }

        internal static object Evaluate(this IPreprocessorCPrimary expression, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            switch (expression.Rule)
            {
                case 1:
                    return expression.String;
                case 2:
                    return expression.Char;
                case 3:
                    return expression.PreCLogicalOrExp.Evaluate(argumentLookup, entry, file, errors);
                case 4:
                    return expression.Identifier.Name;
                case 5:
                    return expression.Number;
            }
            return null;
        }

        internal static bool IsDefined(this IPreprocessorCLogicalOrConditionExp expression, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
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

                                    //Reason: if another template defines this already,
                                    //it exists, but hasn't been resolved.
                                    name = ((ISoftReferenceProductionRuleItem)(ipri)).PrimaryName;
                        }
                    }
                    else
                        errors.SourceError(GrammarCore.CompilerErrors.IsDefinedTemplateParameterMustExpectRule, expression.Line, expression.Column, entry.FileName, name);
                }
                foreach (IEntry ientry in file)
                    if (ientry is IProductionRuleEntry && (!(ientry is IProductionRuleTemplateEntry)))
                        if (((IProductionRuleEntry)ientry).Name == name)
                            return true;
                return false;
            }
            errors.SourceError(GrammarCore.CompilerErrors.InvalidDefinedTarget, expression.Column, expression.Line, entry.FileName, expression.ToString());
            return false;
        }

        internal static IProductionRuleItem Expand(this IPreprocessorDirective directive, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            switch (directive.Type)
            {
                case EntryPreprocessorType.If:
                case EntryPreprocessorType.IfNotDefined:
                case EntryPreprocessorType.IfDefined:
                case EntryPreprocessorType.ElseIf:
                case EntryPreprocessorType.ElseIfDefined:
                case EntryPreprocessorType.Else:
                    return ((IPreprocessorIfDirective)directive).Expand(argumentLookup, entry, file, errors);
                case EntryPreprocessorType.DefineRule:
                    ((IPreprocessorDefineRuleDirective)(directive)).Expand(argumentLookup, entry, file, errors);
                    break;
                case EntryPreprocessorType.AddRule:
                    ((IPreprocessorAddRuleDirective)(directive)).Expand(argumentLookup, entry, file, errors);
                    break;
                case EntryPreprocessorType.Throw:
                    ((IPreprocessorThrowDirective)(directive)).Expand(argumentLookup, entry, file, errors);
                    break;
                case EntryPreprocessorType.Return:
                    return ((IPreprocessorConditionalReturnDirective)(directive)).Expand(argumentLookup, entry, file, errors);
            }
            return null;
        }

        internal static void Expand(this IPreprocessorThrowDirective directive, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            string[] errorData = new string[directive.Arguments.Length];
            int index = 0;
            List<Tuple<string, int, int>> errorLocations = new List<Tuple<string, int, int>>();
            foreach (var item in directive.Arguments)
            {
                if (item.TokenType == GDTokenType.Identifier)
                {
                    var idItem = item as GDTokens.IdentifierToken;
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
                                errorData[index] = specificData.PrimaryName;
                                errorLocations.Add(new Tuple<string,int,int>(pd0.FileName, specificData.Line, specificData.Column));
                                //errors.Add(new CompilerSourceError(pd0.FileName, specificData.Line, specificData.Column, string.Format(GrammarCore.GrammarParserErrorFormat, (int)GDParserErrors.ReferenceError), string.Format("Location related to error {3}:\r\n\tLanguage defined error in:\r\n\t\t{0} ({1}:{2}).", entry.FileName, directive.Line, directive.Column, directive.Reference.Number)));
                            }
                        }
                    }
                }
                else if (item.TokenType == GDTokenType.CharacterLiteral)
                    errorData[index] = ((GDTokens.CharLiteralToken)(item)).GetCleanValue().ToString();
                else if (item.TokenType == GDTokenType.StringLiteral)
                    errorData[index] = ((GDTokens.StringLiteralToken)(item)).GetCleanValue();
                index++;
            }
            if (errorLocations.Count > 0)
                foreach (var errorLocation in errorLocations)
                    errors.SourceError(GrammarCore.CompilerErrors.LanguageDefinedError, errorLocation.Item2, errorLocation.Item3, errorLocation.Item1, directive.Reference.Number.ToString(), string.Format(directive.Reference.Message, errorData));
            else
                errors.SourceError(GrammarCore.CompilerErrors.LanguageDefinedError, directive.Line, directive.Column, entry.FileName, directive.Reference.Number.ToString(), string.Format(directive.Reference.Message, errorData));
        }

        internal static IProductionRuleItem Expand(this IPreprocessorConditionalReturnDirective directive, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            IProductionRule[] result = new IProductionRule[directive.Result.Length];
            for (int i = 0; i < directive.Result.Length; i++)
                result[i] = directive.Result[i].Expand(argumentLookup, entry, file, errors);
            var resultG = new ProductionRuleGroupItem(result, directive.Column, directive.Line, directive.Position);
            return resultG;
        }
        internal static void Expand(this IPreprocessorAddRuleDirective directive, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
        {
            string search = directive.InsertTarget;
            if (search != null && search != string.Empty)
            {
                ProductionRuleEntry foundItem = null;
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
                            foundItem = (ProductionRuleEntry)((IRuleReferenceProductionRuleItem)(ipri)).Reference;
                    }
                }
                if (foundItem == null)
                    foreach (IEntry ie in file)
                        if (ie is ProductionRuleEntry && ((ProductionRuleEntry)ie).Name == search)
                        {
                            foundItem = ie as ProductionRuleEntry;
                            break;
                        }
                if (foundItem == null)
                {
                    errors.SourceError(GrammarCore.CompilerErrors.UndefinedAddRuleTarget, directive.Line, directive.Column, entry.FileName, string.Join<IProductionRule>(" | ", directive.Rules), search);
                    return;
                }
                foreach (IProductionRule ipr in directive.Rules)
                    foundItem.Add(ipr.Expand(argumentLookup, entry, file, errors));
            }
        }
        internal static void Expand(this IPreprocessorDefineRuleDirective directive, ProductionRuleTemplateArgumentSeries argumentLookup, IProductionRuleTemplateEntry entry, GDFile file, ICompilerErrorCollection errors)
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
                foreach (IEntry ie in file)
                    if (ie is INamedEntry && ((INamedEntry)ie).Name == search)
                    {
                        errors.SourceError(GrammarCore.CompilerErrors.DuplicateTermDefined, directive.Column, directive.Line, entry.FileName, search);
                        return;
                    }
                ProductionRuleEntry insertedItem = new ProductionRuleEntry(search, entry.ScanMode, entry.FileName, directive.Column, directive.Line, directive.Position);
                file.Add(insertedItem);
                foreach (IProductionRule ipr in directive.DefinedRules)
                {
                    IProductionRule expanded = ipr.Expand(argumentLookup, entry, file, errors);
                    insertedItem.Add(expanded);
                }
            }
        }
        public static void ExpandTemplates(this GDFile file, ICompilerErrorCollection errors)
        {
            List<IProductionRuleEntry> rules =null;
            do
            {
                rules = new List<IProductionRuleEntry>(ruleEntries);
                /* *
                 * Expand the templates of every rule in the file.
                 * Utilize a list to make the operating set
                 * immutable and expansions won't affect the 
                 * enumeration.
                 * */
                foreach (IProductionRuleEntry rule in rules)
                {
                    rule.ExpandTemplates(file, errors);
                }
                /* *
                 * Templates can create rules which rely on 
                 * templates themselves.
                 * *
                 * Repeat the process until the count stabilizes and
                 * none of the rules require further expansion.
                 * */

            } while (ruleEntries.Count() != rules.Count ||
                ruleEntries.Any(entry=>entry.NeedsExpansion()));
                //Destroy the templates...
            IList<IProductionRuleTemplateEntry> templates = new List<IProductionRuleTemplateEntry>(GetTemplateRulesEnumerator(file));
            foreach (IProductionRuleTemplateEntry t in templates)
                file.Remove(t);
        }
    }
}
