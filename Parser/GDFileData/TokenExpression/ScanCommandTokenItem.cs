using System;
using System.Collections.Generic;
using System.Text;
using Oilexer._Internal;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    public class ScanCommandTokenItem :
        TokenItem,
        IScanCommandTokenItem
    {
        /// <summary>
        /// Data member for <see cref="SearchTarget"/>.
        /// </summary>
        private string searchTarget;
        /// <summary>
        /// Data member for <see cref="SeekPast"/>.
        /// </summary>
        private bool seekPast;

        public ScanCommandTokenItem(string searchTarget, bool seekPast, int column, int line, long position)
            : base(column, line, position)
        {
            this.searchTarget = searchTarget;
            this.seekPast = seekPast;
        }

        #region IScanCommandTokenItem Members

        public string SearchTarget
        {
            get { return this.searchTarget; }
        }

        public bool SeekPast
        {
            get { return this.seekPast; }
        }

        #endregion

        protected override object OnClone()
        {
            ScanCommandTokenItem scti = new ScanCommandTokenItem(searchTarget, seekPast, Column, Line, Position);
            base.CloneData(scti);
            return scti;
        }
        public override string ToString()
        {
            return string.Format("Scan({0}, {1})", this.searchTarget.Encode(), this.seekPast);
        }
    }
}
