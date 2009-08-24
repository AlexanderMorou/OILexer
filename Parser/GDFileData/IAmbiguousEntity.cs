using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
{
    public interface IAmbiguousGDEntity
    {
        /// <summary>
        /// Disambiguifies the <see cref="IAmbiguousEntity"/>.
        /// </summary>
        /// <param name="context">The <see cref="IGDFile"/> used to clear up the ambiguity.</param>
        void Disambiguify(IGDFile context, IEntry root);
    }
}
