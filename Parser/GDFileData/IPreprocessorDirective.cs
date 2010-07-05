using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.ProductionRuleExpression;

namespace Oilexer.Parser.GDFileData
{
    public interface IPreprocessorDirective
    {
        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="IPreprocessorDirective"/> was declared at.
        /// </summary>
        int Column { get; }
        /// <summary>
        /// Returns the line index the <see cref="IPreprocessorDirective"/> was declared at.
        /// </summary>
        int Line { get; }
        /// <summary>
        /// Returns the position the <see cref="IPreprocessorDirective"/> was declared at.
        /// </summary>
        long Position { get; }

        /// <summary>
        /// Returns the type of the preprocessor.
        /// </summary>
        EntryPreprocessorType Type { get; }
        
    }
}
