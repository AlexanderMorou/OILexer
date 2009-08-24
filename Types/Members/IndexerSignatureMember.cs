using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Properties;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class IndexerSignatureMember :
        PropertySignatureMember,
        IIndexerSignatureMember
    {
        private IIndexerSignatureParameterMembers parameters;
        public IndexerSignatureMember(ITypeReference indexerType, ISignatureMemberParentType parentTarget)
            : base(new TypedName(Resources.IndexerName, indexerType), parentTarget)
        {

        }
        public IndexerSignatureMember(string name, ITypeReference indexerType, ISignatureMemberParentType parentTarget)
            : base(new TypedName(name, indexerType), parentTarget)
        {

        }

        public override CodeMemberProperty GenerateCodeDom(ICodeDOMTranslationOptions options)
        {
            if (options.BuildTrail != null)
                options.BuildTrail.Push(this);
            CodeMemberProperty result = base.GenerateCodeDom(options);
            result.Parameters.AddRange(this.Parameters.GenerateCodeDom(options));
            if (options.BuildTrail != null)
                options.BuildTrail.Pop();
            return result;
        }

        #region IIndexerSignatureMember Members

        public IIndexerSignatureParameterMembers Parameters
        {
            get
            {
                if (this.parameters == null)
                    InitializeParameters();
                return this.parameters;
            }
        }

        #endregion

        private void InitializeParameters()
        {
            this.parameters = new IndexerSignatureParameterMembers(this);
        }

        #region IParameteredDeclaration<IIndexerSignatureParameterMember,CodeMemberProperty,IIndexerSignatureMember> Members

        IParameteredParameterMembers<IIndexerSignatureParameterMember, System.CodeDom.CodeMemberProperty, IIndexerSignatureMember> IParameteredDeclaration<IIndexerSignatureParameterMember, System.CodeDom.CodeMemberProperty, IIndexerSignatureMember>.Parameters
        {
            get { return this.Parameters; }
        }

        #endregion

        /// <summary>
        /// Performs a look-up on the <see cref="IndexerSignatureMember"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="IndexerSignatureMember"/> relies on.</param>
        /// <param name="options">The <see cref="ICodeTranslationOptions"/> which is used to 
        /// guide the gathering process.</param>
        public override void GatherTypeReferences(ref ITypeReferenceCollection result, ICodeTranslationOptions options)
        {
            if (result == null)
                result = new TypeReferenceCollection();
            base.GatherTypeReferences(ref result, options);
            if (this.parameters != null)
                this.Parameters.GatherTypeReferences(ref result, options);
        }
    }
}
