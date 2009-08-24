using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;
using System.CodeDom;
using Oilexer.Translation;

namespace Oilexer.Types
{
    /// <summary>
    /// A delegate declaration.
    /// </summary>
    [Serializable]
    public partial class DelegateType :
        ParameteredDeclaredType<IDelegateType, CodeTypeDelegate>,
        IDelegateType
    {

        #region DelegateType Data Members

        private IDelegateTypeParameterMembers parameters;
        /// <summary>
        /// Data member for <see cref="ReturnType"/>.
        /// </summary>
        private ITypeReference returnType;
        #endregion

        #region DelegateType Constructors
        /// <summary>
        /// Creates a new instance of <see cref="DelegateType"/> with the name and
        /// <paramref name="parentTarget"/> provided.
        /// </summary>
        /// <param name="name">The name of the <see cref="DelegateType"/>.</param>
        /// <param name="parentTarget">The <see cref="ITypeParent"/> which contains the <see cref="DelegateType"/>.</param>
        protected internal DelegateType(string name, ITypeParent parentTarget)
            : base(name, parentTarget)
        {
            CheckModule();
        }

        #endregion
        /// <summary>
        /// Generates the <see cref="CodeTypeDelegate"/> that represents the <see cref="DelegateType"/>.
        /// </summary>
        /// <returns>A new instance of a <see cref="CodeTypeDelegate"/> if successful.-null- otherwise.</returns>
        public override CodeTypeDelegate GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            CodeTypeDelegate result = base.GenerateCodeDom(options);
            result.Parameters.AddRange(this.Parameters.GenerateCodeDom(options));
            if (this.returnType != null)
                result.ReturnType = this.ReturnType.GenerateCodeDom(options);
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            return result;
        }
        #region IDelegateType Members

        public IDelegateTypeParameterMembers Parameters
        {
            get
            {
                if (this.parameters == null)
                    this.parameters = new DelegateTypeParameterMembers(this);
                return this.parameters;
            }
        }

        #endregion

        #region IParameteredDeclaration<IDelegateParameterMember,CodeTypeDelegate,ITypeParent> Members

        IParameteredParameterMembers<IDelegateTypeParameterMember, CodeTypeDelegate, ITypeParent> IParameteredDeclaration<IDelegateTypeParameterMember, CodeTypeDelegate, ITypeParent>.Parameters
        {
            get { return this.Parameters; }
        }

        #endregion


        #region IDelegateType Members


        public ITypeReference ReturnType
        {
            get
            {
                return this.returnType;
            }
            set
            {
                this.returnType = value;
            }
        }

        #endregion

    }
}
