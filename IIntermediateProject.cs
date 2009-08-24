using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.Reflection;
using System.ComponentModel;
using System.CodeDom;
using System.Runtime.InteropServices;
using Oilexer.Types.Members;
using Oilexer.Translation;

namespace Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with an intermediate project file which 
    /// contains members spanned across a series of instances.
    /// </summary>
    public interface IIntermediateProject :
        ISegmentableDeclarationTarget<IIntermediateProject>,
        INameSpaceParent,
        ITypeParent,
        ITypeReferenceable,
        IResourceable,
        IAttributeDeclarationTarget
    {
        /// <summary>
        /// Generates the <see cref="CodeCompileUnit"/> that represents the <see cref="IDeclaration{TParent, TDom}"/>.
        /// </summary>
        /// <param name="options">The CodeDOM generator options that directs the generation
        /// process.</param>
        /// <returns>A new <see cref="CodeCompileUnit"/> instance if successful.-null- otherwise.</returns>
        new CodeCompileUnit GenerateCodeDom(ICodeDOMTranslationOptions options);
        /// <summary>
        /// Returns the partial elements of the <see cref="IIntermediateProject"/>.
        /// </summary>
        new IIntermediateProjectPartials Partials { get; }
        /// <summary>
        /// Returns the namespaces contained within the <see cref="IIntermediateProject"/>.
        /// </summary>
        INameSpaceDeclarations NameSpaces { get; }
        /// <summary>
        /// Returns/sets the default namespace used.
        /// </summary>
        INameSpaceDeclaration DefaultNameSpace { get; set; }
        /// <summary>
        /// Returns whether a namespace exits and denotes which specifically it is.
        /// </summary>
        /// <param name="nameSpace">The full path of the namespace to scan for.</param>
        /// <param name="foundSpace">The reference to the namespace to yield if a match
        /// is found.</param>
        /// <returns>true if a namespace with the <see cref="INameSpaceDeclaration.FullName"/> of <paramref name="nameSpace"/>
        /// exists in the <see cref="IIntermediateProject"/> scope; false, otherwise.</returns>
        bool NameSpaceExists(string nameSpace, ref INameSpaceDeclaration foundSpace);
        /// <summary>
        /// Returns the series of modules that makes up the (potentially) multi-file assembly.
        /// </summary>
        IIntermediateModules Modules { get; }
        /// <summary>
        /// Returns/sets the current module that newly created <see cref="IDeclaredType"/> 
        /// instance implementations default to.
        /// </summary>
        /// <remarks>Applies to top-level types only.</remarks>
        IIntermediateModule CurrentDefaultModule { get; set; }
        /// <summary>
        /// Returns the root (first) module of the <see cref="IIntermediateProject"/>.
        /// </summary>
        IIntermediateModule RootModule { get; }
        /// <summary>
        /// Obtains a namespace that matches the <paramref name="nameSpace"/> closest.
        /// </summary>
        /// <param name="nameSpace">Finds the <see cref="INameSpaceDeclaration"/> in the <see cref="IIntermediateProject"/>'s
        /// <see cref="NameSpaces"/>.</param>
        /// <returns>A <see cref="INameSpaceDeclaration"/> that matches <paramref name="nameSpace"/> 
        /// the closest.</returns>
        INameSpaceDeclaration GetClosestMatchingSpace(string nameSpace);
        INameSpaceDeclaration BuildOrMatchNameSpace(string nameSpace);
        INameSpaceDeclaration GetLocalPartial(INameSpaceDeclaration targetSpace);
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        new INameSpaceDeclaration GetRootNameSpace();
        /// <summary>
        /// Returns the <see cref="IMethodMember"/> which acts as an entrypoint.
        /// </summary>
        IMethodMember EntryPoint { get; set; }
        /// <summary>
        /// Returns the number of types declared nested in the <see cref="IIntermediateProject"/>.
        /// </summary>
        /// <param name="includePartials">Whether to include the members declared inside all instances
        /// of the <see cref="IIntermediateProject"/>.</param>
        /// <returns>A <see cref="System.Int32"/> value representing the number of members on the
        /// current instance or all instances based upon <paramref name="includePartials"/>.</returns>
        int GetTypeCount(bool includePartials);
        /// <summary>
        /// Returns/sets the type of assembly that is resulted from the 
        /// <see cref="IIntermediateProject"/>.
        /// </summary>
        ProjectOutputType OutputType { get; set; }
        /// <summary>
        /// Returns the <see cref="IIntermediateProjectInformation"/> which relates to the 
        /// standard set of assembly-level attributes that define the information for the 
        /// resulted assembly file.
        /// </summary>
        IIntermediateProjectInformation AssemblyInformation { get; }
    }
}
