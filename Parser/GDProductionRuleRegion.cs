using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;

namespace Oilexer.Parser
{
    internal class GDProductionRuleRegion :
        IGDRegion
    {
        private IProductionRuleEntry entry;

        public GDProductionRuleRegion(IProductionRuleEntry entry, long bodyStart, long bodyEnd)
        {
            this.entry = entry;
            this.Start = (int)bodyStart;
            this.End = (int)bodyEnd;
        }

        #region IGDRegion Members

        public int Start { get; private set; }

        public int End { get; private set; }

        public string Description
        {
            get { return this.entry.GetBodyString(); }
        }

        public string CollapseForm
        {
            get { return string.Format("..."); }
        }

        #endregion
    }
}
