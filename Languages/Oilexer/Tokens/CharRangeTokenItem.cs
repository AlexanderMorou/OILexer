using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public class CharRangeTokenItem :
        TokenItem,
        ICharRangeTokenItem
    {
        private bool inverted;
        private RegularLanguageSet chars;

        protected internal CharRangeTokenItem(bool inverted, RegularLanguageSet chars, int line, int column, long position)
            : base(line, column, position)
        {
            this.chars = chars;
            this.inverted = inverted;
        }

        public override string ToString()
        {
            return string.Format("[{0}]{1}", this.chars.ToString(), base.ToString());
        }

        #region ICharRangeTokenItem Members

        public RegularLanguageSet Range
        {
            get { return this.chars; }
        }

        public bool Inverted
        {
            get { return this.inverted; }
        }

        #endregion

        protected override object OnClone()
        {
            CharRangeTokenItem r = new CharRangeTokenItem(this.inverted, this.chars, this.Line, this.Column, this.Position);
            base.CloneData(r);
            return r;
        }
    }
}
