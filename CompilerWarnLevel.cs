using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Descriptions sourced from C# Compiler options in the MSDN library.
    /// <seealso cref="http://msdn.microsoft.com/en-us/library/13b90fz7.aspx"/></remarks>
    public enum CompilerWarnLevel
    {
        /// <summary>
        /// No warning levels are emitted.
        /// </summary>
        None,
        /// <summary>
        /// Displays severe warning messages.
        /// </summary>
        Level1,
        /// <summary>
        /// Displays level one warnings, and certain less-severe warnings, such as warnings about hiding class members.
        /// </summary>
        Level2,
        /// <summary>
        /// Displays level two warnings, and certain less-severe warnings, such as warnings about expressions
        /// that always evaluate to true or false.
        /// </summary>
        Level3,
        /// <summary>
        /// Displays level three warnings, and all informational warnings.
        /// </summary>
        Level4
    }
}
