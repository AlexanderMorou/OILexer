using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Slf.Abstract;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
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
        //private static IOilexerGrammarProductionRuleEntry currentEntry = null;
        public static void FinalLink(this OilexerGrammarFile file, ICompilerErrorCollection errors)
        {

            //currentEntry = null;
            IList<IOilexerGrammarProductionRuleEntry> original = ruleEntries.ToList();
            //When '__EXTRACTED' is added, ruleEntries invalidates.
            var availableStock = file.GetTokens().Cast<IOilexerGrammarTokenEntry>().ToList();

            foreach (IOilexerGrammarProductionRuleEntry rule in original)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                rule.Deliteralize(availableStock, file, errors);
            }
            file.ExpungeUnusedTokens();

            foreach (IOilexerGrammarProductionRuleEntry rule in ruleEntries)
            {
                rule.FinalLink(file, errors);
            }
            var namedEntries = from s in file
                               where s is IOilexerGrammarNamedEntry
                               select (IOilexerGrammarNamedEntry)s;
            var namedDuplicates = (from s in namedEntries
                                   let duplicates = (from s2 in namedEntries
                                                     where s != s2
                                                     where s.Name == s2.Name
                                                     select s2).ToArray()
                                   where duplicates.Length > 0
                                   select new { Entry = s, Duplicates = duplicates });
            var passedDuplicates = new List<IOilexerGrammarNamedEntry>();
            foreach (var duplLookup in namedDuplicates)
            {
                if (passedDuplicates.Contains(duplLookup.Entry))
                    continue;
                passedDuplicates.Add(duplLookup.Entry);
                var mainError = errors.SourceModelError(OilexerGrammarCore.CompilerErrors.DuplicateEntryError, new LineColumnPair(duplLookup.Entry.Line, duplLookup.Entry.Column), new LineColumnPair(duplLookup.Entry.Line, duplLookup.Entry.Column + duplLookup.Entry.Name.Length), new Uri(duplLookup.Entry.FileName, UriKind.RelativeOrAbsolute), duplLookup.Entry, new string[] { duplLookup.Entry.Name });
                foreach (var duplicate in duplLookup.Duplicates)
                {
                    passedDuplicates.Add(duplicate);
                    errors.SourceModelError(OilexerGrammarCore.CompilerErrors.DuplicateEntryReference, new LineColumnPair(duplicate.Line, duplicate.Column), new LineColumnPair(duplicate.Line, duplicate.Column + duplicate.Name.Length), new Uri(duplicate.FileName, UriKind.RelativeOrAbsolute), duplicate, mainError, new string[] { duplicate.Name });
                }
            }
            file.ExpungeComments();
            foreach (var rule in ruleEntries)
                if (rule.IsRuleCollapsePoint)
                    rule.ValidateCollapsePoint(errors);
        }

        public static void ValidateCollapsePoint(this IOilexerGrammarProductionRuleEntry entry, ICompilerErrorCollection errors)
        {
            if (!entry.All(r => r.Count == 1 && r.All(k => k is IRuleReferenceProductionRuleItem || k is ITokenReferenceProductionRuleItem && k.RepeatOptions.Options == ScannableEntryItemRepeatOptions.None)))
                errors.ModelError<IOilexerGrammarProductionRuleEntry>(OilexerGrammarCore.CompilerErrors.InvalidRuleCollapsePoint, entry, entry.Name);
        }

        private static void ExpungeSet<T>(this OilexerGrammarFile file, IList<T> list)
            where T :
                IOilexerGrammarEntry
        {
            foreach (T t in list)
                file.Remove(t);
        }

        private static IList<T> GatherType<T>(this OilexerGrammarFile file)
            where T :
                IOilexerGrammarEntry
        {
            IList<T> list = new List<T>();
            foreach (IOilexerGrammarEntry entry in file)
                if (entry is T)
                    if (!(list.Contains((T)entry)))
                        list.Add((T)entry);
            return list;
        }

        private static void ExpungeType<T>(this OilexerGrammarFile file)
            where T :
                IOilexerGrammarEntry
        {
            file.ExpungeSet(file.GatherType<T>());
        }

        private static void ExpungeComments(this OilexerGrammarFile file)
        {
            file.ExpungeType<IOilexerGrammarCommentEntry>();
        }

        private static void ExpungeUnusedTokens(this OilexerGrammarFile file)
        {
            IList<IOilexerGrammarTokenEntry> tokenReferences = new List<IOilexerGrammarTokenEntry>();
            foreach (IOilexerGrammarProductionRuleEntry rule in ruleEntries)
                rule.GetTokenReferences(tokenReferences);
            IList<IOilexerGrammarTokenEntry> unusedTokens = new List<IOilexerGrammarTokenEntry>();
            foreach (IOilexerGrammarTokenEntry entry in file.GetTokenEnumerator())
                if (!tokenReferences.Contains(entry))
                    if (entry.Unhinged || entry is IOilexerGrammarTokenEofEntry)
                        continue;
                    else
                        unusedTokens.Add(entry);
            file.ExpungeSet(unusedTokens);
        }

        public static bool NeedsDeliteralized(this IOilexerGrammarProductionRuleEntry entry)
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

        public static void Deliteralize(this IOilexerGrammarProductionRuleEntry entry, IList<IOilexerGrammarTokenEntry> availableStock, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (entry.NeedsDeliteralized())
            {
                List<IProductionRule> result = new List<IProductionRule>();
                foreach (IProductionRule ipr in entry)
                {
                    IProductionRule resultIPR = null;
                    if (ipr.NeedsDeliteralized())
                        resultIPR = ipr.Deliteralize(entry, availableStock, file, errors);
                    else
                        resultIPR = ipr;
                    if (resultIPR != null)
                        result.Add(resultIPR);
                }
                OilexerGrammarProductionRuleEntry r = ((OilexerGrammarProductionRuleEntry)(entry));
                r.Clear();
                foreach (IProductionRule ipr in result)
                    r.Add(ipr);
                //currentEntry = null;
            }
        }

        public static IProductionRuleSeries Deliteralize(this IProductionRuleSeries series, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (series.NeedsDeliteralized())
            {
                List<IProductionRule> result = new List<IProductionRule>();
                foreach (IProductionRule ipr in series)
                {
                    IProductionRule resultIPR = null;
                    if (ipr.NeedsDeliteralized())
                        resultIPR = ipr.Deliteralize(currentEntry, availableStock, file, errors);
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

        public static IProductionRule Deliteralize(this IProductionRule rule, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (rule.NeedsDeliteralized())
            {

                List<IProductionRuleItem> result = new List<IProductionRuleItem>();
                foreach (IProductionRuleItem ruleItem in rule)
                {
                    IProductionRuleItem resultItem = null;
                    if (ruleItem.NeedsDeliteralized())
                        resultItem = ruleItem.Deliteralize(currentEntry, availableStock, file, errors);
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

        public static IProductionRuleItem Deliteralize(this IProductionRuleItem ruleItem, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (ruleItem.NeedsDeliteralized())
            {
                if (ruleItem is IProductionRuleGroupItem)
                    return ((IProductionRuleGroupItem)(ruleItem)).Deliteralize(currentEntry, availableStock, file, errors);
                else if (ruleItem is ILiteralProductionRuleItem)
                    return ((ILiteralProductionRuleItem)(ruleItem)).Deliteralize(currentEntry, availableStock, file, errors);
                else
                {
                    errors.SourceError(OilexerGrammarCore.CompilerErrors.UnexpectedLiteralEntry, new LineColumnPair(ruleItem.Line, ruleItem.Column), LineColumnPair.Zero, new Uri(currentEntry.FileName, UriKind.RelativeOrAbsolute), ruleItem.GetType().Name);
                    return null;
                }
            }
            else
                return ruleItem;
        }

        public static IProductionRuleGroupItem Deliteralize(this IProductionRuleGroupItem ruleGroupItem, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (((IProductionRuleSeries)(ruleGroupItem)).NeedsDeliteralized())
            {
                IProductionRuleSeries series = ((IProductionRuleSeries)(ruleGroupItem)).Deliteralize(currentEntry, availableStock, file, errors);
                ProductionRuleGroupItem result = new ProductionRuleGroupItem(series.ToArray(), ruleGroupItem.Column, ruleGroupItem.Line, ruleGroupItem.Position);
                result.Name = ruleGroupItem.Name;
                result.RepeatOptions = ruleGroupItem.RepeatOptions;
                return result;
            }
            else
                return ruleGroupItem;
        }

        public static IProductionRuleItem Deliteralize(this ILiteralProductionRuleItem ruleItem, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (ruleItem is ILiteralCharProductionRuleItem)
            {
                return Deliteralize<char, ILiteralCharTokenItem>(((ILiteralCharProductionRuleItem)(ruleItem)), currentEntry, availableStock, file,
                    literal =>
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
                return Deliteralize<string, ILiteralStringTokenItem>(((ILiteralStringProductionRuleItem)(ruleItem)), currentEntry, availableStock, file,
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

        private static IProductionRuleItem Deliteralize<T, TLiteral>(this ILiteralProductionRuleItem<T> literal, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, OilexerGrammarFile file, Func<ILiteralProductionRuleItem<T>, TLiteral> createNewLiteral, Func<TLiteral, IOilexerGrammarTokenEntry, IProductionRuleItem> createLiteralReference, ICompilerErrorCollection errors)
            where TLiteral :
                ILiteralTokenItem<T>
        {
            {
                ITokenItem foundItem = null;
                IOilexerGrammarTokenEntry literalContainerEntry = null;
                if ((foundItem = FindItemFromStock<T>(literal, currentEntry, availableStock, out literalContainerEntry, file, false)) != null)
                {
                    var result = createLiteralReference(((TLiteral)(foundItem)), literalContainerEntry);
                    ((LiteralProductionRuleItem<T>)literal).CloneData(result);
                    return result;
                }
            }
            IOilexerGrammarTokenEntry extractionsToken;
            if ((extractionsToken = availableStock.OilexerGrammarFindScannableEntry("__EXTRACTIONS")) == null)
            {
                extractionsToken = new OilexerGrammarTokenEntry("__EXTRACTIONS", new TokenExpressionSeries(new ITokenExpression[0], literal.Line, literal.Column, literal.Position, currentEntry.FileName), currentEntry.ScanMode, currentEntry.FileName, literal.Column, literal.Line, literal.Position, false, new List<OilexerGrammarTokens.IdentifierToken>(), false);
                availableStock.Add(extractionsToken);
                file.Add(extractionsToken);
            }
            TLiteral tokenLiteralItem = createNewLiteral(literal);
            ((TokenExpressionSeries)extractionsToken.Branches).Add(new TokenExpression(new List<ITokenItem>(new ITokenItem[] { tokenLiteralItem }), currentEntry.FileName, literal.Column, literal.Line, literal.Position));
            var tokenReferenceItem = createLiteralReference(((TLiteral)(tokenLiteralItem)), extractionsToken);
            ((LiteralProductionRuleItem<T>)literal).CloneData(tokenReferenceItem);
            return tokenReferenceItem;
        }

        private static ITokenItem FindItemFromStock<T>(ILiteralProductionRuleItem<T> literal, IOilexerGrammarProductionRuleEntry currentEntry, IList<IOilexerGrammarTokenEntry> availableStock, out IOilexerGrammarTokenEntry entry, OilexerGrammarFile file, bool followReferences)
        {
            entry = null;
            ITokenItem foundItem = null;
            foreach (IOilexerGrammarTokenEntry ite in availableStock)
            {
                if (ite.ForcedRecognizer)
                    continue;
                ITokenItem iti = ite.FindTokenItemByValue<T>(literal.Value, file, followReferences);
                if (iti != null)
                {
                    foundItem = iti;
                    entry = ite;
                    break;
                }
            }
            return foundItem;
        }

        public static void GetTokenReferences(this IOilexerGrammarProductionRuleEntry entry, IList<IOilexerGrammarTokenEntry> list)
        {
            ((IProductionRuleSeries)(entry)).GetTokenReferences(list);
        }

        public static void GetTokenReferences(this IProductionRuleSeries series, IList<IOilexerGrammarTokenEntry> list)
        {
            foreach (IProductionRule rule in series)
                rule.GetTokenReferences(list);
        }

        public static void GetTokenReferences(this IProductionRule rule, IList<IOilexerGrammarTokenEntry> list)
        {
            foreach (IProductionRuleItem ruleItem in rule)
                ruleItem.GetTokenReferences(list);
        }

        public static void GetTokenReferences(this IProductionRuleItem ruleItem, IList<IOilexerGrammarTokenEntry> list)
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

        public static bool NeedsFinalLinking(this IOilexerGrammarProductionRuleEntry entry)
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

        public static void FinalLink(this IOilexerGrammarProductionRuleEntry entry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (entry.NeedsFinalLinking())
            {
                List<IProductionRule> result = new List<IProductionRule>();
                foreach (IProductionRule currentItem in entry)
                {
                    IProductionRule resultIPR = null;
                    if (currentItem.NeedsFinalLinking())
                        resultIPR = currentItem.FinalLink(entry, file, errors);
                    else
                        resultIPR = currentItem;
                    if (resultIPR != null)
                        result.Add(resultIPR);
                }
                OilexerGrammarProductionRuleEntry r = ((OilexerGrammarProductionRuleEntry)(entry));
                r.Clear();
                foreach (IProductionRule ipr in result)
                    r.Add(ipr);
            }
        }

        public static IProductionRuleSeries FinalLink(this IProductionRuleSeries series, IOilexerGrammarProductionRuleEntry currentEntry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (series.NeedsFinalLinking())
            {
                List<IProductionRule> result = new List<IProductionRule>();
                foreach (IProductionRule current in series)
                {
                    IProductionRule resultIPR = null;
                    if (current.NeedsFinalLinking())
                        resultIPR = current.FinalLink(currentEntry, file, errors);
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

        public static IProductionRule FinalLink(this IProductionRule rule, IOilexerGrammarProductionRuleEntry currentEntry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (rule.NeedsFinalLinking())
            {
                List<IProductionRuleItem> result = new List<IProductionRuleItem>();
                foreach (IProductionRuleItem currentItem in rule)
                {
                    IProductionRuleItem resultItem = null;
                    if (currentItem.NeedsFinalLinking())
                        resultItem = currentItem.FinalLink(currentEntry, file, errors);
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

        public static IProductionRuleItem FinalLink(this IProductionRuleItem ruleItem, IOilexerGrammarProductionRuleEntry currentEntry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (ruleItem.NeedsFinalLinking())
            {
                if (ruleItem is IProductionRuleGroupItem)
                    return ((IProductionRuleGroupItem)(ruleItem)).FinalLink(currentEntry, file, errors);
                else if (ruleItem is ISoftReferenceProductionRuleItem)
                    return ((ISoftReferenceProductionRuleItem)(ruleItem)).FinalLink(currentEntry, file, errors);
                else
                {
                    errors.SourceError(OilexerGrammarCore.CompilerErrors.UnexpectedUndefinedEntry, new LineColumnPair(ruleItem.Line, ruleItem.Column), LineColumnPair.Zero, new Uri(currentEntry.FileName, UriKind.RelativeOrAbsolute), ruleItem.GetType().Name);
                    return null;
                }
            }
            else
                return ruleItem;
        }

        public static IProductionRuleGroupItem FinalLink(this IProductionRuleGroupItem ruleGroupItem, IOilexerGrammarProductionRuleEntry currentEntry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            if (((IProductionRuleSeries)(ruleGroupItem)).NeedsFinalLinking())
            {
                IProductionRuleSeries series = ((IProductionRuleSeries)(ruleGroupItem)).FinalLink(currentEntry, file, errors);
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

        public static IRuleReferenceProductionRuleItem FinalLink(this ISoftReferenceProductionRuleItem softReference, IOilexerGrammarProductionRuleEntry currentEntry, OilexerGrammarFile file, ICompilerErrorCollection errors)
        {
            IOilexerGrammarProductionRuleEntry ipre = ruleEntries.OilexerGrammarFindScannableEntry(softReference.PrimaryName);
            if (ipre == null)
                errors.SourceError(OilexerGrammarCore.CompilerErrors.UndefinedTokenReference, new LineColumnPair(softReference.Line, softReference.Column), LineColumnPair.Zero, new Uri(currentEntry.FileName, UriKind.RelativeOrAbsolute), string.Format(" '{0}'", softReference.PrimaryName));
            else
            {
                IRuleReferenceProductionRuleItem ipri = new RuleReferenceProductionRuleItem(ipre, softReference.Column, softReference.Line, softReference.Position); ;
                ((ScannableEntryItem)(softReference)).CloneData(ipri);
                return ipri;
            }
            return null;
        }

        public static void ReplaceReferences(IOilexerGrammarFile source, IOilexerGrammarProductionRuleEntry target, IDictionary<IProductionRuleItem, IProductionRuleItem> elementToElementList)
        {
            OilexerGrammarProductionRuleEntry r = (OilexerGrammarProductionRuleEntry)(target);
            IProductionRuleSeries resultSeries = ReplaceReferences(source, ((IProductionRuleSeries)(r)).ToArray(), elementToElementList);
            r.Clear();
            foreach (var rule in resultSeries)
                r.Add(rule);
        }

        public static IProductionRuleSeries ReplaceReferences(IOilexerGrammarFile source, IProductionRule[] target, IDictionary<IProductionRuleItem, IProductionRuleItem> elementToElementList)
        {
            List<IProductionRule> resultData = new List<IProductionRule>();
            foreach (var rule in target)
                resultData.Add(ReplaceReferences(source, rule, elementToElementList));
            return new ProductionRuleSeries(resultData);
        }

        public static IProductionRule ReplaceReferences(IOilexerGrammarFile source, IProductionRule target, IDictionary<IProductionRuleItem, IProductionRuleItem> elementToElementList)
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
        public static bool ContainsReferences(IOilexerGrammarFile source, IProductionRuleSeries target, IDictionary<IProductionRuleItem, IProductionRuleItem> elementToElementList)
        {
            foreach (var item in target)
                if (ContainsReferences(source, item, elementToElementList))
                    return true;
            return false;
        }
        public static bool ContainsReferences(IOilexerGrammarFile source, IProductionRule target, IDictionary<IProductionRuleItem, IProductionRuleItem> elementToElementList)
        {
            foreach (var item in target)
                if (ContainsReferences(source, item, elementToElementList))
                    return true;
            return false;
        }
        public static bool ContainsReferences(IOilexerGrammarFile source, IProductionRuleItem target, IDictionary<IProductionRuleItem, IProductionRuleItem> elementToElementList)
        {
            if (elementToElementList.ContainsKey(target))
                return true;
            if (target is IProductionRuleGroupItem)
                return ContainsReferences(source, ((IProductionRuleSeries)(target)), elementToElementList);
            return false;
        }
    }
}
