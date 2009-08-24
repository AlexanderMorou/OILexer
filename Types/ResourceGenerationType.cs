using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    public enum ResourceGenerationType
    {
        /// <summary>
        /// The resources for the <see cref="IDeclarationResources"/> are managed through a class.
        /// </summary>
        GeneratedClass,
        /// <summary>
        /// The resources for the <see cref="IDeclarationResources"/> are managed through
        /// a cached field for each item.
        /// </summary>
        GeneratedClassWithCache,
    }
}
