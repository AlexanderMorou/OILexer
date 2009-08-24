using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Expression;
using System.Diagnostics;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    public partial class TypeParameterMember<TDom, TParent> :
        Member<TParent, TDom>,
        ITypeParameterMember<TDom, TParent>,
        IType
        where TParent :
            IDeclaration
        where TDom :
            CodeTypeParameter,
            new()
    {
        /// <summary>
        /// Data member for <see cref="DocumentationComment"/>.
        /// </summary>
        private string documentationComment;
        private TypeParameterSpecialCondition specialCondition;

        /// <summary>
        /// Data member for <see cref="RequiresConstructor"/>
        /// </summary>
        private bool requiresConstructor;

        /// <summary>
        /// Data member for <see cref="Constraints"/>.
        /// </summary>
        private ITypeReferenceCollection constraints;

        /// <summary>
        /// Creates a new instance of <see cref="TypeParameterMember{TDom, TParent}"/>
        /// </summary>
        /// <param name="name">The name of the declaration.</param>
        /// <param name="parentTarget">The <typeparamref name="TParent"/> the 
        /// <see cref="TypeParameterMember{TDom, TParent}"/> exists as a sub-member of.</param>
        protected internal TypeParameterMember(string name, TParent parentTarget)
            : base(name, parentTarget)
        {
        }

        public override TDom GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            TDom tdResult = new TDom();
            if (options.NameHandler.HandlesName(this))
                tdResult.Name = options.NameHandler.HandleName(this);
            else
                tdResult.Name = this.Name;
            tdResult.HasConstructorConstraint = this.requiresConstructor;

            foreach (ITypeReference itr in this.Constraints)
            {
                CodeTypeReference ctr = new CodeTypeReference();
                tdResult.Constraints.Add(itr.GenerateCodeDom(options));
            }
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            return tdResult;
        }

        #region ITypeParameterMember<TDom, TParent> Members

        /// <summary>
        /// <para>Returns/sets whether the <see cref="TypeParameterMember{TDom, TParent}"/> contains the
        /// empty constructor constraint.</para>
        /// <seealso cref=""/>
        /// </summary>
        /// <remarks><code>new()</code></remarks>
        public bool RequiresConstructor
        {
            get
            {
                return this.requiresConstructor;
            }
            set
            {
                this.requiresConstructor = value;
            }
        }

        /// <summary>
        /// Returns the constraints set forth on the <see cref="TypeParameterMember{TDom, TParent}"/>.
        /// </summary>
        /// <remarks>Due to limitations in the CodeDOM framework, <see cref="ValueType"/> and 
        /// <see cref="Object"/> as 'struct' and 'class' are not valid constraints.</remarks>
        public ITypeReferenceCollection Constraints
        {
            get
            {
                if (this.constraints == null)
                    this.constraints = new TypeReferenceCollection();
                return this.constraints;
            }
        }

        #endregion

        #region IType Members

        public ITypeReference GetTypeReference()
        {
            return new ParameterTypeReference(this);
        }

        public ITypeReference GetTypeReference(ITypeReferenceCollection typeParameters)
        {
            throw new NotSupportedException("non-generic type");
        }

        public ITypeReference GetTypeReference(params ITypeReference[] typeReferences)
        {
            return this.GetTypeReference(new TypeReferenceCollection(typeReferences));
        }

        public ITypeReference GetTypeReference(params IType[] typeReferences)
        {
            return this.GetTypeReference((ITypeReferenceCollection)null);
        }

        public ITypeReference GetTypeReference(params object[] typeReferences)
        {
            return this.GetTypeReference((ITypeReferenceCollection)null);
        }

        public bool IsClass
        {
            get { return false; }
        }

        public bool IsInterface
        {
            get { return false; }
        }

        public bool IsEnumerator
        {
            get { return false; }
        }

        public bool IsDelegate
        {
            get { return false; } 
        }

        public bool IsStructure
        {
            get { return false; }
        }


        public bool IsGeneric
        {
            get
            {
                return false;
            }
        }

        public bool IsGenericTypeDefinition
        {
            get { return false; }
        }

        #endregion

        protected override IMemberReferenceExpression OnGetReference()
        {
            throw new NotSupportedException("Type-parameters are not instances.");
        }

        #region IEquatable<IType> Members

        public bool Equals(IType other)
        {
            if (!(other is TypeParameterMember<TDom, TParent>))
                return false;
            return other == this;
        }

        #endregion

        #region IType Members

        public string GetTypeName(ICodeTranslationOptions options, bool commentStyle)
        {
            return this.GetTypeName(options, commentStyle, new ITypeReference[0]);
        }

        public string GetTypeName(ICodeTranslationOptions options, bool commentStyle, ITypeReference[] typeParameterValues)
        {
            if (options.NameHandler.HandlesName(this))
                return options.NameHandler.HandleName(this);
            else
                return this.Name;
        }

        public string GetTypeName(ICodeTranslationOptions options)
        {
            return GetTypeName(options, false, new ITypeReference[0]);
        }

        public string GetTypeName(ICodeTranslationOptions options, ITypeReference[] typeParameterValues)
        {
            return this.GetTypeName(options, false, typeParameterValues);
        }

        #endregion

        #region IFauxable<Type> Members

        public Type GetFauxCast()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region ITypeParameterMember<TDom,TParent> Members

        public TypeParameterSpecialCondition SpecialCondition
        {
            get
            {
                return this.specialCondition;
            }
            set
            {
                this.specialCondition = value;
            }
        }

        #endregion


        /// <summary>
        /// Performs a look-up on the <see cref="TypeParameterMember{TDom, TParent}"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="TypeParameterMember{TDom, TParent}"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.constraints != null)
                foreach (ITypeReference itr in this.Constraints)
                        result.Add(itr);
        }

        #region IAutoCommentFragmentMember Members

        public string DocumentationComment
        {
            get
            {
                return this.documentationComment;
            }
            set
            {
                this.documentationComment = value;
            }
        }

        #endregion
    }
}
