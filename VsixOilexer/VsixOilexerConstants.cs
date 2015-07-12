// Guids.cs
// MUST match guids.h
using System;

namespace AllenCopeland.Abstraction.Slf.Languages.Oilexer.VSIntegration
{
    /* *
     * Outlines the necessary constants used in the required
     * metadata necessary to expose the model to Visual
     * Studio.
     * */
    static class VsixOilexerConstants
    {
        public static class Package
        {
            public const string guidVsixOilexerPkgString = "0bf345d3-6376-4e2f-855b-4fbcef0d8dc1";
            public const string guidVsixOilexerCmdSetString = "3f3207aa-5ec2-45ad-8fdc-d550d226a18e";
        }
        public static class GeneralOptionPane
        {
            public const string CategoryPath = @"Text Editor\OILexer";
            public const string PaneName = "General";
            public const int ResourceCategoryName = 100;
            public const int ResourceObjectName = 101;
        }
    };
}