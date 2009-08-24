using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Types
{
    public interface ISegmentableDeclarationTargetPartials :
        IControlledStateCollection,
        IDisposable
    {
        /// <summary>
        /// Returns the <see cref="ISegmentableDeclarationTarget"/> for the current <see cref="ISegmentableDeclarationTargetPartials"/>.
        /// </summary>
        ISegmentableDeclarationTarget RootDeclaration { get; }
        /// <summary>
        /// Inserts a new partial of the <see cref="RootDeclaration"/> into the 
        /// <see cref="ISegmentableDeclarationTargetPartials"/>
        /// </summary>
        /// <returns>A new instance of <see cref="ISegmentableDeclarationTarget"/> as a partial
        /// of <see cref="RootDeclaration"/>.</returns>
        ISegmentableDeclarationTarget AddNew();
        /// <summary>
        /// Inserts a new partial of the <see cref="RootDeclaration"/> into the 
        /// <see cref="ISegmentableDeclarationTargetPartials"/>, defining the partial
        /// location.  The location must be either the same parent or a partial of the parent
        /// of the initial instance.
        /// </summary>
        /// <returns>A new instance of <see cref="ISegmentableDeclarationTarget"/> as a partial
        /// of <see cref="RootDeclaration"/>.</returns>
        /// <param name="parentTarget">The point at which the instance will be placed.</param>
        ISegmentableDeclarationTarget AddNew(IDeclarationTarget parentTarget);
        /// <summary>
        /// Removes the given partial.
        /// </summary>
        /// <param name="partial">The <see cref="ISegmentableDeclarationTarget"/> to remove.</param>
        void Remove(ISegmentableDeclarationTarget partial);
    }
}
