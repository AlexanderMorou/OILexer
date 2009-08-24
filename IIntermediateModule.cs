using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.Reflection;
using System.ComponentModel;

namespace Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with an intermediate module.
    /// </summary>
    public interface IIntermediateModule
    {
        /// <summary>
        /// Returns/sets the name of the <see cref="IIntermediateModule"/>.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Returns an enumerable instance which contains the <see cref="IDeclaredType"/> instance
        /// implementations which relate to the current <see cref="IIntermediateModule"/>.
        /// </summary>
        IEnumerable<IDeclaredType> DeclaredTypes { get; }
        /// <summary>
        /// Returns the <see cref="IIntermediateProject"/> the 
        /// <see cref="IIntermediateModule"/> is assocaited to.
        /// </summary>
        IIntermediateProject Project { get; }

    }
}
