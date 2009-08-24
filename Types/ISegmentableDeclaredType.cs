using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for an <see cref="IDeclaredType"/>
    /// that can be segmented about a series of instances.
    /// </summary>
    public interface ISegmentableDeclaredType :
        IDeclaredType,
        ISegmentableDeclarationTarget
    {
        /// <summary>
        /// Returns the root <see cref="ISegmentableDeclaredType"/> which is responsible for managing
        /// the data elements of the <see cref="ISegmentableDeclaredType"/>.
        /// </summary>
        /// <returns>An instance of an <see cref="ISegmentableDeclaredType"/> implementation which reflects the root instance of the 
        /// <see cref="ISegmentableDeclaredType"/>.</returns>
        new ISegmentableDeclaredType GetRootDeclaration();
        /// <summary>
        /// Returns the partial elements of the <see cref="ISegmentableDeclaredType"/>.
        /// </summary>
        new ISegmentableDeclaredTypePartials Partials { get; }
        /// <summary>
        /// Returns the number of types declared nested in the <see cref="ISegmentableDeclaredType"/>.
        /// </summary>
        /// <param name="includePartials">Whether to include the members declared inside all instances
        /// of the <see cref="ISegmentableDeclaredType"/>.</param>
        /// <returns>A <see cref="System.Int32"/> value representing the number of members on the
        /// current instance or all instances based upon <paramref name="includePartials"/>.</returns>
        int GetTypeCount(bool includePartials);
        int GetMemberCount(bool includePartials);
    }
}
