using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Runtime.Serialization;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    public abstract class ParameteredParameterMembers<TParameter, TSignatureDom, TParent> :
        Members<TParameter, IParameteredDeclaration<TParameter, TSignatureDom, TParent>, CodeParameterDeclarationExpression>,
        IParameteredParameterMembers<TParameter, TSignatureDom, TParent>
        where TParameter :
            IParameteredParameterMember<TParameter, TSignatureDom, TParent>
        where TParent :
            IDeclarationTarget
        where TSignatureDom :
            CodeObject
    {
        #region ParameteredParameterMembers<TParameter, TSignatureDom, TParent> Constructors
        public ParameteredParameterMembers(IParameteredDeclaration<TParameter, TSignatureDom, TParent> targetDeclaration)
            : base(targetDeclaration)
        {
        }

        #endregion


        #region IParameteredParameterMembers<TItem,TSignatureDom,TParent> Members

        public TParameter AddNew(string name, ITypeReference paramType)
        {
            return this.AddNew(new TypedName(name, paramType));
        }

        public TParameter AddNew(TypedName data)
        {
            TParameter result = GetParameterMember(data);
            this._Add(result.GetUniqueIdentifier(), result);
            return result;
        }

        /// <summary>
        /// Obtains a <typeparamref name="TParameter"/> instance from the
        /// non-abstract implementation of <see cref="ParameteredParameterMembers{TParameter, TSignatureDom, TParent}"/>.
        /// </summary>
        /// <param name="data">The typed name which defines the name and type of the parameter.</param>
        /// <returns>A new instance of the <typeparamref name="TParameter"/>.</returns>
        protected abstract TParameter GetParameterMember(TypedName data);

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
                TParameter param = this[i];
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
