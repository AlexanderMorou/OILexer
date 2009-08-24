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
    /// <summary>
    /// Provides a base class for working with a group token item
    /// which has been flattened.
    /// </summary>
    internal class FlattenedTokenGroupItem :
        FlattenedTokenExpressionSeries,
        IFlattenedTokenItem
    {
        public FlattenedTokenGroupItem(ITokenGroupItem source)
            : base(source)
        {
            this.Source = source;
        }

        #region IFlattenedTokenItem Members

        public ITokenItem Source { get; private set; }

        public ScannableEntryItemRepeatOptions RepeatOptions
        {
            get { return this.Source.RepeatOptions; }
        }

        public new RegularLanguageState GetState()
        {
            var state = base.GetState();
            state.AddSource(this.Source);
            //this.startState = RegularLanguageState.Merger.ToDFA(state);
            return this.startState;
        }


        public RegularLanguageBitArray GetTransitionRange()
        {
            throw new NotImplementedException();
        }

        public bool Optional
        {
            get
            {
                /* *
                 * If only one expression is optional, in some cases the group
                 * can be considered optional.
                 * */
                bool alreadyOptional = this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrMore ||
                                       this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrOne;
                if (alreadyOptional)
                    return true;
                foreach (var expression in this.Values)
                {
                    bool overallOptional = true;
                    foreach (var item in expression.Values)
                    {
                        overallOptional = item.Optional;
                        if (!overallOptional)
                            break;
                    }
                    if (overallOptional)
                        return true;
                }
                return false;
                //foreach (var expression in this.Keys)
                //{
                //    bool currentIsOptional = true;
                //    foreach (var item in expression)
                //        if (item.RepeatOptions != ScannableEntryItemRepeatOptions.ZeroOrMore &&
                //            item.RepeatOptions != ScannableEntryItemRepeatOptions.ZeroOrOne)
                //            currentIsOptional = this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrMore ||
                //                                this.RepeatOptions == ScannableEntryItemRepeatOptions.ZeroOrOne;
                //    if (currentIsOptional)
                //        return true;
                //}
                //return false;
            }
        }

        public new void Initialize()
        {
            base.Initialize();
        }

        #endregion
    }
}
