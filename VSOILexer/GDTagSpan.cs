using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using Oilexer.Parser;
using Microsoft.VisualStudio.Text.Classification;

namespace Oilexer.VSIntegration
{
    public class GDTagSpan :
        TagSpan<GDTokenTag>
    {
        public GDTagSpan(SnapshotSpan span, IGDToken token, IClassificationType classificationType)
            : base(span, new GDTokenTag(token, classificationType))
        {
        }

    }
}
