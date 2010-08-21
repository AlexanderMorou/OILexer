using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a declared type.
    /// </summary>
    public interface IDeclaredType:
        IType,
        IDeclaration,
        IAttributeDeclarationTarget
    {
        /// <summary>
        /// Returns/sets the module the <see cref="IDeclaredType"/> is defined in.
        /// </summary>
        IIntermediateModule Module { get; set; }
        /// <summary>
        /// Returns the <see cref="IIntermediateProject"/> the <see cref="IDeclaredType"/> 
        /// belongs to.
        /// </summary>
        IIntermediateProject Project { get; }
        INameSpaceDeclaration GetNamespace();
    }
}
