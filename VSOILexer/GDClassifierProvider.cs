using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text;
namespace Oilexer.VSIntegration
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("oilexer")]
    [TagType(typeof(GDTokenTag))]
    internal sealed class GDClassifierProvider :
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
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var singleton = buffer.Properties.GetOrCreateSingletonProperty<GDFileBufferedHandler>(() => new GDFileBufferedHandler(buffer, documentFactory, bufferFactory, ClassificationTypeRegistry, contentTypeRegistry));
            return singleton.CreateClassifier() as ITagger<T>;
        }

    }
}
