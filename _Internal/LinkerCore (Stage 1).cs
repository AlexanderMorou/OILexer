using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser;
using Oilexer.Parser.Builder;
namespace Oilexer._Internal
{
    internal static partial class LinkerCore
    {
        /* *
         * Set these before use :]
         * */
        internal static IEnumerable<ITokenEntry> tokenEntries;
        internal static IEnumerable<IProductionRuleEntry> ruleEntries;
        internal static IEnumerable<IProductionRuleTemplateEntry> ruleTemplEntries;
        internal static IEnumerable<IErrorEntry> errorEntries;
        internal static _GDResolutionAssistant resolutionAid;
        
        private static void ResolveToken(this ITokenEntry entry, GDFile file, CompilerErrorCollection errors)
        {
            List<ITokenEntry> lowerTokens = new List<ITokenEntry>();
            if (entry.LowerPrecedenceTokens == null)
            {
                foreach (string s in entry.LowerPrecedenceNames)
                {
                    var match = (from generalEntry in tokenEntries
                                 let tokenEntry = generalEntry as ITokenEntry
                                 where tokenEntry != null &&
                                        tokenEntry.Name == s
                                 select tokenEntry).FirstOrDefault();
                    if (match == null)
                    {
                        GrammarCore.GetParserError(entry.FileName, entry.Line, entry.Column, GDParserErrors.UndefinedTokenReference, string.Format("Lower precedence token {0}", s));
                        break;
                    }
                    lowerTokens.Add(match);
                }
                ((TokenEntry)entry).LowerPrecedenceTokens = lowerTokens.ToArray();
            }
            entry.Branches.ResolveTokenExpressionSeries(entry, file, errors);
        }

        private static void ResolveTokenExpressionSeries(this ITokenExpressionSeries series, ITokenEntry entry, GDFile file, CompilerErrorCollection errors)
        {
            foreach (ITokenExpression ite in series)
                ite.ResolveTokenExpression(entry, file, errors);
        }

        private static void ResolveTokenExpression(this ITokenExpression expression, ITokenEntry entry, GDFile file, CompilerErrorCollection errors)
        {
            IList<ITokenItem> rCopy = (from item in expression
                                       select item).ToList();
            TokenExpression te = expression as TokenExpression;
            IEnumerable<ITokenItem> finalVersion = from item in rCopy
                                                   select (item.ResolveTokenExpressionItem(entry, file, errors));
            te.BaseCollection.Clear();
            foreach (ITokenItem iti in finalVersion)
                te.BaseCollection.Add(iti);
        }

        private static ITokenItem ResolveTokenExpressionItem(this ITokenItem item, ITokenEntry entry, GDFile file, CompilerErrorCollection errors)
        {
            if (item is ITokenGroupItem)
            {
                ((ITokenGroupItem)(item)).ResolveTokenExpressionSeries(entry, file, errors);
                return item;
            }
            else if (item is ISoftReferenceTokenItem)
            {
                return ((ISoftReferenceTokenItem)(item)).ResolveSoftReference(entry, file, errors);
            }
            else if (item is ICommandTokenItem)
            {
                ((ICommandTokenItem)(item)).ResolveSoftReference(entry, file, errors);
                return item;
            }
            else if (item is ILiteralStringTokenItem)
            {
                ILiteralStringTokenItem ilsti = ((ILiteralStringTokenItem)item);
                if (ilsti.Value.Length == 1)
                {
                    LiteralCharTokenItem result = new LiteralCharTokenItem(ilsti.Value[0], ilsti.CaseInsensitive, ilsti.Column, ilsti.Line, ilsti.Position);
                    ((LiteralStringTokenItem)(item)).CloneData(result);
                    return result;
                }
            }//*/
            return item;
        }

        private static void ResolveSoftReference(this ICommandTokenItem source, ITokenEntry entry, GDFile file, System.CodeDom.Compiler.CompilerErrorCollection errors)
        {
            if (source is IScanCommandTokenItem)
            {
                var scanSource = source as ScanCommandTokenItem;
                scanSource.SearchTarget.ResolveTokenExpressionSeries(entry, file, errors);
            }
            else if (source is ISubtractionCommandTokenItem)
            {
                var subtractSource = source as SubtractionCommandTokenItem;
                subtractSource.Left.ResolveTokenExpressionSeries(entry, file, errors);
                subtractSource.Right.ResolveTokenExpressionSeries(entry, file, errors);
            }
        }

        private static ITokenItem ResolveSoftReference(this ISoftReferenceTokenItem item, ITokenEntry entry, GDFile file, CompilerErrorCollection errors)
        {
            ITokenEntry tokenE = tokenEntries.FindScannableEntry(item.PrimaryName);
            if (tokenE != null)
            {
                if (item.SecondaryName != null)
                {
                    ITokenItem iti = tokenE.FindTokenItem(item.SecondaryName);
                    if (iti != null)
                    {
                        if (iti is ILiteralCharTokenItem)
                        {
                            LiteralCharReferenceTokenItem result = new LiteralCharReferenceTokenItem(tokenE, ((ILiteralCharTokenItem)(iti)), item.Column, item.Line, item.Position);
                            if (!(string.IsNullOrEmpty(item.Name)))
                                result.Name = item.Name;
                            if (resolutionAid != null)
                                resolutionAid.ResolvedDualPartToTokenItem(item, tokenE, iti);
                            return result;
                        }
                        else if (iti is ILiteralStringTokenItem)
                        {
                            LiteralStringReferenceTokenItem result = new LiteralStringReferenceTokenItem(tokenE, ((ILiteralStringTokenItem)(iti)), item.Column, item.Line, item.Position);
                            if (!(string.IsNullOrEmpty(item.Name)))
                                result.Name = item.Name;
                            if (resolutionAid != null)
                                resolutionAid.ResolvedDualPartToTokenItem(item, tokenE, iti);
                            return result;
                        }
                        else
                        {
                            /* *
                             * ToDo: Throw an error here for referencing the wrong type of token
                             * */
                        }
                    }
                }
                else
                {
                    TokenReferenceTokenItem result = new TokenReferenceTokenItem(tokenE, item.Column, item.Line, item.Position);
                    ((SoftReferenceTokenItem)(item)).CloneData(result);
                    if (resolutionAid != null)
                        resolutionAid.ResolvedSinglePartToToken(item, tokenE);
                    return result;
                }
            }
            else
                errors.Add(GrammarCore.GetParserError(entry.FileName, item.Line, item.Column, GDParserErrors.UndefinedTokenReference, item.PrimaryName));
            return item;
        }

        private static void ResolveProductionRule<T>(this T entry, GDFile file, CompilerErrorCollection errors)
            where T :
                IProductionRuleEntry 
        {
            if (entry is IProductionRuleTemplateEntry)
                ((IProductionRuleTemplateEntry)(entry)).ResolveProductionRuleTemplate(file, errors);
            entry.ResolveProductionRuleSeries(entry, file, errors); 
        }

        private static void ResolveProductionRuleTemplate(this IProductionRuleTemplateEntry entry, GDFile file, CompilerErrorCollection errors)
        {
            foreach (ProductionRuleTemplatePart part in entry.Parts)
            {
                ITokenEntry reference = null;
                if (part.SpecialExpectancy == TemplatePartExpectedSpecial.None && part.ExpectedSpecific != null && part.ExpectedSpecific is ISoftReferenceProductionRuleItem)
                {
                    var softExpect = part.ExpectedSpecific as ISoftReferenceProductionRuleItem;
                    if ((reference = tokenEntries.FindScannableEntry(softExpect.PrimaryName)) != null)
                    {
                        part.ExpectedSpecific = new TokenReferenceProductionRuleItem(reference, part.Column, part.Line, part.Position);
                        if (resolutionAid != null)
                            resolutionAid.ResolvedSinglePartToToken(softExpect, reference);
                    }
                    else
                        errors.Add(GrammarCore.GetParserError(entry.FileName, part.ExpectedSpecific.Line, part.ExpectedSpecific.Column, GDParserErrors.UndefinedTokenReference, softExpect.PrimaryName));
                    return;
                }
            }
        }

        private static void ResolveProductionRuleSeries<T>(this IProductionRuleSeries series, T entry, GDFile file, CompilerErrorCollection errors)
            where T :
                IProductionRuleEntry
        {
            foreach (IProductionRule ite in series)
                ite.ResolveProductionRule(entry, file, errors);
        }

        private static void ResolveProductionRule<T>(this IProductionRule rule, T entry, GDFile file, CompilerErrorCollection errors)
            where T :
                IProductionRuleEntry
        {
            /* *
             * Copy the source, can't use just a standard IEnumerable because by design
             * it only enumerates the source when requested.  If we tamper with the source,
             * before enumerating the transitionFirst time, the results are distorted.
             * */
            IList<IProductionRuleItem> rCopy = new List<IProductionRuleItem>(rule);

            ProductionRule pr = rule as ProductionRule;
            IEnumerable<IProductionRuleItem> finalVersion = from item in rCopy
                                                            select (item.ResolveProductionRuleItem(entry, file, errors));
            pr.BaseCollection.Clear();
            foreach (IProductionRuleItem iti in finalVersion)
                pr.BaseCollection.Add(iti);
        }

        private static IProductionRuleItem ResolveProductionRuleItem<T>(this IProductionRuleItem item, T entry, GDFile file, CompilerErrorCollection errors)
            where T :
                IProductionRuleEntry
        {
            if (item is IProductionRuleGroupItem)
            {
                ((IProductionRuleGroupItem)(item)).ResolveProductionRuleSeries(entry, file, errors);
                return item;
            }
            else if (item is ILiteralProductionRuleItem)
                return item;
            else if (item is IProductionRulePreprocessorDirective)
                return ((IProductionRulePreprocessorDirective)item).ResolveProductionRuleItem(entry, file, errors);
            else if (item is ISoftTemplateReferenceProductionRuleItem)
                return ((ISoftTemplateReferenceProductionRuleItem)(item)).ResolveTemplateSoftReference(entry, file, errors);
            else if (item is ISoftReferenceProductionRuleItem)
                return ((ISoftReferenceProductionRuleItem)(item)).ResolveSoftReference(entry, file, errors);
            else
                return item;
        }
        public static IProductionRulePreprocessorDirective ResolveProductionRuleItem<T>(this IProductionRulePreprocessorDirective item, T entry, GDFile file, CompilerErrorCollection errors)
            where T :
                IProductionRuleEntry
        {
            return new ProductionRulePreprocessorDirective(item.Directive.ResolveProductionRuleItem(entry, file, errors), item.Column, item.Line, item.Position);
        }
        public static IPreprocessorDirective ResolveProductionRuleItem<T>(this IPreprocessorDirective item, T entry, GDFile file, CompilerErrorCollection errors)
            where T :
                IProductionRuleEntry
        {
            switch (item.Type)
            {
                case EntryPreprocessorType.If:
                case EntryPreprocessorType.IfNotDefined:
                case EntryPreprocessorType.IfDefined:
                case EntryPreprocessorType.ElseIf:
                case EntryPreprocessorType.ElseIfDefined:
                case EntryPreprocessorType.Else:
                    IPreprocessorIfDirective ipid = ((IPreprocessorIfDirective)(item));
                    PreprocessorIfDirective pid = new PreprocessorIfDirective(item.Type, ((IPreprocessorIfDirective)item).Condition, entry.FileName, item.Column, item.Line, item.Position);
                    foreach (IPreprocessorDirective ipd in ipid.Body)
                        ((PreprocessorIfDirective.DirectiveBody)(pid.Body)).Add(ipd.ResolveProductionRuleItem(entry, file, errors));
                    if (ipid.Next != null)
                        pid.Next = (IPreprocessorIfDirective)ipid.Next.ResolveProductionRuleItem(entry, file, errors);
                    return pid;
                case EntryPreprocessorType.DefineRule:
                    IPreprocessorDefineRuleDirective ipdd = ((IPreprocessorDefineRuleDirective)(item));
                    IProductionRule[] dr = new IProductionRule[ipdd.DefinedRules.Length];
                    for (int i = 0; i < ipdd.DefinedRules.Length; i++)
                    {
                        dr[i] = ipdd.DefinedRules[i].Clone();
                        dr[i].ResolveProductionRule(entry, file, errors);
                    }
                    return new PreprocessorDefineRuleDirective(ipdd.DeclareTarget, dr, ipdd.Column, ipdd.Line, ipdd.Position);
                case EntryPreprocessorType.AddRule:
                    IPreprocessorAddRuleDirective ipard = ((IPreprocessorAddRuleDirective)(item));
                    IProductionRule[] ar = new IProductionRule[ipard.Rules.Length];
                    for (int i = 0; i < ipard.Rules.Length; i++)
                    {
                        ar[i] = ipard.Rules[i].Clone();
                        ar[i].ResolveProductionRule(entry, file, errors);
                    }
                    return new PreprocessorAddRuleDirective(ipard.InsertTarget, ar, ipard.Column, ipard.Line, ipard.Position);
                case EntryPreprocessorType.Throw:
                    return item;
                case EntryPreprocessorType.Return:
                    IPreprocessorConditionalReturnDirective ipcrd = ((IPreprocessorConditionalReturnDirective)(item));
                    IProductionRule[] crd = new IProductionRule[ipcrd.Result.Length];
                    for (int i = 0; i < ipcrd.Result.Length; i++)
                    {
                        crd[i] = ipcrd.Result[i].Clone();
                        crd[i].ResolveProductionRule(entry, file, errors);
                    }
                    return new PreprocessorConditionalReturnDirective(crd, ipcrd.Column, ipcrd.Line, ipcrd.Position);
            }
            return item;
        }

        public static IProductionRuleItem ResolveTemplateSoftReference<T>(this ISoftTemplateReferenceProductionRuleItem item, T entry, GDFile file, CompilerErrorCollection errors)
            where T :
                IProductionRuleEntry
        {
            IProductionRuleTemplateEntry iprte = null;
            IList<IProductionRuleTemplateEntry> closeMatches = new List<IProductionRuleTemplateEntry>();
            foreach (IProductionRuleTemplateEntry template in ruleTemplEntries)
            {
                if (template.Name == item.PrimaryName)
                {
                    TemplateArgumentInformation tai = template.GetArgumentInformation();
                    if (item.Parts.Count >= tai.FixedArguments)
                    {
                        closeMatches.Add(template);
                        if (tai.DynamicArguments > 0)
                        {
                            if ((item.Parts.Count - tai.FixedArguments) % tai.DynamicArguments == 0)
                            {
                                if (tai.InvalidArguments == 0)
                                {
                                    iprte = template;
                                    closeMatches.Clear();
                                    break;
                                }
                                else
                                    /* *
                                     * Regardless of the match, according to the template, it has
                                     * invalid argument repeat options.
                                     * */
                                    errors.Add(GrammarCore.GetParserError(template.FileName, template.Line, template.Column, GDParserErrors.InvalidRepeatOptions));
                            }
                        }
                        else if (tai.InvalidArguments == 0)
                        {
                            iprte = template;
                            closeMatches.Clear();
                            break;
                        }
                        else
                            errors.Add(GrammarCore.GetParserError(template.FileName, template.Line, template.Column, GDParserErrors.InvalidRepeatOptions));
                    }
                    else
                    {
                        closeMatches.Add(template);
                        continue;
                    }
                }
            }
            if (iprte != null)
            {
                foreach (IProductionRuleSeries iprs in item.Parts)
                    iprs.ResolveProductionRuleSeries(entry, file, errors);
                TemplateReferenceProductionRuleItem rResult = new TemplateReferenceProductionRuleItem(iprte, new List<IProductionRuleSeries>(item.Parts.ToArray()), item.Column, item.Line, item.Position);
                if (resolutionAid != null)
                    resolutionAid.ResolvedSinglePartToRuleTemplate(item, iprte);
                if (item.RepeatOptions != ScannableEntryItemRepeatInfo.None)
                    rResult.RepeatOptions = item.RepeatOptions;
                if (item.Name != null && item.Name != string.Empty)
                    rResult.Name = item.Name;
                return rResult;
            }
            else if (closeMatches.Count > 0)
                errors.Add(GrammarCore.GetParserError(entry.FileName, item.Line, item.Column, GDParserErrors.DynamicArgumentCountError));
            else
                if (ruleEntries.FindScannableEntry(item.PrimaryName) != null)
                    errors.Add(GrammarCore.GetParserError(entry.FileName, item.Line, item.Column, GDParserErrors.RuleNotTemplate, item.PrimaryName));
            return item;
        }

        public static IProductionRuleItem ResolveSoftReference<T>(this ISoftReferenceProductionRuleItem item, T entry, GDFile file, CompilerErrorCollection errors)
            where T :
                IProductionRuleEntry
        {
            if (entry is IProductionRuleTemplateEntry)
            {
                var templateEntry = entry as IProductionRuleTemplateEntry;
                if (string.IsNullOrEmpty(item.SecondaryName))
                    foreach (IProductionRuleTemplatePart iprtp in templateEntry.Parts)
                        if (iprtp.Name == item.PrimaryName)
                        {
                            TemplateParamReferenceProductionRuleItem result = new TemplateParamReferenceProductionRuleItem(templateEntry, iprtp, item.Column, item.Line, item.Position);
                            if (resolutionAid != null)
                                resolutionAid.ResolvedSinglePartToTemplateParameter(templateEntry, iprtp, item);
                            if (item.RepeatOptions != ScannableEntryItemRepeatInfo.None)
                                result.RepeatOptions = item.RepeatOptions;
                            if (item.Name != null && result.Name == null)
                                result.Name = item.Name;
                            return result;
                        }
            }
            IProductionRuleEntry ipre = ruleEntries.FindScannableEntry(item.PrimaryName);
            if (ipre != null)
            {
                RuleReferenceProductionRuleItem rrpri = new RuleReferenceProductionRuleItem(ipre, item.Column, item.Line, item.Position);
                if (resolutionAid != null)
                    resolutionAid.ResolvedSinglePartToRule(item, ipre);
                ((ProductionRuleItem)(item)).CloneData(rrpri);
                return rrpri;
            }
            else if (ruleTemplEntries.FindScannableEntry(item.PrimaryName) != null)
                errors.Add(GrammarCore.GetParserError(entry.FileName, item.Line, item.Column, GDParserErrors.RuleIsTemplate, item.PrimaryName));
            else
            {
                ITokenEntry tokenE = tokenEntries.FindScannableEntry(item.PrimaryName);
                if (tokenE != null)
                    if (item.SecondaryName != null)
                    {
                        ITokenItem iti = tokenE.FindTokenItem(item.SecondaryName);
                        if (iti != null)
                        {
                            IProductionRuleItem result = null;
                            if (iti is ILiteralCharTokenItem)
                            {
                                if (resolutionAid != null)
                                    resolutionAid.ResolvedDualPartToTokenItem(item, tokenE, iti);
                                result = new LiteralCharReferenceProductionRuleItem(((ILiteralCharTokenItem)(iti)), tokenE, item.Column, item.Line, item.Position, item.IsFlag, item.Counter);
                            }
                            else if (iti is ILiteralStringTokenItem)
                            {
                                if (resolutionAid != null)
                                    resolutionAid.ResolvedDualPartToTokenItem(item, tokenE, iti);
                                result = new LiteralStringReferenceProductionRuleItem(((ILiteralStringTokenItem)(iti)), tokenE, item.Column, item.Line, item.Position, item.IsFlag, item.Counter);
                            }
                            else
                            {
                                /* *
                                 * ToDo: Throw an error here for referencing the wrong type of token
                                 * */
                            }
                            if (result != null)
                            {
                                result.Name = item.Name;
                                result.RepeatOptions = item.RepeatOptions;
                                return result;
                            }
                        }
                    }
                    else
                    {
                        TokenReferenceProductionRuleItem result = new TokenReferenceProductionRuleItem(tokenE, item.Column, item.Line, item.Position);
                        if (resolutionAid != null)
                            resolutionAid.ResolvedSinglePartToToken(item, tokenE);
                        var softee = item as SoftReferenceProductionRuleItem;
                        softee.CloneData(result);
                        return result;
                    }
            }
            return item;
        }

        public static T FindScannableEntry<T>(this IEnumerable<T> items, string name)
            where T :
                IScannableEntry
        {
            foreach (T ipre in items)
                if (((name == "EOF") && (ipre is ITokenEofEntry)) || ((!(ipre is ITokenEofEntry)) && (ipre.Name == name)))
                    return ipre;
            return default(T);
        }

        public static void ResolveTemplates(this GDFile file, CompilerErrorCollection errors)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (errors == null)
                throw new ArgumentNullException("errors");
            foreach (ITokenEntry ite in file.GetTokenEnumerator())
                ite.ResolveToken(file, errors);
            foreach (IProductionRuleEntry ipre in file.GetRulesEnumerator())
                ipre.ResolveProductionRule(file, errors);
            foreach (IProductionRuleTemplateEntry ipre in file.GetTemplateRulesEnumerator())
                ipre.ResolveProductionRule(file, errors);

        }

        internal static IEnumerable<IErrorEntry> GetErrorEnumerator(this IGDFile file)
        {
            return from IEntry item in file
                   where item is IErrorEntry
                   select (IErrorEntry)item;
        }

        internal static IEnumerable<IErrorEntry> GetErrorEnumerator(IEnumerable<IGDFile> files)
        {
            return from file in files
                   from item in file
                   where item is IErrorEntry
                   select (IErrorEntry)item;
        }

        internal static IEnumerable<ITokenEntry> GetTokenEnumerator(IEnumerable<IGDFile> files)
        {
            return from file in files
                   from IEntry item in file
                   where item is ITokenEntry
                   orderby ((ITokenEntry)(item)).Name
                   select (ITokenEntry)item;
        }
        internal static IEnumerable<ITokenEntry> GetTokenEnumerator(this IGDFile file)
        {
            return from IEntry item in file
                   where item is ITokenEntry
                   orderby ((ITokenEntry)(item)).Name
                   select (ITokenEntry)item;
        }

        internal static IEnumerable<IProductionRuleTemplateEntry> GetTemplateRulesEnumerator(IEnumerable<IGDFile> files)
        {
            return from file in files
                   from IEntry item in file
                   where item is IProductionRuleTemplateEntry
                   orderby ((IProductionRuleTemplateEntry)(item)).Name
                   select (IProductionRuleTemplateEntry)item;
        }

        internal static IEnumerable<IProductionRuleTemplateEntry> GetTemplateRulesEnumerator(this IGDFile file)
        {
            return from IEntry item in file
                   where item is IProductionRuleTemplateEntry
                   orderby ((IProductionRuleTemplateEntry)(item)).Name
                   select (IProductionRuleTemplateEntry)item;
        }

        internal static IEnumerable<IProductionRuleEntry> GetRulesEnumerator(IEnumerable<IGDFile> files)
        {
            return from file in files
                   from IEntry item in file
                   where item is IProductionRuleEntry && !(item is IProductionRuleTemplateEntry)
                   orderby ((IProductionRuleEntry)(item)).Name
                   select (IProductionRuleEntry)item;
        }

        internal static IEnumerable<IProductionRuleEntry> GetRulesEnumerator(this IGDFile file)
        {
            return from IEntry item in file
                   where item is IProductionRuleEntry && !(item is IProductionRuleTemplateEntry)
                   orderby ((IProductionRuleEntry)(item)).Name
                   select (IProductionRuleEntry)item;
        }

        internal static void ReplaceReferences(this IGDFile source, IProductionRuleEntry destination, IProductionRuleEntry original)
        {
            var entries = ruleEntries.ToArray();
            for (int i = 0; i < entries.Length; i++)
            {
                var current = entries[i];
                if (current == destination)
                    continue;
                if (current.ContainsReferenceTo(original))
                    current.ReplaceReferences(original, destination);
            }
            source.Remove(original);
        }

        internal static void ReplaceReferences(this IProductionRuleEntry currentEntry, IProductionRuleEntry search, IProductionRuleEntry destination)
        {
            if (currentEntry.ContainsReferenceTo(search))
            {
                var pCurrentEntry = currentEntry as ProductionRuleEntry;
                var series = ((IProductionRuleSeries)(currentEntry)).ReplaceReferences(search, destination);
                pCurrentEntry.BaseCollection.Clear();
                foreach (var expression in series)
                    pCurrentEntry.BaseCollection.Add(expression);
            }
        }

        internal static IProductionRuleSeries ReplaceReferences(this IProductionRuleSeries series, IProductionRuleEntry search, IProductionRuleEntry destination)
        {
            IList<IProductionRule> finalSet = new List<IProductionRule>();
            foreach (var rule in series)
                if (!rule.ContainsReferenceTo(search))
                    finalSet.Add(rule);
                else
                    finalSet.Add(rule.ReplaceReferences(search, destination));
            return new ProductionRuleSeries(finalSet);
        }

        internal static IProductionRule ReplaceReferences(this IProductionRule expression, IProductionRuleEntry search, IProductionRuleEntry destination)
        {
            IProductionRuleItem[] result = new IProductionRuleItem[expression.Count];

            for (int i = 0; i < result.Length; i++)
            {
                var current = expression[i];
                if (current.ContainsReferenceTo(search))
                    result[i] = current.ReplaceReferences(search, destination);
                else
                    result[i] = current;
            }
            return new ProductionRule(new List<IProductionRuleItem>(result), expression.FileName, expression.Column, expression.Line, expression.Position);
        }

        public static IProductionRuleItem ReplaceReferences(this IProductionRuleItem item, IProductionRuleEntry search, IProductionRuleEntry destination)
        {
            if (item is IRuleReferenceProductionRuleItem)
            {
                if (((IRuleReferenceProductionRuleItem)(item)).Reference == search)
                {
                    var result = new RuleReferenceProductionRuleItem(destination, item.Column, item.Line, item.Position);
                    result.Name = item.Name;
                    result.RepeatOptions = item.RepeatOptions;
                    return result;
                }
            }
            else if (item is IProductionRuleGroupItem)
            {
                var result = new ProductionRuleGroupItem(((IProductionRuleSeries)(item)).ReplaceReferences(search, destination).ToArray(), item.Column, item.Line, item.Position);
                result.Name = item.Name;
                result.RepeatOptions = item.RepeatOptions;
                return result;
            }
            return item;
        }

        public static bool ContainsReferenceTo(this IProductionRuleEntry source, IProductionRuleEntry target)
        {
            foreach (var rule in source)
                if (rule.ContainsReferenceTo(target))
                    return true;
            return false;
        }

        public static bool ContainsReferenceTo(this IProductionRuleSeries set, IProductionRuleEntry target)
        {
            foreach (var rule in set)
                if (rule.ContainsReferenceTo(target))
                    return true;
            return false;
        }

        public static bool ContainsReferenceTo(this IProductionRule expression, IProductionRuleEntry target)
        {
            foreach (var item in expression)
                if (item.ContainsReferenceTo(target))
                    return true;
            return false;
        }

        public static bool ContainsReferenceTo(this IProductionRuleItem item, IProductionRuleEntry target)
        {
            if (item is IRuleReferenceProductionRuleItem)
                return ((IRuleReferenceProductionRuleItem)(item)).Reference == target;
            else if (item is IProductionRuleGroupItem)
                return ((IProductionRuleSeries)(item)).ContainsReferenceTo(target);
            return false;
        }

        internal static bool IsEqualTo(this IProductionRuleEntry left, IProductionRuleEntry right)
        {
            if (left.Count != right.Count)
                return false;
            return ((IProductionRuleSeries)(left)).IsEqualTo(right);
        }

        internal static bool IsEqualTo(this IProductionRuleSeries left, IProductionRuleSeries right)
        {
            if (left.Count != right.Count)
                return false;
            for (int i = 0; i < left.Count; i++)
            {
                bool leftMatched = false;
                for (int j = 0; j < right.Count; j++)
                    if (left[i].IsEqualTo(right[i]))
                    {
                        leftMatched = true;
                        break;
                    }
                if (!leftMatched)
                    return false;
            }
            return true;
        }

        internal static bool IsEqualTo(this IProductionRule left, IProductionRule right)
        {
            if (left.Count != right.Count)
                return false;
            for (int i = 0; i < left.Count; i++)
            {
                bool leftMatched = false;
                for (int j = 0; j < right.Count; j++)
                    if (left[i].IsEqualTo(right[i]))
                    {
                        leftMatched = true;
                        break;
                    }
                if (!leftMatched)
                    return false;
            }
            return true;
        }

        internal static bool IsEqualTo(this IProductionRuleItem left, IProductionRuleItem right)
        {
            if (left.GetType() != right.GetType())
                return false;
            if (left is IRuleReferenceProductionRuleItem)
            {
                var leftRef = (IRuleReferenceProductionRuleItem)left;
                var rightRef = (IRuleReferenceProductionRuleItem)right;
                if (leftRef.Reference != rightRef.Reference)
                    return false;
            }
            else if (left is ITokenReferenceProductionRuleItem)
            {
                var leftRef = (ITokenReferenceProductionRuleItem)left;
                var rightRef = (ITokenReferenceProductionRuleItem)right;
                if (leftRef.Reference != rightRef.Reference)
                    return false;
            }
            else if (left is ILiteralCharReferenceProductionRuleItem)
            {
                var leftRef = (ILiteralCharReferenceProductionRuleItem)left;
                var rightRef = (ILiteralCharReferenceProductionRuleItem)right;
                if (leftRef.Literal != rightRef.Literal)
                    return false;
            }
            else if (left is ILiteralStringReferenceProductionRuleItem)
            {
                var leftRef = (ILiteralStringReferenceProductionRuleItem)left;
                var rightRef = (ILiteralStringReferenceProductionRuleItem)right;
                if (leftRef.Literal != rightRef.Literal)
                    return false;
            }
            else if (left is IProductionRuleGroupItem)
                if (!((IProductionRuleSeries)left).IsEqualTo((IProductionRuleGroupItem)right))
                    return false;
            return left.Name == right.Name &&
                   left.RepeatOptions == right.RepeatOptions;
        }
    }
}
