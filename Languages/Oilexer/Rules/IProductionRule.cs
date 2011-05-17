using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules
{
    /// <summary>
    /// Defines properties and methods for working with an expression that defines a part of a
    /// production rule.
    /// </summary>
    public interface IProductionRule :
        IReadOnlyCollection<IProductionRuleItem>,
        IAmbiguousGDEntity,
        IProductionRuleSource
    {
        /// <summary>
        /// Returns the line the <see cref="IItem"/> was declared at.
        /// </summary>
        int Line { get; }
        /// <summary>
        /// Returns the column in the <see cref="Line"/> the <see cref="IItem"/>
        /// was declared at.
        /// </summary>
        int Column { get; }
        /// <summary>
        /// Returns the index in the original stream the <see cref="IItem"/> was declared at.
        /// </summary>
        long Position { get; }
        /// <summary>
        /// Returns the file at which the <see cref="IProductionRule"/> was defined.
        /// </summary>
        string FileName { get; }

        IProductionRule Clone();
    }
}
