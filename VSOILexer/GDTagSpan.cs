using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
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
