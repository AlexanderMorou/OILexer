using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    public interface IResourceable
    {
        /// <summary>
        /// Returns the resources for the <see cref="IResourceable"/>.
        /// </summary>
        IDeclarationResources Resources { get; }
    }
}
