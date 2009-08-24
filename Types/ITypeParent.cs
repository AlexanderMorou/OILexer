using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a valid parent of a type or
    /// instanciable/static declaration.
    /// </summary>
    public interface ITypeParent :
        IDeclaration
    {
        /// <summary>
        /// Returns the <see cref="IClassTypes"/> which denote the <see cref="IClassType"/> instances
        /// of the <see cref="ITypeParent"/>.
        /// </summary>
        IClassTypes Classes { get; }
        /// <summary>
        /// Returns the <see cref="IDelegateTypes"/> which denote the <see cref="IDelegateType"/> instances
        /// of the <see cref="ITypeParent"/>.
        /// </summary>
        IDelegateTypes Delegates { get; }
        /// <summary>
        /// Returns the <see cref="IEnumeratorTypes"/> which denote the <see cref="IEnumeratorType"/>
        /// instances of the <see cref="ITypeParent"/>.
        /// </summary>
        IEnumeratorTypes Enumerators { get; }
        /// <summary>
        /// Returns the <see cref="IInterfaceTypes"/> which denote the <see cref="IInterfaceType"/>
        /// instances of the <see cref="ITypeParent"/>.
        /// </summary>
        IInterfaceTypes Interfaces { get; }
        /// <summary>
        /// Returns the <see cref="IStructTypes"/> which denote the <see cref="IStructType"/>
        /// instances of the <see cref="ITypeParent"/>.
        /// </summary>
        IStructTypes Structures { get; }
        int GetTypeCount();
    }
}
