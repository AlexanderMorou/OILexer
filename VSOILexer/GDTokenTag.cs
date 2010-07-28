using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Oilexer.Parser;
using Microsoft.VisualStudio.Text.Classification;

namespace Oilexer.VSIntegration
{
    public class GDTokenTag :
        ClassificationTag
    {
        public IGDToken Token { get; private set; }
        public GDTokenTag(IGDToken token, IClassificationType classificationType)
            : base(classificationType)
        {
            this.Token = token;
        }
    }
}
