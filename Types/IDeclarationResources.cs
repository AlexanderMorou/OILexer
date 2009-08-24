using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Oilexer.Types.Members;
using System.Resources;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with the resources for a 
    /// <see cref="IDeclarationTarget"/>.
    /// </summary>
    public interface IDeclarationResources :
        IClassType
    {
        /// <summary>
        /// Returns/sets the means to which the information for the resourcable declaration
        /// is stored during code-generation.
        /// </summary>
        ResourceGenerationType GenerationType { get; set; }
        /// <summary>
        /// Returns/sets the string table which relates to the data used by the 
        /// <see cref="IResourceableDeclaration"/>.
        /// </summary>
        IDeclarationResourcesStringTable StringTable { get; }
        [Browsable(false),
         EditorBrowsable(EditorBrowsableState.Never)]
        new IClassTypes Classes { get; }
        [Browsable(false),
         EditorBrowsable(EditorBrowsableState.Never)]
        new IDelegateTypes Delegates { get; }
        [Browsable(false),
         EditorBrowsable(EditorBrowsableState.Never)]
        new IEnumeratorTypes Enumerators { get; }
        [Browsable(false),
         EditorBrowsable(EditorBrowsableState.Never)]
        new IInterfaceTypes Interfaces { get; }
        [Browsable(false),
         EditorBrowsable(EditorBrowsableState.Never)]
        new IStructTypes Structures { get; }
        [Browsable(false),
         EditorBrowsable(EditorBrowsableState.Never)]
        new int GetTypeCount();
        [Browsable(false),
         EditorBrowsable(EditorBrowsableState.Never)]
        new IDeclarationResources Resources { get; }
        new IDeclarationResources GetRootDeclaration();
        /// <summary>
        /// Returns/sets the resource manager property.
        /// </summary>
        IPropertyMember ResourceManager { get; }
        /// <summary>
        /// Writes the resources of the <see cref="IDeclarationResources"/> to a temporary
        /// file that contains the proper name.
        /// </summary>
        /// <returns>A string denoting the temporary file with the resources.</returns>
        string WriteResources();
    }
}
