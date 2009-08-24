using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace Oilexer.Parser
{
    /// <summary>
    /// Defines properties and methods for working with the results of a parse operation.
    /// </summary>
    public interface IParserResults
    {
        /// <summary>
        /// Returns the <see cref="CompilerErrorCollection"/> that relates to the errors that
        /// occurred while trying to parse the input.
        /// </summary>
        CompilerErrorCollection Errors { get; }
        /// <summary>
        /// Returns the resulted object from the parse operation.
        /// </summary>
        object Result { get; }
        /// <summary>
        /// Returns whether the operation was successful.
        /// </summary>
        bool Successful { get; }
    }
}
