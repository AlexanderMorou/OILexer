using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("oilexer")]
    [TagType(typeof(GDOutliningTag))]
    internal sealed class GDOutlinerProvider :
        ITaggerProvider
    {

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var handler = buffer.Properties.GetOrCreateSingletonProperty<GDFileBufferedHandler>(() => new GDFileBufferedHandler(buffer, ClassificationTypeRegistry));
            return handler.CreateOutliner() as ITagger<T>;
        }

    }
}
