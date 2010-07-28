using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser
{
    public interface IGDRegion
    {
        /// <summary>
        /// Returns the <see cref="Int32"/> value representing the point at which
        /// the <see cref="IGDRegion"/> starts.
        /// </summary>
        int Start { get; }
        /// <summary>
        /// Returns the <see cref="Int32"/> value representing the point at which
        /// the <see cref="IGDRegion"/> ends.
        /// </summary>
        int End { get; }
        /// <summary>
        /// The <see cref="String"/> representing the collapsed
        /// tip shown for the region.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The <see cref="String"/> representing the collapsed form
        /// of the region.
        /// </summary>
        string CollapseForm { get; }
    }
}
