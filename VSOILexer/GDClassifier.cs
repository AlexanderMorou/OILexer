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
        private Dictionary<GDTokenType, IClassificationType> classificationTypes;

        public GDClassifier(GDFileBufferedHandler handler)
        {
            this.handler = handler;
            this.classificationTypes = new Dictionary<GDTokenType, IClassificationType>();
            this.classificationTypes.Add(GDTokenType.CharacterLiteral, this.handler.ClassificationRegistry.GetClassificationType("OILexer: Literal: Character"));
            this.classificationTypes.Add(GDTokenType.CharacterRange, this.handler.ClassificationRegistry.GetClassificationType("OILexer: Character Range"));
            this.classificationTypes.Add(GDTokenType.Comment, this.handler.ClassificationRegistry.GetClassificationType("OILexer: Comment"));
            this.classificationTypes.Add(GDTokenType.Identifier, this.handler.ClassificationRegistry.GetClassificationType("OILexer: Identifier"));
            this.classificationTypes.Add(GDTokenType.TokenReference, this.handler.ClassificationRegistry.GetClassificationType("OILexer: Token Reference"));
            this.classificationTypes.Add(GDTokenType.RuleReference, this.handler.ClassificationRegistry.GetClassificationType("OILexer: Rule Reference"));
            this.classificationTypes.Add(GDTokenType.NumberLiteral, this.handler.ClassificationRegistry.GetClassificationType("OILexer: Literal: Number"));
            this.classificationTypes.Add(GDTokenType.Operator, this.handler.ClassificationRegistry.GetClassificationType("OILexer: Operator"));
            this.classificationTypes.Add(GDTokenType.PreprocessorDirective, this.handler.ClassificationRegistry.GetClassificationType("OILexer: Preprocessor Directive"));
            this.classificationTypes.Add(GDTokenType.StringLiteral, this.handler.ClassificationRegistry.GetClassificationType("OILexer: Literal: String"));
            this.classificationTypes.Add(GDTokenType.Whitespace, this.handler.ClassificationRegistry.GetClassificationType("OILexer: Whitespace"));
            this.classificationTypes.Add(GDTokenType.Keyword, this.handler.ClassificationRegistry.GetClassificationType("OILexer: Preprocessor Directive"));
            this.classificationTypes.Add(GDTokenType.Error, this.handler.ClassificationRegistry.GetClassificationType("OILexer: Error"));
            this.classificationTypes.Add(GDTokenType.SoftReference, this.classificationTypes[GDTokenType.Identifier]);
        }

        #region ITagger<GDTokenTag> Members

        public IEnumerable<ITagSpan<GDTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (this.handler == null)
                yield break;
            foreach (var snapshotSpan in spans)
                foreach (var tokenSpan in this.handler.TokensFrom(range: snapshotSpan.Span))
                {
                    if (spans.All(p => p.End < tokenSpan.Span.Start))
                        break;
                    yield return tokenSpan;
                }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.disposeRelaxations-- <= 0)
            {
                this.TagsChanged = null;
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
