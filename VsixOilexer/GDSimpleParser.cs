using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using System.Diagnostics;
using AllenCopeland.Abstraction.Slf.Parsers;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
{
    internal class OilexerBufferedSimpleParser :
        OilexerParser
    {
        private GDFileBufferedHandler handler;

        internal OilexerBufferedSimpleParser(GDFileBufferedHandler handler)
            : base(false, true)
        {
        }

    }
}
