using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Utilities.Collections;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2015 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    /// <summary>
    /// Provides a series of <see cref="ITokenItem"/> instances in a chain which define a 
    /// <see cref="IOilexerGrammarTokenEntry"/> or a sub-expression in a <see cref="IOilexerGrammarTokenEntry"/>.
    /// </summary>
    public interface ITokenExpression :
        IControlledCollection<ITokenItem>,
        ITokenSource
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
    }
}
