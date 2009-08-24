using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a single instance of a <see cref="INameSpaceDeclaration"/>.
    /// </summary>
    public interface INameSpaceDeclarationPartials :
        ISegmentableDeclarationTargetPartials<INameSpaceDeclaration>
    {
        INameSpaceDeclaration AddNew(INameSpaceParent parentTarget);
    }
}
