using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using System.Runtime.Serialization;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    [Serializable]
    public class DelegateTypeParameterMembers :
        Members<IDelegateTypeParameterMember, IParameteredDeclaration<IDelegateTypeParameterMember, CodeTypeDelegate, ITypeParent>, CodeParameterDeclarationExpression>,
        IDelegateTypeParameterMembers
    {
        #region DelegateTypeParameterMembers Constructors
        public DelegateTypeParameterMembers(IDelegateType targetDeclaration)
            : base(targetDeclaration)
        {
        }
        #endregion

        protected override IMembers<IDelegateTypeParameterMember, IParameteredDeclaration<IDelegateTypeParameterMember, CodeTypeDelegate, ITypeParent>, CodeParameterDeclarationExpression> OnGetPartialClone(IParameteredDeclaration<IDelegateTypeParameterMember, CodeTypeDelegate, ITypeParent> parent)
        {
            throw new NotSupportedException("Delegate types aren't segmentable; therefore, the parameters therein are not.");
        }

        #region IParameteredParameterMembers<IDelegateTypeParameterMember,CodeTypeDelegate,ITypeParent> Members

        public IDelegateTypeParameterMember AddNew(string name, ITypeReference paramType)
        {
            return this.AddNew(new TypedName(name, paramType));
        }

        public IDelegateTypeParameterMember AddNew(TypedName data)
        {
            IDelegateTypeParameterMember result = new DelegateTypeParameterMember(data, (IDelegateType)this.TargetDeclaration);
            this._Add(result.GetUniqueIdentifier(), result);
            return result;
        }

        #endregion

        #region IAutoCommentFragmentMembers Members

        public CodeCommentStatementCollection GenerateCommentCodeDom(ICodeDOMTranslationOptions options)
        {
            CodeCommentStatementCollection ccsc = new CodeCommentStatementCollection();
            if (this.Count == 0)
                return ccsc;
            CodeCommentStatement[] typeParamComments = new CodeCommentStatement[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                IDelegateTypeParameterMember param = this[i];
                if (param.DocumentationComment != null && param.DocumentationComment != string.Empty)
                {
                    string paramName = "";
                    if (options.NameHandler.HandlesName(param))
                        paramName = options.NameHandler.HandleName(param);
                    else
                        paramName = param.Name;
                    ccsc.Add(new CodeCommentStatement(new CodeComment(string.Format("<param name=\"{0}\">{1}</param>", paramName, param.DocumentationComment), true)));
                }
            }
            return ccsc;
        }

        #endregion

    }
}
