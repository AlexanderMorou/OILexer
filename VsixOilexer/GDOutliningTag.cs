﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
{
    public class GDOutliningTag :
        IOutliningRegionTag
    {

        public GDOutliningTag(IOilexerGrammarRegion region)
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

        public IOilexerGrammarRegion Region { get; private set; }
    }
}
