using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for an <see cref="IMember"/> which
    /// can alter the member table of the <see cref="IMemberParentType"/>.
    /// </summary>
    public interface ICodeBodyTableMember :
        IExportTableMember
    {
        /// <summary>
        /// Returns/sets whether the <see cref="ICodeBodyMethodMember"/> overrides the base
        /// definition.
        /// </summary>
        bool Overrides { get; set; }
        /// <summary>
        /// Returns/sets whether the <see cref="ICodeBodyMethodMember"/> is the final version
        /// of that member.  If previous versions were <see cref="IsVirtual"/>, this will seal
        /// the override chain.
        /// </summary>
        bool IsFinal { get; set; }
        /// <summary>
        /// Returns/sets whether the <see cref="ICodeBodyMethodMember"/> member can be overridden.
        /// </summary>
        bool IsVirtual { get; set; }
        /// <summary>
        /// Returns/sets whether the <see cref="ICodeBodyMethodMember"/> member has to be overridden.
        /// </summary>
        bool IsAbstract { get; set; }
    }
}
