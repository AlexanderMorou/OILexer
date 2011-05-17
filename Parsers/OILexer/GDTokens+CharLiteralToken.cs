using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    partial class GDTokens
    {
        public class CharLiteralToken :
            GDToken
        {
            private string charDef;
            private bool caseInsensitive;

            public CharLiteralToken(string charDef, bool caseInsensitive, int column, int line, long position)
                : base(column, line, position)
            {
                
                this.charDef = charDef;
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

            public char GetCleanValue()
            {
                int index = caseInsensitive ? 1 : 0;

                if (this.charDef[index + 1] == '\\' && this.charDef.Length >= 4)
                {
                    switch (this.charDef[index + 2])
                    {
                        case 'n':
                            return '\n';
                        case 'r':
                            return '\r';
                        case 'f':
                            return '\f';
                        case '\\':
                            return '\\';
                        case 't':
                            return '\t';
                        case 'u':
                            if (this.charDef.Length == 8)
                            {
                                return (char)Convert.ToInt32(new string(new char[] 
                                    { 
                                        charDef[index + 3], 
                                        charDef[index + 4], 
                                        charDef[index + 5], 
                                        charDef[index + 6] }), 16);
                            }
                            return char.MinValue;
                        case 'x':
                            if (this.charDef.Length == 6)
                            {
                                return (char)Convert.ToInt32(new string(new char[] 
                                    { 
                                        charDef[index + 3], 
                                        charDef[index + 4] }), 16);
                            }
                            return char.MinValue;
                        case 'v':
                            return '\v';
                        case '\'':
                            return '\'';
                    }
                }
                return this.charDef[index + 1];
            }

            public override GDTokenType TokenType
            {
                get { return GDTokenType.CharacterLiteral; }
            }

            public override int Length
            {
                get { return this.charDef.Length; }
            }

            public override bool ConsumedFeed
            {
                get { return true; }
            }

            public string Value
            {
                get
                {
                    return this.charDef;
                }
            }
        }
    }
}
