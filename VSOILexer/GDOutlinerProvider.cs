using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace Oilexer.VSIntegration
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("oilexer")]
    [TagType(typeof(GDOutliningTag))]
    internal sealed class GDOutlinerProvider :
        ITaggerProvider
    {

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal ITextDocumentFactoryService documentFactory = null;

        [Import]
        internal ITextBufferFactoryService bufferFactory = null;

        [Import]
        internal IContentTypeRegistryService contentTypeRegistry = null;

        #region ITaggerProvider Members

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var handler = buffer.Properties.GetOrCreateSingletonProperty<GDFileBufferedHandler>(() => new GDFileBufferedHandler(buffer, documentFactory, bufferFactory, ClassificationTypeRegistry, contentTypeRegistry));
            
            return handler.CreateOutliner() as ITagger<T>;
        }

        #endregion
    }
}
