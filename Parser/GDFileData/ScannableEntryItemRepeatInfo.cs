using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
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
                case ScannableEntryItemRepeatOptions.Specific:
                    if (this.Min == null)
                        if (this.Max == null)
                            return 5;
                        else
                            return this.Max.Value | 5;
                    else
                        if (this.Max == null)
                            return this.Min.Value | 5;
                        else
                            return (this.Min.Value ^ this.Max.Value) | 5;

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
            switch (this.Options & ~ScannableEntryItemRepeatOptions.AnyOrder)
            {
                case ScannableEntryItemRepeatOptions.ZeroOrOne:
                    return "?";
                case ScannableEntryItemRepeatOptions.ZeroOrMore:
                    return "*";
                case ScannableEntryItemRepeatOptions.OneOrMore:
                    return "+";
                case ScannableEntryItemRepeatOptions.Specific:
                    if (Min == Max)
                        return string.Format("{{{0}}}", Min.Value);
                    else if (Min != null)
                        return string.Format("{{{0},}}", Min.Value);
                    else if (Max != null)
                        return string.Format("{{,{0}}}", Max.Value);
                    break;
            }
            return string.Empty;
        }
    }
}
