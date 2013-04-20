using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using GDFD = AllenCopeland.Abstraction.Slf.Languages.Oilexer;
using OM = System.Collections.ObjectModel;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
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
    using GDEntryCollection =
        OM::Collection<GDFD::IEntry>;
    using GDStringCollection =
            OM::Collection<string>;

    /// <summary>
    /// Provides a base implementation of <see cref="IGDFile"/>.
    /// </summary>
    public partial class GDFile :
        GDEntryCollection,
        IGDFile
    {
        private IGDFileOptions options;
        private GDStringCollection _files;
        private IList<string> includes;
        private IControlledCollection<string> files;
        private IList<IGDRegion> myRegions = new List<IGDRegion>();
        private IControlledCollection<IGDRegion> regions;
        private GDFile()
        {
        }

        public GDFile(string fileName)
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
                    this.files = new ReadOnlyCollection<string>(_files);
                }
                return this.files;
            }
        }

        private void EnsureFilesCopy()
        {
            if (this._files == null)
                this._files = new GDStringCollection();
        }

        public static IGDFile operator +(IGDFile leftSide, GDFile rightSide)
        {
            GDFile result = new GDFile();
            result.EnsureFilesCopy();
            foreach (string s in leftSide.Files)
                if (!result._files.Contains(s))
                    result._files.Add(s);
            foreach (string s in rightSide.Files)
                if (!result._files.Contains(s))
                    result._files.Add(s);
            foreach (IEntry igfe in leftSide)
                result.Add(igfe);

            foreach (IEntry igfe in rightSide)
                result.Add(igfe);

            result.options = ((MyOptions)(leftSide.Options)) + ((MyOptions)(rightSide.Options));

            return result;
        }

        #region IGDFile Members

        public IGDFileOptions Options
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

        public IControlledCollection<IGDRegion> Regions
        {
            get
            {
                if (this.regions == null)
                    this.regions = new ReadOnlyCollection<IGDRegion>(this.myRegions);
                return this.regions;
            }
        }

        #endregion

        internal void AddCommentRegion(GDTokens.CommentToken commentToken)
        {
            this.myRegions.Add(new GDCommentRegion(commentToken));
        }

        internal void AddRuleRegion(IProductionRuleEntry entry, long bodyStart, long bodyEnd)
        {
            this.myRegions.Add(new GDProductionRuleRegion(entry, bodyStart, bodyEnd));
        }

        internal void AddIfRegion(IPreprocessorIfDirective directive, long bodyStart, long bodyEnd)
        {
            this.myRegions.Add(new GDPreprocessorIfDirectiveRegion(directive, bodyStart, bodyEnd));
        }

        internal void AddRuleGroupRegion(IProductionRuleGroupItem group, long openParen, long endParen)
        {
            this.myRegions.Add(new GDProductionRuleGroupRegion(group, openParen + 1, endParen));
        }

        internal void AddTokenGroupRegion(ITokenGroupItem group, long openParen, long endParen)
        {
            this.myRegions.Add(new GDTokenGroupRegion(group, openParen + 1, endParen));
        }

        internal void AddTokenRegion(ITokenEntry ite, long bodyStart, long bodyEnd)
        {
            this.myRegions.Add(new GDTokenRegion(ite, bodyStart, bodyEnd));
        }
    }
}
