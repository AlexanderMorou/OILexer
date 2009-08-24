using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{

    /// <summary>
    /// Defines properties and methods for working with a common item for an 
    /// <see cref="IScannableEntry"/>.
    /// </summary>
    public interface IScannableEntryItem
    {
        /// <summary>
        /// Returns the name of the <see cref="IScannableEntryItem"/>, if it was defined.
        /// </summary>
        /// <remarks>Can be null.</remarks>
        string Name { get; set; }
        /// <summary>
        /// Returns the repeat options of the <see cref="IScannableEntryItem"/>
        /// </summary>
        ScannableEntryItemRepeatOptions RepeatOptions { get; set; }
        /// <summary>
        /// Returns the line the <see cref="IItem"/> was declared at.
        /// </summary>
        int Line { get; }
        /// <summary>
        /// Returns the column in the <see cref="Line"/> the <see cref="IItem"/>
        /// was declared at.
        /// </summary>
        int Column { get; }
        /// <summary>
        /// Returns the index in the original stream the <see cref="IItem"/> was declared at.
        /// </summary>
        long Position { get; }
        /// <summary>
        /// Creates a copy of the current <see cref="IScannableEntryItem"/>.
        /// </summary>
        /// <returns>A new <see cref="IScannableEntryItem"/> with the data
        /// members of the current <see cref="IScannableEntryItem"/>.</returns>
        IScannableEntryItem Clone();
    }
}
