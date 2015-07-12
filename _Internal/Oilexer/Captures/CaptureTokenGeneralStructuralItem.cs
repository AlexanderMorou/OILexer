using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal class CaptureTokenGeneralStructuralItem :
        CaptureTokenStructuralItem,
        ICaptureTokenGeneralStructuralItem
    {
        private ControlledCollection<ITokenSource> sources;
        public CaptureTokenGeneralStructuralItem(IEnumerable<ITokenSource> items)
        {
            this.sources = new ControlledCollection<ITokenSource>(items.ToArray());
        }

        protected override IControlledCollection<ITokenSource> OnGetSources()
        {
            return this.sources;
        }

        protected override ICaptureTokenStructuralItem PerformUnion(ICaptureTokenStructuralItem rightElement)
        {
            return new CaptureTokenGeneralStructuralItem(this.sources.Concat(rightElement.Sources).ToArray());
        }
    }
}
