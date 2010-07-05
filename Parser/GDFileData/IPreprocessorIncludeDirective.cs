using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public interface IPreprocessorStringTerminalDirective :
        IPreprocessorDirective
    {
        string Literal { get; }
        StringTerminalKind Kind { get; }
    }
}
