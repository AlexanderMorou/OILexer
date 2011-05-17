using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
{
    internal sealed class GDOutlinerTagSpan :
        TagSpan<GDOutliningTag>
    {
        public GDOutlinerTagSpan(SnapshotSpan span, GDOutliningTag tag)
            : base(span, tag)
        {
        }
    }
}
