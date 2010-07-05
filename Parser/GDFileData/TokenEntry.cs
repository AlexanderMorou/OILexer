using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData.TokenExpression;

namespace Oilexer.Parser.GDFileData
{
    public class TokenEntry :
        NamedEntry,
        ITokenEntry
    {
        private EntryScanMode scanMode;
        private bool unhinged;
        internal ITokenExpressionSeries branches;
        private List<string> lowerPrecedenceNames;
        private ITokenEntry[] lowerPrecedenceTokens;
        private bool forcedRecognizer = false;
        public TokenEntry(string name, ITokenExpressionSeries branches, EntryScanMode scanMode, string fileName, int column, int line, long position, bool unhinged, List<string> lowerPrecedences, bool forcedRecognizer)
            : base(name, fileName, column, line, position)
        {
            this.lowerPrecedenceNames = lowerPrecedences;
            this.scanMode = scanMode;
            this.branches = branches;
            this.unhinged = unhinged;
            this.forcedRecognizer = forcedRecognizer;
        }
        public TokenEntry(string name, ITokenExpressionSeries branches, EntryScanMode scanMode, string fileName, int column, int line, long position, bool unhinged, ITokenEntry[] lowerPrecedences, bool forcedRecognizer)
            : base(name, fileName, column, line, position)
        {
            this.lowerPrecedenceTokens = lowerPrecedences;
            this.scanMode = scanMode;
            this.branches = branches;
            this.unhinged = unhinged;
            this.forcedRecognizer = forcedRecognizer;
        }

        #region IScannableEntry Members

        public EntryScanMode ScanMode
        {
            get { return this.scanMode; }
        }

        #endregion

        #region ITokenEntry
        /// <summary>
        /// Returns the <see cref="ITokenExpressionSeries"/> which defines the branches of
        /// the <see cref="TokenEntry"/>.
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
        #endregion

        #region ITokenEntry Members

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

        public List<string> LowerPrecedenceNames
        {
            get {
                if (this.lowerPrecedenceNames == null)
                    this.lowerPrecedenceNames = new List<string>();
                return this.lowerPrecedenceNames;
            }
        }

        public ITokenEntry[] LowerPrecedenceTokens
        {
            get { return this.lowerPrecedenceTokens; }
            internal set { this.lowerPrecedenceTokens = value; }
        }

        public bool ForcedRecognizer { get { return this.forcedRecognizer; } }

        #endregion
    }
}
