using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public class LiteralCharReferenceTokenItem :
        LiteralReferenceTokenItem<char, ILiteralCharTokenItem>,
        ILiteralCharReferenceTokenItem
    {
        public LiteralCharReferenceTokenItem(IOilexerGrammarTokenEntry entryReference, ILiteralCharTokenItem reference, int column, int line, long position)
            : base(entryReference, reference, column, line, position)
        {

        }

        protected override object OnClone()
        {
            LiteralCharReferenceTokenItem result = new LiteralCharReferenceTokenItem(this.Source, this.Literal, this.Column, this.Line, this.Position);
            this.CloneData(result);
            return result;
        }
    }
}
