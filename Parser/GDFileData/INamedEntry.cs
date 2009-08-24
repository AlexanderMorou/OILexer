using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// Defines properties and methods for working with a common-ground named entry in a
    /// grammar description file.
    /// </summary>
    public interface INamedEntry :
        IEntry
    {
        /// <summary>
        /// Returns the name of the <see cref="INamedEntry"/>.
        /// </summary>
        string Name { get;  }
    }
}
