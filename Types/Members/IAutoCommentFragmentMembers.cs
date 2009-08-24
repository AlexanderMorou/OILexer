using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    public interface IAutoCommentFragmentMembers
    {
        CodeCommentStatementCollection GenerateCommentCodeDom(ICodeDOMTranslationOptions options);
    }
}
