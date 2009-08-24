using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
using System.Collections.ObjectModel;
using Oilexer.Utilities.Collections;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Inlining
{
    internal static class InliningCore
    {
        public static InlinedTokenEntry Inline(ITokenEntry entry)
        {
            if (entry is ITokenEofEntry)
                return new InlinedTokenEofEntry((ITokenEofEntry)entry);
            else
                return new InlinedTokenEntry(entry);
        }

        public static ITokenExpressionSeries Inline(ITokenExpressionSeries source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            return new InlinedTokenExpressionSeries(source, sourceRoot, root, oldNewLookup);
        }

        public static ITokenExpression[] Inline(ITokenExpression[] source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            int c = 0;
            for (int i = 0; i < source.Length; i++)
                if (source[i].Count > 0)
                    c++;
            ITokenExpression[] result = new ITokenExpression[c];
            for (int i = 0, index = 0; i < source.Length; i++)
                if (source[i].Count > 0)
                    result[index++] = Inline(source[i], sourceRoot, root, oldNewLookup);
            return result;
        }

        public static ITokenExpression Inline(ITokenExpression source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            return new InlinedTokenExpression(source, sourceRoot, root, oldNewLookup);
        }

        public static Collection<ITokenItem> Inline(IControlledStateCollection<ITokenItem> source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            Collection<ITokenItem> result = new Collection<ITokenItem>();
            foreach (var item in source)
            {
                var currentSet = new List<ITokenItem>();
                currentSet.Add(item);
                for (int i = 0; i < currentSet.Count; i++)
                {
                    var subItem = currentSet[i];
                    if (subItem is ITokenGroupItem && subItem.RepeatOptions == ScannableEntryItemRepeatOptions.None && string.IsNullOrEmpty(subItem.Name) &&
                        ((ITokenGroupItem)(subItem)).Count == 1)
                    {
                        int j = i;
                        foreach (var element in ((ITokenGroupItem)(subItem))[0])
                            currentSet.Insert(++j, element);
                        currentSet.Remove(currentSet[i]);
                        i--;
                        continue;
                    }
                }
                foreach (var subItem in currentSet)
                {
                    result.Add(Inline(subItem, sourceRoot, root, oldNewLookup));
                }
            }
            return result;
        }

        public static ITokenItem Inline(ITokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            if (source is ITokenGroupItem)
            {
                var kSource = ((ITokenGroupItem)(source));
                if (kSource.Count == 1 && kSource[0].Count == 1)
                {
                    var lSource = kSource[0][0];
                    if (lSource.RepeatOptions == ScannableEntryItemRepeatOptions.None ||
                        kSource.RepeatOptions == ScannableEntryItemRepeatOptions.None)
                    {
                        var result = Inline(lSource, sourceRoot, root, oldNewLookup);
                        if (lSource.RepeatOptions == ScannableEntryItemRepeatOptions.None)
                            result.RepeatOptions = kSource.RepeatOptions;
                        else if (kSource.RepeatOptions == ScannableEntryItemRepeatOptions.None)
                            result.RepeatOptions = lSource.RepeatOptions;
                        if (string.IsNullOrEmpty(kSource.Name))
                            result.Name = lSource.Name;
                        else if (string.IsNullOrEmpty(lSource.Name))
                            result.Name = kSource.Name;
                        return result;
                    }
                }
                return Inline((ITokenGroupItem)source, sourceRoot, root, oldNewLookup);
            }
            else if (source is ILiteralCharTokenItem)
                return Inline((ILiteralCharTokenItem)source, sourceRoot, root, oldNewLookup);
            else if (source is ILiteralStringTokenItem)
                return Inline((ILiteralStringTokenItem)source, sourceRoot, root, oldNewLookup);
            else if (source is IScanCommandTokenItem)
                return Inline((IScanCommandTokenItem)source, sourceRoot, root, oldNewLookup);
            else if (source is ITokenReferenceTokenItem)
            {
                var kSource = (ITokenReferenceTokenItem)source;
                var rSource = kSource.Reference;
                if (rSource.Branches.Count == 1 && rSource.Branches[0].Count == 1)
                {
                    var lSource = rSource.Branches[0][0];
                    if (lSource.RepeatOptions == ScannableEntryItemRepeatOptions.None ||
                        kSource.RepeatOptions == ScannableEntryItemRepeatOptions.None)
                    {
                        var result = Inline(lSource, sourceRoot, root, oldNewLookup);
                        if (lSource.RepeatOptions == ScannableEntryItemRepeatOptions.None)
                            result.RepeatOptions = kSource.RepeatOptions;
                        else if (kSource.RepeatOptions == ScannableEntryItemRepeatOptions.None)
                            result.RepeatOptions = lSource.RepeatOptions;
                        if (string.IsNullOrEmpty(kSource.Name))
                            result.Name = lSource.Name;
                        else if (string.IsNullOrEmpty(lSource.Name))
                            result.Name = kSource.Name;
                        return result;
                    }
                }
                return Inline((ITokenReferenceTokenItem)source, sourceRoot, root, oldNewLookup);
            }
            else if (source is ICharRangeTokenItem)
                return Inline((ICharRangeTokenItem)source, sourceRoot, root, oldNewLookup);
            else if (source is ILiteralStringReferenceTokenItem)
                return Inline((ILiteralStringReferenceTokenItem)source, sourceRoot, root, oldNewLookup);
            else if (source is ILiteralCharReferenceTokenItem)
                return Inline((ILiteralCharReferenceTokenItem)source, sourceRoot, root, oldNewLookup);
            throw new NotImplementedException("Not supported");
        }

        public static IInlinedTokenItem Inline(ITokenReferenceTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedTokenReferenceTokenItem(source, sourceRoot, root, oldNewLookup);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(ICharRangeTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedCharRangeTokenItem(source, sourceRoot, root);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(ITokenGroupItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedTokenGroupItem(source, sourceRoot, root, oldNewLookup);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(ILiteralStringTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedLiteralStringTokenItem(source, sourceRoot, root);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(ILiteralCharTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedLiteralCharTokenItem(source, sourceRoot, root);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(ILiteralCharReferenceTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedLiteralCharReferenceTokenItem(source, sourceRoot, root);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(ILiteralStringReferenceTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedLiteralStringReferenceTokenItem(source, sourceRoot, root);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }

        public static IInlinedTokenItem Inline(IScanCommandTokenItem source, ITokenEntry sourceRoot, InlinedTokenEntry root, IDictionary<ITokenItem, ITokenItem> oldNewLookup)
        {
            var result = new InlinedScanCommandTokenItem(source, sourceRoot, root);
            if (!oldNewLookup.ContainsKey(source))
                oldNewLookup.Add(source, result);
            return result;
        }
    }
}
