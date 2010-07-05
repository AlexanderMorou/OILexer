using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public interface IPreprocessorDefineSymbolDirective :
        IPreprocessorDirective
    {
        string SymbolName { get; }

        string Value { get; }
    }
}
