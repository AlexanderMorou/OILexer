using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;
using System.Linq;

namespace Oilexer.Expression
{
    [Serializable]
    public class MethodInvokeExpression :
        MemberParentExpression<CodeMethodInvokeExpression>,
        IMethodInvokeExpression
    {
        /// <summary>
        /// Data member for <see cref="Reference"/>.
        /// </summary>
        private IMethodReferenceExpression reference;
        /// <summary>
        /// Data member for <see cref="ArgumentExpressions"/>.
        /// </summary>
        private IExpressionCollection argumentExpressions;
        public MethodInvokeExpression(IMethodReferenceExpression reference, IExpressionCollection argumentExpressions)
        {
            this.reference = reference;
            this.argumentExpressions = argumentExpressions;
        }
        #region IMethodInvokeExpression Members

        /// <summary>
        /// Returns the <see cref="IMethodReferenceExpression"/> that generated the <see cref="MethodInvokeExpression"/>.
        /// </summary>
        public IMethodReferenceExpression Reference
        {
            get { return this.reference; }
        }

        /// <summary>
        /// Returns the <see cref="IExpressionCollection{IExpression, CodeExpression}"/> which denotes the
        /// values to use for the invoke.
        /// </summary>
        public IExpressionCollection ArgumentExpressions
        {
            get
            {
                if (this.argumentExpressions == null)
                    this.argumentExpressions = new ExpressionCollection();
                return this.argumentExpressions;
            }
        }

        #endregion

        public override CodeMethodInvokeExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            CodeMethodInvokeExpression cmie = base.GenerateCodeDom(options);
            cmie.Method = this.reference.GenerateCodeDom(options);
            cmie.Parameters.AddRange(this.ArgumentExpressions.GenerateCodeDom(options));
            return cmie;
        }

        /// <summary>
        /// Performs a look-up on the <see cref="MethodInvokeExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="MethodInvokeExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.argumentExpressions != null)
                this.ArgumentExpressions.GatherTypeReferences(ref result, options);
            if (this.reference != null)
                this.reference.GatherTypeReferences(ref result, options);
        }
    }
}
