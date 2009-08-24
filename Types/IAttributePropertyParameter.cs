using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a named constructor argument on an
    /// attribute declaration of a type/member.
    /// </summary>
    public interface IAttributePropertyParameter :
        IAttributeConstructorParameter
    {
        /// <summary>
        /// Returns/sets the name pertinent to the property parameter.
        /// </summary>
        string Name { get; set; }
    }
}
