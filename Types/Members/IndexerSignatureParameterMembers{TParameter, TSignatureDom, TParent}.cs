using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// A generic-form indexer property's parameter members.
    /// </summary>
    /// <typeparam name="TParameter">The type the indexer uses to define its parameter 
    ///     data.</typeparam>
    /// <typeparam name="TSignatureDom">The type of Dom object yielded by the signature.</typeparam>
    /// <typeparam name="TParent">The parent of the parameter instances.</typeparam>
    [Serializable]
    public abstract class IndexerSignatureParameterMembers<TParameter, TSignatureDom, TParent> :
        ParameteredParameterMembers<TParameter, TSignatureDom, TParent>,
        IIndexerSignatureParameterMembers<TParameter, TSignatureDom, TParent>
        where TParameter :
            IIndexerSignatureParameterMember<TParameter, TSignatureDom, TParent>
        where TSignatureDom :
            CodeMemberProperty
        where TParent :
            IDeclarationTarget
    {
        /// <summary>
        /// Creates a new instance of <see cref="IndexerSignatureParameterMembers{TParameter, TSignatureDom, TParent}"/> with
        /// the <paramref name="targetDeclaration"/> provided.
        /// </summary>
        /// <param name="targetDeclaration">The <see cref="IParameteredDeclaration{TParameter, TSignatureDom, TParent}"/> which the 
        /// <see cref="IndexerSignatureParameterMembers{TParameter, TSignatureDom, TParent}"/> belongs to.</param>
        public IndexerSignatureParameterMembers(IParameteredDeclaration<TParameter, TSignatureDom, TParent> targetDeclaration)
            : base(targetDeclaration)
        {
        }

        protected override IMembers<TParameter, IParameteredDeclaration<TParameter, TSignatureDom, TParent>, CodeParameterDeclarationExpression> OnGetPartialClone(IParameteredDeclaration<TParameter, TSignatureDom, TParent> parent)
        {
            throw new NotSupportedException("Indexers aren't segmentable.");
        }
    }
}
