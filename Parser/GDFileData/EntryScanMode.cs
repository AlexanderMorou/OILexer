using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public enum EntryScanMode
    {
        /// <summary>
        /// The rule/token is allowed to span the same as the calling rule.
        /// </summary>
        Inherited,
        /// <summary>
        /// The rule/token is allowed to span multiple lines.
        /// </summary>
        Multiline,
        /// <summary>
        /// The rule/token is allowed to span a single line.
        /// </summary>
        SingleLine
    }
}
