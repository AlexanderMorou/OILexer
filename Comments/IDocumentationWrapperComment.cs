using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Comments
{
    /// <summary>
    /// Defines properties and methods for woring with a comment which wraps the comments
    /// of other members.
    /// </summary>
    public interface IDocumentationWrapperComment :
        IDocumentationComment
    {
        /// <summary>
        /// Returns the tag that wraps the individual elements.
        /// </summary>
        string WrappedElementTag { get; }
        /// <summary>
        /// Returns whether the tag that wraps the individual elements self-terminates
        /// or terminates with &lt;/<see cref="WrappedElementTag"/>&gt;.
        /// </summary>
        bool WrapperSelfTerminates { get; }
    }
}
