using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

namespace Oilexer.Types.Members
{
    public interface IIndexerMember :
        IParameteredDeclaration<IIndexerParameterMember, CodeMemberProperty, IIndexerMember>,
        IPropertyMember
    {
        /// <summary>
        /// Returns the parameters of the <see cref="IIndexerMember"/>.
        /// </summary>
        /// <returns>An instance of an implementation of  <see cref="IIndexerParameterMembers"/> 
        /// which denote the parameters of the <see cref="IIndexerMember"/>.</returns>
        new IIndexerParameterMembers Parameters { get; }
        /// <summary>
        /// Returns the name of the <see cref="IIndexerMember"/>.
        /// </summary>
        /// <exception cref="System.NotSupportedException">When <see cref="IDeclarationTarget.Name"/> is set.</exception>
        new string Name { get; }
    }
}
