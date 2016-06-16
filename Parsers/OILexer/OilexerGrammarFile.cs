using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using OilexerGrammarFD = AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using OM = System.Collections.ObjectModel;
/*---------------------------------------------------------------------\
| Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
|----------------------------------------------------------------------|
| The Abstraction Project's code is provided under a contract-release  |
| basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
\-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Rules;
    using AllenCopeland.Abstraction.Slf.Languages.Oilexer.Tokens;
    using AllenCopeland.Abstraction.Utilities.Collections;
    /* *
         * Aliases, uses namespace alias qualifiers to shorten
         * type names.
         * */
    using OilexerGrammarEntryCollection =
        OM::Collection<OilexerGrammarFD::IOilexerGrammarEntry>;
    using OilexerGrammarStringCollection =
            OM::Collection<string>;
    using System.IO;

    /// <summary>
    /// Provides a base implementation of <see cref="IOilexerGrammarFile"/>.
    /// </summary>
    public partial class OilexerGrammarFile :
        OilexerGrammarEntryCollection,
        IOilexerGrammarFile
    {
        private string relativeRoot;
        private IOilexerGrammarFileOptions options;
        private OilexerGrammarStringCollection _files;
        private IList<string> includes;
        private IControlledCollection<string> files;
        private IList<IOilexerGrammarRegion> myRegions = new List<IOilexerGrammarRegion>();
        private IControlledCollection<IOilexerGrammarRegion> regions;
        private OilexerGrammarFile()
        {
        }

        public OilexerGrammarFile(string fileName)
        {
            this.EnsureFilesCopy();
            this._files.Add(fileName);
        }

        public IControlledCollection<string> Files
        {
            get
            {
                if (this.files == null)
                {
                    EnsureFilesCopy();
                    this.files = new ControlledCollection<string>(_files);
                }
                return this.files;
            }
        }

        private void EnsureFilesCopy()
        {
            if (this._files == null)
                this._files = new OilexerGrammarStringCollection();
        }

        public static IOilexerGrammarFile operator +(IOilexerGrammarFile leftSide, OilexerGrammarFile rightSide)
        {
            OilexerGrammarFile result = new OilexerGrammarFile();
            result.EnsureFilesCopy();
            foreach (string s in leftSide.Files)
                if (!result._files.Contains(s))
                    result._files.Add(s);
            foreach (string s in rightSide.Files)
                if (!result._files.Contains(s))
                    result._files.Add(s);
            foreach (IOilexerGrammarEntry igfe in leftSide)
                result.Add(igfe);

            foreach (IOilexerGrammarEntry igfe in rightSide)
                result.Add(igfe);

            result.options = ((MyOptions)(leftSide.Options)) + ((MyOptions)(rightSide.Options));

            return result;
        }

        #region IOilexerGrammarFile Members

        public IOilexerGrammarFileOptions Options
        {
            get
            {
                if (this.options == null)
                    this.options = new MyOptions();
                return this.options;
            }
        }

        public IList<string> Includes
        {
            get
            {
                if (this.includes == null)
                    this.includes = new List<string>();
                return this.includes;
            }
        }

        public IControlledCollection<IOilexerGrammarRegion> Regions
        {
            get
            {
                if (this.regions == null)
                    this.regions = new ControlledCollection<IOilexerGrammarRegion>(this.myRegions);
                return this.regions;
            }
        }

        #endregion

        internal void AddCommentRegion(OilexerGrammarTokens.CommentToken commentToken)
        {
            this.myRegions.Add(new OilexerGrammarCommentRegion(commentToken));
        }

        internal void AddRuleRegion(IOilexerGrammarProductionRuleEntry entry, long bodyStart, long bodyEnd)
        {
            this.myRegions.Add(new OilexerGrammarProductionRuleRegion(entry, bodyStart, bodyEnd));
        }

        internal void AddIfRegion(IPreprocessorIfDirective directive, long bodyStart, long bodyEnd)
        {
            this.myRegions.Add(new OilexerGrammarPreprocessorIfDirectiveRegion(directive, bodyStart, bodyEnd));
        }

        internal void AddRuleGroupRegion(IProductionRuleGroupItem group, long openParen, long endParen)
        {
            this.myRegions.Add(new OilexerGrammarProductionRuleGroupRegion(group, openParen + 1, endParen));
        }

        internal void AddTokenGroupRegion(ITokenGroupItem group, long openParen, long endParen)
        {
            this.myRegions.Add(new OilexerGrammarTokenGroupRegion(group, openParen + 1, endParen));
        }

        internal void AddTokenRegion(IOilexerGrammarTokenEntry ite, long bodyStart, long bodyEnd)
        {
            this.myRegions.Add(new OilexerGrammarTokenRegion(ite, bodyStart, bodyEnd));
        }
        public string RelativeRoot
        {
            get
            {
                return this.relativeRoot ?? (this.relativeRoot = this.GetRelativeRoot());
            }
        }
        private string GetRelativeRoot()
        {
            string relativeRoot = null;
            var parts = (from string f in this.Files
                         orderby f.Length descending
                         select f.ToLower()).First().Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < parts.Length; i++)
            {
                string currentRoot = string.Join(@"\", parts, 0, parts.Length - i);
                if (this.Files.All(p => p.ToLower().Contains(currentRoot)))
                {
                    relativeRoot = currentRoot;
                    break;
                }
            }
            return relativeRoot;
        }


        public IDictionary<string, string> DefinedSymbols { get; internal set; }
    }
}
