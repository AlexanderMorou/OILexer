using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
using Oilexer.Utilities.Collections;
using Oilexer._Internal.Flatform.Tokens.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens
{
    /// <summary>
    /// Common interface for flattened token items.
    /// </summary>
    internal interface IFlattenedTokenItem
    {
        /// <summary>
        /// Returns the <see cref="ItokenItem"/>the <see cref="IFlattenedTokenItem"/> 
        /// is based upon.
        /// </summary>
        ITokenItem Source { get; }
        /// <summary>
        /// Returns the repeat options of the <see cref="IScannableEntryItem"/>
        /// </summary>
        ScannableEntryItemRepeatOptions RepeatOptions { get; }
        /// <summary>
        /// Returns the state for the current <see cref="IFlattenedTokenItem"/>.
        /// </summary>
        /// <returns>A <see cref="RegularLanguageState"/> which represents
        /// the <see cref="IFlattenedTokenItem"/></returns>
        RegularLanguageState GetState();
        /// <summary>
        /// Returns the initial footprint for state transition to this state.
        /// </summary>
        /// <returns>A <see cref="RegularLanguageBitArray"/> denoting the characters
        /// covered required to transition to the 
        /// <see cref="IFlattenedTokenItem"/>.</returns>
        RegularLanguageBitArray GetTransitionRange();
        /// <summary>
        /// Returns whether the <see cref="IFlattenedTokenItem"/>
        /// is optional.
        /// </summary>
        bool Optional { get; }
        /// <summary>
        /// Initializes the <see cref="IFlattenedTokenItem"/>.
        /// </summary>
        void Initialize();
    }
}
