using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer.Inlining;
/*---------------------------------------------------------------------\
| Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
|----------------------------------------------------------------------|
| The Abstraction Project's code is provided under a contract-release  |
| basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
\-------------------------------------------------------------------- */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    internal static partial class OilexerGrammarLinkerCore
    {
        /* *
         * Set these before use :]
         * */
        internal static IEnumerable<IOilexerGrammarTokenEntry> tokenEntries;
        internal static IEnumerable<IOilexerGrammarProductionRuleEntry> ruleEntries;
        internal static IEnumerable<IOilexerGrammarProductionRuleTemplateEntry> ruleTemplEntries;
        internal static IEnumerable<IOilexerGrammarErrorEntry> errorEntries;
        internal static _OilexerGrammarResolutionAssistant resolutionAid;

        private static void ResolveToken(this IOilexerGrammarTokenEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            List<IOilexerGrammarTokenEntry> lowerTokens = new List<IOilexerGrammarTokenEntry>();
            if (entry.LowerPrecedenceTokens == null)
            {
                foreach (var s in entry.LowerPrecedenceNames)
                {
                    var match = (from generalEntry in tokenEntries
                                 let tokenEntry = generalEntry as IOilexerGrammarTokenEntry
                                 where tokenEntry != null &&
                                        tokenEntry.Name == s.Name
                                 select tokenEntry).FirstOrDefault();
                    if (match == null)
                    {
                        errors.SourceModelError<IOilexerGrammarTokenEntry>(OilexerGrammarCore.CompilerErrors.UndefinedTokenReference, new LineColumnPair(s.Line, s.Column), new LineColumnPair(s.Line, s.Column + s.Name.Length), new Uri(entry.FileName, UriKind.RelativeOrAbsolute), entry, string.Format(@", lower precedence: '{0}'", s.Name));
                        break;
                    }
                    lowerTokens.Add(match);
                }
                ((OilexerGrammarTokenEntry)entry).LowerPrecedenceTokens = lowerTokens.ToArray();
            }
            entry.Branches.ResolveTokenExpressionSeries(entry, file, errors);
        }

        private static void ResolveTokenExpressionSeries(this ITokenExpressionSeries series, IOilexerGrammarTokenEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            foreach (ITokenExpression ite in series)
                ite.ResolveTokenExpression(entry, file, errors);
        }

        private static void ResolveTokenExpression(this ITokenExpression expression, IOilexerGrammarTokenEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
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

        private static ITokenItem ResolveTokenExpressionItem(this ITokenItem item, IOilexerGrammarTokenEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
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

        private static void ResolveSoftReference(this ICommandTokenItem source, IOilexerGrammarTokenEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
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
            else if (source is IBaseEncodeGraphCommand)
            {
                var graph = source as BaseEncodeGraphCommand;
                graph.EncodeTarget.ResolveTokenExpressionSeries(entry, file, errors);
            }
        }

        private static ITokenItem ResolveSoftReference(this ISoftReferenceTokenItem item, IOilexerGrammarTokenEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            IOilexerGrammarTokenEntry tokenE = tokenEntries.OilexerGrammarFindScannableEntry(item.PrimaryName);
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
                            errors.SourceModelError<ISoftReferenceTokenItem>(OilexerGrammarCore.CompilerErrors.UndefinedTokenReference, new LineColumnPair(item.Line, item.Column), new LineColumnPair(item.Line, item.Column + item.PrimaryName.Length), new Uri(entry.FileName, UriKind.RelativeOrAbsolute), item, string.Format(" '{0}'", item.PrimaryName));
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
            else if (item.SecondaryName == null)
                errors.SourceModelError<ISoftReferenceTokenItem>(OilexerGrammarCore.CompilerErrors.UndefinedTokenReference, new LineColumnPair(item.Line, item.Column), new LineColumnPair(item.Line, item.Column + item.PrimaryName.Length), new Uri(entry.FileName, UriKind.RelativeOrAbsolute), item, string.Format(" '{0}'", item.PrimaryName));
            else
                errors.SourceModelError<ISoftReferenceTokenItem>(OilexerGrammarCore.CompilerErrors.UndefinedTokenReference, new LineColumnPair(item.Line, item.Column), new LineColumnPair(item.SecondaryToken.Line, item.SecondaryToken.Column + item.SecondaryToken.Length), new Uri(entry.FileName, UriKind.RelativeOrAbsolute), item, string.Format(" '{0}'", item.PrimaryName));
            return item;
        }

        private static void ResolveProductionRule<T>(this T entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
            where T :
                IOilexerGrammarProductionRuleEntry
        {
            if (entry is IOilexerGrammarProductionRuleTemplateEntry)
                ((IOilexerGrammarProductionRuleTemplateEntry)(entry)).ResolveProductionRuleTemplate(file, errors);
            entry.ResolveProductionRuleSeries(entry, file, errors);
        }

        private static void ResolveProductionRuleTemplate(this IOilexerGrammarProductionRuleTemplateEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            foreach (ProductionRuleTemplatePart part in entry.Parts)
            {
                IOilexerGrammarTokenEntry reference = null;
                if (part.SpecialExpectancy == TemplatePartExpectedSpecial.None && part.ExpectedSpecific != null && part.ExpectedSpecific is ISoftReferenceProductionRuleItem)
                {
                    var softExpect = part.ExpectedSpecific as ISoftReferenceProductionRuleItem;
                    if ((reference = tokenEntries.OilexerGrammarFindScannableEntry(softExpect.PrimaryName)) != null)
                    {
                        part.ExpectedSpecific = new TokenReferenceProductionRuleItem(reference, part.Column, part.Line, part.Position);
                        if (resolutionAid != null)
                            resolutionAid.ResolvedSinglePartToToken(softExpect, reference);
                    }
                    else
                        errors.SourceModelError<ISoftReferenceProductionRuleItem>(OilexerGrammarCore.CompilerErrors.UndefinedTokenReference, new LineColumnPair(part.ExpectedSpecific.Column, part.ExpectedSpecific.Line), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), softExpect, string.Format(" '{0}'", softExpect.PrimaryName));
                    return;
                }
            }
        }

        private static void ResolveProductionRuleSeries<T>(this IProductionRuleSeries series, T entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
            where T :
                IOilexerGrammarProductionRuleEntry
        {
            foreach (IProductionRule ite in series)
                ite.ResolveProductionRule(entry, file, errors);
        }

        private static void ResolveProductionRule<T>(this IProductionRule rule, T entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
            where T :
                IOilexerGrammarProductionRuleEntry
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

        private static IProductionRuleItem ResolveProductionRuleItem<T>(this IProductionRuleItem item, T entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
            where T :
                IOilexerGrammarProductionRuleEntry
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
        public static IProductionRulePreprocessorDirective ResolveProductionRuleItem<T>(this IProductionRulePreprocessorDirective item, T entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
            where T :
                IOilexerGrammarProductionRuleEntry
        {
            return new ProductionRulePreprocessorDirective(item.Directive.ResolveProductionRuleItem(entry, file, errors), item.Column, item.Line, item.Position);
        }
        public static IPreprocessorDirective ResolveProductionRuleItem<T>(this IPreprocessorDirective item, T entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
            where T :
                IOilexerGrammarProductionRuleEntry
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

        private static List<T> GetList<T>(T item) { return new List<T>(); }

        public static IProductionRuleItem ResolveTemplateSoftReference<T>(this ISoftTemplateReferenceProductionRuleItem item, T entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
            where T :
                IOilexerGrammarProductionRuleEntry
        {
            IOilexerGrammarProductionRuleTemplateEntry iprte = null;
            var closeMatches = GetList(new { Entry = (IOilexerGrammarProductionRuleTemplateEntry)null, ArgumentInformation = default(TemplateArgumentInformation) });
            foreach (IOilexerGrammarProductionRuleTemplateEntry template in ruleTemplEntries)
            {
                if (template.Name == item.PrimaryName)
                {
                    TemplateArgumentInformation tai = template.GetArgumentInformation();
                    if (item.Parts.Count >= tai.FixedArguments)
                    {
                        closeMatches.Add(new { Entry = template, ArgumentInformation = tai });
                        if (tai.DynamicArguments > 0)
                        {
                            if ((item.Parts.Count - tai.FixedArguments) % tai.DynamicArguments == 0)
                            {
                                if (tai.InvalidArguments == 0)
                                {
                                    iprte = template;
                                    break;
                                }
                            }
                        }
                        else if (tai.InvalidArguments == 0)
                        {
                            if (item.Parts.Count == tai.FixedArguments)
                            {
                                iprte = template;
                                break;
                            }
                        }
                    }
                    else
                    {
                        closeMatches.Add(new { Entry = template, ArgumentInformation = tai });
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
            {
                var fixedMatch = (from templateArguments in closeMatches
                                  let arguments = templateArguments.ArgumentInformation
                                  where arguments.FixedArguments > 0
                                  orderby arguments.FixedArguments descending
                                  select templateArguments.Entry);
                IOilexerGrammarProductionRuleTemplateEntry closestMismatch = null;
                foreach (var mismatch in fixedMatch)
                {
                    if (item.Parts.Count > mismatch.Parts.Count)
                    {
                        closestMismatch = mismatch;
                        break;
                    }
                }
                /**/
                if (closestMismatch == null)
                    closestMismatch = fixedMatch.Last();
                if (fixedMatch != null)
                    errors.SourceModelError<ISoftTemplateReferenceProductionRuleItem, IOilexerGrammarProductionRuleTemplateEntry>(OilexerGrammarCore.CompilerErrors.FixedArgumentMismatch, new LineColumnPair(item.Line, item.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), item, closestMismatch, new string[] { closestMismatch.Name, closestMismatch.Parts.Count.ToString(), item.Parts.Count.ToString() });
                else
                {
                    var dynamicMatch = (from templateArguments in closeMatches
                                        let arguments = templateArguments.ArgumentInformation
                                        where arguments.DynamicArguments > 0
                                        select templateArguments.Entry).FirstOrDefault();
                    if (dynamicMatch != null)
                        errors.SourceModelError<IOilexerGrammarProductionRuleTemplateEntry>(OilexerGrammarCore.CompilerErrors.DynamicArgumentCountError, new LineColumnPair(item.Line, item.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), dynamicMatch);
                    else
                    {
                        var invalidMatch = (from templateArguments in closeMatches
                                            let arguments = templateArguments.ArgumentInformation
                                            where arguments.InvalidArguments > 0
                                            select templateArguments.Entry).FirstOrDefault();
                        if (invalidMatch != null)
                            errors.SourceModelError<IOilexerGrammarProductionRuleTemplateEntry>(OilexerGrammarCore.CompilerErrors.InvalidRepeatOptions, new LineColumnPair(item.Line, item.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), invalidMatch);
                        else if (ruleEntries.OilexerGrammarFindScannableEntry(item.PrimaryName) != null)
                            errors.SourceModelError<ISoftTemplateReferenceProductionRuleItem>(OilexerGrammarCore.CompilerErrors.RuleNotTemplate, new LineColumnPair(item.Line, item.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), item, item.PrimaryName);
                    }
                }
                //errors.Add(OilexerGrammarCore.GetParserError(entry.FileName, item.Line, item.Column, OilexerGrammarParserErrors.DynamicArgumentCountError));
            }
            else if (ruleEntries.OilexerGrammarFindScannableEntry(item.PrimaryName) != null)
                errors.SourceModelError<ISoftTemplateReferenceProductionRuleItem>(OilexerGrammarCore.CompilerErrors.RuleNotTemplate, new LineColumnPair(item.Line, item.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), item, item.PrimaryName);
            else
            {
                var matches = (from template in ruleTemplEntries
                               where template.Name == item.PrimaryName
                               select template).Count();
                if (matches > 0)
                    errors.SourceError(OilexerGrammarCore.CompilerErrors.FixedArgumentMismatch, new LineColumnPair(item.Line, item.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), item.PrimaryName);
                else
                    errors.SourceError(OilexerGrammarCore.CompilerErrors.UndefinedRuleReference, new LineColumnPair(item.Line, item.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), item.PrimaryName);
            }
            return item;
        }

        public static IProductionRuleItem ResolveSoftReference<T>(this ISoftReferenceProductionRuleItem item, T entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
            where T :
                IOilexerGrammarProductionRuleEntry
        {
            if (entry is IOilexerGrammarProductionRuleTemplateEntry)
            {
                var templateEntry = entry as IOilexerGrammarProductionRuleTemplateEntry;
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
            IOilexerGrammarProductionRuleEntry ipre = ruleEntries.OilexerGrammarFindScannableEntry(item.PrimaryName);
            if (ipre != null)
            {
                RuleReferenceProductionRuleItem rrpri = new RuleReferenceProductionRuleItem(ipre, item.Column, item.Line, item.Position);
                if (resolutionAid != null)
                    resolutionAid.ResolvedSinglePartToRule(item, ipre);
                ((ProductionRuleItem)(item)).CloneData(rrpri);
                return rrpri;
            }
            else if (ruleTemplEntries.OilexerGrammarFindScannableEntry(item.PrimaryName) != null)
                errors.SourceModelError<ISoftReferenceProductionRuleItem>(OilexerGrammarCore.CompilerErrors.RuleIsTemplate, new LineColumnPair(item.Line, item.Column), LineColumnPair.Zero, new Uri(entry.FileName, UriKind.RelativeOrAbsolute), item, item.PrimaryName);
            else
            {
                IOilexerGrammarTokenEntry tokenE = tokenEntries.OilexerGrammarFindScannableEntry(item.PrimaryName);
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

        public static IProductionRuleItem GetReference(this ITokenItem target, IOilexerGrammarTokenEntry entry)
        {
            if (target is ILiteralStringTokenItem)
                return new LiteralStringReferenceProductionRuleItem(((ILiteralStringTokenItem)(target)), entry, 0, 0, 0);
            else if (target is ILiteralCharTokenItem)
                return new LiteralCharReferenceProductionRuleItem(((ILiteralCharTokenItem)(target)), entry, 0, 0, 0);
            else
                throw new ArgumentException("target must be either a string or character literal item.", "target");
        }

        public static IProductionRuleItem GetReference(this IOilexerGrammarTokenEntry target)
        {
            return new TokenReferenceProductionRuleItem(target, 0, 0, 0);
        }

        public static IProductionRuleItem GetReference(this IGrammarTokenSymbol target)
        {
            if (target is IGrammarConstantItemSymbol)
            {
                var gcis = (IGrammarConstantItemSymbol)target;
                return gcis.SourceItem.GetReference(gcis.Source);
            }
            else if (target is IGrammarConstantEntrySymbol || target is IGrammarVariableSymbol)
            {
                var igts = (IGrammarTokenSymbol)(target);
                return igts.Source.GetReference();
            }
            else
                throw new ArgumentException("target must be either a constant item token, constant token, or variable token symbol.", "target");
        }

        public static T OilexerGrammarFindScannableEntry<T>(this IEnumerable<T> items, string name)
            where T :
                IOilexerGrammarScannableEntry
        {
            foreach (T ipre in items)
                if (((name == "EOF") && (ipre is IOilexerGrammarTokenEofEntry)) || ((!(ipre is IOilexerGrammarTokenEofEntry)) && (ipre.Name == name)))
                    return ipre;
            return default(T);
        }

        public static void IdentityResolution(this OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (errors == null)
                throw new ArgumentNullException("errors");
            var entries = from entry in file
                          where entry is IOilexerGrammarTokenEntry ||
                                entry is IOilexerGrammarProductionRuleEntry ||
                                entry is IOilexerGrammarProductionRuleTemplateEntry
                          select entry;
            entries.AsParallel().ForAll(entry =>
            {
                if (entry is IOilexerGrammarTokenEntry)
                    ((IOilexerGrammarTokenEntry)(entry)).ResolveToken(file, errors);
                else if (entry is IOilexerGrammarProductionRuleEntry)
                    ((IOilexerGrammarProductionRuleEntry)(entry)).ResolveProductionRule(file, errors);
                else if (entry is IOilexerGrammarProductionRuleTemplateEntry)
                    ((IOilexerGrammarProductionRuleTemplateEntry)(entry)).ResolveProductionRule(file, errors);
            });

        }

        internal static IEnumerable<IOilexerGrammarErrorEntry> GetErrorEnumerator(this IOilexerGrammarFile file)
        {
            return from IOilexerGrammarEntry item in file
                   where item is IOilexerGrammarErrorEntry
                   select (IOilexerGrammarErrorEntry)item;
        }

        internal static IEnumerable<IOilexerGrammarErrorEntry> GetErrorEnumerator(IEnumerable<IOilexerGrammarFile> files)
        {
            return from file in files
                   from item in file
                   where item is IOilexerGrammarErrorEntry
                   select (IOilexerGrammarErrorEntry)item;
        }

        internal static IEnumerable<IOilexerGrammarTokenEntry> GetTokenEnumerator(IEnumerable<IOilexerGrammarFile> files)
        {
            return from file in files
                   from IOilexerGrammarEntry item in file
                   where item is IOilexerGrammarTokenEntry
                   orderby ((IOilexerGrammarTokenEntry)(item)).Name
                   select (IOilexerGrammarTokenEntry)item;
        }
        internal static IEnumerable<IOilexerGrammarTokenEntry> GetTokenEnumerator(this IOilexerGrammarFile file)
        {
            return from IOilexerGrammarEntry item in file
                   where item is IOilexerGrammarTokenEntry
                   orderby ((IOilexerGrammarTokenEntry)(item)).Name
                   select (IOilexerGrammarTokenEntry)item;
        }

        internal static IEnumerable<IOilexerGrammarProductionRuleTemplateEntry> GetTemplateRulesEnumerator(IEnumerable<IOilexerGrammarFile> files)
        {
            return from file in files
                   from IOilexerGrammarEntry item in file
                   where item is IOilexerGrammarProductionRuleTemplateEntry
                   orderby ((IOilexerGrammarProductionRuleTemplateEntry)(item)).Name
                   select (IOilexerGrammarProductionRuleTemplateEntry)item;
        }

        internal static IEnumerable<IOilexerGrammarProductionRuleTemplateEntry> GetTemplateRulesEnumerator(this IOilexerGrammarFile file)
        {
            return from IOilexerGrammarEntry item in file
                   where item is IOilexerGrammarProductionRuleTemplateEntry
                   orderby ((IOilexerGrammarProductionRuleTemplateEntry)(item)).Name
                   select (IOilexerGrammarProductionRuleTemplateEntry)item;
        }

        internal static IEnumerable<IOilexerGrammarProductionRuleEntry> GetRulesEnumerator(IEnumerable<IOilexerGrammarFile> files)
        {
            return from file in files
                   from IOilexerGrammarEntry item in file
                   where item is IOilexerGrammarProductionRuleEntry && !(item is IOilexerGrammarProductionRuleTemplateEntry)
                   orderby ((IOilexerGrammarProductionRuleEntry)(item)).Name
                   select (IOilexerGrammarProductionRuleEntry)item;
        }

        internal static IEnumerable<IOilexerGrammarProductionRuleEntry> GetRulesEnumerator(this IOilexerGrammarFile file)
        {
            return from IOilexerGrammarEntry item in file
                   where item is IOilexerGrammarProductionRuleEntry && !(item is IOilexerGrammarProductionRuleTemplateEntry)
                   orderby ((IOilexerGrammarProductionRuleEntry)(item)).Name
                   select (IOilexerGrammarProductionRuleEntry)item;
        }

        internal static void ReplaceReferences(this IOilexerGrammarFile source, IOilexerGrammarProductionRuleEntry destination, IOilexerGrammarProductionRuleEntry original)
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

        internal static void ReplaceReferences(this IOilexerGrammarProductionRuleEntry currentEntry, IOilexerGrammarProductionRuleEntry search, IOilexerGrammarProductionRuleEntry destination)
        {
            if (currentEntry.ContainsReferenceTo(search))
            {
                var pCurrentEntry = currentEntry as OilexerGrammarProductionRuleEntry;
                var series = ((IProductionRuleSeries)(currentEntry)).ReplaceReferences(search, destination);
                pCurrentEntry.BaseCollection.Clear();
                foreach (var expression in series)
                    pCurrentEntry.BaseCollection.Add(expression);
            }
        }

        internal static IProductionRuleSeries ReplaceReferences(this IProductionRuleSeries series, IOilexerGrammarProductionRuleEntry search, IOilexerGrammarProductionRuleEntry destination)
        {
            IList<IProductionRule> finalSet = new List<IProductionRule>();
            foreach (var rule in series)
                if (!rule.ContainsReferenceTo(search))
                    finalSet.Add(rule);
                else
                    finalSet.Add(rule.ReplaceReferences(search, destination));
            return new ProductionRuleSeries(finalSet);
        }

        internal static IProductionRule ReplaceReferences(this IProductionRule expression, IOilexerGrammarProductionRuleEntry search, IOilexerGrammarProductionRuleEntry destination)
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

        public static IProductionRuleItem ReplaceReferences(this IProductionRuleItem item, IOilexerGrammarProductionRuleEntry search, IOilexerGrammarProductionRuleEntry destination)
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

        public static bool ContainsReferenceTo(this IOilexerGrammarProductionRuleEntry source, IOilexerGrammarProductionRuleEntry target)
        {
            foreach (var rule in source)
                if (rule.ContainsReferenceTo(target))
                    return true;
            return false;
        }

        public static bool Contains(this IProductionRuleSeries series, IProductionRuleItem target)
        {
            Stack<IProductionRuleSeries> setsToLookAfter = new Stack<IProductionRuleSeries>();
            setsToLookAfter.Push(series);
            while (setsToLookAfter.Count > 0)
            {
                var currentSet = setsToLookAfter.Pop();
                foreach (var expression in currentSet)
                    foreach (var item in expression)
                    {
                        if (item == target)
                            return true;
                        if (item is IProductionRuleGroupItem)
                            setsToLookAfter.Push((IProductionRuleGroupItem)item);
                    }
            }
            return false;
        }

        public static bool Contains(this ITokenExpressionSeries series, ITokenItem target)
        {
            Stack<ITokenExpressionSeries> setsToLookAfter = new Stack<ITokenExpressionSeries>();
            setsToLookAfter.Push(series);
            while (setsToLookAfter.Count > 0)
            {
                var currentSet = setsToLookAfter.Pop();
                foreach (var expression in currentSet)
                    foreach (var item in expression)
                    {
                        if (item == target)
                            return true;
                        //if (item is ITokenGroupItem)
                        //    setsToLookAfter.Push((ITokenGroupItem)item);
                    }
            }
            return false;
        }

        public static bool ContainsReferenceTo(this IProductionRuleSeries set, IOilexerGrammarProductionRuleEntry target)
        {
            foreach (var rule in set)
                if (rule.ContainsReferenceTo(target))
                    return true;
            return false;
        }

        public static bool ContainsReferenceTo(this IProductionRule expression, IOilexerGrammarProductionRuleEntry target)
        {
            foreach (var item in expression)
                if (item.ContainsReferenceTo(target))
                    return true;
            return false;
        }

        public static bool ContainsReferenceTo(this IProductionRuleItem item, IOilexerGrammarProductionRuleEntry target)
        {
            if (item is IRuleReferenceProductionRuleItem)
                return ((IRuleReferenceProductionRuleItem)(item)).Reference == target;
            else if (item is IProductionRuleGroupItem)
                return ((IProductionRuleSeries)(item)).ContainsReferenceTo(target);
            return false;
        }

        internal static bool IsEqualTo(this IOilexerGrammarProductionRuleEntry left, IOilexerGrammarProductionRuleEntry right)
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
