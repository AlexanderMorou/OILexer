using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    [Serializable]
    public class DirectionExpression :
        Expression<CodeDirectionExpression>,
        IDirectionExpression
    {
        private FieldDirection direction;
        private IExpression directedExpression;
        public DirectionExpression(FieldDirection direction, IExpression directedExpression)
        {
            this.directedExpression = directedExpression;
            this.direction = direction;
        }

        public override CodeDirectionExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            return new CodeDirectionExpression(direction, directedExpression.GenerateCodeDom(options));
        }

        #region IDirectionExpression Members

        public FieldDirection Direction
        {
            get
            {
                return this.direction;
            }
            set
            {
                this.direction = value;
            }
        }

        public IExpression DirectedExpression
        {
            get
            {
                return this.directedExpression;
            }
            set
            {
                this.directedExpression = value;
            }
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="DirectionExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="DirectionExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.directedExpression != null)
                this.DirectedExpression.GatherTypeReferences(ref result, options);
        }
    }
}
