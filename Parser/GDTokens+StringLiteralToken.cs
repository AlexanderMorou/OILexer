using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser
{

    partial class GDTokens
    {
        public class StringLiteralToken :
            GDToken
        {
            private string value;
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
                if (this.Value.Length <= 2)
                    return value;
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
                                if (GDParser.GDTokenizer.IsHexadecimalChar(s[i]) && GDParser.GDTokenizer.IsHexadecimalChar(s[i + 1]))
                                    result.Append((char)Convert.ToInt32(string.Format("{0}{1}", s[i], s[i + 1]), 16));
                                //only by one, the iteration will pick up the next.
                                i++;
                                break;
                            case 'u':
                                i++;
                                if (GDParser.GDTokenizer.IsHexadecimalChar(s[i]) && GDParser.GDTokenizer.IsHexadecimalChar(s[i + 1]) && GDParser.GDTokenizer.IsHexadecimalChar(s[i + 2]) && GDParser.GDTokenizer.IsHexadecimalChar(s[i + 3]))
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
                return result.ToString();
            }

            public override GDTokenType TokenType
            {
                get { return GDTokenType.StringLiteral; }
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
