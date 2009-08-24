using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Types;
using System.Runtime.Serialization;
using Oilexer._Internal;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    /// <summary>
    /// An indexer pertinent to an array.
    /// </summary>
    [Serializable]
    public class ArrayIndexerExpression :
        MemberParentExpression<CodeArrayIndexerExpression>,
        IArrayIndexerExpression,
        ISerializable
    {
        /// <summary>
        /// Data member for <see cref="Indices"/>
        /// </summary>
        private IExpressionCollection indices;
        private IExpression target;
        /// <summary>
        /// Creates a new instnace of <see cref="ArrayIndexerExpression"/> with the 
        /// <paramref name="target"/> and <parmaref name="indices"/> provided
        /// </summary>
        /// <param name="target">The array expression that needs accessed because it is an
        /// array.</param>
        /// <param name="indices">The Indices that are needed to access the member of the 
        /// array.</param>
        public ArrayIndexerExpression(IExpression target, IExpressionCollection indices)
        {
            this.target = target;
            foreach (IExpression ie in indices)
                this.Indices.Add(ie);
        }

        protected ArrayIndexerExpression(SerializationInfo info, StreamingContext context)
        {
            this.target = _SerializationMethods.DeserializeExpression("Target", info);
            this.indices = _SerializationMethods.DeserializeExpressionCollection("Indices", info);
        }

        #region IArrayIndexerExpression Members

        /// <summary>
        /// Returns a series of expressions that relate to the indices that access the members
        /// of the array.
        /// </summary>
        public IExpressionCollection Indices
        {
            get {
                if (indices == null)
                    indices = new ExpressionCollection();
                return this.indices;
            }
        }

        public IExpression Reference
        {
            get { return this.target; }
        }

        #endregion

        public override CodeArrayIndexerExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            CodeArrayIndexerExpression caie = base.GenerateCodeDom(options);
            caie.TargetObject = target.GenerateCodeDom(options);
            if (this.indices != null)
                caie.Indices.AddRange(this.Indices.GenerateCodeDom(options));
            return caie;
        }

        /// <summary>
        /// Performs a look-up on the <see cref="ArrayIndexerExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="ArrayIndexerExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.target != null)
                this.target.GatherTypeReferences(ref result, options);
            if (this.indices != null)
                this.indices.GatherTypeReferences(ref result, options);
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _SerializationMethods.SerializeExpression("Target", info, this.target);
            _SerializationMethods.SerializeExpressionCollection("Indices", info, this.indices);
        }

        #endregion
    }
}
