using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal interface ICaptureTokenLiteralStructuralItem :
        ICaptureTokenStructuralItem
    {
        new IControlledCollection<ILiteralTokenItem> Sources { get; }
        string Name { get; }
        object Value { get; }
    }
}
