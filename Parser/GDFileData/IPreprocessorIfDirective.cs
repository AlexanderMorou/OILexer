using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    /// <summary>
    /// Defines properties and methods for working with a preprocessor if directive.
    /// </summary>
    public interface IPreprocessorIfDirective :
        IPreprocessorDirective
    {
        /// <summary>
        /// Returns the transitionFirst if directive in the series.
        /// </summary>
        IPreprocessorIfDirective First { get; }
        /// <summary>
        /// Returns the last if directive in the series.
        /// </summary>
        /// <remarks>If the <see cref="Last"/>'s <see cref="Condition"/> is null, 
        /// it was defined as an else directive.</remarks>
        IPreprocessorIfDirective Last { get; }
        /// <summary>
        /// Returns the next <see cref="IPreprocessorIfDirective"/> in the list.
        /// </summary>
        IPreprocessorIfDirective Next { get; }
        /// <summary>
        /// Returns hte previous <see cref="IPreprocessorIfDirective"/> in the list.
        /// </summary>
        IPreprocessorIfDirective Previous { get; }
        /// <summary>
        /// Returns the <see cref="IPreprocessorCLogicalOrConditionExp"/> which relates
        /// to the condition of the <see cref="IPreprocessorIfDirective"/>.
        /// </summary>
        IPreprocessorCLogicalOrConditionExp Condition { get; }
        /// <summary>
        /// Returns the <see cref="IPreprocessorDirectives"/> used as the body of the <see cref="IPreprocessofIfDirective"/>.
        /// </summary>
        IPreprocessorDirectives Body { get; }
    }
}
