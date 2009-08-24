using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class SnippetMembers :
        Members<ISnippetMember, IMemberParentType, CodeSnippetTypeMember>,
        ISnippetMembers
    {
        public SnippetMembers(IMemberParentType targetDeclaration):
            base(targetDeclaration)
        {
        }
        public SnippetMembers(IMemberParentType targetDeclaration, IDictionary<string, ISnippetMember> members)
            : base(targetDeclaration, members)
        {
        }
        protected override IMembers<ISnippetMember, IMemberParentType, CodeSnippetTypeMember> OnGetPartialClone(IMemberParentType parent)
        {
            return new SnippetMembers(parent, this.dictionaryCopy);
        }

        #region ISnippetMembers Members

        public ISnippetMember AddNew(string text)
        {
            ISnippetMember snippetMember = new SnippetMember(this.TargetDeclaration);
            snippetMember.Text = text;
            base.Add("SnippetMember" + String.Format("{0:0000}",this.Count), snippetMember);
            return snippetMember;
        }

        #endregion
    }
}
