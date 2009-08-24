using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using Oilexer.Comments;
using System.CodeDom;
using Oilexer.Translation;
using System.Linq;

namespace Oilexer.Expression
{
    [Serializable]
    public class MethodReferenceExpression :
        Expression<CodeMethodReferenceExpression>,
        IMethodReferenceExpression

    {
        /// <summary>
        /// Data member for <see cref="Name"/>.
        /// </summary>
        protected string name;

        /// <summary>
        /// Data member for <see cref="Reference"/>.
        /// </summary>
        private IMemberParentExpression reference;

        /// <summary>
        /// Data member for <see cref="TypeArguments"/>.
        /// </summary>
        private TypeReferenceCollection typeArguments;

        public MethodReferenceExpression(string name, IMemberParentExpression reference)
        {
            this.reference = reference;
            this.name = name;
        }

        public override CodeMethodReferenceExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            CodeMethodReferenceExpression cmreResult = base.GenerateCodeDom(options);
            if (options.NameHandler.HandlesName(this.name))
                cmreResult.MethodName = options.NameHandler.HandleName(this.name);
            else
                cmreResult.MethodName = this.Name;
            cmreResult.TargetObject = this.Reference.GenerateCodeDom(options);
            foreach (ITypeReference itr in this.TypeArguments)
            {
                cmreResult.TypeArguments.Add(itr.GenerateCodeDom(options));
            }
            return cmreResult;
        }
        
        #region IMethodReferenceExpression Members

        public IMethodInvokeExpression Invoke()
        {
            return new MethodInvokeExpression(this, new ExpressionCollection());
        }

        public IMemberParentExpression Reference
        {
            get { return this.reference; ; }
        }

        /// <summary>
        /// Returns/sets the name of the method reference.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return this.name;
            }
        }

        public ITypeReferenceCollection TypeArguments
        {
            get {
                if (this.typeArguments == null)
                    this.typeArguments = new TypeReferenceCollection();
                return this.typeArguments;
            }
        }

        public IMethodInvokeExpression Invoke(object[] arguments)
        {
            IMethodInvokeExpression result = this.Invoke();
            foreach (object vt in arguments)
                if (vt is IExpression)
                    result.ArgumentExpressions.Add((IExpression)vt);
                else if (vt is ValueType || vt is string)
                    result.ArgumentExpressions.Add(new PrimitiveExpression(vt));
            return result;
        }

        public IMethodInvokeExpression Invoke(IExpression[] arguments)
        {
            IMethodInvokeExpression result = this.Invoke();
            foreach (IExpression ie in arguments)
                result.ArgumentExpressions.Add(ie);
            return result;
        }

        #endregion


        #region IMethodReferenceExpression Members

        public virtual IMethodReferenceComment GetReferenceParticle()
        {
            throw new NotSupportedException("GetReferenceParticle is not supported by default due to method reference expressions missing a parameter type-list.");
        }

        #endregion

        #region IMemberReferenceExpression Members


        IMemberReferenceComment IMemberReferenceExpression.GetReferenceParticle()
        {
            return this.GetReferenceParticle();
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="MethodReferenceExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="MethodReferenceExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.reference != null)
                this.reference.GatherTypeReferences(ref result, options);
            if (this.typeArguments != null)
                foreach (ITypeReference itr in this.TypeArguments)
                        result.Add(itr);
        }
    }
}
