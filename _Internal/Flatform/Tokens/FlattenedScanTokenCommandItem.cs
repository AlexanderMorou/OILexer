using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oilexer.Parser.GDFileData;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Utilities.Collections;
using System.Collections;
using Oilexer._Internal.Flatform.Tokens.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens
{
    internal class FlattenedScanTokenCommandItem :
        IFlattenedTokenItem
    {
        private RegularLanguageState state;
        private RegularLanguageBitArray initArray;
        public FlattenedScanTokenCommandItem(IScanCommandTokenItem source)
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
                RegularLanguageState init = new RegularLanguageState();
                string target = ((IScanCommandTokenItem)(Source)).SearchTarget;
                RegularLanguageState current = init;
                foreach (char item in target)
                {
                    RegularLanguageState nextState = new RegularLanguageState();
                    BitArray targetArray = new BitArray(1);
                    BitArray inverseTarget = new BitArray(1);
                    inverseTarget[0] = true;
                    targetArray[0] = true;
                    current.MoveTo(new RegularLanguageBitArray(targetArray, (int)item), nextState);
                    var invSet = new RegularLanguageBitArray(inverseTarget, (int)item);
                    invSet.IsInverted = true;
                    current.MoveTo(invSet, init);

                    current = nextState;
                }
                this.state = init;
            }
            return this.state;
        }

        public RegularLanguageBitArray GetTransitionRange()
        {
            if (this.initArray == null)
#if OLD_REGULAR_LANGUAGE_STATE
                initArray = this.GetState().OutTransitions.First().Key;
#else
                initArray = this.GetState().OutTransitions.First().Check;
#endif
            return this.initArray;
        }

        public bool Optional
        {
            get
            {
                return this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrMore ||
                       this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrOne;
            }
        }

        public void Initialize()
        {
            //this.GetTransitionRange();
        }

        #endregion
    }
}
