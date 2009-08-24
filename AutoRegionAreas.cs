using System;
using System.Collections.Generic;
using System.Text;

namespace Oilexer
{
    /// <summary>
    /// Defines the areas that auto-regions are created for.
    /// </summary>
    [Flags]
    public enum AutoRegionAreas : int
    {
        /// <summary>
        /// Creates no regions.
        /// </summary>
        None = 0,
        /// <summary>
        /// Creates a region around every defined class.
        /// </summary>
        Class = 1,
        /// <summary>
        /// Creates a region around a series of defined classes.
        /// </summary>
        Classes = 1024,
        /// <summary>
        /// Creates a region around a defined delegate.
        /// </summary>
        Delegate = 2,
        /// <summary>
        /// Creates a region around a series of defined delegates.
        /// </summary>
        Delegates = 2048,
        /// <summary>
        /// Creates a region around a defined enumerator.
        /// </summary>
        Enumerator = 4,
        /// <summary>
        /// Creates a region around a series of defined enumerators.
        /// </summary>
        Enumerators = 4096,
        /// <summary>
        /// Creates a region around a defined interface.
        /// </summary>
        Interface = 8,
        /// <summary>
        /// Creates a region around a series of defined interfaces.
        /// </summary>
        Interfaces = 8192,
        /// <summary>
        /// Creates a region around a defined structure.
        /// </summary>
        Structure = 16,
        /// <summary>
        /// Creates a region around a series of defined structures.
        /// </summary>
        Structures = 16384,
        /// <summary>
        /// Creates a region around all nested types.
        /// </summary>
        NestedTypes = 32,
        /// <summary>
        /// Creates a region around every constructor.
        /// </summary>
        Constructor = 64,
        /// <summary>
        /// Creates a region around all the constructors on a type.
        /// </summary>
        Constructors = 32768,
        /// <summary>
        /// Creates a region around every field.
        /// </summary>
        Field = 128,
        /// <summary>
        /// Creates a region around all the fields on a type.
        /// </summary>
        Fields = 65536,
        /// <summary>
        /// Creates a region around every method.
        /// </summary>
        Method = 256,
        /// <summary>
        /// Creates a region around all the methods on a type.
        /// </summary>
        Methods = 131072,
        /// <summary>
        /// Creates a region around every property.
        /// </summary>
        Property = 512,
        /// <summary>
        /// Creates a region around all the properties on a type.
        /// </summary>
        Properties = 262144,
        /// <summary>
        /// Creates a region around every type.
        /// </summary>
        Type = Class | Delegate | Enumerator | Interface | Structure,
        /// <summary>
        /// Creates a region around every classification of types.
        /// </summary>
        Types = Classes | Delegates | Enumerators | Interfaces | Structures,
        /// <summary>
        /// Creates a region around every member.
        /// </summary>
        Member = Constructor | Field | Method | Property,
        /// <summary>
        /// Creates a region around every group of members.
        /// </summary>
        Members = Constructors | Fields | Methods | Properties,
        /// <summary>
        /// Creates a region around nested types overall and every classification of member.
        /// </summary>
        Standard = NestedTypes | Members
    }
}
