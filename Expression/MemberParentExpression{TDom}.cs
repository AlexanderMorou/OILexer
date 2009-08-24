using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Types.Members;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    /// <summary>
    /// Provides an expression that has members.
    /// </summary>
    /// <typeparam name="TDom">The <see cref="CodeObject"/> the</typeparam>
    [Serializable]
    public abstract partial class MemberParentExpression<TDom> :
        Expression<TDom>,
        IMemberParentExpression<TDom>
            where TDom :
            CodeExpression,
            new()
    {
        public MemberParentExpression()
        {

        }


        #region IExpression Members

        CodeExpression IExpression.GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return this.GenerateCodeDom(options);
        }

        #endregion

        #region IMemberParentExpression Members


        public IMethodReferenceExpression GetMethod(string name, Type[] typeArguments)
        {
            ITypeReference[] types = new ITypeReference[typeArguments.Length];
            for (int i = 0; i < typeArguments.Length; i++)
                types[i] = typeArguments[i].GetTypeReference();
            return this.GetMethod(name, types);
        }

        public IMethodReferenceExpression GetMethod(string name, params ITypeReference[] typeArguments)
        {
            MethodReferenceExpression result = new MethodReferenceExpression(name, this);
            result.TypeArguments.AddRange(typeArguments);
            return result;
        }

        public IPropertyReferenceExpression GetProperty(string name)
        {
            return new PropertyReferenceExpression(name, this);
        }

        public IIndexerReferenceExpression GetIndex(params object[] parameters)
        {
            IExpressionCollection exprCol = new ExpressionCollection();
            foreach (object obj in parameters)
                if (obj is IExpression)
                    exprCol.Add((IExpression)obj);
                else if (obj is ValueType || obj is string)
                    exprCol.Add(new PrimitiveExpression(obj));
                else if (obj is IMember)
                    exprCol.Add(((IMember)obj).GetReference());
            return new IndexerReferenceExpression(this, exprCol);
        }

        #endregion


        #region IFieldReferenceTarget Members

        public IFieldReferenceExpression GetField(string name)
        {
            return new FieldReferenceExpression(name, this);
        }

        #endregion

        /*
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeDOMTranslationOptions options)
        {
            return;
        }
        //*/
    }
}
