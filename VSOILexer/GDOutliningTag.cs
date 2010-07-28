using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Oilexer.Parser;

namespace Oilexer.VSIntegration
{
    public class GDOutliningTag :
        IOutliningRegionTag
    {

        public GDOutliningTag(IGDRegion region)
        {
            this.Region = region;
        }

        #region IOutliningRegionTag Members

        public object CollapsedForm
        {
            get { return this.Region.CollapseForm; }
        }

        public object CollapsedHintForm
        {
            get { return this.Region.Description; }
        }

        public bool IsDefaultCollapsed
        {
            get { return false; }
        }

        public bool IsImplementation
        {
            get { return true; }
        }

        #endregion

        public IGDRegion Region { get; private set; }
    }
}
