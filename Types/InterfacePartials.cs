using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    [Serializable]
    public class InterfacePartials :
        SegmentableDeclaredTypePartials<IInterfaceType, CodeTypeDeclaration>,
        IInterfacePartials
    {
        public InterfacePartials(IInterfaceType rootDeclaration) 
            : base(rootDeclaration)
        {

        }

        protected override IInterfaceType GetNewPartial(ITypeParent parent)
        {
            return new InterfaceType(this.RootDeclaration, parent);
        }
    }
}
