using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    [Serializable]
    public class ClassPartials :
        SegmentableDeclaredTypePartials<IClassType, CodeTypeDeclaration>,
        IClassPartials
    {
        public ClassPartials(IClassType rootDeclaration)
            : base(rootDeclaration)
        {
        }


        /// <summary>
        /// Obtains a new partial of the <see cref="IClassType"/> relative to 
        /// <see cref="SegmentableDeclaredTypePartials{TItem, TDom}.RootDeclaration"/>.
        /// </summary>
        /// <returns>A new instance of an <see cref="IClassType"/> partial implementation if successful.</returns>
        /// <exception cref="System.InvalidOperationException"><see cref="ClassPartials"/> is in an
        /// invalid state.</exception>
        protected override IClassType GetNewPartial(ITypeParent parent)
        {
            if (base.RootDeclaration is IDeclarationResources)
                return new DeclarationResources((IDeclarationResources)this.RootDeclaration, parent);
            else
                return new ClassType(this.RootDeclaration, parent);
        }
    }
}
