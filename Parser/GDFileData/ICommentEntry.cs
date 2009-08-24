using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public interface ICommentEntry :
        IEntry
    {
        /// <summary>
        /// Returns the pathExplorationComment the <see cref="ICommentEntry"/> represents.
        /// </summary>
        string Comment { get; }
    }
}
