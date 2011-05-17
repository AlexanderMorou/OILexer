using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using AllenCopeland.Abstraction.Slf.Compilers.Oilexer;
using System.Globalization;
using AllenCopeland.Abstraction.Slf._Internal.Oilexer;
using System.Threading.Tasks;
#if x64
using SlotType = System.UInt64;
#elif x86
using SlotType = System.UInt32;
#endif
using AllenCopeland.Abstraction.Slf.FiniteAutomata;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens
{
    public partial class RegularLanguageSet :
        FiniteAutomataBitSet<RegularLanguageSet>,
        IEquatable<RegularLanguageSet>
    {
        private string stringForm;
        public static readonly RegularLanguageSet CompleteSet = BuildCompleteSet();

        private static RegularLanguageSet BuildCompleteSet()
        {
            var result = new RegularLanguageSet();
            result.Set(null, 0, 0, FullSetLength, true);
            return result;
        }
        private const int FullSetLength = char.MaxValue + 1;
        public RegularLanguageSet()
        {
        }

        internal static RegularLanguageSet GetRegularLanguageSet(bool isNegativeSet, char[] singleTons, Tuple<char, char>[] ranges, UnicodeCategory[] categories)
        {
            int max = 0;
            var result = new RegularLanguageSet();
            for (int i = 0; i < singleTons.Length; i++)
            {
                char singleTon = singleTons[i];
                if (singleTons[i] > max)
                    max = singleTon;
            }
            for (int i = 0; i < ranges.Length; i++)
            {
                var range = ranges[i];
                if (range.Item2 > max)
                    max = range.Item2;
            }

            max = (int)Math.Ceiling(((double)(max + 1)) / (double)SlotBitCount);
            SlotType[] dataSet = new SlotType[max];
            for (int i = 0; i < singleTons.Length; i++)
            {
                char singleTon = singleTons[i];
                dataSet[(SlotType)singleTon / SlotBitCount] |= (SlotType)(ShiftValue << (int)(singleTon % SlotBitCount));
            }

            for (int i = 0; i < ranges.Length; i++)
            {
                var range = ranges[i];
                for (char j = range.Item1; j <= range.Item2; j++)
                    dataSet[(SlotType)j / SlotBitCount] |= (SlotType)(ShiftValue << (int)(j % SlotBitCount));
            }
            result.Set(dataSet, 0, (uint)(max + 1), FullSetLength, false);
            for (int i = 0; i < categories.Length; i++)
            {
                var category = categories[i];
                result |= ParserCompilerExtensions.unicodeCategoryData[category];
            }
            result.IsNegativeSet = isNegativeSet;
            return result;
        }

        protected override RegularLanguageSet GetCheck()
        {
            /* *
             * For expediency reasons: new TCheck()
             * yields a call to Activator.CreateInstance<T>()
             * which uses reflection.
             * */
            return new RegularLanguageSet();
        }

        public RegularLanguageSet(char c)
        {
            SlotType[] values = new SlotType[] { 1 };
            base.Set(values, (uint)c, 1, FullSetLength, false);
        }

        public RegularLanguageSet(string characters)
            : this(true, characters) { }

        public RegularLanguageSet(params char[] characters)
            : this(true, characters) { }

        public RegularLanguageSet(bool caseSensitive, string characters)
            : this(caseSensitive, characters.ToCharArray())
        {
        }

        public RegularLanguageSet(UnicodeCategory category, UnicodeCategory[] characterData, int maxPoint)
        {
            int max = (int)Math.Ceiling(((double)(maxPoint + 1)) / (double)SlotBitCount);
            SlotType[] dataSet = new SlotType[max];
            Parallel.For(0, maxPoint + 1, j =>
                {
                    SlotType i = (SlotType)j;
                    if (characterData[i] == category)
                        lock (dataSet)
                        {
                            dataSet[i / SlotBitCount] |= (SlotType)(ShiftValue << (int)(i % SlotBitCount));
                        }
                });
            base.Set(dataSet, 0, (uint)(maxPoint + 1), FullSetLength, false, true);
        }

        public RegularLanguageSet(bool caseSensitive, params char[] characters)
        {
            int length = 0;
            
            for (int i = 0; i < characters.Length; i++)
            {
                char c = characters[i];
                if (!caseSensitive)
                {
                    char maxC = (char)Math.Max(c, char.IsLower(c) ? char.ToUpper(c) : char.IsUpper(c) ? char.ToLower(c) : c);
                    if (maxC > length)
                        length = maxC;
                }
                else if (c > length)
                    length = c;
            }
            BitArray set = new BitArray(length + 1);
            for (int i = 0; i < characters.Length; i++)
            {
                char c = characters[i];
                set[c] = true;
                if (!caseSensitive)
                {
                    bool isU = char.IsUpper(c),
                         isL = char.IsLower(c);

                    if (isU || isL)
                        set[isU ? char.ToLower(c) : char.ToUpper(c)] = true;
                }
            }
            this.Set(set, false);
        }


        private unsafe void Set(BitArray characters, bool inverted)
        {
            SlotType[] values2 = characters.ObtainFiniteSeries(FullSetLength);
            base.Set(values2, 0, (uint)characters.Length, FullSetLength, inverted);
        }
        /// <summary>
        /// Determines whether the <paramref name="character"/>
        /// provided exists within the <see cref="RegularLanguageSet"/>.
        /// </summary>
        /// <param name="character">The <see cref="Char"/> to check for.</param>
        /// <returns>true if the <see cref="RegularLanguageSet"/> covers the
        /// <paramref name="character"/> provided; false, otherwise.</returns>
        public bool Contains(char character)
        {
            if ((!(base.IsNegativeSet)) && 
                (character <  this.Offset ||
                 character > (this.Offset + this.Length)))
                return false;
            return this[(uint)character];
        }

        public override string ToString()
        {
            if (stringForm == null)
            {
                var thisRef = this;
                UnicodeCategory[] categories;
                ParserCompilerExtensions.PropagateUnicodeCategories(ref thisRef, out categories);
                if (categories != null && categories.Length > 0)
                {
                    var thisRefString = GetSetString(thisRef, categories);
                    var thisString = GetSetString(this);
                    if (thisRefString.Length > thisString.Length)
                        stringForm = thisString;
                    else
                        stringForm = thisRefString;
                }
                else
                    stringForm = GetSetString(this);
            }
            return stringForm;
        }

        private static string GetSetString(RegularLanguageSet set, UnicodeCategory[] categories = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            if (set.IsNegativeSet)
                sb.Append('^');
            var ranges = set.GetRange();

            foreach (var element in ranges)
            {
                switch (element.Which)
                {
                    case SwitchPairElement.A:
                        sb.Append(EncodeChar(element.A.Value));
                        break;
                    case SwitchPairElement.B:
                        sb.AppendFormat("{0}-{1}", EncodeChar(element.B.Value.Start), EncodeChar(element.B.Value.End));
                        break;
                    default:
                        break;
                }
            }
            if (categories != null)
                foreach (var category in categories)
                    sb.Append(ParserCompilerExtensions.GetUnicodeCategoryString(category));
            sb.Append("]");
            return sb.ToString();
        }
        
        private static string EncodeChar(char c)
        {
            switch (c)
            {
                case '\t':
                    return @"\t";
                case '\r':
                    return @"\r";
                case '\n':
                    return @"\n";
                case '\x85':
                    return @"\x85";
                case '\0':
                    return @"\0";
                case '\v':
                    return @"\v";
                default:
                    if (c > 255)
                    {
                        var baseHexVal = string.Format("{0:x}", (int)c);
                        while (baseHexVal.Length < 4)
                            baseHexVal = "0" + baseHexVal;
                        return string.Format(@"\u{0}", baseHexVal);
                    }
                    return c.ToString();

            }
        }

        protected internal RangeData GetRange()
        {
            return new RangeData(this);
        }

    }
}
