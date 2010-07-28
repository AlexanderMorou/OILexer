using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using System.Diagnostics;

namespace Oilexer.VSIntegration
{
    internal sealed class GDOutliner :
        ITagger<GDOutliningTag>,
        IDisposable
    {
        private GDFileBufferedHandler handler;
        private int disposeRelaxations = 0;

        public GDOutliner(GDFileBufferedHandler handler)
        {
            this.handler = handler;
        }

        #region ITagger<GDOutliningTag> Members

        public IEnumerable<ITagSpan<GDOutliningTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var span in spans)
                foreach (var regionTag in this.handler.OutliningSpansFrom(range: span.Span))
                    yield return regionTag;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        #endregion

        internal void MakeTagChanges()
        {
            if (this.TagsChanged != null)
                this.TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(this.handler.Buffer.CurrentSnapshot, new Span(0, this.handler.Buffer.CurrentSnapshot.Length))));
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.disposeRelaxations-- <= 0)
            {
                this.TagsChanged = null;
                this.handler.OnOutlinerDisposed();
                this.handler = null;
            }
        }

        #endregion

        internal void RelaxDispose()
        {
            disposeRelaxations++;
        }
    }
}
