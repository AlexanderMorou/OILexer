using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    /// <summary>
    /// <para>Defines properties and methods for the signature of an indexer.</para>
    /// <seealso cref="IPropertySignatureMember"/>.
    /// </summary>
    public interface IIndexerSignatureMember :
        IPropertySignatureMember,
        IParameteredDeclaration<IIndexerSignatureParameterMember, CodeMemberProperty, IIndexerSignatureMember>
    {
        /// <summary>
        /// Returns the parameters of the <see cref="IIndexerSignatureMember"/>.
        /// </summary>
        /// <returns>An instance of an implementation of  <see cref="IIndexerSignatureParameterMembers"/> 
        /// which denote the parameters of the <see cref="IIndexerSignatureMember"/>.</returns>
        new IIndexerSignatureParameterMembers Parameters { get; }
    }
}
