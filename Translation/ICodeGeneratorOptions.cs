using System;
using System.Collections.Generic;
using System.Text;
using Oilexer.Types;
using System.CodeDom.Compiler;

namespace Oilexer.Translation
{
    /// <summary>
    /// Defines properties and methods for aiding in the generation of either
    /// code or code hierarchy objects.
    /// </summary>
    public interface ICodeTranslationOptions
    {
        /// <summary>
        /// Returns whether to resolve import references when types are encountered.
        /// </summary>
        bool AutoResolveReferences { get; set; }

        /// <summary>
        /// Returns the <see cref="ICodeDOMGeneratorNameHandler"/> that translates the <see cref="IDeclaration"/>
        /// names as necessary.
        /// </summary>
        ICodeGeneratorNameHandler NameHandler { get; }

        /// <summary>
        /// Returns the namespace import list.
        /// </summary>
        ICollection<string> ImportList { get; }

        /// <summary>
        /// Returns/sets the current scope of the generation process.
        /// </summary>
        INameSpaceDeclaration CurrentNameSpace { get; set; }

        IDeclaredType CurrentType { get; set; }

        /// <summary>
        /// Returns the trail of entities being generated.
        /// </summary>
        Stack<IDeclarationTarget> BuildTrail { get; }

        /// <summary>
        /// Returns/sets whether auto-regions are generated based upon member types.
        /// </summary>
        AutoRegionAreas AutoRegions { get; set; }

        /// <summary>
        /// Returns/sets whether regions are allowed in the generation process.
        /// </summary>
        bool AllowRegions { get; set; }

        /// <summary>
        /// Returns/sets whether types are allowed to be spanned across multiple instances.
        /// </summary>
        bool AllowPartials { get; set; }

        /// <summary>
        /// Returns whether the <see cref="ICodeGeneratorOptions"/> supports regions for the given
        /// <paramref name="area"/>.
        /// </summary>
        /// <param name="area">The <see cref="AutoRegionAreas"/> that the <see cref="ICodeGeneratorOptions"/>
        /// supports given the current state of the <see cref="ICodeGeneratorOptions"/>.</param>
        /// <returns>true, if the <see cref="ICodeGeneratorOptions"/> implementation supports
        /// auto-regioning for the <paramref name="area"/> given; false, otherwise.</returns>
        bool AutoRegionsFor(AutoRegionAreas area);

        /// <summary>
        /// Returns/sets whether the auto-documentation comments are handled during the 
        /// generation process.
        /// </summary>
        bool AutoComments { get; set; }

    }
}
