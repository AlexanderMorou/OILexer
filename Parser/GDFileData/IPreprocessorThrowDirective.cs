using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public interface IPreprocessorThrowDirective :
        IPreprocessorDirective
    {
        /// <summary>
        /// Returns the reference to the error referenced by the throw directive.
        /// </summary>
        IErrorEntry Reference { get; }

        /// <summary>
        /// Returns the arguments associated with the throw directive, all tokens.
        /// </summary>
        IGDToken[] Arguments { get; }
    }
}
