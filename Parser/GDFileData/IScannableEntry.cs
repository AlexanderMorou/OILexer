using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public interface IScannableEntry :
        INamedEntry
    {
        /// <summary>
        /// Returns the means to which the <see cref="IScannableEntry"/> handles new lines.
        /// </summary>
        EntryScanMode ScanMode { get; }
    }
}
