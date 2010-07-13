using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer.Utilities.Collections;
using OM = System.Collections.ObjectModel;
using GDFD = Oilexer.Parser.GDFileData;
using System.CodeDom.Compiler;

namespace Oilexer.Parser
{
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
        private IReadOnlyCollection<string> files;

        private GDFile()
        {
        }

        public GDFile(string fileName)
        {
            this.EnsureFilesCopy();
            this._files.Add(fileName);
        }

        public IReadOnlyCollection<string> Files
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
        #endregion
    }
}
