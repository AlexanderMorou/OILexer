using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 /*---------------------------------------------------------------------\
 | Copyright © 2008-2011 Allen C. [Alexander Morou] Copeland Jr.        |
 |----------------------------------------------------------------------|
 | The Abstraction Project's code is provided under a contract-release  |
 | basis.  DO NOT DISTRIBUTE and do not use beyond the contract terms.  |
 \-------------------------------------------------------------------- */

namespace AllenCopeland.Abstraction.Slf.Parsers.Oilexer
{
    public class GDCommentRegion : 
        IGDRegion
    {
        private GDTokens.CommentToken comment;
        public GDCommentRegion(GDTokens.CommentToken comment)
        {
            this.comment = comment;
        }


        #region IGDRegion Members

        public int Start
        {
            get { return (int)this.comment.Position; }
        }

        public int End
        {
            get { return this.Start + this.comment.Length; }
        }

        public string Description
        {
            get { return this.comment.Comment; }
        }

        public string CollapseForm
        {
            get { return string.Format("{0} ...", this.comment.Comment.Substring(0, 4)); }
        }

        #endregion
    }
}
