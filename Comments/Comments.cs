using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Comments
{
    public class Comments :
        ControlledStateCollection<IComment>,
        IComments
    {

        #region IComments Members

        public IComment AddComment(string comment)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IDocumentationComment AddDocComment(string comment)
        {
            return new DocumentationStringComment(comment);
        }


        public CodeCommentStatementCollection GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            List<IComment> nonDocCom = new List<IComment>();
            List<IDocumentationComment> docCom = new List<IDocumentationComment>();
            foreach (IComment ic in this)
                if (ic is IDocumentationComment)
                    docCom.Add((IDocumentationComment)ic);
                else
                    nonDocCom.Add(ic);
            CodeCommentStatementCollection ccsc = new CodeCommentStatementCollection();

            nonDocCom.AddRange(docCom.ToArray());
            docCom.Clear();
            docCom = null;

            foreach (IComment ic in nonDocCom)
                ccsc.Add(new CodeCommentStatement(ic.GenerateCodeDom(options)));
            return ccsc;
        }

        #endregion
    }
}
