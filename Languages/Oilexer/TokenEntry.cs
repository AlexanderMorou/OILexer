using System;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
using AllenCopeland.Abstraction.Slf.Parsers.Oilexer;
using System.Linq;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public class OilexerGrammarTokenEntry :
        OilexerGrammarNamedEntry,
        IOilexerGrammarTokenEntry
    {
        private int? fixedLength;
        private EntryScanMode scanMode;
        private bool unhinged;
        internal ITokenExpressionSeries branches;
        private List<OilexerGrammarTokens.IdentifierToken> lowerPrecedenceNames;
        private IOilexerGrammarTokenEntry[] lowerPrecedenceTokens;
        private bool forcedRecognizer = false;
        public OilexerGrammarTokenEntry(string name, ITokenExpressionSeries branches, EntryScanMode scanMode, string fileName, int column, int line, long position, bool unhinged, List<OilexerGrammarTokens.IdentifierToken> lowerPrecedences, bool forcedRecognizer)
            : base(name, fileName, column, line, position)
        {
            this.lowerPrecedenceNames = lowerPrecedences;
            this.scanMode = scanMode;
            this.branches = branches;
            this.unhinged = unhinged;
            this.forcedRecognizer = forcedRecognizer;
        }

        public OilexerGrammarTokenEntry(string name, ITokenExpressionSeries branches, EntryScanMode scanMode, string fileName, int column, int line, long position, bool unhinged, IOilexerGrammarTokenEntry[] lowerPrecedences, bool forcedRecognizer)
            : base(name, fileName, column, line, position)
        {
            this.lowerPrecedenceTokens = lowerPrecedences;
            this.scanMode = scanMode;
            this.branches = branches;
            this.unhinged = unhinged;
            this.forcedRecognizer = forcedRecognizer;
        }

        //#region IOilexerGrammarScannableEntry Members

        public EntryScanMode ScanMode
        {
            get { return this.scanMode; }
        }

        //#endregion

        //#region IOilexerGrammarTokenEntry
        /// <summary>
        /// Returns the <see cref="ITokenExpressionSeries"/> which defines the branches of
        /// the <see cref="OilexerGrammarTokenEntry"/>.
        /// </summary>
        public ITokenExpressionSeries Branches
        {
            get
            {
                return this.branches;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.Branches.GetHashCode() ^ this.Name.GetHashCode() ^ 
                this.ScanMode.GetHashCode() ^ this.Line.GetHashCode() ^ this.FileName.GetHashCode() ^ 
                this.Column.GetHashCode() ^ this.Position.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} :=\r\n\t{1};", this.Name, this.branches.ToString());
        }

        public bool Unhinged
        {
            get
            {
                return this.unhinged;
            }
            set
            {
                this.unhinged = value;
            }
        }

        public List<OilexerGrammarTokens.IdentifierToken> LowerPrecedenceNames
        {
            get {
                if (this.lowerPrecedenceNames == null)
                    this.lowerPrecedenceNames = new List<OilexerGrammarTokens.IdentifierToken>();
                return this.lowerPrecedenceNames;
            }
        }

        public IOilexerGrammarTokenEntry[] LowerPrecedenceTokens
        {
            get { return this.lowerPrecedenceTokens; }
            internal set { this.lowerPrecedenceTokens = value; }
        }

        public IEnumerable<IOilexerGrammarTokenEntry> AllLowerPrecedenceTokens
        {
            get
            {
                foreach (OilexerGrammarTokenEntry token in LowerPrecedenceTokens.Cast<OilexerGrammarTokenEntry>())
                {
                    yield return token;
                    foreach (var lowerToken in token.AllLowerPrecedenceTokens)
                        yield return lowerToken;
                }
            }
        }

        public bool ForcedRecognizer { get { return this.forcedRecognizer; } }

        public bool IsFixedLength
        {
            get
            {
                return this.FixedLength != -1;
            }
        }

        private static int GetFixedLength(ITokenExpressionSeries series)
        {
            var result = -1;
            foreach (var expression in series)
            {
                var currentLength = GetFixedLength(expression);
                if (result == -1 && currentLength != -1)
                    result = currentLength;
                else if (currentLength != result)
                    return -1;
            }
            return result;
        }

        private static int GetFixedLength(ITokenExpression expression)
        {
            int result = 0;
            foreach (var item in expression)
            {
                var currentLength = GetFixedLength(item);
                if (currentLength == -1)
                    return -1;
                result += currentLength;
            }
            return result;
        }

        private static int GetFixedLength(ITokenItem item)
        {
            int currentResult = -1;
            if (item is ILiteralTokenItem)
                currentResult = GetFixedLength((ILiteralTokenItem)item);
            else if (item is ISubtractionCommandTokenItem)
                currentResult = GetFixedLength(((ISubtractionCommandTokenItem)item).Left);
            else if (!(item is ICommandTokenItem))
                currentResult = 1;
            else if (item is ITokenGroupItem)
                currentResult = GetFixedLength((ITokenExpressionSeries)item);
            if (currentResult == -1)
                return -1;
            else
                return AdjustFixedLength(currentResult, item.RepeatOptions);
        }

        private static int GetFixedLength(ILiteralTokenItem literal)
        {
            if (literal is ILiteralStringTokenItem)
                return ((ILiteralStringTokenItem)literal).Value.Length;
            else if (literal is ILiteralStringReferenceTokenItem)
                return ((ILiteralStringReferenceTokenItem)literal).Literal.Value.Length;
            else if (literal is ILiteralCharTokenItem || literal is ILiteralCharReferenceTokenItem)
                return 1;
            return -1;
        }

        private static int AdjustFixedLength(int baseLength, ScannableEntryItemRepeatInfo repeatData)
        {
            if (repeatData.Min == repeatData.Max && repeatData.Min != null)
                return baseLength * repeatData.Min.Value;
            else if ((repeatData.Options & ~(ScannableEntryItemRepeatOptions.AnyOrder | ScannableEntryItemRepeatOptions.MaxReduce | ScannableEntryItemRepeatOptions.Specific)) == ScannableEntryItemRepeatOptions.None)
                return baseLength;
            else
                return -1;
        }

        public int FixedLength
        {
            get
            {
                if (this.fixedLength == null)
                    this.fixedLength = GetFixedLength(this.Branches);
                return this.fixedLength.Value;

            }
        }

        //#endregion

        public bool Contextual { get; internal set; }
    }
}
