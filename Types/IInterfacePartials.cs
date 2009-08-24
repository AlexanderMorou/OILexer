using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a partials collection for an
    /// <see cref="IInterfaceType"/>.
    /// </summary>
    public interface IInterfacePartials :
        ISegmentableDeclaredTypePartials<IInterfaceType, CodeTypeDeclaration>
    {
    }
}
