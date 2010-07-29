using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using Oilexer.Parser;
using Microsoft.VisualStudio.Text.Classification;
using System.Diagnostics;
using System.Threading;

namespace Oilexer.VSIntegration
{
    internal sealed class GDClassifier :
        ITagger<GDTokenTag>,
        IDisposable
    {
        private GDFileBufferedHandler handler;
        private int disposeRelaxations = 0;

        public GDClassifier(GDFileBufferedHandler handler)
        {
            this.handler = handler;
        }

        #region ITagger<GDTokenTag> Members

        public IEnumerable<ITagSpan<GDTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (this.handler == null)
                yield break;
            foreach (var snapshotSpan in spans)
                foreach (var tokenSpan in this.handler.TokensFrom(range: snapshotSpan.Span))
                    yield return tokenSpan;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.disposeRelaxations-- <= 0)
            {
                this.handler.OnClassifierDisposed();
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
