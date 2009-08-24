using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// The repeat options for the <see cref="IScannableEntryItem"/>.
    /// </summary>
    [FlagsAttribute]
    public enum ScannableEntryItemRepeatOptions
    {
        /// <summary>
        /// The <see cref="IScannableEntryItem"/> does not have any repeat options declared on it.
        /// </summary>
        None = 0,
        /// <summary>
        /// The <see cref="IScannableEntryItem"/> should be encountered zero or one times (optional).
        /// </summary>
        ZeroOrOne = 0x31,
        /// <summary>
        /// The <see cref="IScannableEntryItem"/> should be encountered zero or more times.
        /// </summary>
        ZeroOrMore = 0x52,
        /// <summary>
        /// The <see cref="IScannableEntryItem"/> should be encountered at least once, perhaps more.
        /// </summary>
        OneOrMore = 0x64,
        /// <summary>
        /// The elements of the <see cref="IScannableEntryItem"/> should be encountered 
        /// in any order.
        /// </summary>
        /// <remarks>Only works on groupings, <see cref="ZeroOrMore"/>, <see cref="ZeroOrOne"/>
        /// and <see cref="OneOrMore"/> indicate repeat frequency of all items.
        /// <para><see cref="ZeroOrMore"/> - All items may be encountered
        /// at least once, but they don't have to be.</para>
        /// <para><see cref="ZeroOrOne"/> - All items may be encountered
        /// exactly once, but they don't have to be encountered at all.</para>
        /// <para><see cref="OneOrMore"/> - All items must be encountered
        /// at least once, perhaps more.</para>
        /// <para><see cref="None"/> - All items must be encountered
        /// at least once.</para></remarks>
        AnyOrder = 0x8,
    }
}
