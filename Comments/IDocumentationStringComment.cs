using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Comments
{
    /// <summary>
    /// Defines properties and methods for working with a documentation comment denoted
    /// by a simple string.
    /// </summary>
    public interface IDocumentationStringComment :
        IDocumentationComment
    {
        /// <summary>
        /// Returns/sets the string that represents the <see cref="IDocumentationStringComment"/>.
        /// </summary>
        string Comment { get; set;}
    }
}
