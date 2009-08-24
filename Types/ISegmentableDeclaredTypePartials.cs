using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Types
{
    public interface ISegmentableDeclaredTypePartials :
        ISegmentableDeclarationTargetPartials,
        IDisposable
    {
        /// <summary>
        /// Returns the <see cref="ISegmentableDeclaredType"/> for the current <see cref="ISegmentableDeclaredTypePartials"/>.
        /// </summary>
        new ISegmentableDeclaredType RootDeclaration { get; }
        /// <summary>
        /// Inserts a new partial of the <see cref="RootDeclaration"/> into the 
        /// <see cref="ISegmentableDeclaredTypePartials"/>
        /// </summary>
        /// <returns>A new instance of <see cref="ISegmentableDeclaredType"/> as a partial
        /// of <see cref="RootDeclaration"/>.</returns>
        new ISegmentableDeclaredType AddNew();
        /// <summary>
        /// Inserts a new partial of the <see cref="RootDeclaration"/> into the 
        /// <see cref="ISegmentableDeclaredTypePartials"/>, defining the partial
        /// location.  The location must be either the same parent or a partial of the parent
        /// of the initial instance.
        /// </summary>
        /// <returns>A new instance of <see cref="ISegmentableDeclaredType"/> as a partial
        /// of <see cref="RootDeclaration"/>.</returns>
        /// <param name="parentTarget">The point at which the instance will be placed.</param>
        ISegmentableDeclaredType AddNew(ITypeParent parentTarget);
    }
}
