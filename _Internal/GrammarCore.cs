using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Oilexer.Expression;
using Oilexer.Parser;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Translation;
using Oilexer.Properties;

namespace Oilexer._Internal
{
    internal static class GrammarCore
    {
        internal const string GrammarParserErrorFormat = "OILexP{0}";
        /// <summary>
        /// Obtains a <see cref="CompmilerError"/> for the <paramref name="fileName"/>,
        /// <paramref name="line"/>, <paramref name="column"/>, with the <paramref name="error"/>
        /// type and message <paramref name="text"/> provided.
        /// </summary>
        /// <param name="fileName">The <see cref="String"/> path of th file the
        /// compiler error is for.</param>
        /// <param name="line">The <see cref="System.Int32"/> which denotes the line of the
        /// <paramref name="fileName"/> the error occurred on.</param>
        /// <param name="column">The <see cref="System.Int32"/> on the <paramref name="line"/>
        /// at which the error occurred.</param>
        /// <param name="error">The <see cref="GDParserErrors"/> value denoting the type of error.</param>
        /// <param name="text">The extra information text associated to the error message.</param>
        /// <returns>A <see cref="CompilerError"/> associated to the error.</returns>
        public static CompilerError GetParserError(string fileName, int line, int column, GDParserErrors error, string text)
        {
            switch (error)
            {
                case GDParserErrors.Expected:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), string.Format(Resources.GrammarParserErrors_Expected, text));
                case GDParserErrors.ExpectedEndOfFile:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), Resources.GrammarParserErrors_ExpectedEndOfFile);
                case GDParserErrors.ExpectedEndOfLine:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), Resources.GrammarParserErrors_ExpectedEndOfLine);
                case GDParserErrors.Unexpected:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), string.Format(Resources.GrammarParserErrors_Unexpected, text));
                case GDParserErrors.UnexpectedEndOfFile:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), Resources.GrammarParserErrors_UnexpectedEndOfFile);
                case GDParserErrors.UnexpectedEndOfLine:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), Resources.GrammarParserErrors_UnexpectedEndOfLine);
                case GDParserErrors.UnknownSymbol:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), string.Format(Resources.GrammarParserErrors_UnknownSymbol, text));
                case GDParserErrors.InvalidEscape:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), Resources.GrammarParserErrors_InvalidEscape);
                case GDParserErrors.IncludeFileNotFound:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), string.Format(Resources.GrammarParserErrors_IncludeFileNotFound, text));
                case GDParserErrors.InvalidRepeatOptions:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), Resources.GrammarParserErrors_InvalidRepeatOptions);
                case GDParserErrors.RuleNotTemplate:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), string.Format(Resources.GrammarParserErrors_RuleNotTemplate, text));
                case GDParserErrors.RuleIsTemplate:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), string.Format(Resources.GrammarParserErrors_RuleIsTemplate, text));
                case GDParserErrors.UndefinedTokenReference:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), string.Format(Resources.GrammarParserErrors_UndefinedTokenReference, text));
                case GDParserErrors.UndefinedRuleReference:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), string.Format(Resources.GrammarParserErrors_UndefinedRuleReference, text));
                case GDParserErrors.NoStartDefined:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), string.Format(Resources.GrammarParserErrors_NoStartDefined, text));
                case GDParserErrors.InvalidStartDefined:
                    return new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), string.Format(Resources.GrammarParserErrors_InvalidStartDefined, text));
                case GDParserErrors.RuleNeverUsed:
                    CompilerError ce = new CompilerError(fileName, line, column, string.Format(GrammarParserErrorFormat, (int)error), string.Format(Resources.GrammarParserErrors_RuleNeverUsed, text));
                    ce.IsWarning = true;
                    return ce;
                default:
                    break;
            }
            return null;
        }

        public static CompilerError GetParserError(string fileName, int line, int column, GDParserErrors error)
        {
            return GetParserError(fileName, line, column, error, string.Empty);
        }

        public static string CombinePaths(string pathA, string pathB)
        {
            string path = Path.Combine(pathA, pathB);
            List<string> paths = new List<string>(path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries));
            Stack<string> finalForm = new Stack<string>();
            foreach (string s in paths)
                if (s == ".." && finalForm.Count > 0)
                    finalForm.Pop();
                else if (s != "..")
                    if (s != ".")
                        finalForm.Push(s);
            paths.Clear();
            paths = new List<string>(finalForm);
            paths.Reverse();
            return string.Join("\\", paths.ToArray());
        }

        internal static string EncodePrim<T>(T p)
        {
            StringFormWriter sfw = new StringFormWriter();
            CSharpCodeTranslator csct = new CSharpCodeTranslator();
            csct.Options = new IntermediateCodeTranslatorOptions(false);
            csct.Target = sfw;
            csct.TranslateExpression(new PrimitiveExpression(p));
            string r = sfw.ToString();
            sfw.Dispose();
            csct.Target = null;
            csct.Options = null;
            return r;
        }
        public static ITokenItem FindTokenItem(this ITokenEntry inst, string name)
        {
            return inst.Branches.FindTokenItem(name);
        }

        public static ITokenItem FindTokenItemByValue<T>(this ITokenEntry inst, T value, GDFile file)
        {
            return inst.Branches.FindTokenItemByValue(value,file);
        }

        private static ITokenItem FindTokenItemByValue<T>(this ITokenExpressionSeries series, T value, GDFile file)
        {
            foreach (ITokenExpression ite in series)
            {
                ITokenItem iti = ite.Count == 1 ? ite.FindTokenItemByValue(value, file) : null;
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
        /// <param name="file">The <see cref="GDFile"/> in which the <see cref="ITokenExpression"/> is within.</param>
        /// <returns>A <see cref="ITokenItem"/> if an element within the <paramref name="expression"/> contains the
        /// <paramref name="value"/> provided.</returns>
        private static ITokenItem FindTokenItemByValue<T>(this ITokenExpression expression, T value, GDFile file)
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
                    iti = ((ILiteralReferenceTokenItem)(iti)).Literal;
                    goto _check;
                }
                else if (iti is ITokenGroupItem)
                {
                    ITokenItem find = ((ITokenGroupItem)(iti)).FindTokenItemByValue(value, file);
                    if (find != null)
                        return find;
                }
                else if (iti is ITokenReferenceTokenItem)
                {
                    if (!file.Contains(((ITokenReferenceTokenItem)iti).Reference))
                    {
                        ITokenItem iti2 = ((ITokenReferenceTokenItem)iti).Reference.FindTokenItemByValue(value, file);
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
