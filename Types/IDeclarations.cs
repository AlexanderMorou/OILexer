using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a series of <see cref="IDeclaration"/> elements.
    /// </summary>
    public interface IDeclarations :
        IControlledStateDictionary,
        ITypeReferenceable
    {
        event EventHandler<EventDeclarationArgs<IDeclaration>> ItemAdded;
        event EventHandler<EventDeclarationArgs<IDeclaration>> ItemRemoved;
        /// <summary>
        /// Removes an instance by its uniqueidentifier.
        /// </summary>
        /// <param name="uniqueIdentifier">The unique identifier that relates to the
        /// key of the <see cref="IDeclaration"/> element in the <see cref="IDeclarations"/> 
        /// dictionary.</param>
        void Remove(string uniqueIdentifier);
        /// <summary>
        /// Removes all elements in the <see cref="IDeclarations"/> dictionary.
        /// </summary>
        void Clear();
    }
}
