using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;
using Oilexer.Parser.GDFileData;
using System.Collections;
using Oilexer.Utilities.Collections;
using Oilexer._Internal.Flatform.Tokens.StateSystem;
/* * 
 * Oilexer is an open-source project and must be released
 * as per the license associated to the project.
 * */
namespace Oilexer._Internal.Flatform.Tokens
{
    internal class FlattenedTokenLiteralItem :
        IFlattenedTokenItem
    {
        private IControlledStateCollection<RegularLanguageState> tail;
        private RegularLanguageState state;
        private ITokenItem target;
        private RegularLanguageBitArray initialRange;
        /// <summary>
        /// Creates a new <see cref="FlattenedTokenLiteralItem"/>
        /// with the <paramref name="target"/> provided.
        /// </summary>
        /// <param name="target">The <see cref="ITokenItem"/> which is represented
        /// by the current <see cref="FlattenedTokenLiteralItem"/>.</param>
        public FlattenedTokenLiteralItem(ITokenItem target)
        {
            this.target = target;
            if (target is ILiteralReferenceTokenItem)
                this.Source = ((ILiteralReferenceTokenItem)(target)).Literal;
            else
                this.Source = target;
        }


        #region IFlattenedTokenItem Members

        public ITokenItem Source { get; private set; }

        public ScannableEntryItemRepeatOptions RepeatOptions
        {
            get
            {
                if (this.Source == null)
                    return ScannableEntryItemRepeatOptions.None;
                return this.Source.RepeatOptions;
            }
        }

        public RegularLanguageState GetState()
        {
            if (this.state == null)
                InitializeState();
            return this.state;
        }

        /* *
         * Instantiates the literal token item's state.
         * */
        private void InitializeState()
        {
            RegularLanguageState target = new RegularLanguageState();
            ILiteralTokenItem tokenItem = (ILiteralTokenItem)this.Source;
            char[] str = null;
            bool caseSensitive = true;
            if (tokenItem is ILiteralStringTokenItem)
            {
                ILiteralStringTokenItem tokenLiteral = (ILiteralStringTokenItem)(tokenItem);
                str = tokenLiteral.Value.ToCharArray();
                caseSensitive = !tokenLiteral.CaseInsensitive;
            }
            else if (tokenItem is ILiteralCharTokenItem)
            {
                ILiteralCharTokenItem tokenLiteral = (ILiteralCharTokenItem)(tokenItem);
                str = new char[1] { tokenLiteral.Value };
                caseSensitive = !tokenLiteral.CaseInsensitive;
            }
            RegularLanguageState current = target;
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                char? b = null;
                if (!caseSensitive)
                    if (char.IsUpper(ch))
                        b = char.ToLower(ch);
                    else if (char.IsLower(ch))
                        b = char.ToUpper(ch);
                int length = 1;
                if (b != null)
                {
                    int min = Math.Min(ch, b.Value);
                    int max = Math.Max(ch, b.Value);
                    length = max - min + 1;
                }
                BitArray k = new BitArray(length);
                RegularLanguageBitArray cK = null;
                if (b != null)
                {
                    int min = Math.Min(ch, b.Value);
                    int max = Math.Max(ch, b.Value);
                    k[0] = true;
                    k[max - min] = true;
                    cK = new RegularLanguageBitArray(k, min);
                    cK.Reduce();
                }
                else
                {
                    k[0] = true;
                    cK = new RegularLanguageBitArray(k, (int)ch);
                }
                RegularLanguageState nextPoint = new RegularLanguageState();
                current.MoveTo(cK, nextPoint);
                current = nextPoint;
            }
            current.AddSource(this.Source);
            this.tail = new ControlledStateCollection<RegularLanguageState>(new RegularLanguageState[1] { current });
            this.state = target;
        }

        public RegularLanguageBitArray GetTransitionRange()
        {
            if (this.initialRange == null)
                this.initialRange = InitializeInitialRange();
            return this.initialRange;
        }

        private RegularLanguageBitArray InitializeInitialRange()
        {
            char ch = char.MinValue;
            bool caseSensitive = true;
            ILiteralTokenItem tokenItem = (ILiteralTokenItem)this.Source;
            if (tokenItem is ILiteralStringTokenItem)
            {
                ILiteralStringTokenItem tokenLiteral = (ILiteralStringTokenItem)(tokenItem);
                ch = tokenLiteral.Value[0];
                caseSensitive = !tokenLiteral.CaseInsensitive;
            }
            else if (tokenItem is ILiteralCharTokenItem)
            {
                ILiteralCharTokenItem tokenLiteral = (ILiteralCharTokenItem)(tokenItem);
                ch = tokenLiteral.Value;
                caseSensitive = !tokenLiteral.CaseInsensitive;
            }
            char c2 = ch;
            if (!(caseSensitive))
                if (char.IsUpper(ch))
                    c2 = char.ToLower(ch);
                else if (char.IsLower(ch))
                    c2 = char.ToUpper(ch);
            uint min = Math.Min(ch, c2);
            uint max = Math.Max(ch, c2);

            RegularLanguageBitArray result = new RegularLanguageBitArray((int)(max - min + 1));
            result.Offset = min;
            result[min] = true;
            if (min != max)
                result[max] = true;
            result.Reduce();
            return result;
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
            if (this.initialRange == null)
                this.initialRange = this.InitializeInitialRange();
        }

        #endregion
    }
}
