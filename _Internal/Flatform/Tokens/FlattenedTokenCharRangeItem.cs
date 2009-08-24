using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Utilities.Collections;
using Oilexer._Internal.Flatform.Tokens.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens
{
    internal class FlattenedTokenCharRangeItem :
        IFlattenedTokenItem
    {
        private RegularLanguageState state = null;
        private RegularLanguageBitArray initialRange = null;

        public FlattenedTokenCharRangeItem(ICharRangeTokenItem source)
        {
            this.Source = source;
        }

        #region IFlattenedTokenItem Members

        public ITokenItem Source { get; private set; }

        public ScannableEntryItemRepeatOptions RepeatOptions
        {
            get { return this.Source.RepeatOptions; }
        }

        public RegularLanguageState GetState()
        {
            if (this.state == null)
            {
                RegularLanguageState result = new RegularLanguageState();
                RegularLanguageState final = new RegularLanguageState();
                result.MoveTo(this.GetTransitionRange(), final);
                final.AddSource(this.Source);
                this.state = result;
            }
            return this.state;
        }

        /// <summary>
        /// Obtains the operating range for transitioning into the
        /// current <see cref="FlattenedTokenCharRangeItem"/>'s
        /// state.
        /// </summary>
        /// <returns></returns>
        public RegularLanguageBitArray GetTransitionRange()
        {
            if (initialRange == null)
                this.InitializeTransitionRange();
            return this.initialRange;
        }

        private void InitializeTransitionRange()
        {
            ICharRangeTokenItem source = (ICharRangeTokenItem)this.Source;
            this.initialRange = new RegularLanguageBitArray(source.Range, source.Inverted);
            this.initialRange.Reduce();
        }

        public bool Optional
        {
            get
            {
                return this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrMore ||
                       this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrOne;
            }
        }

        #endregion

        #region IFlattenedTokenItem Members


        public void Initialize()
        {
            this.InitializeTransitionRange();
        }

        #endregion
    }
}
