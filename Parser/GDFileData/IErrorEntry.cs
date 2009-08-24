using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// Defines properties and methods for working with an error entry which denotes information
    /// about a grammar-defined error and its constant number.
    /// </summary>
    public interface IErrorEntry :
        INamedEntry
    {
        /// <summary>
        /// Returns the message associated with the entry.
        /// </summary>
        string Message { get; }
        /// <summary>
        /// Returns the error number associated with the <see cref="IErrorEntry"/>.
        /// </summary>
        int Number { get; }
    }
}
