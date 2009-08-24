using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public interface IPreprocessorCExp
    {
        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="IPreprocessorCExp"/> was declared at.
        /// </summary>
        int Column { get; }
        /// <summary>
        /// Returns the line index the <see cref="IPreprocessorCExp"/> was declared at.
        /// </summary>
        int Line { get; }
        /// <summary>
        /// Returns the position the <see cref="IPreprocessorCExp"/> was declared at.
        /// </summary>
        long Position { get; }
    }
}
