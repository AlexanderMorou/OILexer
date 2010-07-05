using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Parser.GDFileData;
using Oilexer._Internal.Inlining;

namespace Oilexer.Parser.Builder
{
    internal class TokenEqualityLevel :
        List<InlinedTokenEntry>
    {
        public TokenEqualityLevel(IEnumerable<InlinedTokenEntry> set)
            : base(set)
        {
        }

        public TokenEqualityLevel()
        {

        }
        
        new public void Sort()
        {
            InlinedTokenEntry[] items = (from i in this
                                         orderby i.Name
                                         select i).ToArray();
            this.Clear();
            this.AddRange(items);
        }
    }
}
