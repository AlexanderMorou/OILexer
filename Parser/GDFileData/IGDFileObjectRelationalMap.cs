using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Parser;
using Oilexer.Parser.GDFileData;

namespace Oilexer.Parser.GDFileData
{
    public interface IGDFileObjectRelationalMap :
        IControlledStateDictionary<IScannableEntry, IEntryObjectRelationalMap>
    {
        /// <summary>
        /// Returns the <see cref="IGDFile"/> from which the object relational 
        /// map is derived.
        /// </summary>
        IGDFile Source { get; }
    }
}
