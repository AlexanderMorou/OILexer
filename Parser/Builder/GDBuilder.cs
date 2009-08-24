using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer._Internal;
using Oilexer.Expression;
using Oilexer.Statements;
using System.Linq;
using System.IO;
using System.CodeDom.Compiler;
using OM = System.Collections.ObjectModel;
using GDFD = Oilexer.Parser.GDFileData;

namespace Oilexer.Parser.Builder
{
    using GDEntryCollection =
        OM::Collection<GDFD::IEntry>;
    using GDStringCollection =
        OM::Collection<string>;
    using Oilexer._Internal.Inlining;
    using Oilexer.Parser.GDFileData;
    using Oilexer.Utilities.Collections;
    public class GDBuilder :
        IGDBuilder
    {
        #region IGDBuilder Members

        public IIntermediateProject Build(IParserResults<IGDFile> parserResults)
        {
            IGDLinker igdl = new GDLinker();
            if (parserResults.Errors.HasErrors)
                return null;

            InitLookups(parserResults);
            if (!Link(igdl, parserResults, parserResults.Errors))
                return null;
            if (!InlineTokens(igdl, parserResults, parserResults.Errors))
                return null;
            CleanupRules(parserResults);
            IIntermediateProject result = parserResults.Result.Build(parserResults.Errors);
            _Internal.LinkerCore.errorEntries = null;
            _Internal.LinkerCore.tokenEntries = null;
            _Internal.LinkerCore.ruleEntries = null;
            _Internal.LinkerCore.ruleTemplEntries = null;
            return result;
        }

        private static void CleanupRules(IParserResults<IGDFile> parserResults)
        {
            foreach (var rule in _Internal.LinkerCore.ruleEntries)
                CleanupRule(rule);
        }

        private static void CleanupRule(IProductionRuleSeries series)
        {
            List<IProductionRule> terminalList = new List<IProductionRule>();
            foreach (var expression in series)
            {
                //Only dispose rules that were null post-cleanup.
                if (expression.Count == 0)
                    continue;
                CleanupRule(expression);
                if (expression.Count == 0)
                    terminalList.Add(expression);
            }
            foreach (var expression in terminalList)
                ((ReadOnlyCollection<IProductionRule>)series).baseCollection.Remove(expression);
        }

        private static void CleanupRule(IProductionRule expression)
        {
            List<IProductionRuleGroupItem> terminalList = new List<IProductionRuleGroupItem>();
            foreach (var item in expression)
            {
                if (item is IProductionRuleGroupItem)
                {
                    var gItem = (IProductionRuleGroupItem)item;
                    if (gItem.Count == 0)
                        terminalList.Add(gItem);
                    else
                        CleanupRule(gItem);
                }
            }
            foreach (var item in terminalList)
                ((ProductionRule)(expression)).baseCollection.Remove(item);
        }

        private static bool InlineTokens(IGDLinker igdl, IParserResults<IGDFile> parserResults, CompilerErrorCollection errors)
        {
            GDFD::ITokenEntry[] originals = _Internal.LinkerCore.tokenEntries.ToArray();
            InlinedTokenEntry[] result = new InlinedTokenEntry[originals.Length];
            Dictionary<ITokenEntry, InlinedTokenEntry> originalNewLookup = new Dictionary<ITokenEntry, InlinedTokenEntry>();
            for (int i = 0; i < result.Length; i++)
                result[i] = InliningCore.Inline(originals[i]);
            for (int i = 0; i < result.Length; i++)
                originalNewLookup.Add(originals[i], result[i]);
            for (int i = 0; i < result.Length; i++)
                result[i].ResolveLowerPrecedencesAgain(originalNewLookup);
            GDEntryCollection gdec = parserResults.Result as GDEntryCollection;
            if (gdec == null)
                return false;
            for (int i = 0; i < originals.Length; i++)
            {
                gdec.Insert(gdec.IndexOf(originals[i]), result[i]);
                /* *
                 * The rule building phase will require direct references
                 * to the tokens, because it uses them in a context lookup.
                 * *
                 * Time to replace every reference for the token with
                 * the inlined version.
                 * *
                 * This includes literal reference members for the same reason.
                 * */
                ReplaceTokenReference(originals[i], result[i]);
                parserResults.Result.Remove(originals[i]);
            }
            return true;
        }

        private static void ReplaceTokenReference(ITokenEntry sourceElement, InlinedTokenEntry destination)
        {
            foreach (var rule in _Internal.LinkerCore.ruleEntries)
                ReplaceTokenReference(rule, sourceElement, destination);
        }

        private static void ReplaceTokenReference(IProductionRuleSeries target, ITokenEntry sourceElement, InlinedTokenEntry destination)
        {
            foreach (var expression in target)
                ReplaceTokenReference(expression, sourceElement, destination);
        }

        private static void ReplaceTokenReference(IProductionRule target, ITokenEntry sourceElement, InlinedTokenEntry destination)
        {
            foreach (var item in target)
                ReplaceTokenReference(item, sourceElement, destination);
        }

        private static void ReplaceTokenReference(IProductionRuleItem target, ITokenEntry sourceElement, InlinedTokenEntry destination)
        {
            if (target is IProductionRuleGroupItem)
                ReplaceTokenReference((IProductionRuleSeries)target, sourceElement, destination);
            else if (target is ITokenReferenceProductionRuleItem)
            {
                var tTarget = target as TokenReferenceProductionRuleItem;
                //If the targets don't match, do not do anything.
                if (tTarget.Reference != sourceElement)
                    return;
                tTarget.Reference = destination;
            }
            else if (target is ILiteralStringReferenceProductionRuleItem)
            {
                var tTarget = target as LiteralStringReferenceProductionRuleItem;
                if (tTarget.Source != sourceElement)
                    return;
                tTarget.Source = destination;
                //The inlined token provides a lookup to assist things.
                tTarget.Literal = (ILiteralStringTokenItem)destination.OldNewLookup[tTarget.Literal];
            }
            else if (target is ILiteralCharReferenceProductionRuleItem)
            {
                var tTarget = target as LiteralCharReferenceProductionRuleItem;
                if (tTarget.Source != sourceElement)
                    return;
                tTarget.Source = destination;
                tTarget.Literal = (ILiteralCharTokenItem)destination.OldNewLookup[tTarget.Literal];
            }
        }

        private static bool Link(IGDLinker igdl, IParserResults<IGDFile> parserResults, CompilerErrorCollection errors)
        {
            ((GDFile)parserResults.Result).Add(new TokenEofEntry());
            igdl.ResolveTemplates((GDFile)parserResults.Result, parserResults.Errors);
            if (parserResults.Errors.HasErrors)
                return false;
            igdl.ExpandTemplates((GDFile)parserResults.Result, parserResults.Errors);
            if (parserResults.Errors.HasErrors)
                return false;
            igdl.FinalLink((GDFile)parserResults.Result, parserResults.Errors);
            if (parserResults.Errors.HasErrors)
                return false;
            return true;
        }

        private static void InitLookups(IParserResults<IGDFile> parserResults)
        {
            _Internal.LinkerCore.errorEntries = (parserResults.Result.GetErrorEnumerator());
            _Internal.LinkerCore.tokenEntries = (parserResults.Result.GetTokenEnumerator());
            _Internal.LinkerCore.ruleEntries = (parserResults.Result.GetRulesEnumerator());
            _Internal.LinkerCore.ruleTemplEntries = (parserResults.Result.GetTemplateRulesEnumerator());
        }
        #endregion
    }
}
