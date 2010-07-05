using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Utilities.Common;
using Oilexer.Parser;
namespace Oilexer._Internal
{
    partial class LinkerCore
    {
        static IDictionary<string, string> OperatorNameLookup = new Dictionary<string, string>() 
            { 
                { "/",  "ForwardSlash"       }, 
                { "\\", "Backslash"          }, 
                { "@",  "At"                 }, 
                { "*",  "Asterisk"           }, 
                { "<",  "LessThan"           }, 
                { ">",  "GreaterThan"        },
                { "(",  "LeftParenthesis"    }, 
                { ")",  "RightParenthesis"   }, 
                { "&",  "Ampersand"          }, 
                { "%",  "Percent"            }, 
                { "-",  "Minus"              }, 
                { "~",  "Tilda"              }, 
                { "`",  "GraveAccent"        }, 
                { "'",  "SingleQuote"        }, 
                { "\"", "DoubleQuote"        },
                { ";",  "SemiColon"          },
                { ":",  "Colon"              },
                { ",",  "Comma"              }, 
                { ".",  "Period"             }, 
                { "[",  "LeftSquareBracket"  }, 
                { "]",  "RightSquareBracket" }, 
                { "{",  "LeftCurlyBracket"   }, 
                { "}",  "RightCurlyBracket"  }, 
                { "|",  "Pipe"               }, 
                { "+",  "Plus"               }, 
                { "=",  "Eq"                 }, 
                { "!",  "Exclamation"        }, 
                { "#",  "Pound"              },
                { "$",  "Dollar"             },
                { "\r", "CR"                 },
                { "\n", "LF"                 },
                { " ",  "_"                  }, 
                { "\t", "Tab"                }, 
                { "^",  "Circumflex"         }, 
                { "0",  "Zero"               },
                { "1",  "One"                },
                { "2",  "Two"                },
                { "3",  "Three"              },
                { "4",  "Four"               },
                { "5",  "Five"               },
                { "6",  "Six"                },
                { "7",  "Seven"              },
                { "8",  "Eight"              },
                { "9",  "Nine"               }, 
                { "\0", "NullChar"           },
            };
        private static IProductionRuleEntry currentEntry = null;
        public static void FinalLink(this GDFile file, CompilerErrorCollection errors)
        {
            currentEntry = null;
            IList<IProductionRuleEntry> original = ruleEntries.ToList();
            //When '__EXTRACTED' is added, ruleEntries invalidates.
            //ruleEntries = (GetRulesEnumerator(file));
            foreach (IProductionRuleEntry rule in original)
                rule.Deliteralize(file.GetTokenEnumerator().ToList(), file, errors);
            file.ExpungeUnusedTokens();
            foreach (IProductionRuleEntry rule in ruleEntries)
            {
                currentEntry = rule;
                rule.FinalLink(file, errors);
            }
            currentEntry = null;
            file.ExpungeComments();
        }

        private static void ExpungeType<T>(this GDFile file, IList<T> list)
            where T :
                IEntry
        {
            foreach (T t in list)
                file.Remove(t);
        }

        private static IList<T> GatherType<T>(this GDFile file) 
            where T : 
                IEntry
        {
            IList<T> list = new List<T>();
            foreach (IEntry entry in file)
                if (entry is T)
                    if (!(list.Contains((T)entry)))
                        list.Add((T)entry);
            return list;
        }

        private static void ExpungeType<T>(this GDFile file)
            where T : 
                IEntry
        {
            file.ExpungeType(file.GatherType<T>());
        }

        private static void ExpungeComments(this GDFile file)
        {
            file.ExpungeType<ICommentEntry>();
        }

        private static void ExpungeUnusedTokens(this GDFile file)
        {
            IList<ITokenEntry> tokenReferences = new List<ITokenEntry>();
            foreach (IProductionRuleEntry rule in ruleEntries)
                rule.GetTokenReferences(tokenReferences);
            IList<ITokenEntry> unusedTokens = new List<ITokenEntry>();
            foreach (ITokenEntry entry in file.GetTokenEnumerator())
                if (!tokenReferences.Contains(entry))
                    if (entry.Unhinged || entry is ITokenEofEntry)
                        continue;
                    else
                        unusedTokens.Add(entry);
            file.ExpungeType(unusedTokens);
        }

        public static bool NeedsDeliteralized(this IProductionRuleEntry entry)
        {
            return ((IProductionRuleSeries)(entry)).NeedsDeliteralized();
        }

        public static bool NeedsDeliteralized(this IProductionRuleSeries series)
        {
            foreach (IProductionRule rule in series)
                if (rule.NeedsDeliteralized())
                    return true;
            return false;
        }

        public static bool NeedsDeliteralized(this IProductionRule rule)
        {
            foreach (IProductionRuleItem ruleItem in rule)
                if (ruleItem.NeedsDeliteralized())
                    return true;
            return false;
        }

        public static bool NeedsDeliteralized(this IProductionRuleItem ruleItem)
        {
            if (ruleItem is IProductionRuleGroupItem)
            {
                if (((IProductionRuleSeries)(ruleItem)).NeedsDeliteralized())
                    return true;
            }
            else if (ruleItem is ILiteralProductionRuleItem)
                return true;
            return false;
        }

        public static void Deliteralize(this IProductionRuleEntry entry, IList<ITokenEntry> availableStock, GDFile file, CompilerErrorCollection errors)
        {
            if (entry.NeedsDeliteralized())
            {
                currentEntry = entry;
                List<IProductionRule> result = new List<IProductionRule>();
                foreach (IProductionRule ipr in entry)
                {
                    IProductionRule resultIPR = null;
                    if (ipr.NeedsDeliteralized())
                        resultIPR = ipr.Deliteralize(availableStock, file, errors);
                    else
                        resultIPR = ipr;
                    if (resultIPR != null)
                        result.Add(resultIPR);
                }
                ProductionRuleEntry r = ((ProductionRuleEntry)(entry));
                r.Clear();
                foreach (IProductionRule ipr in result)
                    r.Add(ipr);
                currentEntry = null;
            }
        }

        public static IProductionRuleSeries Deliteralize(this IProductionRuleSeries series, IList<ITokenEntry> availableStock, GDFile file, CompilerErrorCollection errors)
        {
            if (series.NeedsDeliteralized())
            {
                List<IProductionRule> result = new List<IProductionRule>();
                foreach (IProductionRule ipr in series)
                {
                    IProductionRule resultIPR = null;
                    if (ipr.NeedsDeliteralized())
                        resultIPR = ipr.Deliteralize(availableStock,file, errors);
                    else
                        resultIPR = ipr;
                    if (resultIPR != null)
                        result.Add(resultIPR);
                }
                if (result.Count == 0)
                    return null;
                return new ProductionRuleSeries(result);
            }
            return series;
        }

        public static IProductionRule Deliteralize(this IProductionRule rule, IList<ITokenEntry> availableStock, GDFile file, CompilerErrorCollection errors)
        {
            if (rule.NeedsDeliteralized())
            {
                
                List<IProductionRuleItem> result = new List<IProductionRuleItem>();
                foreach (IProductionRuleItem ruleItem in rule)
                {
                    IProductionRuleItem resultItem = null;
                    if (ruleItem.NeedsDeliteralized())
                        resultItem = ruleItem.Deliteralize(availableStock, file, errors);
                    else
                        resultItem = ruleItem;
                    if (resultItem != null)
                        result.Add(resultItem);
                }
                if (result.Count == 0)
                    return null;
                return new ProductionRule(result, rule.FileName, rule.Column, rule.Line, rule.Position);
            }
            else
                return rule;
        }

        public static IProductionRuleItem Deliteralize(this IProductionRuleItem ruleItem, IList<ITokenEntry> availableStock, GDFile file, CompilerErrorCollection errors)
        {
            if (ruleItem.NeedsDeliteralized())
            {
                if (ruleItem is IProductionRuleGroupItem)
                    return ((IProductionRuleGroupItem)(ruleItem)).Deliteralize(availableStock, file, errors);
                else if (ruleItem is ILiteralProductionRuleItem)
                    return ((ILiteralProductionRuleItem)(ruleItem)).Deliteralize(availableStock, file, errors);
                else
                {
                    errors.Add(GrammarCore.GetParserError(currentEntry.FileName, ruleItem.Line, ruleItem.Column, GDParserErrors.Unexpected, ruleItem.GetType().Name));
                    return null;
                }
            }
            else
                return ruleItem;
        }

        public static IProductionRuleGroupItem Deliteralize(this IProductionRuleGroupItem ruleGroupItem, IList<ITokenEntry> availableStock, GDFile file, CompilerErrorCollection errors)
        {
            if (((IProductionRuleSeries)(ruleGroupItem)).NeedsDeliteralized())
            {
                IProductionRuleSeries series = ((IProductionRuleSeries)(ruleGroupItem)).Deliteralize(availableStock, file, errors);
                ProductionRuleGroupItem result = new ProductionRuleGroupItem(series.ToArray(), ruleGroupItem.Column, ruleGroupItem.Line, ruleGroupItem.Position);
                result.Name = ruleGroupItem.Name;
                result.RepeatOptions = ruleGroupItem.RepeatOptions;
                return result;
            }
            else
                return ruleGroupItem;
        }

        public static IProductionRuleItem Deliteralize(this ILiteralProductionRuleItem ruleItem, IList<ITokenEntry> availableStock, GDFile file, CompilerErrorCollection errors)
        {
            if (ruleItem is ILiteralCharProductionRuleItem)
            {
                return Deliteralize<char, ILiteralCharTokenItem>(((ILiteralCharProductionRuleItem)(ruleItem)), availableStock, file,
                    literal=>
                    {
                        LiteralCharTokenItem result = new LiteralCharTokenItem(literal.Value, ((ILiteralCharProductionRuleItem)(literal)).CaseInsensitive, literal.Column, literal.Line, literal.Position);
                        if (literal.Name != null && literal.Name != string.Empty)
                            result.Name = literal.Name;
                        else
                        {
                            result.Name = ExtractName(result.Value);
                        }
                        return result;
                    },
                    (literal, destination) =>
                    {
                        if (destination.Name != "__EXTRACTIONS" &&
                            (destination.Branches.Count == 1 &&
                             destination.Branches[0].Count == 1))
                            return new TokenReferenceProductionRuleItem(destination, literal.Column, literal.Line, literal.Position);
                        else if (!(literal.Name != null && literal.Name != string.Empty))
                            literal.Name = ExtractName(literal.Value);
                        return new LiteralCharReferenceProductionRuleItem((ILiteralCharTokenItem)literal, destination, literal.Column, literal.Line, literal.Position, ruleItem.Flag, ruleItem.Counter); 
                    }, errors);
            }
            else if (ruleItem is ILiteralStringProductionRuleItem)
            {
                return Deliteralize<string, ILiteralStringTokenItem>(((ILiteralStringProductionRuleItem)(ruleItem)), availableStock, file,
                    literal =>
                    {
                        LiteralStringTokenItem result = new LiteralStringTokenItem(literal.Value, ((ILiteralStringProductionRuleItem)(literal)).CaseInsensitive, literal.Column, literal.Line, literal.Position, false);
                        if (literal.Name != null && literal.Name != string.Empty)
                            result.Name = literal.Name;
                        else
                        {
                            result.Name = ExtractName(result.Value);
                        }
                        return result;
                    },
                    (literal, destination) =>
                    {
                        if (destination.Name != "__EXTRACTIONS" &&
                            (destination.Branches.Count == 1 &&
                             destination.Branches[0].Count == 1))
                            return new TokenReferenceProductionRuleItem(destination, literal.Column, literal.Line, literal.Position);
                        else if (string.IsNullOrEmpty(literal.Name))
                            literal.Name = ExtractName(literal.Value);
                        LiteralStringReferenceProductionRuleItem result = new LiteralStringReferenceProductionRuleItem((ILiteralStringTokenItem)literal, destination, literal.Column, literal.Line, literal.Position, ruleItem.Flag, ruleItem.Counter);
                        if (ruleItem.Flag)
                            result.Name = ExtractName(literal.Value);
                        return result;
                    }, errors);
            }
            return ruleItem;
        }

        private static string ExtractName(string original)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in original)
                if (OperatorNameLookup.ContainsKey(c.ToString()))
                    sb.Append(OperatorNameLookup[c.ToString()]);
                else
                    sb.Append(c);
            return sb.ToString();
        }

        public static string ExtractName(char original)
        {
            return ExtractName(original.ToString());
        }

        private static IProductionRuleItem Deliteralize<T, TLiteral>(this ILiteralProductionRuleItem<T> literal, IList<ITokenEntry> availableStock, GDFile file, Func<ILiteralProductionRuleItem<T>, TLiteral> createNewLiteral, Func<TLiteral, ITokenEntry, IProductionRuleItem> createLiteralReference, CompilerErrorCollection errors)
            where TLiteral :
                ILiteralTokenItem<T>
        {
            {
                ITokenItem foundItem = null;
                ITokenEntry literalContainerEntry = null;
                if ((foundItem = FindItemFromStock<T>(literal, availableStock, out literalContainerEntry, file, false)) != null)
                {
                    var result = createLiteralReference(((TLiteral)(foundItem)), literalContainerEntry);
                    ((LiteralProductionRuleItem<T>)literal).CloneData(result);
                    return result;
                }
            }
            if (availableStock.FindScannableEntry("__EXTRACTIONS") == null)
            {
                ITokenEntry extractionsToken = new TokenEntry("__EXTRACTIONS", new TokenExpressionSeries(new ITokenExpression[0], literal.Line, literal.Column, literal.Position, currentEntry.FileName), currentEntry.ScanMode, currentEntry.FileName, literal.Column, literal.Line, literal.Position, false, new List<string>(), false);
                availableStock.Add(extractionsToken);
                file.Add(extractionsToken);
            }
            {
                ITokenEntry extractionsToken = availableStock.FindScannableEntry("__EXTRACTIONS");
                if (extractionsToken == null)
                    return null;
                else
                {
                    TLiteral tokenLiteralItem = createNewLiteral(literal);
                    ((TokenExpressionSeries)extractionsToken.Branches).Add(new TokenExpression(new List<ITokenItem>(new ITokenItem[] { tokenLiteralItem }), currentEntry.FileName, literal.Column, literal.Line, literal.Position));
                    var tokenReferenceItem = createLiteralReference(((TLiteral)(tokenLiteralItem)), extractionsToken);
                    ((LiteralProductionRuleItem<T>)literal).CloneData(tokenReferenceItem);
                    return tokenReferenceItem;
                }
            }
        }

        private static ITokenItem FindItemFromStock<T>(ILiteralProductionRuleItem<T> literal, IList<ITokenEntry> availableStock, out ITokenEntry entry, GDFile file, bool followReferences)
        {
            entry = null;
            ITokenItem foundItem = null;
            foreach (ITokenEntry ite in availableStock)
            {
                ITokenItem iti = ite.FindTokenItemByValue<T>(literal.Value, file, followReferences);
                if (iti != null)
                {
                    foundItem = iti;
                    entry = ite;
                }
            }
            return foundItem;
        }

        public static void GetTokenReferences(this IProductionRuleEntry entry, IList<ITokenEntry> list)
        {
            ((IProductionRuleSeries)(entry)).GetTokenReferences(list);
        }

        public static void GetTokenReferences(this IProductionRuleSeries series, IList<ITokenEntry> list) 
        {
            foreach (IProductionRule rule in series)
                rule.GetTokenReferences(list);
        }

        public static void GetTokenReferences(this IProductionRule rule, IList<ITokenEntry> list)
        {
            foreach (IProductionRuleItem ruleItem in rule)
                ruleItem.GetTokenReferences(list);
        }

        public static void GetTokenReferences(this IProductionRuleItem ruleItem, IList<ITokenEntry> list) 
        {
            if (ruleItem is IProductionRuleGroupItem)
                ((IProductionRuleSeries)(ruleItem)).GetTokenReferences(list);
            else
            {
                if (ruleItem is ITokenReferenceProductionRuleItem)
                {
                    if (!list.Contains(((ITokenReferenceProductionRuleItem)(ruleItem)).Reference))
                        list.Add(((ITokenReferenceProductionRuleItem)(ruleItem)).Reference);
                }
                else if (ruleItem is ILiteralReferenceProductionRuleItem)
                    if (!(list.Contains(((ILiteralReferenceProductionRuleItem)(ruleItem)).Source)))
                        list.Add(((ILiteralReferenceProductionRuleItem)(ruleItem)).Source);
            }
        }

        public static bool NeedsFinalLinking(this IProductionRuleEntry entry)
        {
            return ((IProductionRuleSeries)(entry)).NeedsFinalLinking();
        }

        public static bool NeedsFinalLinking(this IProductionRuleSeries series)
        {
            foreach (IProductionRule rule in series)
                if (rule.NeedsFinalLinking())
                    return true;
            return false;
        }

        public static bool NeedsFinalLinking(this IProductionRule rule)
        {
            if (rule.Count == 0)
                return true;
            foreach (IProductionRuleItem ruleItem in rule)
                if (ruleItem.NeedsFinalLinking())
                    return true;
            return false;
        }

        public static bool NeedsFinalLinking(this IProductionRuleItem ruleItem)
        {
            if (ruleItem is IProductionRuleGroupItem)
            {
                if (((IProductionRuleSeries)(ruleItem)).NeedsFinalLinking())
                    return true;
            }
            else if (ruleItem is ISoftReferenceProductionRuleItem)
                return true;
            return false;
        }

        public static void FinalLink(this IProductionRuleEntry entry, GDFile file, CompilerErrorCollection errors)
        {
            if (entry.NeedsFinalLinking())
            {
                List<IProductionRule> result = new List<IProductionRule>();
                foreach (IProductionRule currentItem in entry)
                {
                    IProductionRule resultIPR = null;
                    if (currentItem.NeedsFinalLinking())
                        resultIPR = currentItem.FinalLink(file, errors);
                    else
                        resultIPR = currentItem;
                    if (resultIPR != null)
                        result.Add(resultIPR);
                }
                ProductionRuleEntry r = ((ProductionRuleEntry)(entry));
                r.Clear();
                foreach (IProductionRule ipr in result)
                    r.Add(ipr);
            }
        }

        public static IProductionRuleSeries FinalLink(this IProductionRuleSeries series, GDFile file, CompilerErrorCollection errors)
        {
            if (series.NeedsFinalLinking())
            {
                List<IProductionRule> result = new List<IProductionRule>();
                foreach (IProductionRule current in series)
                {
                    IProductionRule resultIPR = null;
                    if (current.NeedsFinalLinking())
                        resultIPR = current.FinalLink(file, errors);
                    else
                        resultIPR = current;
                    /* *
                     * It can be null because methods like this for 
                     * production rule series return null when the 
                     * set was empty.
                     * *
                     * A good example is groups.
                     * */
                    if (resultIPR != null)
                        result.Add(resultIPR);
                }
                if (result.Count == 0)
                    return null;
                return new ProductionRuleSeries(result);
            }
            return series;
        }

        public static IProductionRule FinalLink(this IProductionRule rule, GDFile file, CompilerErrorCollection errors)
        {
            if (rule.NeedsFinalLinking())
            {
                List<IProductionRuleItem> result = new List<IProductionRuleItem>();
                foreach (IProductionRuleItem currentItem in rule)
                {
                    IProductionRuleItem resultItem = null;
                    if (currentItem.NeedsFinalLinking())
                        resultItem = currentItem.FinalLink(file, errors);
                    else
                        resultItem = currentItem;
                    if (resultItem != null)
                        result.Add(resultItem);
                }
                /* *
                 * If the only element remaining after template expansion
                 * was an empty group, the current expression is 
                 * unnecessary.
                 * */
                if (result.Count == 0)
                    return null;
                return new ProductionRule(result, rule.FileName, rule.Column, rule.Line, rule.Position);
            }
            else
                return rule;
        }

        public static IProductionRuleItem FinalLink(this IProductionRuleItem ruleItem, GDFile file, CompilerErrorCollection errors)
        {
            if (ruleItem.NeedsFinalLinking())
            {
                if (ruleItem is IProductionRuleGroupItem)
                    return ((IProductionRuleGroupItem)(ruleItem)).FinalLink(file, errors);
                else if (ruleItem is ISoftReferenceProductionRuleItem)
                    return ((ISoftReferenceProductionRuleItem)(ruleItem)).FinalLink(file, errors);
                else
                {
                    errors.Add(GrammarCore.GetParserError(currentEntry.FileName, ruleItem.Line, ruleItem.Column, GDParserErrors.Unexpected, ruleItem.GetType().Name));
                    return null;
                }
            }
            else
                return ruleItem;
        }

        public static IProductionRuleGroupItem FinalLink(this IProductionRuleGroupItem ruleGroupItem, GDFile file, CompilerErrorCollection errors)
        {
            if (((IProductionRuleSeries)(ruleGroupItem)).NeedsFinalLinking())
            {
                IProductionRuleSeries series = ((IProductionRuleSeries)(ruleGroupItem)).FinalLink(file, errors);
                if (series == null)
                    return null;
                ProductionRuleGroupItem result = new ProductionRuleGroupItem(series.ToArray(), ruleGroupItem.Column, ruleGroupItem.Line, ruleGroupItem.Position);
                result.Name = ruleGroupItem.Name;
                result.RepeatOptions = ruleGroupItem.RepeatOptions;
                return result;
            }
            else
                return ruleGroupItem;
        }

        public static IRuleReferenceProductionRuleItem FinalLink(this ISoftReferenceProductionRuleItem softReference, GDFile file, CompilerErrorCollection errors)
        {
            IProductionRuleEntry ipre = ruleEntries.FindScannableEntry(softReference.PrimaryName);
            if (ipre == null)
                errors.Add(GrammarCore.GetParserError(currentEntry.FileName, softReference.Line, softReference.Column, GDParserErrors.UndefinedTokenReference, softReference.PrimaryName));
            else
            {
                IRuleReferenceProductionRuleItem ipri = new RuleReferenceProductionRuleItem(ipre, softReference.Column, softReference.Line, softReference.Position); ;
                ((ScannableEntryItem)(softReference)).CloneData(ipri);
                return ipri;
            }
            return null;
        }

        public static void ReplaceReferences(IGDFile source, IProductionRuleEntry target, IDictionary<IProductionRuleItem, IProductionRuleItem> elementToElementList)
        {
            ProductionRuleEntry r = (ProductionRuleEntry)(target);
            IProductionRuleSeries resultSeries = ReplaceReferences(source, ((IProductionRuleSeries)(r)).ToArray(), elementToElementList);
            r.Clear();
            foreach (var rule in resultSeries)
                r.Add(rule);
        }

        public static IProductionRuleSeries ReplaceReferences(IGDFile source, IProductionRule[] target, IDictionary<IProductionRuleItem, IProductionRuleItem> elementToElementList)
        {
            List<IProductionRule> resultData = new List<IProductionRule>();
            foreach (var rule in target)
                resultData.Add(ReplaceReferences(source, rule, elementToElementList));
            return new ProductionRuleSeries(resultData);
        }

        public static IProductionRule ReplaceReferences(IGDFile source, IProductionRule target, IDictionary<IProductionRuleItem, IProductionRuleItem> elementToElementList)
        {
            List<IProductionRuleItem> result = new List<IProductionRuleItem>();
            foreach (var item in target)
            {
                if (elementToElementList.ContainsKey(item))
                    result.Add(elementToElementList[item]);
                else
                {
                    if (item is IProductionRuleGroupItem && 
                        ContainsReferences(source, item, elementToElementList))
                    {
                        var gItem = ((IProductionRuleGroupItem)(item));
                        var rItemElements = ReplaceReferences(source, gItem.ToArray(), elementToElementList);
                        ProductionRuleGroupItem rItem = new ProductionRuleGroupItem(rItemElements.ToArray(), gItem.Column, gItem.Line, gItem.Position);
                        rItem.Name = gItem.Name;
                        rItem.RepeatOptions = gItem.RepeatOptions;
                        result.Add(rItem);
                    }
                    else
                        result.Add(item);
                }
            }
            return new ProductionRule(result, target.FileName, target.Column, target.Line, target.Position);
        }
        public static bool ContainsReferences(IGDFile source, IProductionRuleSeries target, IDictionary<IProductionRuleItem, IProductionRuleItem> elementToElementList)
        {
            foreach (var item in target)
                if (ContainsReferences(source, item, elementToElementList))
                    return true;
            return false;
        }
        public static bool ContainsReferences(IGDFile source, IProductionRule target, IDictionary<IProductionRuleItem, IProductionRuleItem> elementToElementList)
        {
            foreach (var item in target)
                if (ContainsReferences(source, item, elementToElementList))
                    return true;
            return false;
        }
        public static bool ContainsReferences(IGDFile source, IProductionRuleItem target, IDictionary<IProductionRuleItem, IProductionRuleItem> elementToElementList)
        {
            if (elementToElementList.ContainsKey(target))
                return true;
            if (target is IProductionRuleGroupItem)
                return ContainsReferences(source, ((IProductionRuleSeries)(target)), elementToElementList);
            return false;
        }
    }
}
