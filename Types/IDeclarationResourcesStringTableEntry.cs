using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types.Members;

namespace Oilexer.Types
{
    public interface IDeclarationResourcesStringTableEntry :
        IDisposable
    {
        /// <summary>
        /// Returns/sets the name of the <see cref="IDeclarationResourcesStringTableEntry"/>.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Returns/sets the value of the <see cref="IDeclarationResourcesStringTableEntry"/>.
        /// </summary>
        string Value { get; set; }
        /// <summary>
        /// Returns the data member used to store the value of the <see cref="IDeclarationResourcesStringTableEntry"/>.
        /// </summary>
        IFieldMember DataMember { get; }
        /// <summary>
        /// Returns the auto-generated member used to retrieve the value of the <see cref="IDeclarationResourcesStringTableEntry"/>.
        /// </summary>
        IPropertyMember AutoMember { get; }
        /// <summary>
        /// Rebuilds the <see cref="AutoMember"/> based on whether there is to be a
        /// <see cref="DataMember"/> cache used.
        /// </summary>
        void Rebuild();
    }
}
