using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public class ScanCommandTokenItem :
        CommandTokenItem,
        IScanCommandTokenItem
    {
        private ITokenExpressionSeries searchTarget;
        /// <summary>
        /// Data member for <see cref="SeekPast"/>.
        /// </summary>
        private bool seekPast;

        public ScanCommandTokenItem(ITokenExpressionSeries searchTarget, bool seekPast, int column, int line, long position)
            : base(new ITokenExpressionSeries[1] { searchTarget }, column, line, position)
        {
            this.seekPast = seekPast;
            this.searchTarget = searchTarget;
        }

        /// <summary>
        /// Returns the <see cref="CommandType"/> associated to the command.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="CommandType.ScanCommand"/>.
        /// </remarks>
        public override CommandType Type
        {
            get { return CommandType.ScanCommand; }
        }

        #region IScanCommandTokenItem Members

        public ITokenExpressionSeries SearchTarget
        {
            get { return this.searchTarget; }
        }

        public bool SeekPast
        {
            get { return this.seekPast; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Scan({0}, {1})", this.searchTarget, this.seekPast);
        }

        protected override object OnClone()
        {
            ScanCommandTokenItem scti = new ScanCommandTokenItem(this.searchTarget, this.seekPast, Column, Line, Position);
            base.CloneData(scti);
            return scti;
        }
    }
}
