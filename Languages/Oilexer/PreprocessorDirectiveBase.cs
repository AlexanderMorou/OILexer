using System;
using System.Collections.Generic;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2016 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer
{
    public abstract class PreprocessorDirectiveBase :
        IPreprocessorDirective
    {
        /// <summary>
        /// Data member for <see cref="Column"/>.
        /// </summary>
        private int column;
        /// <summary>
        /// Data member for <see cref="Line"/>.
        /// </summary>
        private int line;
        /// <summary>
        /// Data member for <see cref="Position"/>.
        /// </summary>
        private long position;

        /// <summary>
        /// Creates a new <see cref="PreprocessorDirectiveBase"/> with the 
        /// <paramref name="column"/>, <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="fileName">The file in which the <see cref="PreprocessorDirectiveBase"/> was declared
        /// in.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="PreprocessorDirectiveBase"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="PreprocessorDirectiveBase"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="PreprocessorDirectiveBase"/> 
        /// was declared at.</param>
        public PreprocessorDirectiveBase(int column, int line, long position)
        {
            this.column = column;
            this.line = line;
            this.position = position;
        }

        //#region IPreprocessorDirective Members

        public abstract EntryPreprocessorType Type
        {
            get;
        }

        /// <summary>
        /// Returns the column at the current <see cref="Line"/> the 
        /// <see cref="PreprocessorDirectiveBase"/> was declared at.
        /// </summary>
        public int Column
        {
            get { return this.column; }
        }

        /// <summary>
        /// Returns the line index the <see cref="PreprocessorDirectiveBase"/> was declared at.
        /// </summary>
        public int Line
        {
            get { return this.line; }
        }

        /// <summary>
        /// Returns the position in the file the <see cref="PreprocessorDirectiveBase"/> was declared at.
        /// </summary>
        public long Position
        {
            get { return this.position; }
        }

        //#endregion
    }
}
