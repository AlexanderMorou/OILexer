using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Oilexer._Internal;
using Oilexer.Parser.GDFileData.TokenExpression;

namespace Oilexer.Parser
{
    partial class GDParser
    {
        public sealed class GDTokenizer :
            Tokenizer<IGDToken>,
            IGDTokenizer
        {
            /// <summary>
            /// Data member determining whether the tokenizer is in multiline mode.
            /// </summary>
            private bool multiLineMode = true;
            /// <summary>
            /// Creates a new <see cref="GDTokenizer"/> with the
            /// <see cref="Stream"/>, <paramref name="s"/>, and the 
            /// <paramref name="fileName"/> provided.
            /// </summary>
            /// <param name="s">The <see cref="Stream"/> from which
            /// to read the tokens from.</param>
            /// <param name="fileName">The <see cref="String"/> identifier
            /// which is used to identify the <see cref="Stream"/>, <paramref name="s"/>,
            /// when errors occur.</param>
            public GDTokenizer(Stream s, string fileName)
                : base(s, fileName)
            {
            }

            /* *
             * The linear approach.
             * */
            protected override NextTokenResults NextTokenInternal(long parserState)
            {
                char c = LookAhead(0);
                bool isOp = false;
                if (IsWhitespaceChar(c))
                {
                    NextTokenResults w;
                    if ((w = ParseWhitespace(this.MultiLineMode)).Successful)
                        return NextTokenInternal(parserState);
                    else
                        return w;
                }
                if (c == ':')
                    isOp = AreOperatorChars(c, LookAhead(1), LookAhead(2));
                else if (IsOperatorChar(c))
                    isOp = true;
                if (isOp && c != '#')
                    return ParseOperator();
                else if (IsIdentifierChar(c))
                    return ParseIdentifier();
                else if (c == '#')
                {
                    long position = this.Position;
                    NextTokenResults ntr = ParsePreprocessor();
                    if (ntr.Successful)
                        return ntr;
                    NextTokenResults op = ParseOperator();
                    return op;
                }
                else if (c == '"')
                    return ParseString();
                else if (c == '/')
                    return ParseComment();
                else if (char.IsNumber(c))
                    return ParseNumber();
                else if (c == '\'')
                    return ParseCharacterLiteral();
                else if (c == '@' && LookAhead(1) == '"')
                    return ParseString();
                else if (c == '@' && LookAhead(1) == '\'')
                    return ParseCharacterLiteral();
                else if (c == '[')
                    return ParseCharacterRange();
                return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, GetLineIndex(), GetColumnIndex(), GDParserErrors.UnknownSymbol));
            }

            private NextTokenResults ParseCharacterRange()
            {
                char c = LookAhead(0);
                bool invert = LookAhead(1) == '^';
                bool hasHyphen = LookAhead(invert ? 2 : 1) == '-';
                int lookAhead = invert ? hasHyphen ? 3 : 2 : hasHyphen ? 2 : 1;
                int start = lookAhead;
                char rangeStart = char.MinValue;
                bool ranged=false;
                char last = char.MinValue;
                List<char> result = new List<char>();
                if (hasHyphen)
                    result.Add('-');
                while ((c = LookAhead(lookAhead)) != ']')
                {
                    if (lookAhead == start && c == '-')
                        return new NextTokenResults(GrammarCore.GetParserError(this.FileName, this.GetLineIndex(this.Position + lookAhead), this.GetColumnIndex(this.Position + lookAhead), GDParserErrors.Unexpected, "-"));
                    else if (c == '\r' || c == '\n')
                        return new NextTokenResults(GrammarCore.GetParserError(this.FileName, this.GetLineIndex(this.Position + lookAhead), this.GetColumnIndex(this.Position + lookAhead), GDParserErrors.UnexpectedEndOfLine));
                    else if (c == char.MinValue)
                        return new NextTokenResults(GrammarCore.GetParserError(this.FileName, this.GetLineIndex(this.Position + lookAhead), this.GetColumnIndex(this.Position + lookAhead), GDParserErrors.UnexpectedEndOfFile));
                    else if (c == '\\')
                    {
                        switch (c = LookAhead(++lookAhead))
                        {
                            //\x[0-9A-Fa-f][0-9A-Fa-f]
                            case 'x':
                                c = LookAhead(++lookAhead);
                                char c2 = LookAhead(++lookAhead);
                                if (('0' <= c && c <= '9' || 'A' <= c && c <= 'F' || 'a' <= c && c <= 'f') &&
                                    ('0' <= c2 && c2 <= '9' || 'A' <= c2 && c2 <= 'F' || 'a' <= c2 && c2 <= 'f'))
                                {
                                    string s = new string(new char[] { c, c2 });
                                    c = (char)(Convert.ToInt32(s, 16));
                                }
                                else
                                {
                                    lookAhead -= 2;
                                    goto default;
                                }
                                break;
                            case 'n':
                                c = '\n';
                                break;
                            case 'r':
                                c = '\r';
                                break;
                            case 'v':
                                c = '\v';
                                break;
                            case 't':
                                c = '\t';
                                break;
                            case 'f':
                                c = '\f';
                                break;
                            case ']':
                            case '-':
                            case '\\':
                                break;
                            default:
                                //Syntax error.
                                return new NextTokenResults(GrammarCore.GetParserError(this.FileName, this.GetLineIndex(this.Position + lookAhead), this.GetColumnIndex(this.Position + lookAhead), GDParserErrors.InvalidEscape));
                        }
                    }
                    if (c == '-')
                    {
                        rangeStart = last;
                        ranged = true;
                    }
                    else if (ranged)
                    {
                        for (char x = rangeStart; x <= c; x++)
                            if (!result.Contains(x))
                                result.Add(x);
                        ranged = false;
                        rangeStart = char.MinValue;
                    }
                    else
                        result.Add(c);
                    last = c;
                    lookAhead++;
                }
                result.Sort();

                return new NextTokenResults(new GDTokens.CharacterRangeToken(invert, result.ToArray(), lookAhead + 1, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
            }

            private NextTokenResults ParseCharacterLiteral()
            {
                int index = 0;
                bool caseInsensitive = false;
                if (LookAhead(index) == '@')
                {
                    caseInsensitive = true;
                    index++;
                }
                if (LookAhead(index) == '\'')
                {
                    int l = GetLineIndex();
                    int ci = GetColumnIndex();
                    long p = Position;
                    if (LookAhead(index + 1) == '\\')
                    {
                        switch (LookAhead(index + 2))
                        {
                            case 'x':
                                if (IsHexadecimalChar(LookAhead(index + 3)) && IsHexadecimalChar(LookAhead(index + 4)) &&
                                    LookAhead(index + 5) == '\'')
                                    return new NextTokenResults(new GDTokens.CharLiteralToken(new string(Flush(index + 6)), caseInsensitive, ci, l, p));
                                break;
                            case 'u':
                                if (IsHexadecimalChar(LookAhead(index + 3)) && IsHexadecimalChar(LookAhead(index + 4)) &&
                                    IsHexadecimalChar(LookAhead(index + 5)) && IsHexadecimalChar(LookAhead(index + 6)) &&
                                    LookAhead(index + 7) == '\'')
                                    return new NextTokenResults(new GDTokens.CharLiteralToken(new string(Flush(index + 8)), caseInsensitive, ci, l, p));
                                break;
                            case 'v':
                            case 'f':
                            case 't':
                            case '\\':
                            case '\'':
                            case 'n':
                            case 'r':
                                if (LookAhead(index + 3) == '\'')
                                    return new NextTokenResults(new GDTokens.CharLiteralToken(new string(Flush(index + 4)), caseInsensitive, ci, l, p));
                                else
                                    return new NextTokenResults(GrammarCore.GetParserError(FileName, l, ci + index + 3, GDParserErrors.Expected, "\\'"));
                        }
                    }
                    else if (LookAhead(index + 2) == '\'')
                    {
                        return new NextTokenResults(new GDTokens.CharLiteralToken(new string(Flush(index + 3)), caseInsensitive, ci, l, p));
                    }
                }
                return new NextTokenResults(GrammarCore.GetParserError(this.FileName, this.GetLineIndex(), this.GetColumnIndex(), GDParserErrors.UnknownSymbol));
            }
            private NextTokenResults ParseNumber()
            {
                char c = LookAhead(0);
                bool isHex = false;
                if (char.IsNumber(c))
                {
                    int ci = this.GetColumnIndex(), l = this.GetLineIndex();
                    long p = this.Position;
                    int lookAhead = 1;
                    if (LookAhead(0) == '0' && LookAhead(1) == 'x')
                    {
                        isHex = true;
                        lookAhead++;
                    }
                    while (((c = LookAhead(lookAhead)) != char.MinValue) &&
                             c != '\r' &&
                             c != '\n' &&
                             c >= '0' && c <= '9' || isHex && (c >= 'A' && c <= 'F' || c >= 'a' && c <= 'f'))
                        lookAhead++;
                    string number = new string(Flush(lookAhead));
                    return new NextTokenResults(new GDTokens.NumberLiteral(number, ci, l, p));
                }
                return new NextTokenResults(GrammarCore.GetParserError(this.FileName, this.GetLineIndex(), this.GetColumnIndex(), GDParserErrors.UnknownSymbol));
            }

            private NextTokenResults ParseComment()
            {
                char c = LookAhead(0);
                if (c != '/')
                    return new NextTokenResults(GrammarCore.GetParserError(this.FileName, this.GetLineIndex(), this.GetColumnIndex(), GDParserErrors.Expected, "/"));
                c = LookAhead(1);
                if (!(c == '/' || c == '*'))
                    return new NextTokenResults(GrammarCore.GetParserError(this.FileName, this.GetLineIndex(), this.GetColumnIndex(), GDParserErrors.Expected, "/ or *"));
                int ci = this.GetColumnIndex(), l = this.GetLineIndex();
                long p = this.Position;
                string commentHeader = new string(this.Flush(2));
                string commentBody = string.Empty;
                if (c == '/')
                {
                    commentBody = this.Scan("\r\n", false);
                    if (commentBody == null)
                    {
                        this.Position -= 2;
                        return new NextTokenResults(GrammarCore.GetParserError(this.FileName, l, ci, GDParserErrors.UnexpectedEndOfLine));
                    }
                }
                else if (c == '*')
                {
                    commentBody = this.Scan("*/", true);
                    if (commentBody == null)
                    {
                        this.Position -= 2;
                        return new NextTokenResults(GrammarCore.GetParserError(this.FileName, l, ci, GDParserErrors.UnexpectedEndOfFile));
                    }

                }
                return new NextTokenResults(new GDTokens.CommentToken(string.Format("{0}{1}", commentHeader, commentBody), ci, l, p));
            }
            private NextTokenResults ParseWhitespace()
            {
                return ParseWhitespace(true);
            }

            private NextTokenResults ParseWhitespace(bool allowEOL)
            {
                int ci = this.GetColumnIndex(), l = this.GetLineIndex();
                long p = this.Position;
                char c = this.LookAhead(0);
                if (IsWhitespaceChar(c))
                {
                    int lookAhead = 0;
                    while (IsWhitespaceChar(c = this.LookAhead(++lookAhead)))
                        if ((!(allowEOL)) && ((c == '\r') || (c == '\n')))
                        {
                            return new NextTokenResults(GrammarCore.GetParserError(FileName, l, c, GDParserErrors.UnexpectedEndOfLine));
                        }
                    string whiteSpace = new string(this.Flush(lookAhead));
                    return new NextTokenResults(new GDTokens.WhitespaceToken(whiteSpace, l, ci, p));
                }
                return new NextTokenResults(GrammarCore.GetParserError(FileName, l, c, GDParserErrors.UnknownSymbol));
            }

            private NextTokenResults ParsePreprocessor()
            {
                char c = LookAhead(0);
                if (c == '#')
                {
                    c = LookAhead(1);
                    switch (c)
                    {
                        case 'A':
                            if (Follows("ssemblyName", 2))
                            {
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.AssemblyNameDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            }
                            break;
                        case 'a':
                            if (Follows("ddrule", 2))
                            {
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.AddRuleDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            }
                            break;
                        case 'i':
                            if (LookAhead(2) == 'f')
                            {
                                bool isDef = false;
                                if (IsWhitespaceOrOperatorChar(c = LookAhead(3)))
                                {
                                    return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.IfDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                                }
                                else if (((isDef = (c == 'd')) || c == 'n') && Follows("def", isDef ? 3 : 4) && IsWhitespaceChar(LookAhead(isDef ? 6 : 7)))
                                    return new NextTokenResults(new GDTokens.PreprocessorDirective(isDef ? GDTokens.PreprocessorType.IfDefinedDirective : GDTokens.PreprocessorType.IfNotDefinedDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            }
                            else if (Follows("nclude", 2) && IsWhitespaceChar(LookAhead(8)))
                            {
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.IncludeDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            }
                            break;
                        case 'e':
                            if ((c = LookAhead(2)) == 'l')
                            {
                                if (IsWhitespaceOrOperatorChar(LookAhead(5)))
                                {
                                    if (Follows("se", 3))
                                    {
                                        return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.ElseDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                                    }
                                    else if (Follows("if", 3))
                                    {
                                        return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.ElseIfDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                                    }
                                }
                                else if (IsWhitespaceOrOperatorChar(LookAhead(8)) && Follows("lifdef", 3))
                                {
                                    return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.ElseIfDefinedDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                                }
                            }
                            else if (c == 'n')
                            {
                                if (Follows("dif", 3) && IsWhitespaceOrOperatorChar(LookAhead(6)))
                                    return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.EndIfDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            }
                            break;
                        case 'L':
                            if (Follows("exerName", 2))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.LexerNameDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                        case 'P':
                            if (Follows("arserName", 2))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.ParserNameDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                        case 'd':
                            if (Follows("efine", 2) && IsWhitespaceOrOperatorChar(LookAhead(7)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.DefineDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));

                            break;
                        case 'n':
                        case 'N':
                            if (Follows("amespace", 2) && IsWhitespaceOrOperatorChar(LookAhead(10)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.NamespaceDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                        case 't':
                            if (Follows("hrow", 2) && IsWhitespaceOrOperatorChar(LookAhead(6)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.ThrowDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                        case 'T':
                            if (Follows("okenPrefix", 2) && IsWhitespaceOrOperatorChar(LookAhead(12)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.TokenPrefixDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            else if (Follows("okenSuffix", 2) && IsWhitespaceOrOperatorChar(LookAhead(12)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.TokenSuffixDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                        case 'r':
                            if (Follows("eturn", 2) && IsWhitespaceOrOperatorChar(LookAhead(7)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.ReturnDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                        case 'R':
                            if (Follows("oot", 2) && IsWhitespaceOrOperatorChar(LookAhead(5)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.RootDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            else if (Follows("ulePrefix", 2) && IsWhitespaceOrOperatorChar(LookAhead(11)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.RulePrefixDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            else if (Follows("uleSuffix", 2) && IsWhitespaceOrOperatorChar(LookAhead(11)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.RuleSuffixDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                        case 'G':
                            if (Follows("rammarName", 2) && IsWhitespaceOrOperatorChar(LookAhead(12)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.GrammarNameDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                    }
                }
                return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, GetLineIndex(), GetColumnIndex(), GDParserErrors.UnknownSymbol));
            }

            private bool IsWhitespaceOrOperatorChar(char p)
            {
                return IsWhitespaceChar(p) || IsOperatorChar(p);
            }

            private bool Follows(string find, int howFar)
            {
                for (int i = howFar; i < (find.Length + howFar); i++)
                {
                    if (LookAhead(i) != find[i - howFar])
                        return false;
                }
                return true;
            }

            private NextTokenResults ParseIdentifier()
            {
                char c = this.LookAhead(0);
                int lookAhead = 1;
                //[A-Za-z_]+
                if (!(IsIdentifierChar(c)))
                    //At least one.
                    return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, GetLineIndex(), GetColumnIndex(), GDParserErrors.UnknownSymbol));
                while (true)
                {
                    c = this.LookAhead(lookAhead);
                    if (!(IsIdentifierChar(c)))
                    {
                        if (c == char.MinValue)
                            return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, this.GetLineIndex(), this.GetColumnIndex(), GDParserErrors.UnexpectedEndOfFile));
                        break;
                    }
                    lookAhead++;
                }
                //[A-Za-z_]*
                while (true)
                {
                    c = this.LookAhead(lookAhead);
                    if (!(IsIdentifierChar(c, false)))
                    {
                        if (c == char.MinValue)
                            return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, this.GetLineIndex(), this.GetColumnIndex(), GDParserErrors.UnexpectedEndOfFile));
                        break;
                    }
                    lookAhead++;
                }
                int ci = this.GetColumnIndex(), l = this.GetLineIndex();
                long p = this.Position;
                string name = new string(this.Flush(lookAhead));
                return new NextTokenResults(new GDTokens.IdentifierToken(name, ci, l, p));
            }

            private NextTokenResults ParseString()
            {
                char c = LookAhead(0);
                bool caseInsensitive = false;
                if (c == '@')
                {
                    caseInsensitive = true;
                    c = LookAhead(1);
                }
                if (c == '"')
                {
                    int lookAhead = caseInsensitive ? 2 : 1;
                    while (true)
                    {
                        c = LookAhead(lookAhead);
                        if (c == '\\')
                        {
                            lookAhead++;
                            c = LookAhead(lookAhead);
                            switch (c)
                            {
                                case 'x':
                                    if (IsHexadecimalChar(LookAhead(lookAhead + 1)) && IsHexadecimalChar(LookAhead(lookAhead + 2)))
                                        lookAhead += 2;
                                    else
                                        return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, GetLineIndex(), GetColumnIndex(), GDParserErrors.InvalidEscape));
                                    break;
                                case 'u':
                                    if (IsHexadecimalChar(LookAhead(lookAhead + 1)) && IsHexadecimalChar(LookAhead(lookAhead + 2)) && IsHexadecimalChar(LookAhead(lookAhead + 3)) && IsHexadecimalChar(LookAhead(lookAhead + 4)))
                                        lookAhead += 4;
                                    else
                                        return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, GetLineIndex(), GetColumnIndex(), GDParserErrors.InvalidEscape));
                                    break;
                                case 'v':
                                case '\\':
                                case '"':
                                case 'f':
                                case 'r':
                                case 'n':
                                case 't':
                                    break;
                                default:
                                    return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, GetLineIndex(), GetColumnIndex(), GDParserErrors.InvalidEscape));
                            }
                        }
                        else if (c == '"')
                        {
                            lookAhead++;
                            break;
                        }
                        lookAhead++;
                    }
                    int ci = this.GetColumnIndex(), l = this.GetLineIndex();
                    long p = this.Position;
                    string @string = new string(this.Flush(lookAhead));
                    return new NextTokenResults(new GDTokens.StringLiteralToken(@string, caseInsensitive, ci, l, p));
                }
                return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, GetLineIndex(), GetColumnIndex(), GDParserErrors.UnknownSymbol));
            }
            internal bool MultiLineMode
            {
                get
                {
                    return this.multiLineMode;
                }
                set
                {
                    this.multiLineMode = value;
                }
            }
            private NextTokenResults ParseOperator()
            {
                char c = LookAhead(0);
                GDTokens.OperatorType? ot = null;
                switch (c)
                {
                    case '(':
                        ot = GDTokens.OperatorType.LeftParenthesis;
                        break;
                    case ')':
                        ot = GDTokens.OperatorType.RightParenthesis;
                        break;
                    case '-':
                        ot = GDTokens.OperatorType.Minus;
                        break;
                    case ':':
                        c = LookAhead(1);
                        if (c == '=')
                            ot = GDTokens.OperatorType.TokenSeparator;
                        else if (c == ':')
                        {
                            if (LookAhead(2) == '=')
                            {
                                ot = GDTokens.OperatorType.ProductionRuleSeparator;
                            }
                            else
                                return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, GetLineIndex(), GetColumnIndex(), GDParserErrors.UnknownSymbol));
                        }
                        else
                            ot = GDTokens.OperatorType.OptionsSeparator;
                        break;
                    case '*':
                        ot = GDTokens.OperatorType.ZeroOrMore;
                        break;
                    case '+':
                        ot = GDTokens.OperatorType.OneOrMore;
                        break;
                    case '?':
                        ot = GDTokens.OperatorType.ZeroOrOne;
                        break;
                    case '<':
                        ot = GDTokens.OperatorType.TemplatePartsStart;
                        break;
                    case '|':
                        ot = GDTokens.OperatorType.LeafSeparator;
                        break;
                    case ';':
                        ot = GDTokens.OperatorType.EntryTerminal;
                        break;
                    case '>':
                        ot = GDTokens.OperatorType.TemplatePartsEnd;
                        break;
                    case '!':
                        ot = GDTokens.OperatorType.ProductionRuleFlag;
                        break;
                    case ',':
                        ot = GDTokens.OperatorType.TemplatePartsSeparator;
                        break;
                    case '.':
                        ot = GDTokens.OperatorType.Period;
                        break;
                    case '=':
                        ot = GDTokens.OperatorType.ErrorSeparator;
                        break;
                    case '{':
                        ot = GDTokens.OperatorType.LeftCurlyBrace;
                        break;
                    case '}':
                        ot = GDTokens.OperatorType.RightCurlyBrace;
                        break;
                    case '#':
                        ot = GDTokens.OperatorType.CounterNotification;
                        break;
                    default:
                        return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, GetLineIndex(), GetColumnIndex(), GDParserErrors.UnknownSymbol));
                }
                return new NextTokenResults(new GDTokens.OperatorToken(ot.Value, this.GetColumnIndex(), this.GetLineIndex(), this.Position));
            }

            internal static bool IsIdentifierChar(char c) { return IsIdentifierChar(c, true); }
            internal static bool IsIdentifierChar(char c, bool first)
            {
                if (first)
                    return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';
                else
                    return c >= '0' && c <= '9' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c == '_';

            }

            internal static bool IsWhitespaceChar(char p)
            {
                return (p == '\t' || p == '\v' || p == '\f' || p == '\r' || p == '\n' || p == ' ');
            }

            internal static bool IsHexadecimalChar(char p)
            {
                return p >= '0' && p <= '9' || p >= 'a' && p <= 'f' || p >= 'A' && p <= 'F';
            }

            internal static bool IsOperatorChar(char p) { return AreOperatorChars(p, char.MinValue, char.MinValue); }
            internal static bool AreOperatorChars(char p, char q) { return AreOperatorChars(p, q, char.MinValue); }
            internal static bool AreOperatorChars(char p, char q, char r)
            {
                switch (p)
                {
                    case ':':
                        if ((q == char.MinValue) && (r == char.MinValue))
                            return true;
                        else if (q == ':' && r == '=')
                            return true;
                        else if (q == '=')
                            return true;
                        return true;
                        /* *
                         * GDTokens.OperatorType.ProductionRuleSeparator | 
                         * GDTokens.OperatorType.TokenSeparator  
                         * */
                    case '(':
                        //GDTokens.OperatorType.LeftParenthesis
                    case ')':
                        //GDTokens.OperatorType.RightParenthesis
                    case '#':
                        //GDTokens.OperatorType.CounterNotification
                    case '-':
                        //GDTokens.OperatorType.Minus
                    case '*':
                        //GDTokens.OperatorType.ZeroOrMore
                    case '!':
                        //GDTokens.OperatorType.ProductionRuleFlag
                    case '+':
                        //GDTokens.OperatorType.OneOrMore
                    case '?':
                        //GDTokens.OperatorType.ZeroOrOne
                    case '<':
                        //GDTokens.OperatorType.TemplatePartsStart
                    case '|':
                        //GDTokens.OperatorType.LeafSeparator
                    case ';':
                        //GDTokens.OperatorType.EntryTerminal
                    case '>':
                        //GDTokens.OperatorType.TemplatePartsEnd
                    case '.':
                        //GDTokens.OperatorType.Period
                    case ',':
                        //GDTokens.OperatorType.TemplatePartsSeparator
                    case '=':
                        //GDTokens.OperatorType.ErrorSeparator
                    case '{':
                        //GDTokens.OperatorType.LeftCurlyBrace
                    case '}':
                        //GDTokens.OperatorType.RightCurlyBrace
                        return true;
                    default:
                        return false;
                }
            }

            internal void ParseWhitespaceInternal()
            {
                this.ParseWhitespace(this.MultiLineMode);
            }
        }
    }
}
