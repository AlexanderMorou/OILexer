﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.FiniteAutomata.Tokens;
using System.Globalization;

namespace Oilexer.Parser.Builder
{
    internal class UnicodeTarget :
        ControlledStateDictionary<UnicodeCategory, IUnicodeTargetCategory>,
        IUnicodeTarget
    {
        public UnicodeTarget(RegularLanguageDFAState target, bool targetIsOrigin)
        {
            this.TargetIsOrigin = targetIsOrigin;
            this.Target = target;
        }
        #region IUnicodeTarget Members

        public RegularLanguageDFAState Target { get; private set; }

        public IUnicodeTargetCategory Add(UnicodeCategory category)
        {
            UnicodeTargetCategory result = new UnicodeTargetCategory(category);
            base.dictionaryCopy.Add(category, result);
            return result;
        }

        public IUnicodeTargetPartialCategory Add(UnicodeCategory category, RegularLanguageSet negativeAssertion)
        {
            UnicodeTargetPartialCategory result = new UnicodeTargetPartialCategory(category, negativeAssertion);
            base.dictionaryCopy.Add(category, result);
            return result;
        }

        #endregion

        #region IEquatable<IUnicodeTarget> Members

        public bool Equals(IUnicodeTarget other)
        {
            if (other.TargetIsOrigin != this.TargetIsOrigin)
                return false;
            if (other.Count != this.Count)
                return false;
            if (other.Target != this.Target)
                return false;
            return other.All(p =>
            {
                return this.ContainsKey(p.Key) && this[p.Key].Equals(p.Value);
            });
        }

        #endregion

        public override int GetHashCode()
        {
            int result = 0;
            foreach (var category in this)
                result ^= category.GetHashCode();
            return result ^ this.Count;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is IUnicodeTarget)
                return this.Equals((IUnicodeTarget)obj);
            return false;
        }
        public bool TargetIsOrigin { get; private set; }
    }
}
