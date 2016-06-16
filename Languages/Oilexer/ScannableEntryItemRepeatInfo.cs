using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    /// <summary>
    /// The repeat options for the <see cref="IScannableEntryItem"/>.
    /// </summary>
    public struct ScannableEntryItemRepeatInfo
    {
        public static readonly ScannableEntryItemRepeatInfo None = new ScannableEntryItemRepeatInfo(ScannableEntryItemRepeatOptions.None);
        public static readonly ScannableEntryItemRepeatInfo ZeroOrOne = new ScannableEntryItemRepeatInfo(ScannableEntryItemRepeatOptions.ZeroOrOne);
        public static readonly ScannableEntryItemRepeatInfo ZeroOrMore = new ScannableEntryItemRepeatInfo(ScannableEntryItemRepeatOptions.ZeroOrMore);
        public static readonly ScannableEntryItemRepeatInfo OneOrMore = new ScannableEntryItemRepeatInfo(ScannableEntryItemRepeatOptions.OneOrMore);
        public static readonly ScannableEntryItemRepeatInfo AnyOrder = new ScannableEntryItemRepeatInfo(ScannableEntryItemRepeatOptions.AnyOrder);
        public static readonly ScannableEntryItemRepeatInfo MaxReduce = new ScannableEntryItemRepeatInfo(ScannableEntryItemRepeatOptions.MaxReduce);

        public ScannableEntryItemRepeatOptions Options { get; private set; }

        public int? Min { get; private set; }
        public int? Max { get; private set; }

        public ScannableEntryItemRepeatInfo(ScannableEntryItemRepeatOptions options)
            : this()
        {
            this.Options = options;
        }

        public ScannableEntryItemRepeatInfo(int? min, int? max)
            : this()
        {
            if (min == null && max == null)
                throw new ArgumentNullException("max", "min and max cannot both be null");
            this.Options = ScannableEntryItemRepeatOptions.Specific;
            this.Min = min;
            this.Max = max;
        }

        public static bool operator ==(ScannableEntryItemRepeatInfo left, ScannableEntryItemRepeatInfo right)
        {
            if (left.Options != right.Options)
                return false;
            if (left.Options == ScannableEntryItemRepeatOptions.Specific)
                return left.Min == right.Min &&
                       left.Max == right.Max;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is ScannableEntryItemRepeatInfo)
                return ((ScannableEntryItemRepeatInfo)(obj)) == this;
            return false;
        }

        public override int GetHashCode()
        {
            switch (this.Options)
            {
                case ScannableEntryItemRepeatOptions.None:
                    return 0;
                case ScannableEntryItemRepeatOptions.ZeroOrOne:
                    return 1;
                case ScannableEntryItemRepeatOptions.ZeroOrMore:
                    return 2;
                case ScannableEntryItemRepeatOptions.OneOrMore:
                    return 3;
                case ScannableEntryItemRepeatOptions.AnyOrder:
                    return 4;
                case ScannableEntryItemRepeatOptions.MaxReduce:
                    return 5;
                case ScannableEntryItemRepeatOptions.Specific:
                    if (this.Min == null)
                        if (this.Max == null)
                            return 5;
                        else
                            return this.Max.Value << 6 | 5;
                    else
                        if (this.Max == null)
                            return this.Min.Value << 3 | 5;
                        else
                            return (this.Min.Value << 3 ^ this.Max.Value << 6) | 5;

            }
            return -1;
        }

        public static bool operator !=(ScannableEntryItemRepeatInfo left, ScannableEntryItemRepeatInfo right)
        {
            if (left.Options == right.Options)
                return false;
            if (left.Options == ScannableEntryItemRepeatOptions.Specific)
                return left.Min != right.Min ||
                       left.Max != right.Max;
            return true;
        }

        public static ScannableEntryItemRepeatInfo operator |(ScannableEntryItemRepeatInfo left, ScannableEntryItemRepeatInfo right)
        {
            if (left.Options == ScannableEntryItemRepeatOptions.Specific &&
                right.Options == ScannableEntryItemRepeatOptions.Specific)
                throw new InvalidOperationException("left and right are both specific match requirements");
            if (left.Options == ScannableEntryItemRepeatOptions.Specific)
            {
                ScannableEntryItemRepeatInfo result = new ScannableEntryItemRepeatInfo(left.Min, left.Max);
                result.Options = left.Options | right.Options;
                return result;
            }
            if (right.Options == ScannableEntryItemRepeatOptions.Specific)
            {
                ScannableEntryItemRepeatInfo result = new ScannableEntryItemRepeatInfo(right.Min, right.Max);
                result.Options = left.Options | right.Options;
                return result;
            }
            return new ScannableEntryItemRepeatInfo(left.Options | right.Options);
        }

        public override string ToString()
        {
            bool maxReduce = ((this.Options & ~ScannableEntryItemRepeatOptions.AnyOrder) & ScannableEntryItemRepeatOptions.MaxReduce) == ScannableEntryItemRepeatOptions.MaxReduce;
            bool anyOrder = ((this.Options & ~ScannableEntryItemRepeatOptions.MaxReduce) & ScannableEntryItemRepeatOptions.AnyOrder) == ScannableEntryItemRepeatOptions.AnyOrder;
            switch (this.Options & ~(ScannableEntryItemRepeatOptions.AnyOrder | ScannableEntryItemRepeatOptions.MaxReduce))
            {
                case ScannableEntryItemRepeatOptions.ZeroOrOne:
                    return string.Format("?{0}{1}", maxReduce ? "$" : string.Empty, anyOrder ? "@" : string.Empty);
                case ScannableEntryItemRepeatOptions.ZeroOrMore:
                    return string.Format("*{0}{1}", maxReduce ? "$" : string.Empty, anyOrder ? "@" : string.Empty);
                case ScannableEntryItemRepeatOptions.OneOrMore:
                    return string.Format("+{0}{1}", maxReduce ? "$" : string.Empty, anyOrder ? "@" : string.Empty);
                case ScannableEntryItemRepeatOptions.Specific:
                    if (Min == Max)
                        return string.Format("{{{0}}}{1}{2}", Min.Value, maxReduce ? "$" : string.Empty, anyOrder ? "@" : string.Empty);
                    else if (Min != null && Max != null)
                        return string.Format("{{{0},{1}}}{2}{3}", Min.Value, Max.Value, maxReduce ? "$" : string.Empty, anyOrder ? "@" : string.Empty);
                    else if (Min != null)
                        return string.Format("{{{0},}}{1}{2}", Min.Value, maxReduce ? "$" : string.Empty, anyOrder ? "@" : string.Empty);
                    else if (Max != null)
                        return string.Format("{{,{0}}}{1}{2}", Max.Value, maxReduce ? "$" : string.Empty, anyOrder ? "@" : string.Empty);
                    break;
            }
            return maxReduce ? "$" : string.Empty;
        }
    }
}
