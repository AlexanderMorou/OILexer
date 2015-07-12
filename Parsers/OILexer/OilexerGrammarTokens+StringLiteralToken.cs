using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{

    partial class OilexerGrammarTokens
    {
        public class StringLiteralToken :
            OilexerGrammarToken
        {
            private string value;
            private string cleanValue;
            private bool caseInsensitive;
            public StringLiteralToken(string value, bool caseInsensitive, int column, int line, long position)
                : base(column, line, position)
            {
                this.value = value;
                this.caseInsensitive = caseInsensitive;
            }

            /// <summary>
            /// Returns whether there was an '@' in front of the <see cref="CharLiteralToken"/> when
            /// it was parsed.  Indicates that when used the text should have case ingored.
            /// </summary>
            public bool CaseInsensitive
            {
                get
                {
                    return this.caseInsensitive;
                }
            }

            public string Value
            {
                get
                {
                    return this.value;
                }
            }

            /// <summary>
            /// Cleans up the value of the <see cref="StringLiteralToken"/> removing the quotes
            /// and replaces escape characters with their respective normal value.
            /// </summary>
            /// <returns>The cleaned up version of the <see cref="Value"/>.</returns>
            public string GetCleanValue()
            {
                if (this.cleanValue == null)
                {
                    if (this.Value.Length < 2)
                    {
                        this.cleanValue = value;
                        return this.cleanValue;
                    }
                    string s = this.Value.Substring(1, this.Value.Length - 2);
                    if (caseInsensitive)
                        s = s.Substring(1);
                    StringBuilder result = new StringBuilder();
                    for (int i = 0; i < s.Length; i++)
                    {
                        char c = s[i];
                        if (c == '\\')
                        {
                            i++;
                            c = s[i];
                            switch (c)
                            {
                                case '"':
                                case '\\':
                                    result.Append(c);
                                    break;
                                case 'v':
                                    result.Append('\v');
                                    break;
                                case 'f':
                                    result.Append('\f');
                                    break;
                                case 'r':
                                    result.Append('\r');
                                    break;
                                case 'n':
                                    result.Append('\n');
                                    break;
                                case 't':
                                    result.Append('\t');
                                    break;
                                case 'x':
                                    i++;
                                    if (OilexerParser.Lexer.IsHexadecimalChar(s[i]) && OilexerParser.Lexer.IsHexadecimalChar(s[i + 1]))
                                        result.Append((char)Convert.ToInt32(string.Format("{0}{1}", s[i], s[i + 1]), 16));
                                    //only by one, the iteration will pick up the next.
                                    i++;
                                    break;
                                case 'u':
                                    i++;
                                    if (OilexerParser.Lexer.IsHexadecimalChar(s[i]) && OilexerParser.Lexer.IsHexadecimalChar(s[i + 1]) && OilexerParser.Lexer.IsHexadecimalChar(s[i + 2]) && OilexerParser.Lexer.IsHexadecimalChar(s[i + 3]))
                                        result.Append((char)Convert.ToInt32(string.Format("{0}{1}{2}{3}", s[i], s[i + 1], s[i + 2], s[i + 3]), 16));
                                    //only by three, same reason as above.
                                    i += 3;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                            result.Append(c);
                    }
                    this.cleanValue = result.ToString();
                }
                return this.cleanValue;
            }

            public override OilexerGrammarTokenType TokenType
            {
                get { return OilexerGrammarTokenType.StringLiteral; }
            }

            public override int Length
            {
                get { return this.value.Length; }
            }

            public override bool ConsumedFeed
            {
                get { return true; }
            }

            public override string ToString()
            {
                return this.Value;
            }
        }
    }
}
