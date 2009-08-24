using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Expression;
using Oilexer._Internal;
using Oilexer.Utilities.Collections;
using Oilexer.Translation;

namespace Oilexer.Types
{
    [Serializable]
    public class TypeReferenceBase :
        ITypeReference
    {
        #region TypeReferenceBase Data members
        /// <summary>
        /// Data member for <see cref="TypeInstance"/>.
        /// </summary>
        protected IType typeInstance;

        /// <summary>
        /// Data member for <see cref="ArrayElementType"/>.
        /// </summary>
        private ITypeReference arrayElementType;

        /// <summary>
        /// Data member for <see cref="TypeParameters"/>.
        /// </summary>
        private TypeReferenceCollection typeParameters;

        /// <summary>
        /// Data member for <see cref="ArrayRank"/>.
        /// </summary>
        private int arrayRank;

        /// <summary>
        /// Data member for <see cref="Nullable"/>.
        /// </summary>
        private bool nullable;

        /// <summary>
        /// Data member for <see cref="PointerRank"/>.
        /// </summary>
        private int pointerRank;

        /// <summary>
        /// Data member for <see cref="ResolutionOptions"/>.
        /// </summary>
        private TypeReferenceResolveOptions resolutionOptions;

        #endregion

        #region TypeReferenceBase Constructors
        protected TypeReferenceBase(IType typeInstance)
            : this(typeInstance, (ITypeReferenceCollection)null)
        {
        }
        protected TypeReferenceBase(IType typeInstance, ITypeReferenceCollection typeParameters)
        {
            this.typeInstance = typeInstance;
            if (typeParameters != null)
                this.TypeParameters.AddRange(typeParameters.ToArray());
        }

        protected TypeReferenceBase(ITypeReference arrayElementType, int arrayRank)
        {
            this.arrayRank = arrayRank;
            this.arrayElementType = arrayElementType;
        }
        #endregion

        #region ITypeReference Members

        /// <summary>
        /// Returns the <see cref="IType"/> which the <see cref="ITypeReference"/> represents.
        /// </summary> 
        /// <returns>The <see cref="typeReference"/> if <see cref="ArrayRank"/> is zero or <see cref="ArrayElementType"/> is null.</returns>
        public IType TypeInstance
        {
            get
            {
                if (this.ArrayRank > 0 && this.ArrayElementType != null)
                    return this.ArrayElementType.TypeInstance;
                return this.typeInstance;
            }
            set
            {
                if (this.ArrayRank > 0 && this.ArrayElementType != null)
                    this.ArrayElementType.TypeInstance = value;
                else
                    this.typeInstance = value;
            }
        }

        /// <summary>
        /// Returns the <see cref="CodeTypeReference"/> which links back to the
        /// <see cref="TypeInstance"/>.
        /// </summary>
        /// <returns>A <see cref="CodeTypeReference"/> which links back to the <see cref="TypeInstance"/>.</returns>
        public virtual CodeTypeReference GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (this.ArrayElementType != null && this.ArrayRank > 0)
                return new CodeTypeReference(this.ArrayElementType.GenerateCodeDom(options), this.ArrayRank);
            else
            {
                CodeTypeReference ctr = new CodeTypeReference();
                CodeTypeReference n = null;
                if (nullable)
                    n = new CodeTypeReference(typeof(Nullable<>));
                bool autoResolve = false;
                if ((this.resolutionOptions & TypeReferenceResolveOptions.GlobalType) == TypeReferenceResolveOptions.GlobalType)
                    ctr.Options |= CodeTypeReferenceOptions.GlobalReference;
                if ((this.resolutionOptions & TypeReferenceResolveOptions.FullType) == TypeReferenceResolveOptions.FullType || (this.resolutionOptions & TypeReferenceResolveOptions.GlobalType) == TypeReferenceResolveOptions.GlobalType)
                {
                    autoResolve = options.AutoResolveReferences;
                    if (autoResolve)
                        options.AutoResolveReferences = false;
                }
                if ((this.resolutionOptions & TypeReferenceResolveOptions.TypeParameter) == TypeReferenceResolveOptions.TypeParameter)
                    ctr.Options |= CodeTypeReferenceOptions.GenericTypeParameter;
                ctr.BaseType = this.TypeInstance.GetTypeName(options, TypeParameters.ToArray());
                if (autoResolve && ((this.resolutionOptions & TypeReferenceResolveOptions.FullType) == TypeReferenceResolveOptions.FullType || (this.resolutionOptions & TypeReferenceResolveOptions.GlobalType) == TypeReferenceResolveOptions.GlobalType))
                    options.AutoResolveReferences = autoResolve;
                //Clear auto-inferred entries.
                ctr.TypeArguments.Clear();
                foreach (ITypeReference itr in this.TypeParameters)
                    if (itr !=null)
                        ctr.TypeArguments.Add(itr.GenerateCodeDom(options));
                if (nullable)
                {
                    n.TypeArguments.Add(ctr);
                    ctr = n;
                }
                //Pointers: Not supported by CodeDOM.
                return ctr;
            }
        }

        #endregion

        #region ITypeReference Members



        public int PointerRank
        {
            get
            {
                if (this.ArrayRank > 0 && this.ArrayElementType != null)
                    return this.ArrayElementType.PointerRank;
                return this.pointerRank;
            }
            set
            {
                if (this.ArrayRank > 0 && this.ArrayElementType != null)
                    this.ArrayElementType.PointerRank = value;
                this.pointerRank = value;
            }
        }

        public bool Nullable
        {
            get
            {
                if (this.ArrayRank > 0 && this.ArrayElementType != null)
                    return this.ArrayElementType.Nullable;
                return this.nullable;
            }
            set
            {
                if (this.ArrayRank > 0 && this.ArrayElementType != null)
                    this.ArrayElementType.Nullable = value;
                this.nullable = value;
            }
        }

        public int ArrayRank
        {
            get
            {
                return this.arrayRank;
            }
            set
            {
                this.arrayRank = value;
            }
        }

        public ITypeReference ArrayElementType
        {
            get
            {
                return this.arrayElementType;
            }
            set
            {
                this.arrayElementType = value;
            }
        }

        #endregion
        public static TypeReferenceBase MakeArray(ITypeReference typeRef, int arrayRank)
        {
            if (typeRef == null)
                throw new ArgumentNullException("typeRef");
            else if (arrayRank <= 0)
                throw new ArgumentOutOfRangeException("arrayRank");
            return new TypeReferenceBase(typeRef, arrayRank);
        }

        #region ITypeReference Members


        /// <summary>
        /// Returns the <see cref="ITypeReference"/> list containing data about the TypeParameters for the
        /// <see cref="TypeReferenceBase"/>.
        /// </summary>
        public ITypeReferenceCollection TypeParameters
        {
            get
            {
                if (this.ArrayElementType != null && this.ArrayRank > 0)
                    return this.ArrayElementType.TypeParameters;
                else if (this.typeParameters == null)
                    this.typeParameters = new TypeReferenceCollection();
                return this.typeParameters;
            }
        }

        #endregion

        #region ITypeReference Members
        public ICreateNewObjectExpression GetNewExpression(IExpressionCollection arguments)
        {
            return new CreateNewObjectExpression(this, arguments);
        }

        public IMemberParentExpression GetMemberExpression(bool isStatic)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IMemberParentExpression GetMemberExpression(IDeclaration link)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Returns a new <see cref="ITypeReference"/> as an array type reference given the rank provided.
        /// </summary>
        /// <param name="rank">The rank of the array.</param>
        /// <returns>A new <see cref="ITypeReference"/> as an array type reference given the rank provided.</returns>
        public virtual ITypeReference MakeArray(int rank)
        {
            return TypeReferenceBase.MakeArray(this, rank);
        }

        public TypeReferenceResolveOptions ResolutionOptions
        {
            get
            {
                return this.resolutionOptions;
            }
            set
            {
                this.resolutionOptions = value;
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (!(obj is ITypeReference))
                return false;
            return this.Equals((ITypeReference)obj);
        }

        public override string ToString()
        {
            if (this.ArrayElementType != null)
            {
                List<int> arrayRanks = new List<int>();
                
                ITypeReference arrayElementItem = this;
                while (arrayElementItem.ArrayElementType != null)
                {
                    arrayRanks.Add(arrayElementItem.ArrayRank);
                    arrayElementItem = arrayElementItem.ArrayElementType;
                }

                string[] arrayRankTexts = new string[arrayRanks.Count];
                for (int i = 0; i < arrayRankTexts.Length; i++)
                    arrayRankTexts[i] = GetArrayRankText(arrayRanks[i]);
                return String.Format("{0}{1}", arrayElementItem.ToString(), String.Join("", arrayRankTexts));
            }
            string pointer = string.Empty;
            for (int i = 0; i < this.pointerRank; i++)
                pointer += "*";
            string[] typeParamNames = new string[this.TypeParameters.Count];
            if (this.TypeParameters.Count > 0)
            {
                for (int i = 0; i < this.TypeParameters.Count; i++)
                    typeParamNames[i] = TypeParameters[i].ToString();
                return String.Format("{0}<{1}>{2}{3}", this.typeInstance.GetTypeName(CodeGeneratorHelper.DefaultDomOptions), String.Join(",", typeParamNames), Nullable ? "?" : string.Empty, pointer);
            }
            else
                return string.Format("{0}{1}{2}",this.typeInstance.GetTypeName(CodeGeneratorHelper.DefaultDomOptions), Nullable ? "?" : string.Empty, pointer);
        }

        private static string GetArrayRankText(int arrayRank)
        {
            string arrayData = "";
            for (int i = 0; i < arrayRank - 1; i++)
                arrayData += ",";
            return String.Format("[{0}]", arrayData);
        }
        public ITypeReferenceExpression GetTypeExpression()
        {
            return new TypeReferenceExpression(this);
        }

        #region IEquatable<ITypeReference> Members

        public bool Equals(ITypeReference other)
        {
            if (this.ArrayElementType != null && this.ArrayRank > 0 && other.ArrayElementType != null &&
                ((!this.ArrayElementType.Equals(other.ArrayElementType)) || (this.ArrayRank != other.ArrayRank)))
                    return false;
            if (this.TypeParameters.Count != other.TypeParameters.Count)
                return false;
            if (!(this.TypeInstance.Equals(other.TypeInstance)))
                return false;
            if (!this.Nullable && other.Nullable)
                return false;
            if (this.PointerRank != other.PointerRank)
                return false;
            for (int i = 0; i < this.TypeParameters.Count; i++)
                if (!(this.TypeParameters[i].Equals(other.TypeParameters[i])))
                    return false;
            return true;
        }

        #endregion

        public override int GetHashCode()
        {
            if (this.ArrayRank == 0 || this.ArrayElementType == null)
                return this.typeInstance.GetHashCode() ^ this.ResolutionOptions.GetHashCode() ^ this.GetTypeParamHash();
            return this.ArrayRank.GetHashCode() ^ this.ArrayElementType.GetHashCode();
        }

        private int GetTypeParamHash()
        {
            int result = 0;
            foreach (ITypeReference itr in this.TypeParameters)
                result ^= itr.GetHashCode();
            return result;
        }



        #region ITypeReference Members

        public bool ByRef { get; set; }

        #endregion
    }
}
