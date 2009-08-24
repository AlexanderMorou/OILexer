using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Types.Members
{
    public interface IAutoCommentFragmentMember
    {
        /// <summary>
        /// Returns/sets the comment associated with the <see cref="IAutoCommentFragmentMember"/>.
        /// </summary>
        string DocumentationComment { get; set; }
    }
}
