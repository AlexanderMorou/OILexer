using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal;
using AllenCopeland.Abstraction.Slf.Compilers;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Properties;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using AllenCopeland.Abstraction.Slf.Ast;
using AllenCopeland.Abstraction.Slf.Abstract;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */
namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer
{
    internal static partial class OilexerGrammarCore
    {
        internal const string GrammarParserErrorFormat = "OILexP{0}";

        internal static string Encode(this string target, bool addQuotes = true)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in target)
            {
                switch (c)
                {
                    case ' ':
                        sb.Append(" ");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\\':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\v':
                        sb.Append("\\v");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\0':
                        sb.Append("\\0");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '$':
                    case '<':
                    case '>':
                        sb.Append(c);
                        break;
                    default:
                        if (c > 255)
                        {
                            string charEncode = string.Format("{0:X}", (int)c);
                            while (charEncode.Length < 4)
                                charEncode = "0" + charEncode;
                            sb.AppendFormat("\\u{0}", charEncode);
                        }
                        else
                        {
                            var unicodeCategory = char.GetUnicodeCategory(c);
                            switch (unicodeCategory)
                            {
                                case System.Globalization.UnicodeCategory.SpacingCombiningMark:
                                case System.Globalization.UnicodeCategory.Control:
                                case System.Globalization.UnicodeCategory.Surrogate:
                                case System.Globalization.UnicodeCategory.CurrencySymbol:
                                case System.Globalization.UnicodeCategory.SpaceSeparator:
                                case System.Globalization.UnicodeCategory.ParagraphSeparator:
                                case System.Globalization.UnicodeCategory.EnclosingMark:
                                case System.Globalization.UnicodeCategory.LineSeparator:
                                case System.Globalization.UnicodeCategory.Format:
                                case System.Globalization.UnicodeCategory.NonSpacingMark:
                                case System.Globalization.UnicodeCategory.ModifierLetter:
                                case System.Globalization.UnicodeCategory.OtherLetter:
                                case System.Globalization.UnicodeCategory.OtherSymbol:
                                case System.Globalization.UnicodeCategory.PrivateUse:
                                case System.Globalization.UnicodeCategory.OtherNotAssigned:
                                    {
                                        string charEncode = string.Format("{0:X}", (int)c);
                                        while (charEncode.Length < 2)
                                            charEncode = "0" + charEncode;
                                        sb.AppendFormat("\\x{0}", charEncode);

                                    }
                                    break;
                                case System.Globalization.UnicodeCategory.ModifierSymbol:
                                case System.Globalization.UnicodeCategory.MathSymbol:
                                case System.Globalization.UnicodeCategory.FinalQuotePunctuation:
                                case System.Globalization.UnicodeCategory.InitialQuotePunctuation:
                                case System.Globalization.UnicodeCategory.LetterNumber:
                                case System.Globalization.UnicodeCategory.LowercaseLetter:
                                case System.Globalization.UnicodeCategory.OpenPunctuation:
                                case System.Globalization.UnicodeCategory.OtherPunctuation:
                                case System.Globalization.UnicodeCategory.OtherNumber:
                                case System.Globalization.UnicodeCategory.DecimalDigitNumber:
                                case System.Globalization.UnicodeCategory.DashPunctuation:
                                case System.Globalization.UnicodeCategory.TitlecaseLetter:
                                case System.Globalization.UnicodeCategory.UppercaseLetter:
                                case System.Globalization.UnicodeCategory.ClosePunctuation:
                                case System.Globalization.UnicodeCategory.ConnectorPunctuation:
                                    sb.Append(c);
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                }
            }
            if (addQuotes)
                if (target.Length == 1)
                    return string.Format("'{0}'", sb.ToString());
                else
                    return string.Format("\"{0}\"", sb.ToString());
            else
                return sb.ToString();
        }


        /// <summary>
        /// Obtains a <see cref="ParserSyntaxError"/> for the <paramref name="fileName"/>,
        /// <paramref name="line"/>, <paramref name="column"/>, with the <paramref name="error"/>
        /// type and message <paramref name="text"/> provided.
        /// </summary>
        /// <param name="fileName">The <see cref="String"/> path of th file the
        /// compiler error is for.</param>
        /// <param name="line">The <see cref="System.Int32"/> which denotes the line of the
        /// <paramref name="fileName"/> the error occurred on.</param>
        /// <param name="column">The <see cref="System.Int32"/> on the <paramref name="line"/>
        /// at which the error occurred.</param>
        /// <param name="error">The <see cref="OilexerGrammarParserErrors"/> value denoting the type of error.</param>
        /// <param name="text">The extra information text associated to the error message.</param>
        /// <returns>A <see cref="ParserSyntaxError"/> associated to the error.</returns>
        public static ParserSyntaxError GetSyntaxError(string fileName, int line, int column, OilexerGrammarParserErrors error, string text)
        {
            Uri location = new Uri(fileName, UriKind.RelativeOrAbsolute);
            switch (error)
            {
                case OilexerGrammarParserErrors.Expected:
                    return new ParserSyntaxError(source: location, start: new LineColumnPair(line, column), end: LineColumnPair.Zero, errorText: string.Format(ParserResources.GrammarParserErrors_Expected, text));
                case OilexerGrammarParserErrors.ExpectedEndOfFile:
                    return new ParserSyntaxError(source: location, start: new LineColumnPair(line, column), end: LineColumnPair.Zero, errorText: ParserResources.GrammarParserErrors_ExpectedEndOfFile);
                case OilexerGrammarParserErrors.ExpectedEndOfLine:
                    return new ParserSyntaxError(source: location, start: new LineColumnPair(line, column), end: LineColumnPair.Zero, errorText: ParserResources.GrammarParserErrors_ExpectedEndOfLine);
                case OilexerGrammarParserErrors.Unexpected:
                    return new ParserSyntaxError(source: location, start: new LineColumnPair(line, column), end: LineColumnPair.Zero, errorText: string.Format(ParserResources.GrammarParserErrors_Unexpected, text));
                case OilexerGrammarParserErrors.UnexpectedEndOfFile:
                    return new ParserSyntaxError(source: location, start: new LineColumnPair(line, column), end: LineColumnPair.Zero, errorText: ParserResources.GrammarParserErrors_UnexpectedEndOfFile);
                case OilexerGrammarParserErrors.UnexpectedEndOfLine:
                    return new ParserSyntaxError(source: location, start: new LineColumnPair(line, column), end: LineColumnPair.Zero, errorText: ParserResources.GrammarParserErrors_UnexpectedEndOfLine);
                case OilexerGrammarParserErrors.UnknownSymbol:
                    return new ParserSyntaxError(source: location, start: new LineColumnPair(line, column), end: LineColumnPair.Zero, errorText: string.Format(ParserResources.GrammarParserErrors_UnknownSymbol, text));
                case OilexerGrammarParserErrors.InvalidEscape:
                    return new ParserSyntaxError(source: location, start: new LineColumnPair(line, column), end: LineColumnPair.Zero, errorText: ParserResources.GrammarParserErrors_InvalidEscape);
                case OilexerGrammarParserErrors.IncludeFileNotFound:
                    return new ParserSyntaxError(source: location, start: new LineColumnPair(line, column), end: LineColumnPair.Zero, errorText: string.Format(ParserResources.GrammarParserErrors_IncludeFileNotFound, text));
                case OilexerGrammarParserErrors.FixedArgumentCountError:
                    return new ParserSyntaxError(source: location, start: new LineColumnPair(line, column), end: LineColumnPair.Zero, errorText: text);
                default:
                    break;
            }
            return null;
        }



        public static ParserSyntaxError GetParserError(string fileName, int line, int column, OilexerGrammarParserErrors error)
        {
            return GetSyntaxError(fileName, line, column, error, string.Empty);
        }

        public static string CombinePaths(string pathA, string pathB)
        {
            string path = Path.Combine(pathA, pathB);
            List<string> paths = new List<string>(path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries));
            Stack<string> finalForm = new Stack<string>();
            foreach (string s in paths)
                if (s == "..")
                {
                    if (finalForm.Count > 0) 
                        finalForm.Pop();
                }
                else if (s != ".")
                    finalForm.Push(s);
            paths.Clear();
            paths = new List<string>(finalForm);
            paths.Reverse();
            return string.Join(@"\", paths.ToArray());
        }
        internal static string EncodePrim(string p)
        {
            return p.EscapeStringOrCharCILAndCS();
        }

        internal static string EncodePrim(char p)
        {
            return p.ToString().EscapeStringOrCharCILAndCS(false);
        }

        public static ITokenItem FindTokenItem(this IOilexerGrammarTokenEntry inst, string name)
        {
            return inst.Branches.FindTokenItem(name);
        }

        public static ITokenItem FindTokenItemByValue<T>(this IOilexerGrammarTokenEntry inst, T value, OilexerGrammarFile file, bool followReferences)
        {
            return inst.Branches.FindTokenItemByValue(value, file, followReferences);
        }

        private static ITokenItem FindTokenItemByValue<T>(this ITokenExpressionSeries series, T value, OilexerGrammarFile file, bool followReferences)
        {
            foreach (ITokenExpression ite in series)
            {
                ITokenItem iti = ite.Count == 1 ? ite.FindTokenItemByValue(value, file, followReferences) : null;
                if (iti != null)
                    return iti;
            }
            return null;
        }

        /// <summary>
        /// Obtains a token element by <paramref name="value"/> by scanning the <paramref name="expression"/> it is
        /// contained within.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="value"/> the <see cref="ITokenItem"/> is
        /// supposed to be.</typeparam>
        /// <param name="expression">The <see cref="ITokenExpression"/> to scan for the <paramref name="value"/>
        /// provided.</param>
        /// <param name="value">The <typeparamref name="T"/> instance relative to the
        /// search criteria.</param>
        /// <param name="file">The <see cref="OilexerGrammarFile"/> in which the <see cref="ITokenExpression"/> is within.</param>
        /// <returns>A <see cref="ITokenItem"/> if an element within the <paramref name="expression"/> contains the
        /// <paramref name="value"/> provided.</returns>
        private static ITokenItem FindTokenItemByValue<T>(this ITokenExpression expression, T value, OilexerGrammarFile file, bool followReferences)
        {
            for (int i = 0; i < expression.Count; i++)
            {
                ITokenItem iti = expression[i];
            _check:
                if (iti is ILiteralTokenItem<T>)
                {
                    if (iti is ILiteralCharTokenItem)
                        if (((ILiteralCharTokenItem)(iti)).CaseInsensitive)
                        {
                            if (((char)(object)(value)).ToString().ToLower() == ((ILiteralCharTokenItem)(iti)).Value.ToString().ToLower())
                                return iti;
                        }
                        else
                        {
                            if (value.Equals(((ILiteralCharTokenItem)(iti)).Value))
                                return iti;
                        }
                    else if (iti is ILiteralStringTokenItem)
                    {
                        if (((ILiteralStringTokenItem)(iti)).CaseInsensitive)
                        {
                            if (((string)(object)(value)).ToLower() == ((ILiteralStringTokenItem)(iti)).Value.ToLower())
                                return iti;
                        }
                        else
                        {
                            if (value.Equals(((ILiteralStringTokenItem)(iti)).Value))
                                return iti;
                        }
                    }
                    else if (((ILiteralTokenItem<T>)(iti)).Value.Equals(value))
                        return iti;
                }
                else if (iti is ILiteralReferenceTokenItem)
                {
                    
                    var siti = ((ILiteralReferenceTokenItem)(iti));
                    if (!file.Contains(siti.Source) ||
                        followReferences)
                    {
                        iti = siti.Literal;
                    }
                    else
                        continue;
                    goto _check;
                }
                else if (iti is ITokenGroupItem)
                {
                    //If it's a group, search within the elements of the group.
                    ITokenItem find = ((ITokenGroupItem)(iti)).FindTokenItemByValue(value, file, followReferences);
                    if (find != null)
                        return find;
                }
                else if (iti is ITokenReferenceTokenItem)
                {
                    if (!file.Contains(((ITokenReferenceTokenItem)iti).Reference))
                    {
                        //In the event that the element was purged, but its value 
                        //exists elsewhere, obtain the alternate element.
                        ITokenItem iti2 = ((ITokenReferenceTokenItem)iti).Reference.FindTokenItemByValue(value, file, followReferences);
                        if (iti2 != null)
                            return iti2;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Finds a token item by the <paramref name="name"/> provided in the
        /// <paramref name="series"/>.
        /// </summary>
        /// <param name="series">The <see cref="ITokenExpressionSeries"/> to scan for the <paramref name="name"/>disposedIndex
        /// element.</param>
        /// <param name="name">The <see cref="String"/> value relative to the <see cref="ITokenItem"/>
        /// to return.</param>
        /// <returns>An <see cref="ITokenItem"/> instance, if successful; null otherwise.</returns>
        private static ITokenItem FindTokenItem(this ITokenExpressionSeries series, string name)
        {
            foreach (ITokenExpression ite in series)
            {
                ITokenItem iti = ite.FindTokenItem(name);
                if (iti != null)
                    return iti;
            }
            return null;
        }

        private static ITokenItem FindTokenItem(this ITokenExpression expression, string name)
        {
            ITokenItem find = null;
            foreach (ITokenItem iti in expression)
                if (iti.Name == name)
                    return iti;
                else if (iti is ITokenGroupItem)
                {
                    if ((find = ((ITokenGroupItem)(iti)).FindTokenItem(name)) != null)
                        return find;
                    else
                        continue;
                }
            return null;
        }
    }
}
