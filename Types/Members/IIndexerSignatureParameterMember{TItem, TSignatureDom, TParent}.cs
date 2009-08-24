using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Reflection;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// Defines properties and methods for working with the parameter member of a generic-form
    /// indexer property.
    /// </summary>
    /// <typeparam name="TParameter">The type the indexer uses to define
    /// its parameter data.</typeparam>
    /// <typeparam name="TSignatureDom">The type of Dom object yielded by the signature.</typeparam>
    /// <typeparam name="TParent">The parent of the parameter instances.</typeparam>
    public interface IIndexerSignatureParameterMember<TParameter, TSignatureDom, TParent> :
        IParameteredParameterMember<TParameter, TSignatureDom, TParent>//,
        //IFauxableReliant<ParameterInfo, Type>
        where TParameter :
            IIndexerSignatureParameterMember<TParameter, TSignatureDom, TParent>
        where TSignatureDom :
            CodeMemberProperty
        where TParent :
            IDeclarationTarget
    {
    }
}
