using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;
using System.IO;
using Oilexer._Internal;
using System.Threading;

namespace Oilexer.FiniteAutomata.Tokens
{
    public class RegularLanguageDFARootState :
        RegularLanguageDFAState
    {
        private ITokenEntry entry;

        public RegularLanguageDFARootState(ITokenEntry entry)
        {
            this.entry = entry;
        }

        internal void Reduce(RegularCaptureType captureType)
        {
            Reduce(this, captureType == RegularCaptureType.Recognizer);
        }

        internal ITokenEntry Entry
        {
            get
            {
                return this.entry;
            }
        }
    }
}
