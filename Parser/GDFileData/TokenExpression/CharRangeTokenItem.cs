using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Oilexer._Internal;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    public class CharRangeTokenItem :
        TokenItem,
        ICharRangeTokenItem
    {
        private BitArray chars;
        private bool inverted;

        public CharRangeTokenItem(bool inverted, BitArray chars, int line, int column, long position)
            : base(line, column, position)
        {
            this.inverted = inverted;
            this.chars = chars;
            //chars[0] = false;
        }

        public override string ToString()
        {
            return string.Format("[{0}]{1}", ProjectConstructor.BitArrayToString(this.chars, this.inverted), base.ToString());
        }

        #region ICharRangeTokenItem Members

        public BitArray Range
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
