using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;

namespace Oilexer.Parser.Builder
{
    /// <summary>
    /// Defines properties and methods for the definition of a production rule.
    /// </summary>
    public interface IProductionRuleDefinition
    {
        /// <summary>
        /// Returns the interface of the <see cref="IProductionRuleDefinition"/>.
        /// </summary>
        IInterfaceType InterfaceForm { get; }
        /// <summary>
        /// Returns hte <see cref="IClassType"/> of the <see cref="IProductionRuleDefinition"/>
        /// which stores the data for the specific rule definition.
        /// </summary>
        IClassType ClassForm { get; }
    }
}
