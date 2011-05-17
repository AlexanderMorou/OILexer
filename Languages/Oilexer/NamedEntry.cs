using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public class NamedEntry :
        Entry,
        INamedEntry
    {
        /// <summary>
        /// Data member for <see cref="Name"/>.
        /// </summary>
        private string name;

        public NamedEntry(string name, string fileName, int column, int line, long position)
            : base(fileName, column, line, position)
        {
            this.name = name;
        }
        #region INamedEntry Members

        /// <summary>
        /// Returns the name of the <see cref="NamedEntry"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        #endregion
    }
}
