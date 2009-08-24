using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    public interface IInterfaceImplementableType
    {
        /// <summary>
        /// Returns the implementation listing which relate to the series of interfaces
        /// the <see cref="IInterfaceImplementableType"/> implements in code.
        /// </summary>
        ITypeReferenceCollection ImplementsList { get; }
    }
}
