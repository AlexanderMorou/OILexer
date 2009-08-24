using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// Defines properties and methods for working with an entry to a grammar description file.
    /// </summary>
    public interface IEntry
    {
        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="IEntry"/> was declared at.
        /// </summary>
        int Column { get; }
        /// <summary>
        /// Returns the line index the <see cref="IEntry"/> was declared at.
        /// </summary>
        int Line { get; }
        /// <summary>
        /// Returns the position the <see cref="IEntry"/> was declared at.
        /// </summary>
        long Position { get; }
        /// <summary>
        /// Returns the file the <see cref="IEntry"/> was declared in.
        /// </summary>
        string FileName { get; }
    }
}
