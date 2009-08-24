using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a declaration target.
    /// </summary>
    public interface IDeclarationTarget :
        IDisposable
    {
        /// <summary>
        /// Returns the parent of the current target.
        /// </summary>
        IDeclarationTarget ParentTarget { get; set; }
        /// <summary>
        /// Returns/sets the name of the <see cref="IDeclarationTarget"/>.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Obtains the root namespace which the top-most level type the <see cref="ParentTarget"/> 
        /// may be or points to.
        /// </summary>
        /// <returns>The root <see cref="INameSpaceDeclaration"/> in the declaration target 
        /// hierarchy.</returns>
        INameSpaceDeclaration GetRootNameSpace();
    }
}
