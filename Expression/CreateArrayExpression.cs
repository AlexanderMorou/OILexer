using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.CodeDom;
using System.Runtime.Serialization;
using Oilexer._Internal;
using Oilexer.Translation;

namespace Oilexer.Expression
{
    [Serializable]
    public partial class CreateArrayExpression :
        Expression<CodeArrayCreateExpression>,
        ICreateArrayExpression,
        ISerializable
    {
        private int size;
        private IExpression sizeExpression;
        private IExpressionCollection initializers;
        private ITypeReference arrayType;
        public CreateArrayExpression(ITypeReference arrayType, int size)
        {
            this.size = size;
            this.ArrayType = arrayType;
        }

        protected CreateArrayExpression(SerializationInfo info, StreamingContext context)
        {
            SerializationType method = (SerializationType)info.GetValue("SerializationMeans", typeof(SerializationType));
            bool sizeExp = ((method & SerializationType.SizeExpression) == SerializationType.SizeExpression);
            bool sizePrim = ((method & SerializationType.SizePrimitive) == SerializationType.SizePrimitive) && !sizeExp;
            bool initializers = ((method & SerializationType.Initializers) == SerializationType.Initializers);
            if (sizeExp)
                this.sizeExpression = _SerializationMethods.DeserializeExpression("SizeExpression", info);
            else if (sizePrim)
                this.size = info.GetInt32("Size");
            if (initializers)
                this.initializers = _SerializationMethods.DeserializeExpressionCollection("Initializers", info);
            this.arrayType = _SerializationMethods.DeserializeTypeReference("ArrayType", info);
        }

        public CreateArrayExpression(ITypeReference arrayType, IExpression[] initializers)
        {
            this.initializers = new ExpressionCollection(initializers);
            this.ArrayType = arrayType;
        }

        public CreateArrayExpression(ITypeReference arrayType, IExpression sizeExpression)
        {
            this.sizeExpression = sizeExpression;
            this.ArrayType = arrayType;
        }
        public CreateArrayExpression(IType arrayType, int size)
            :this(arrayType.GetTypeReference(),size)
        {
        }

        public CreateArrayExpression(IType arrayType, IExpression[] initializers)
            : this(arrayType.GetTypeReference(), initializers)
        {
        }

        public CreateArrayExpression(IType arrayType, IExpression sizeExpression)
            : this(arrayType.GetTypeReference(), sizeExpression)
        {
        }
        public override CodeArrayCreateExpression GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            CodeArrayCreateExpression result = new CodeArrayCreateExpression();
            result.CreateType = this.ArrayType.GenerateCodeDom(options);
            if (this.initializers != null && this.initializers.Count > 0)
                result.Initializers.AddRange(this.initializers.GenerateCodeDom(options));
            if (this.sizeExpression != null)
                result.SizeExpression = this.sizeExpression.GenerateCodeDom(options);
            else
                result.Size = this.size;
            return result;
        }
        #region ICreateArrayExpression Members

        public IExpression SizeExpression
        {
            get
            {
                return this.sizeExpression;
            }
            set
            {
                this.SizeExpression = value;
                if (value is PrimitiveExpression)
                {
                    PrimitiveExpression primExp = value as PrimitiveExpression;
                    if (primExp.Value is int || primExp.Value is uint ||
                        primExp.Value is byte || primExp.Value is sbyte ||
                        primExp.Value is long || primExp.Value is ulong ||
                        primExp.Value is short || primExp.Value is ushort)
                        this.size = (int)primExp.Value;
                }
            }
        }

        public int Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
                if (this.sizeExpression is PrimitiveExpression)
                    (sizeExpression as PrimitiveExpression).Value = value;
                else
                    this.sizeExpression = new PrimitiveExpression(value);
            }
        }

        public IExpressionCollection Initializers
        {
            get { 
                return this.initializers; }
        }

        public ITypeReference ArrayType
        {
            get
            {
                return arrayType;
            }
            set
            {
                this.arrayType = value;
            }
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="CreateArrayExpression"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="CreateArrayExpression"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            if (this.initializers != null)
                this.Initializers.GatherTypeReferences(ref result, options);
            if (this.sizeExpression != null)
                this.SizeExpression.GatherTypeReferences(ref result, options);
            result.Add(this.arrayType);
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationType method =  SerializationType.None;
            if (this.sizeExpression is PrimitiveExpression)
            {
                PrimitiveExpression primExp = this.sizeExpression as PrimitiveExpression;
                if (primExp.Value is int || primExp.Value is uint ||
                    primExp.Value is byte || primExp.Value is sbyte ||
                    primExp.Value is long || primExp.Value is ulong ||
                    primExp.Value is short || primExp.Value is ushort)
                {
                    info.AddValue("Size", (int)primExp.Value);
                    method |= SerializationType.SizePrimitive;
                }
                else
                {
                    method |= SerializationType.SizeExpression;
                    _SerializationMethods.SerializeExpression("SizeExpression", info, this.sizeExpression);
                }
            }
            else if (this.sizeExpression != null)
            {
                method |= SerializationType.SizeExpression;
                _SerializationMethods.SerializeExpression("SizeExpression", info, this.sizeExpression);
            }
            if (this.initializers != null && this.initializers.Count > 0)
            {
                method |= SerializationType.Initializers;
                _SerializationMethods.SerializeExpressionCollection("Initializers", info, this.initializers);            
            }
            info.AddValue("SerializationMeans", method, typeof(SerializationType));
            _SerializationMethods.SerializeTypeReference("ArrayType", info, this.arrayType);
        }

        #endregion
    }
}
