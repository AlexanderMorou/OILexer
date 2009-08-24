using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// Defines properties and methods for working with a series of production rule
    /// template preprocessor items.
    /// </summary>
    public interface IPreprocessorDirectives :
        IReadOnlyCollection<IPreprocessorDirective>
    {
    }
}
