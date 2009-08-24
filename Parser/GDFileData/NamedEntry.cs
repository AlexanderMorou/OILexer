using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer.Parser.GDFileData
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
