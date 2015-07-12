using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllenCopeland.Abstraction.Slf._Internal.Oilexer.Captures
{
    internal interface ICaptureTokenGroupStructuralItem :
        ICaptureTokenStructuralItem,
        ICaptureTokenStructure
    {
        new ICaptureTokenGroupStructuralItem Or(ICaptureTokenStructuralItem item);

        new ICaptureTokenGroupStructuralItem Concat(ICaptureTokenStructuralItem item);
    }
}
