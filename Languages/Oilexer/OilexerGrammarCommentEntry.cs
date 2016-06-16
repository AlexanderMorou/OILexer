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
    public class OilexerGrammarCommentEntry :
        OilexerGrammarEntry,
        IOilexerGrammarCommentEntry
    {
        /// <summary>
        /// Data member for <see cref="Comment"/>
        /// </summary>
        private string comment;

        /// <summary>
        /// Creates a new <see cref="OilexerGrammarEntry"/> with the <paramref name="column"/>, 
        /// <paramref name="line"/>, and <paramref name="position"/>.
        /// </summary>
        /// <param name="comment">The pathExplorationComment the <see cref="OilexerGrammarCommentEntry"/> represents.</param>
        /// <param name="column">The column at the current <paramref name="line"/> the 
        /// <see cref="OilexerGrammarCommentEntry"/> was declared at. </param>
        /// <param name="line">The line index the <see cref="OilexerGrammarCommentEntry"/> was declared at.</param>
        /// <param name="position">The position in the file the <see cref="OilexerGrammarCommentEntry"/> 
        /// was declared at.</param>
        public OilexerGrammarCommentEntry(string comment, string fileName, int column, int line, long position)
            : base(fileName, column, line, position)
        {
            this.comment = comment;
        }

        public override string ToString()
        {
            return this.comment;
        }

        //#region IOilexerGrammarCommentEntry Members

        /// <summary>
        /// Returns the comment the <see cref="OilexerGrammarCommentEntry"/> represents.
        /// </summary>
        public string Comment
        {
            get { return this.comment; }
        }

        //#endregion
    }
}
