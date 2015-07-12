using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal interface ICaptureTokenLinkedSetStructuralItem :
        IDictionary<string, ICaptureTokenLiteralStructuralItem>,
        ICaptureTokenStructuralItem
    {
    }
}
