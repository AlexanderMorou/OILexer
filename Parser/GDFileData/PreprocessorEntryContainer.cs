using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public class PreprocessorEntryContainer :
        PreprocessorDirectiveBase,
        IPreprocessorEntryContainer
    {
        public PreprocessorEntryContainer(IEntry contained, int column, int line, long position)
            : base(column, line, position)
        {
            this.Contained = contained;
        }

        #region IPreprocessorEntryContainer Members

        public IEntry Contained { get; private set; }

        #endregion

        public override EntryPreprocessorType Type
        {
            get { return EntryPreprocessorType.EntryContainer; }
        }
    }
}
