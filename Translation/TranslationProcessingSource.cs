using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Translation
{
    /// <summary>
    /// The type of generator to use.
    /// </summary>
    public enum TranslationProcessingSource
    {
        /// <summary>
        /// Use the code-dom processing source.
        /// </summary>
        CodeDom,
        /// <summary>
        /// Use the Objectified Intermediate Language (OIL) processing source.
        /// </summary>
        OIL,
    }
}
