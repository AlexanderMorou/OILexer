using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer
{
    /// <summary>
    /// Defines properties and methods for working with information about a project's
    /// resulted <see cref="IAssemblyInformation"/>.
    /// </summary>
    public interface IIntermediateProjectInformation :
        IAssemblyInformation
    {
        Guid ProjectGuid { get; set; }
        /// <summary>
        /// Returns/sets the name of the project.
        /// </summary>
        new string AssemblyName { get; set; }
        /// <summary>
        /// Returns/sets the title of the project.
        /// </summary>
        new string Title { get; set; }
        /// <summary>
        /// Returns/sets the description of the project.
        /// </summary>
        new string Description { get; set; }
        /// <summary>
        /// Returns/sets the company name of the project.
        /// </summary>
        new string Company { get; set; }
        /// <summary>
        /// Returns/sets the product name of the project.
        /// </summary>
        new string Product { get; set; }
        /// <summary>
        /// Returns/sets the copyright information of the project.
        /// </summary>
        new string Copyright { get; set; }
        /// <summary>
        /// Returns/sets the trademark of the project.
        /// </summary>
        new string Trademark { get; set; }
        /// <summary>
        /// Returns/sets the culture, relative to the <see cref="CultureInfo"/>, of the assembly.
        /// </summary>
        new ICultureIdentifier Culture { get; set; }
        /// <summary>
        /// Returns/sets the version of the project file.
        /// </summary>
        new Version FileVersion { get; set; }
        /// <summary>
        /// Returns/sets the version of the project.
        /// </summary>
        new Version AssemblyVersion { get; set; }
    }
}
