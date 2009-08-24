using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Utilities.Collections;
using Oilexer.Expression;

namespace Oilexer.Types
{
    /// <summary>
    /// Defines properties and methods for working with a declared member's string table.
    /// </summary>
    public interface IDeclarationResourcesStringTable :
        IControlledStateDictionary<string, IDeclarationResourcesStringTableEntry>
    {
        IDeclarationResourcesStringTableEntry Add(string name, string value);
        void Remove(string name);
        void Clear();
        IDeclarationResourcesStringTable GetPartialClone(IDeclarationResources targetDeclaration);
        /// <summary>
        /// Instructs the string table to rebuild the members because the generation type changed.
        /// </summary>
        /// <param name="newGenerationType"></param>
        void Rebuild();
    }
}
