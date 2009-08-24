using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    public interface ISegmentableDeclarationTarget :
        IDeclarationTarget
    {
        /// <summary>
        /// Returns whether the <see cref="ISegmentableDeclarationTarget"/> is
        /// a segment of a series.
        /// </summary>
        bool IsPartial { get; }
        /// <summary>
        /// Returns whether the <see cref="ISegmentableDeclarationTarget"/> is the root
        /// instance.
        /// </summary>
        bool IsRoot { get; }
        /// <summary>
        /// Returns the root <see cref="ISegmentableDeclarationTarget"/> which is responsible for managing
        /// the data elements of the <see cref="ISegmentableDeclarationTarget"/>.
        /// </summary>
        /// <returns>An instance of an <see cref="ISegmentableDeclarationTarget"/> implementation which reflects the root instance of the 
        /// <see cref="ISegmentableDeclarationTarget"/>.</returns>
        ISegmentableDeclarationTarget GetRootDeclaration();
        /// <summary>
        /// Returns the partial elements of the <see cref="ISegmentableDeclaredType"/>.
        /// </summary>
        ISegmentableDeclarationTargetPartials Partials { get; }

    }
}
