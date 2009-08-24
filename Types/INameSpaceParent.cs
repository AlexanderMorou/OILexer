using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a <see cref="IDeclarationTarget"/>
    /// which can parent an <see cref="INameSpaceDeclaration"/>.
    /// </summary>
    public interface INameSpaceParent :
        ISegmentableDeclarationTarget
    {
        /// <summary>
        /// Returns the number of namespaces declared in the <see cref="INameSpaceParent"/>.
        /// </summary>
        /// <param name="includePartials">Whether to include the members declared inside all instances
        /// of the <see cref="INameSpaceParent"/>.</param>
        /// <returns>A <see cref="System.Int32"/> value representing the number of members on the
        /// current instance or all instances based upon <paramref name="includePartials"/>.</returns>
        int GetNameSpaceCount(bool includePartials);
    }
}
