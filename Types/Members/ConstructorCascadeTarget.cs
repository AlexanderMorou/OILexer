using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// The target of a series of constructor cascade arguments.
    /// </summary>
    [Serializable]
    public enum ConstructorCascadeTarget
    {
        /// <summary>
        /// The constructor cascade isn't used.
        /// </summary>
        Undefined,
        /// <summary>
        /// The constructor cascade should occur on the base-type.
        /// </summary>
        Base,
        /// <summary>
        /// The constructor cascade should occur on the current type.
        /// </summary>
        This
    }
}
