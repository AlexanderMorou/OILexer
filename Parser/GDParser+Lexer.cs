using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Oilexer._Internal;
using Oilexer.Parser.GDFileData.TokenExpression;
using System.Globalization;

namespace Oilexer.Parser
{
    partial class GDParser
    {
        public sealed class Lexer :
            Tokenizer<IGDToken>,
            IGDTokenizer
        {
            /// <summary>
            /// Data member determining whether the tokenizer is in multiline mode.
            /// </summary>
            private bool multiLineMode = true;

            /// <summary>
            /// Creates a new <see cref="Lexer"/> with the
            /// <see cref="Stream"/>, <paramref name="s"/>, and the 
            /// <paramref name="fileName"/> provided.
            /// </summary>
            /// <param name="s">The <see cref="Stream"/> from which
            /// to read the tokens from.</param>
            /// <param name="fileName">The <see cref="String"/> identifier
            /// which is used to identify the <see cref="Stream"/>, <paramref name="s"/>,
            /// when errors occur.</param>
            public Lexer(Stream s, string fileName)
                : base(s, fileName)
            {
            }

            private void ___Test()
            {
                Console.WriteLine(this);
            }

            internal Lexer()
                : base()
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
                else if (c == '@' && LookAhead(1) == '"')
                    return ParseString();
                else if (c == '@' && LookAhead(1) == '\'')
                    return ParseCharacterLiteral();
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
                else if (c == '[')
                    return ParseCharacterRange();
                else if (c == '\0')
                    return new NextTokenResults();
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
                List<char> singleTons = new List<char>();
                List<Tuple<char, char>> ranges = new List<Tuple<char, char>>();
                List<UnicodeCategory> categories = new List<UnicodeCategory>();
                if (hasHyphen)
                    singleTons.Add('-');
                while ((c = LookAhead(lookAhead)) != ']')
                {
                    if (lookAhead == start && c == '-')
                        return new NextTokenResults(GrammarCore.GetParserError(this.FileName, this.GetLineIndex(this.Position + lookAhead), this.GetColumnIndex(this.Position + lookAhead), GDParserErrors.Unexpected, "-"));
                    else if (c == '\r' || c == '\n')
                        return new NextTokenResults(GrammarCore.GetParserError(this.FileName, this.GetLineIndex(this.Position + lookAhead), this.GetColumnIndex(this.Position + lookAhead), GDParserErrors.UnexpectedEndOfLine));
                    else if (c == char.MinValue)
                        return new NextTokenResults(GrammarCore.GetParserError(this.FileName, this.GetLineIndex(this.Position + lookAhead), this.GetColumnIndex(this.Position + lookAhead), GDParserErrors.UnexpectedEndOfFile));
                    else if (c == ':')
                    {
                        //UppercaseLetter         -- Lu (letter, upper)
                        //LowercaseLetter         -- Ll (letter, lower)
                        //TitlecaseLetter         -- Lt (letter, titlecase)
                        //ModifierLetter          -- Lm (letter, modifier)
                        //OtherLetter             -- Lo (letter, other)
                        //NonSpacingMark          -- Mn (mark, nonspacing)
                        //SpaceCombiningMark      -- Mc (mark, combining)
                        //EnclosingMark           -- Me (mark, enclosing)
                        //DecimalDigitNumber      -- Nd (number, decimal digit)
                        //LetterNumber            -- Nl (number, letter)
                        //OtherNumber             -- No (number, other)
                        //SpaceSeparator          -- Zs (separator, space)
                        //LineSeparator           -- Zl (separator, line)
                        //ParagraphSeparator      -- Zp (separator, paragraph)
                        //Control                 -- Cc (other, control)
                        //Format                  -- Cf (other, format)
                        //Surrogate               -- Cs (other, surrogate)
                        //PrivateUse              -- Co (other, private use)
                        //OtherNotAssigned        -- Cn (other, not assigned)
                        //ConnectorPunctuation    -- Pc (punctuation, connector)
                        //DashPunctuation         -- Pd (punctuation, dash)
                        //OpenPunctuation         -- Ps (punctuation, open /start)
                        //ClosePunctuation        -- Pe (punctuation, close/end)
                        //InitialQuotePunctuation -- Pi (punctuation, initial quote)
                        //FinalQuotePunctuation   -- Pf (punctuation, final quote)
                        //OtherPunctuation        -- Po (punctuation, other)
                        //MathSymbol              -- Sm (symbol, math)
                        //CurrencySymbol          -- Sc (symbol, currency)
                        //ModifierSymbol          -- Sk (symbol, modifier)
                        //OtherSymbol             -- So (symbol, other)
                        //Mark*                   -- M  (mark, nonspacing, combining, enclosing)
                        c = LookAhead(lookAhead + 1);
                        switch (c)
                        {
                            case 'L'://ultmo
                                #region Letters
                                c = LookAhead(lookAhead + 2);
                                if (c != ':' && LookAhead(lookAhead + 3) == ':')
                                {
                                    if (c == 'u' ||
                                        c == 'l' ||
                                        c == 't' ||
                                        c == 'm' ||
                                        c == 'o')
                                    {
                                        lookAhead += 4;
                                        switch (c)
                                        {
                                            case 'u': //:Lu: (letter, upper)
                                                categories.Add(UnicodeCategory.UppercaseLetter);
                                                break;
                                            case 'l': //:Ll: (letter, lower)
                                                categories.Add(UnicodeCategory.LowercaseLetter);
                                                break;
                                            case 't': //:Lt: (letter, titlecase)
                                                categories.Add(UnicodeCategory.TitlecaseLetter);
                                                break;
                                            case 'm': //:Lm: (letter, modifier)
                                                categories.Add(UnicodeCategory.ModifierLetter);
                                                break;
                                            case 'o': //:Lo: (letter, other)
                                                categories.Add(UnicodeCategory.OtherLetter);
                                                break;
                                        }
                                        continue;
                                    }
                                    else
                                    {
                                        c = ':';
                                        break;
                                    }
                                }
                                else if (c == ':') // L
                                {
                                    lookAhead += 3;
                                    categories.Add(UnicodeCategory.UppercaseLetter);
                                    categories.Add(UnicodeCategory.LowercaseLetter);
                                    categories.Add(UnicodeCategory.TitlecaseLetter);
                                    categories.Add(UnicodeCategory.ModifierLetter);
                                    categories.Add(UnicodeCategory.OtherLetter);
                                    continue;
                                }
                                #endregion
                                break;
                            case 'M'://nce
                                #region Marks
                                c = LookAhead(lookAhead + 2);
                                if (c != ':' && LookAhead(lookAhead + 3) == ':')
                                {
                                    if (c == 'n' ||
                                        c == 'c' ||
                                        c == 'e')
                                        lookAhead += 4;
                                    else
                                    {
                                        c = ':';
                                        break;
                                    }
                                    switch (c)
                                    {
                                        case 'n': //:Mn: (mark, nonspacing)
                                            categories.Add(UnicodeCategory.NonSpacingMark);
                                            break;
                                        case 'c': //:Mc: (mark, combining)
                                            categories.Add(UnicodeCategory.SpacingCombiningMark);
                                            break;
                                        case 'e': //:Me: (mark, enclosing)
                                            categories.Add(UnicodeCategory.EnclosingMark);
                                            break;
                                    }
                                    continue;
                                }
                                else if (c == ':') //:M: (mark, nonspacing, combining, enclosing)
                                {
                                    lookAhead += 3;
                                    categories.Add(UnicodeCategory.NonSpacingMark);
                                    categories.Add(UnicodeCategory.SpacingCombiningMark);
                                    categories.Add(UnicodeCategory.EnclosingMark);
                                    continue;
                                }
                                else
                                    c = ':';
                                #endregion
                                break;
                            case 'N': //dlo
                                #region Numbers
                                c = LookAhead(lookAhead + 2);
                                if (c != ':' && LookAhead(lookAhead + 3) == ':')
                                {
                                    if (c == 'd' ||
                                        c == 'l' ||
                                        c == 'o')
                                        lookAhead += 4;
                                    else
                                    {
                                        c = ':';
                                        break;
                                    }
                                    switch (c)
                                    {
                                        case 'd': //:Nd: (number, decimal digit)
                                            categories.Add(UnicodeCategory.DecimalDigitNumber);
                                            break;
                                        case 'l': //:Nl: (number, letter)
                                            categories.Add(UnicodeCategory.LetterNumber);
                                            break;
                                        case 'o': //:No: (number, other)
                                            categories.Add(UnicodeCategory.OtherNumber);
                                            break;
                                    }
                                    continue;
                                }
                                else if (c == ':') //:N: (number, decimal digit, letter, other)
                                {
                                    lookAhead += 3;
                                    categories.Add(UnicodeCategory.DecimalDigitNumber);
                                    categories.Add(UnicodeCategory.LetterNumber);
                                    categories.Add(UnicodeCategory.OtherNumber);
                                    continue;
                                }
                                else
                                    c = ':';
                                #endregion
                                break;
                            case 'Z'://slp
                                #region Separators
                                c = LookAhead(lookAhead + 2);
                                if (c != ':' && LookAhead(lookAhead + 3) == ':')
                                {
                                    if (c == 's' ||
                                        c == 'l' ||
                                        c == 'p')
                                        lookAhead += 4;
                                    else
                                    {
                                        c = ':';
                                        break;
                                    }
                                    switch (c)
                                    {
                                        case 's': // :Zs: (separator, space)
                                            categories.Add(UnicodeCategory.SpaceSeparator);
                                            break;
                                        case 'l': // :Zl: (separator, line)
                                            categories.Add(UnicodeCategory.LineSeparator);
                                            break;
                                        case 'p': // :Zp: (separator, paragraph)
                                            categories.Add(UnicodeCategory.ParagraphSeparator);
                                            break;
                                    }
                                    continue;
                                }
                                else if (c == ':') // :Z: (separator, space, line, paragraph)
                                {
                                    lookAhead += 3;
                                    categories.Add(UnicodeCategory.SpaceSeparator);
                                    categories.Add(UnicodeCategory.LineSeparator);
                                    categories.Add(UnicodeCategory.ParagraphSeparator);
                                    continue;
                                }
                                else
                                    c = ':';
                                #endregion
                                break;
                            case 'C'://cfson
                                #region Other
                                c = LookAhead(lookAhead + 2);
                                if (c != ':' && LookAhead(lookAhead + 3) == ':')
                                {
                                    if (c == 'c' ||
                                        c == 'f' ||
                                        c == 's' ||
                                        c == 'o' ||
                                        c == 'n')
                                        lookAhead += 4;
                                    else
                                    {
                                        c = ':';
                                        break;
                                    }
                                    switch (c)
                                    {
                                        case 'c': // :Cc: (other, control)
                                            categories.Add(UnicodeCategory.Control);
                                            break;
                                        case 'f': // :Cf: (other, format)
                                            categories.Add(UnicodeCategory.Format);
                                            break;
                                        case 's': // :Cs: (other, surrogate)
                                            categories.Add(UnicodeCategory.Surrogate);
                                            break;
                                        case 'o': // :Co: (other, private use)
                                            categories.Add(UnicodeCategory.PrivateUse);
                                            break;
                                        case 'n': // :Cn: (other, not assigned)
                                            categories.Add(UnicodeCategory.OtherNotAssigned);
                                            break;
                                    }
                                    continue;
                                }
                                else if (c == ':') // :C: (other, control, format, surrogate, private use, not assigned)
                                {
                                    lookAhead += 3;
                                    categories.Add(UnicodeCategory.Control);
                                    categories.Add(UnicodeCategory.Format);
                                    categories.Add(UnicodeCategory.Surrogate);
                                    categories.Add(UnicodeCategory.PrivateUse);
                                    categories.Add(UnicodeCategory.OtherNotAssigned);
                                    continue;
                                }
                                else
                                    c = ':';
                                #endregion
                                break;
                            case 'P'://cdseifo
                                #region Punctuation 
                                c = LookAhead(lookAhead + 2);
                                if (c != ':' && LookAhead(lookAhead + 3) == ':')
                                {
                                    if (c == 'c' ||
                                        c == 'd' ||
                                        c == 's' ||
                                        c == 'e' ||
                                        c == 'i' ||
                                        c == 'f' ||
                                        c == 'o')
                                        lookAhead += 4;
                                    else
                                    {
                                        c = ':';
                                        break;
                                    }
                                    switch (c)
                                    {
                                        case 'c': // Pc (punctuation, connector)
                                            categories.Add(UnicodeCategory.ConnectorPunctuation);
                                            break;
                                        case 'd': // :Pd: (punctuation, dash)
                                            categories.Add(UnicodeCategory.DashPunctuation);
                                            break;
                                        case 's': // :Ps: (punctuation, open /start)
                                            categories.Add(UnicodeCategory.OpenPunctuation);
                                            break;
                                        case 'e': // :Pe: (punctuation, close/end)
                                            categories.Add(UnicodeCategory.ClosePunctuation);
                                            break;
                                        case 'i': // :Pi: (punctuation, initial quote)
                                            categories.Add(UnicodeCategory.InitialQuotePunctuation);
                                            break;
                                        case 'f': // :Pf: (punctuation, final quote)
                                            categories.Add(UnicodeCategory.FinalQuotePunctuation);
                                            break;
                                        case 'o': // :Po: (punctuation, other)
                                            categories.Add(UnicodeCategory.OtherPunctuation);
                                            break;
                                    }
                                    continue;
                                }
                                else if (c == ':') // :P: (punctuation, connector, dash, open, close, initial quote, final quote, other)
                                {
                                    lookAhead += 3;
                                    categories.Add(UnicodeCategory.ConnectorPunctuation);
                                    categories.Add(UnicodeCategory.DashPunctuation);
                                    categories.Add(UnicodeCategory.OpenPunctuation);
                                    categories.Add(UnicodeCategory.ClosePunctuation);
                                    categories.Add(UnicodeCategory.InitialQuotePunctuation);
                                    categories.Add(UnicodeCategory.FinalQuotePunctuation);
                                    categories.Add(UnicodeCategory.OtherPunctuation);
                                    continue;
                                }
                                else
                                    c = ':';
                                #endregion
                                break;
                            case 'S'://mcko
                                #region Symbols
                                c = LookAhead(lookAhead + 2);
                                if (c != ':')
                                {
                                    if (LookAhead(lookAhead + 3) == ':')
                                    {
                                        if (c == 'm' ||
                                            c == 'c' ||
                                            c == 'k' ||
                                            c == 'o')
                                            lookAhead += 4;
                                        else
                                            goto s_fallThrough;
                                        switch (c)
                                        {
                                            case 'm': // Sm (symbol, math)
                                                categories.Add(UnicodeCategory.MathSymbol);
                                                break;
                                            case 'c': // Sc (symbol, currency)
                                                categories.Add(UnicodeCategory.CurrencySymbol);
                                                break;
                                            case 'k': // Sk (symbol, modifier)
                                                categories.Add(UnicodeCategory.ModifierSymbol);
                                                break;
                                            case 'o': // So (symbol, other)
                                                categories.Add(UnicodeCategory.OtherSymbol);
                                                break;
                                        }
                                        continue;
                                    }
                                }
                                else// :S: (symbol, math, currency, modifier, other)
                                {
                                    lookAhead += 3;
                                    categories.Add(UnicodeCategory.MathSymbol);
                                    categories.Add(UnicodeCategory.CurrencySymbol);
                                    categories.Add(UnicodeCategory.ModifierSymbol);
                                    categories.Add(UnicodeCategory.OtherSymbol);
                                    continue;
                                }
                        s_fallThrough:
                                c = ':';
                                #endregion
                                break;
                            default:
                                singleTons.Add(':');
                                lookAhead++;
                                continue;
                        }
                    }
                    else if (c == '\\')
                    {
                        switch (c = LookAhead(++lookAhead))
                        {
                            //\x[0-9A-Fa-f]{2}
                            case 'x':
                                {
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
                                }
                                break;
                            //\u[0-9A-Fa-f]{4}
                            case 'u':
                                {
                                    c = LookAhead(++lookAhead);
                                    char c2 = LookAhead(++lookAhead);
                                    char c3 = LookAhead(++lookAhead);
                                    char c4 = LookAhead(++lookAhead);
                                    if (('0' <= c && c <= '9' || 'A' <= c && c <= 'F' || 'a' <= c && c <= 'f') &&
                                        ('0' <= c2 && c2 <= '9' || 'A' <= c2 && c2 <= 'F' || 'a' <= c2 && c2 <= 'f') &&
                                        ('0' <= c3 && c3 <= '9' || 'A' <= c3 && c3 <= 'F' || 'a' <= c3 && c3 <= 'f') &&
                                        ('0' <= c4 && c4 <= '9' || 'A' <= c4 && c4 <= 'F' || 'a' <= c4 && c4 <= 'f'))
                                    {
                                        string s = new string(new char[] { c, c2, c3, c4 });
                                        c = (char)(Convert.ToInt32(s, 16));
                                    }
                                    else
                                    {
                                        lookAhead -= 4;
                                        goto default;
                                    }
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
                            case '[': case ']':
                            case '-':
                            case ':':
                            case '\\':
                                break;
                            default:
                                //Syntax error.
                                return new NextTokenResults(GrammarCore.GetParserError(this.FileName, this.GetLineIndex(this.Position + lookAhead), this.GetColumnIndex(this.Position + lookAhead), GDParserErrors.InvalidEscape));
                        }
                    }
                    if (c == '-')
                    {
                        singleTons.Remove(last);
                        rangeStart = last;
                        ranged = true;
                    }
                    else if (ranged)
                    {
                        ranges.Add(new Tuple<char, char>((char)Math.Min(c, rangeStart), (char)Math.Max(rangeStart, c)));
                        ranged = false;
                        rangeStart = char.MinValue;
                    }
                    else
                        singleTons.Add(c);
                    last = c;
                    lookAhead++;
                }
                if (ranged)
                {
                    singleTons.Add(rangeStart);
                    if (!hasHyphen)
                        singleTons.Add('-');
                }
                singleTons.Sort();

                return new NextTokenResults(new GDTokens.CharacterRangeToken(invert, singleTons.ToArray(), ranges.ToArray(), categories.Distinct().ToArray(), lookAhead + 1, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
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
                    int lookAhead = 0;
                    while ((c = LookAhead(lookAhead++)) != '\r' && c != '\n') ;

                    commentBody = new string(this.Flush(lookAhead - 1));
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
                return new NextTokenResults(new GDTokens.CommentToken(string.Format("{0}{1}", commentHeader, commentBody), ci, l, p, commentBody.Contains(Environment.NewLine)));
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
                                if (IsWhitespaceOrOperatorOrNullChar(c = LookAhead(3)))
                                {
                                    return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.IfDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                                }
                                else if (((isDef = (c == 'd')) || c == 'n') && Follows("def", isDef ? 3 : 4) && IsWhitespaceOrNullChar(LookAhead(isDef ? 6 : 7)))
                                    return new NextTokenResults(new GDTokens.PreprocessorDirective(isDef ? GDTokens.PreprocessorType.IfDefinedDirective : GDTokens.PreprocessorType.IfNotDefinedDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            }
                            else if (Follows("nclude", 2) && IsWhitespaceOrNullChar(LookAhead(8)))
                            {
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.IncludeDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            }
                            break;
                        case 'e':
                            if ((c = LookAhead(2)) == 'l')
                            {
                                if (IsWhitespaceOrOperatorOrNullChar(LookAhead(5)))
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
                                else if (IsWhitespaceOrOperatorOrNullChar(LookAhead(8)) && Follows("lifdef", 3))
                                {
                                    return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.ElseIfDefinedDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                                }
                            }
                            else if (c == 'n')
                            {
                                if (Follows("dif", 3) && IsWhitespaceOrOperatorOrNullChar(LookAhead(6)))
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
                            if (Follows("efine", 2) && IsWhitespaceOrOperatorOrNullChar(LookAhead(7)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.DefineDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));

                            break;
                        case 'n':
                        case 'N':
                            if (Follows("amespace", 2) && IsWhitespaceOrOperatorOrNullChar(LookAhead(10)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.NamespaceDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                        case 't':
                            if (Follows("hrow", 2) && IsWhitespaceOrOperatorOrNullChar(LookAhead(6)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.ThrowDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                        case 'T':
                            if (Follows("okenPrefix", 2) && IsWhitespaceOrOperatorOrNullChar(LookAhead(12)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.TokenPrefixDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            else if (Follows("okenSuffix", 2) && IsWhitespaceOrOperatorOrNullChar(LookAhead(12)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.TokenSuffixDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                        case 'r':
                            if (Follows("eturn", 2) && IsWhitespaceOrOperatorOrNullChar(LookAhead(7)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.ReturnDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                        case 'R':
                            if (Follows("oot", 2) && IsWhitespaceOrOperatorOrNullChar(LookAhead(5)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.RootDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            else if (Follows("ulePrefix", 2) && IsWhitespaceOrOperatorOrNullChar(LookAhead(11)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.RulePrefixDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            else if (Follows("uleSuffix", 2) && IsWhitespaceOrOperatorOrNullChar(LookAhead(11)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.RuleSuffixDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                        case 'G':
                            if (Follows("rammarName", 2) && IsWhitespaceOrOperatorOrNullChar(LookAhead(12)))
                                return new NextTokenResults(new GDTokens.PreprocessorDirective(GDTokens.PreprocessorType.GrammarNameDirective, this.GetLineIndex(), this.GetColumnIndex(), this.Position));
                            break;
                    }
                }
                return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, GetLineIndex(), GetColumnIndex(), GDParserErrors.UnknownSymbol));
            }

            private bool IsWhitespaceOrNullChar(char p)
            {
                return p == char.MinValue ||
                       IsWhitespaceChar(p);
            }

            private bool IsWhitespaceOrOperatorOrNullChar(char p)
            {
                return p == char.MinValue ||
                       IsWhitespaceOrOperatorChar(p);
            }

            private bool IsWhitespaceOrOperatorChar(char p)
            {
                return IsWhitespaceChar(p) || IsOperatorChar(p);
            }

            private bool Follows(string find, int howFar, bool caseSensitive)
            {
                if (!caseSensitive)
                    find = find.ToLower();
                for (int i = howFar; i < (find.Length + howFar); i++)
                    if (caseSensitive)
                    {
                        if (LookAhead(i) != find[i - howFar])
                            return false;
                    }
                    else if (char.ToLower(LookAhead(i)) != find[i - howFar])
                        return false;
                return true;
            }

            private bool Follows(string find, int howFar)
            {
                return Follows(find, howFar, true);
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
                        else if (c == '\0')
                        {
                            return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, GetLineIndex(), GetColumnIndex(), GDParserErrors.UnexpectedEndOfFile));
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
                            ot = GDTokens.OperatorType.ColonEquals;
                        else if (c == ':')
                        {
                            if (LookAhead(2) == '=')
                            {
                                ot = GDTokens.OperatorType.ColonColonEquals;
                            }
                            else
                                return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, GetLineIndex(), GetColumnIndex(), GDParserErrors.UnknownSymbol));
                        }
                        else
                            ot = GDTokens.OperatorType.OptionsSeparator;
                        break;
                    case '*':
                        c = LookAhead(1);
                        if (c == '*')
                            ot = GDTokens.OperatorType.AsteriskAsterisk;
                        else
                            ot = GDTokens.OperatorType.ZeroOrMore;
                        break;
                    case '+':
                        ot = GDTokens.OperatorType.Plus;
                        break;
                    case '?':
                        ot = GDTokens.OperatorType.ZeroOrOne;
                        break;
                    case '<':
                        ot = GDTokens.OperatorType.LessThan;
                        break;
                    case '|':
                        c = LookAhead(1);
                        if (c == '|')
                            ot = GDTokens.OperatorType.PipePipe;
                        else
                            ot = GDTokens.OperatorType.Pipe;
                        break;
                    case ';':
                        ot = GDTokens.OperatorType.SemiColon;
                        break;
                    case '>':
                        ot = GDTokens.OperatorType.GreaterThan;
                        break;
                    case '!':
                        c = LookAhead(1);
                        if (c == '=')
                            ot = GDTokens.OperatorType.ExclaimEqual;
                        else
                            ot = GDTokens.OperatorType.Exclaim;
                        break;
                    case ',':
                        ot = GDTokens.OperatorType.Comma;
                        break;
                    case '.':
                        ot = GDTokens.OperatorType.Period;
                        break;
                    case '=':
                        c = LookAhead(1);
                        if (c == '=')
                            ot = GDTokens.OperatorType.EqualEqual;
                        else
                            ot = GDTokens.OperatorType.Equals;
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
                    case '&':
                        c = LookAhead(1);
                        if (c == '&')
                            ot = GDTokens.OperatorType.AndAnd;
                        else
                            return new NextTokenResults(_Internal.GrammarCore.GetParserError(FileName, GetLineIndex(), GetColumnIndex(), GDParserErrors.UnknownSymbol));
                        break;
                    case '$':
                        ot = GDTokens.OperatorType.ForcedStringForm;
                        break;
                    case '@':
                        ot = GDTokens.OperatorType.AtSign;
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

            internal static bool IsOperatorChar(char p)
            {
                return AreOperatorChars(p, char.MinValue, char.MinValue);
            }

            internal static bool AreOperatorChars(char p, char q)
            {
                return AreOperatorChars(p, q, char.MinValue);
            }

            internal static bool AreOperatorChars(char p, char q, char r)
            {
                switch (p)
                {
                    case '*':
                        //GDTokens.OperatorType.ZeroOrMore
                        if (q == char.MinValue && r == char.MinValue)
                            return true;
                        else if (q == '*' && r == char.MinValue)
                            return true;
                        return true;
                    case '=':
                    case '!':
                        //GDTokens.OperatorType.ErrorSeparator
                        //GDTokens.OperatorType.ProductionRuleFlag
                        if (q == char.MinValue && r == char.MinValue)
                            return true;
                        else if (q == '=' && r == char.MinValue)
                            //GDTokens.OperatorType.EqualsEquals
                            //GDTokens.OperatorType.ExclaimEquals
                            return true;
                        return true;
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
                    case '+':
                        //GDTokens.OperatorType.OneOrMore
                    case '@':
                        //GDTokens.OperatorType.AtSign
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
                    case '{':
                        //GDTokens.OperatorType.LeftCurlyBrace
                    case '}':
                        //GDTokens.OperatorType.RightCurlyBrace
                    case '$':
                        //GDTokens.OperatorType.ForcedStringForm
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
