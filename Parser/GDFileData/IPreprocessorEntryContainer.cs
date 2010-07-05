using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// Defines properties and methods for working with an entry contained
    /// within a preprocessor whose inclusion is determined upon evaluating
    /// the condition of the preprocessor in which it resides.
    /// </summary>
    public interface IPreprocessorEntryContainer :
        IPreprocessorDirective
    {
        /// <summary>
        /// Returns the <see cref="IEntry"/> contained by the entry
        /// container.
        /// </summary>
        IEntry Contained { get; }
    }
}
