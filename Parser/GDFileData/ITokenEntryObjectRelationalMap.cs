using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public interface ITokenEntryObjectRelationalMap :
        IEntryObjectRelationalMap
    {
        new ITokenEntry Entry { get; }
    }
}
