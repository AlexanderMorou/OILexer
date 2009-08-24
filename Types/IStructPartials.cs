using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    public interface IStructPartials :
        ISegmentableDeclaredTypePartials<IStructType, CodeTypeDeclaration>
    {
    }
}
