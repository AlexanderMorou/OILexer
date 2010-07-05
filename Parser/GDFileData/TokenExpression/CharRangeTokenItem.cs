using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Oilexer.Parser.Builder;
using Oilexer.FiniteAutomata.Tokens;

namespace Oilexer.Parser.GDFileData.TokenExpression
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
