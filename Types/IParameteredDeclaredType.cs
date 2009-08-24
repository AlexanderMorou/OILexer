using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Oilexer.Utilities.Collections;
using Oilexer.Types.Members;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a parameterized type.  That is,
    /// a type which contains type-parameters, and potential conditions on those parameters
    /// as a restriction on the types supplied.
    /// </summary>
    public interface IParameteredDeclaredType :
        IDeclaredType//,
        //IFauxable<Type>
    {
        /// <summary>
        /// Returns a dictionary of the type parameters for the current <see cref="IParameteredDeclaredType"/>.
        /// </summary>
        ITypeParameterMembers TypeParameters { get; }
    }
}
