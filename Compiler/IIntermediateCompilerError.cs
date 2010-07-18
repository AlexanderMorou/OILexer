using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace Oilexer.Compiler
{
    /// <summary>
    /// Defines properties and methods for working with an error, or warning, from a 
    /// compile process.
    /// </summary>
    public interface IIntermediateCompilerError
    {
        /// <summary>
        /// Gets or sets the column number where the source of the error occurs.
        /// </summary>
        /// <Returns>
        /// The column number of the source file where the intermediateCompiler encountered the error.
        /// </Returns>
        int Column { get; }
        /// <summary>
        /// Gets or sets the error number.
        /// </summary>
        /// <Returns>
        /// The error number as a string.
        /// </Returns>
        string ErrorNumber { get; }
        /// <summary>
        /// Gets or sets the text of the error message.
        /// </summary>
        /// <Returns>
        /// The text of the error message.
        /// </Returns>
        string ErrorText { get; }
        /// <summary>
        /// Gets or sets the file name of the source file that contains the code which
        /// caused the error.
        /// </summary>
        /// <Returns>
        /// The file name of the source file that contains the code which caused the
        /// error.
        /// </Returns>
        string FileName { get; }
        /// <summary>
        /// Gets or sets a value that indicates whether the error is a warning.
        /// </summary>
        /// <Returns>
        /// true if the error is a warning; otherwise, false.
        /// </Returns>
        bool IsWarning { get; }
        /// <summary>
        /// Gets or sets the line number where the source of the error occurs.
        /// </summary>
        /// <Returns>
        /// The line number of the source file where the intermediateCompiler encountered the error.
        /// </Returns>
        int Line { get; }
    }
}
