using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;

namespace Oilexer.Parser
{
    
    internal sealed class GDProductionRuleGroupRegion :
        IGDRegion
    {
        private IProductionRuleGroupItem groupItem;
        public GDProductionRuleGroupRegion(IProductionRuleGroupItem groupItem, long start, long end)
        {
            this.Start = (int)start;
            this.End = (int)end;
            this.groupItem = groupItem;
        }
        #region IGDRegion Members

        public int Start { get; private set; }

        public int End { get; private set; }

        public string Description { get { return this.groupItem.GetBodyString(); } }

        public string CollapseForm
        {
            get { return "..."; }
        }

        #endregion
    }
}
