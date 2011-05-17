using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text.Classification;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
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
