using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData.TokenExpression
{
    /// <summary>
    /// Defines properties and methods for working with a token item that links to another 
    /// <see cref="ITokenEntry"/>.
    /// </summary>
    public interface ITokenReferenceTokenItem :
        ITokenItem
    {
        /// <summary>
        /// Returns the <see cref="ITokenEntry"/> that the <see cref="ITokenReferenceTokenItem"/>
        /// references.
        /// </summary>
        ITokenEntry Reference { get; }
    }
}
