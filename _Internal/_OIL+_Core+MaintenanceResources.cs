using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Resources;
using System.Globalization;
namespace Oilexer._Internal
{
    static partial class _OIL
    {
        partial class _Core
        {
            internal static class MaintenanceResources
            {
                private static ResourceManager resourceManager;
                private static string autoRegions_BasePattern = null;
                private static string autoRegions_Classes = null;
                private static string autoRegions_Delegates = null;
                private static string autoRegions_Enumerators = null;
                private static string autoRegions_Interfaces = null;
                private static string autoRegions_NestedTypes = null;
                private static string autoRegions_Structures = null;
                static MaintenanceResources()
                {
                    resourceManager = new ResourceManager(typeof(_OIL));
                }

                /// <summary>
                /// Returns a string related to the base pattern of the auto regions.
                /// </summary>
                internal static string AutoRegions_BasePattern
                {
                    get
                    {
                        if (autoRegions_BasePattern == null)
                            autoRegions_BasePattern = ResourceManager.GetString("AutoRegions_BasePattern");
                        return autoRegions_BasePattern;
                    }
                }

                /// <summary>
                /// Returns a string supplimenting <see cref="AutoRegions_BasePattern"/>
                /// related to the classes grouping auto region.
                /// </summary>
                internal static string AutoRegions_Classes
                {
                    get
                    {
                        if (autoRegions_Classes == null)
                            autoRegions_Classes = ResourceManager.GetString("AutoRegions_Classes");
                        return autoRegions_Classes;
                    }
                }
                /// <summary>
                /// Returns a string supplimenting <see cref="AutoRegions_BasePattern"/>
                /// related to the delegates grouping auto region.
                /// </summary>
                internal static string AutoRegions_Delegates
                {
                    get
                    {
                        if (autoRegions_Delegates == null)
                            autoRegions_Delegates = ResourceManager.GetString("AutoRegions_Delegates");
                        return autoRegions_Delegates;
                    }
                }
                /// <summary>
                /// Returns a string supplimenting <see cref="AutoRegions_BasePattern"/>
                /// related to the enumerators grouping auto region.
                /// </summary>
                internal static string AutoRegions_Enumerators
                {
                    get
                    {
                        if (autoRegions_Enumerators == null)
                            autoRegions_Enumerators = ResourceManager.GetString("AutoRegions_Enumerators");
                        return autoRegions_Enumerators;
                    }
                }
                /// <summary>
                /// Returns a string supplimenting <see cref="AutoRegions_BasePattern"/>
                /// related to the interfaces grouping auto region.
                /// </summary>
                internal static string AutoRegions_Interfaces
                {
                    get
                    {
                        if (autoRegions_Interfaces == null)
                            autoRegions_Interfaces = ResourceManager.GetString("AutoRegions_Interfaces");
                        return autoRegions_Interfaces;
                    }
                }
                /// <summary>
                /// Returns a string supplimenting <see cref="AutoRegions_BasePattern"/>
                /// related to the nested types grouping auto region.
                /// </summary>
                internal static string AutoRegions_NestedTypes
                {
                    get
                    {
                        if (autoRegions_NestedTypes == null)
                            autoRegions_NestedTypes = ResourceManager.GetString("AutoRegions_NestedTypes");
                        return autoRegions_NestedTypes;
                    }
                }
                /// <summary>
                /// Returns a string supplimenting <see cref="AutoRegions_BasePattern"/>
                /// related to the structures grouping auto region.
                /// </summary>
                internal static string AutoRegions_Structures
                {
                    get
                    {
                        if (autoRegions_Structures == null)
                            autoRegions_Structures = ResourceManager.GetString("AutoRegions_Structures");
                        return autoRegions_Structures;
                    }
                }

                internal static ResourceManager ResourceManager
                {
                    get
                    {
                        return resourceManager;
                    }
                }
            }
        }
    }
}