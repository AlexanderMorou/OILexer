using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with a generic-form indexer property's 
    /// parameter members.
    /// </summary>
    /// <typeparam name="TParameter">The type the indexer uses to define
    /// its parameter data.</typeparam>
    /// <typeparam name="TSignatureDom">The type of Dom object yielded by the signature.</typeparam>
    /// <typeparam name="TParent">The parent of the parameter instances.</typeparam>
    public interface IIndexerSignatureParameterMembers<TParameter, TSignatureDom, TParent> :
        IParameteredParameterMembers<TParameter, TSignatureDom, TParent>
        where TParameter :
            IIndexerSignatureParameterMember<TParameter, TSignatureDom, TParent>
        where TSignatureDom :
            CodeMemberProperty 
        where TParent :
            IDeclarationTarget
    {
    }
}
