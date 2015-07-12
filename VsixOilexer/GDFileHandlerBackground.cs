using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration;

namespace  AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
{
    internal static class GDFileHandlerBackground
    {
        private static Dictionary<string, GDFileHandlerBase> activeFiles = new Dictionary<string, GDFileHandlerBase>();

        public static bool HandlerExistsFor(string file)
        {
            return activeFiles.ContainsKey(file);
        }

        public static GDFileBufferedHandler GetPrimaryHandlerFor(string file, ITextBuffer buffer, IClassificationTypeRegistryService classificationTypeRegistry)
        {
            if (HandlerExistsFor(file))
            {
                var handler = activeFiles[file];
                if (handler is GDFileBufferedHandler)
                    return (GDFileBufferedHandler)handler;
                else
                {
                    handler = new GDFileBufferedHandler(buffer, classificationTypeRegistry);
                    
                }
            }
            else
            {
                
            }
        }

        public static GDFileHandlerBase GetSecondaryHandlerFor(string file)
        {
        }
    }
}
