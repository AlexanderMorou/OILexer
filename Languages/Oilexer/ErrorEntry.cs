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
    public class ErrorEntry :
        NamedEntry,
        IErrorEntry
    {
        /// <summary>
        /// Data member for <see cref="Message"/>.
        /// </summary>
        private string message;
        /// <summary>
        /// Data member for <see cref="Number"/>.
        /// </summary>
        private int number;
        public ErrorEntry(string name, string message, int number, string fileName, int column, int line, long position)
            : base(name, fileName, column, line, position)
        {
            this.message = message;
            this.number = number;
        }

        #region IErrorEntry Members

        /// <summary>
        /// Returns the message associated with the entry.
        /// </summary>
        public string Message
        {
            get { return this.message; }
        }

        /// <summary>
        /// Returns the error number associated with the <see cref="IErrorEntry"/>.
        /// </summary>
        public int Number
        {
            get { return this.number; }
        }

        #endregion
    }
}
