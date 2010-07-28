using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser;
using Microsoft.VisualStudio.Text;
using Oilexer.Parser.GDFileData;
using System.Diagnostics;

namespace Oilexer.VSIntegration
{
    internal class GDBufferedSimpleParser :
        GDParser
    {
        private GDFileBufferedHandler handler;

        internal GDBufferedSimpleParser(GDFileBufferedHandler handler)
            : base(false, true)
        {
        }

    }
}
