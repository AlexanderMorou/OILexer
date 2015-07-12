using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
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
        public class CharacterRangeToken :
            OilexerGrammarToken
        {
            private RegularLanguageSet ranges;
            bool inverted;
            private int length;

            internal RegularLanguageSet Ranges
            {
                get
                {
                    return this.ranges;
                }
            }

            internal CharacterRangeToken(bool inverted, char[] singleTons, Tuple<char, char>[] rangeSet, UnicodeCategory[] letterCategories, int length, int line, int column, long position)
                : base(line, column, position)
            {
                this.ranges = RegularLanguageSet.GetRegularLanguageSet(inverted, singleTons, rangeSet, letterCategories);
                this.length = length;
                this.inverted = inverted;
            }

            public bool Inverted
            {
                get
                {
                    return this.inverted;
                }
            }

            public override string ToString()
            {
                return this.ranges.ToString();
            }

            public override OilexerGrammarTokenType TokenType
            {
                get { return OilexerGrammarTokenType.CharacterRange; }
            }

            public override int Length
            {
                get { return this.length; }
            }

            public override bool ConsumedFeed
            {
                get { return false; }
            }
        }
    }
}
