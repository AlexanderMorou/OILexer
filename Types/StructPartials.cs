using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    [Serializable]
    internal class StructPartials :
        SegmentableDeclaredTypePartials<IStructType, CodeTypeDeclaration>,
        IStructPartials
    {
        public StructPartials(IStructType rootDeclaration)
            :base(rootDeclaration)
        {
        }

        protected override IStructType GetNewPartial(ITypeParent parent)
        {
            return new StructType(this.RootDeclaration, parent);
        }
    }
}
