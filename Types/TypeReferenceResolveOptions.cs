using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types
{
    [Flags]
    public enum TypeReferenceResolveOptions
    {
        UseGeneratorOptions = 0,
        /// <summary>
        /// Identifies that no matter what the type should yield a full path back to the 
        /// defining type.
        /// </summary>
        FullType=1,
        GlobalType=2,
        TypeParameter=4
    }
}
