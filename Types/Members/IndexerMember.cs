using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using Oilexer.Properties;
using Oilexer.Translation;

namespace Oilexer.Types.Members
{
    [Serializable]
    public class IndexerMember :
        PropertyMember,
        IIndexerMember
    {
        private IIndexerParameterMembers parameters;
        public IndexerMember(ITypeReference indexerType, IMemberParentType parentTarget)
            : base(new TypedName(Resources.IndexerName, indexerType), parentTarget)
        {
        }

        public IndexerMember(string name, ITypeReference indexerType, IMemberParentType parentTarget)
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

        #region IIndexerMember Members

        public IIndexerParameterMembers Parameters
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
            this.parameters = new IndexerParameterMembers(this);
        }

        #region IParameteredDeclaration<IIndexerParameterMember,CodeMemberProperty,IIndexerMember> Members

        IParameteredParameterMembers<IIndexerParameterMember, System.CodeDom.CodeMemberProperty, IIndexerMember> IParameteredDeclaration<IIndexerParameterMember, System.CodeDom.CodeMemberProperty, IIndexerMember>.Parameters
        {
            get { return this.Parameters; }
        }

        #endregion
        /// <summary>
        /// Returns a unique identifier for the <see cref="IndexerMember"/>.
        /// </summary>
        /// <returns>A unique <see cref="System.String"/> pertinent to the 
        /// <see cref="IndexerMember"/>.</returns>
        public override string GetUniqueIdentifier()
        {
            string signature = this.Name;
            IIndexerParameterMembers parameters = this.Parameters;
            /* *
             * Add the parameters, use the parameter type information to yield a type-signature.
             * Names aren't as important because they are more likely to repeat in a type-strict
             * system that translates various types into an common understanding underneath.
             * */
            if (parameters.Count > 0)
            {
                string[] names = new string[parameters.Count];
                for (int i = 0; i < parameters.Count; i++)
                    /* *
                     * ToDo: Add 'GetTypeName' to type-references, stand-alone types
                     * may not contain necessary type-arguments.
                     * */
                    names[i] = parameters.Values[i].ParameterType.TypeInstance.
                        GetTypeName(CodeGeneratorHelper.DefaultDomOptions);
                signature += string.Format("({0})", string.Join(", ", names)); ;
            }
            else
                signature += "()";
            if (this.PrivateImplementationTarget != null)
                return string.Format("{0} on {1}", signature, this.PrivateImplementationTarget.TypeInstance.GetTypeName(CodeGeneratorHelper.DefaultDomOptions));
            else
                return signature;
        }
        /// <summary>
        /// Returns the name of the <see cref="IndexerMember"/>.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Thrown when the property is set.</exception>
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                throw new NotSupportedException("Indexers are fixed in name.");
            }
        }
        /// <summary>
        /// Performs a look-up on the <see cref="IndexerMember"/> to determine its 
        /// dependencies.
        /// </summary>
        /// <param name="result">A <see cref="ITypeReferenceCollection"/> which
        /// relates to the <see cref="ITypeReference"/> instance implementations
        /// that the <see cref="IndexerMember"/> relies on.</param>
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
