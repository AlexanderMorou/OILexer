using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with an <see cref="IClassType"/>'s 
    /// partial instances.
    /// </summary>
    public interface IClassPartials :
        ISegmentableDeclaredTypePartials<IClassType, CodeTypeDeclaration>
    {
    }
}
